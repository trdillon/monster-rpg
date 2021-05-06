using System.Collections;
using Itsdits.Ravar.Animation;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Monster.Condition;
using Itsdits.Ravar.Monster.Move;
using Itsdits.Ravar.Util;
using UnityEngine;

namespace Itsdits.Ravar.Battle
{
    /// <summary>
    /// Controller class that handles the flow and logic of the Battle scene.
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        [Header("UI and Animator Controllers")]
        [Tooltip("BattleUIController that handles the UI of the Battle scene.")]
        [SerializeField] private BattleUIController _ui;
        [Tooltip("BattleAnimator that handles the animations in the Battle scene.")]
        [SerializeField] private BattleAnimator _animator;
        
        private BattlerController _battler;
        private PlayerController _player;
        private MonsterParty _battlerParty;
        private MonsterParty _playerParty;
        private MonsterObj _playerMonster;
        private MonsterObj _enemyMonster;

        private BattleState _state;
        private BattleState? _prevState;

        private int _escapeAttempts;
        private bool _isCharBattle;
        private bool _isMonsterDown;
        
        private void OnEnable()
        {
            _state = BattleState.Start;
            GameSignals.BATTLE_START.AddListener(OnBattleStart);
            GameSignals.BATTLE_MOVE_SELECT.AddListener(OnMoveSelect);
            GameSignals.ENCOUNTER_START.AddListener(OnEncounterStart);
            GameSignals.PARTY_CHANGE.AddListener(OnPartyChange);
        }

        private void OnDisable()
        {
            GameSignals.BATTLE_START.RemoveListener(OnBattleStart);
            GameSignals.BATTLE_MOVE_SELECT.RemoveListener(OnMoveSelect);
            GameSignals.ENCOUNTER_START.RemoveListener(OnEncounterStart);
            GameSignals.PARTY_CHANGE.RemoveListener(OnPartyChange);
        }
        
        private void OnBattleStart(BattleItem battleItem)
        {
            _state = BattleState.Busy;
            _player = battleItem.Player;
            _battler = battleItem.Battler;
            _playerParty = _player.Party;
            _battlerParty = _battler.Party;
            _isCharBattle = true;
            StartCoroutine(SetupBattle());
        }

        private void OnEncounterStart(EncounterItem encounterItem)
        {
            _state = BattleState.Busy;
            _player = encounterItem.Player;
            _enemyMonster = encounterItem.Monster;
            _playerParty = _player.Party;
            _isCharBattle = false;
            StartCoroutine(SetupBattle());
        }

        private void OnMoveSelect(BattleMove battleMove)
        {
            _playerMonster.CurrentMove = _playerMonster.Moves[battleMove.MoveNumber];

            if (battleMove.State == BattleState.ForgetSelection)
            {
                _prevState = null;
                StartCoroutine(ForgetMove(_playerMonster, _playerMonster.CurrentMove));
            }
            else if (battleMove.State == BattleState.MoveSelection)
            {
                if (_playerMonster.CurrentMove.Energy == 0)
                {
                    return;
                }

                StartCoroutine(ExecuteTurn(BattleAction.Move));
            }
        }

        private void OnPartyChange(int newMonster)
        {
            StartCoroutine(SwitchMonster(_playerParty.Monsters[newMonster]));
        }

        private IEnumerator SetupBattle()
        {
            _ui.HideHud();
            if (!_isCharBattle)
            {
                yield return _ui.TypeDialog("BATTLE_WILD_ENCOUNTER");
                
                // Setup the monsters and send them into the battle.
                _playerMonster = _playerParty.GetHealthyMonster();
                _animator.PlayerMonster.Setup(_playerMonster);
                _animator.EnemyMonster.Setup(_enemyMonster);
                _animator.ShowMonsterSprites();
                
                // Send the move list to the UI controller to be shown when the move selector is enabled.
                _ui.GetMoveList(_playerMonster.Moves);
            }
            else
            {
                // Show the characters and battle intro dialog.
                _animator.ShowCharacterSprites(_player, _battler);
                yield return _ui.TypeDialog("BATTLE_CHAR_ENCOUNTER");
                
                // Setup the monsters and send them into the battle.
                _enemyMonster = _battlerParty.GetHealthyMonster();
                _playerMonster = _playerParty.GetHealthyMonster();
                _animator.EnemyMonster.Setup(_enemyMonster);
                _animator.PlayerMonster.Setup(_playerMonster);
                _animator.ReplaceCharactersWithMonsters();
                
                // Send the move list to the UI controller to be shown when the move selector is enabled.
                _ui.GetMoveList(_playerMonster.Moves);
            }

            // Finish setup and pass control to the UI for action selection.
            _escapeAttempts = 0;
            _state = BattleState.ActionSelection;
            _ui.ActionSelection();
            yield return _ui.TypeDialog("BATTLE_ACTION_SELECT");
        }

        private IEnumerator ExecuteTurn(BattleAction playerAction)
        {
            _state = BattleState.ExecutingTurn;
            if (playerAction == BattleAction.Move)
            {
                // Make sure both monsters have a valid move.
                _enemyMonster.CurrentMove = _enemyMonster.GetRandomMove();
                if (_playerMonster.CurrentMove == null || _enemyMonster.CurrentMove == null)
                {
                    BattleOver(BattleResult.Error);
                    yield break;
                }

                // Check who goes first.
                int playerPriority = _playerMonster.CurrentMove.Base.Priority;
                int enemyPriority = _enemyMonster.CurrentMove.Base.Priority;
 
                var isPlayerFirst = true;
                if (enemyPriority > playerPriority)
                {
                    isPlayerFirst = false;
                }   
                else if (playerPriority == enemyPriority)
                {
                    isPlayerFirst = _playerMonster.Speed >= _enemyMonster.Speed;
                }

                MonsterObj firstMonster = isPlayerFirst ? _playerMonster : _enemyMonster;
                MonsterObj secondMonster = isPlayerFirst ? _enemyMonster : _playerMonster;

                // Store in case it gets downed and switched out before its move.
                MonsterObj lastMonster = secondMonster;

                // Execute the first move.
                yield return UseMove(firstMonster, secondMonster, firstMonster.CurrentMove);
                yield return CleanUpTurn(firstMonster);
                if (_state == BattleState.BattleOver)
                {
                    yield break;
                }  

                // Execute the second move.
                if (lastMonster.CurrentHp > 0)
                {
                    yield return UseMove(secondMonster, firstMonster, secondMonster.CurrentMove);
                    yield return CleanUpTurn(secondMonster);
                    if (_state == BattleState.BattleOver)
                    {
                        yield break;
                    }   
                }
            }
            else if (playerAction == BattleAction.SwitchMonster)
            {
                _state = BattleState.Busy;
                // Wait until OnPartyChange has been triggered and the monster has been changed.
                yield return YieldHelper.THREE_SECONDS;

                // Now it's the enemy's turn.
                MoveObj enemyMove = _enemyMonster.GetRandomMove();
                if (enemyMove == null)
                {
                    BattleOver(BattleResult.Error);
                    yield break;
                }

                yield return UseMove(_enemyMonster, _playerMonster, enemyMove);
                yield return CleanUpTurn(_enemyMonster);
                if (_state == BattleState.BattleOver)
                {
                    yield break;
                }
            }
            else if (playerAction == BattleAction.UseItem)
            {
                //TODO - refactor this when item system is implemented.
                // Use the item.
                //_dialogBox.EnableActionSelector(false);
                yield return ActivateCrystal();
            }
            else if (playerAction == BattleAction.Run)
            {
                // Run from the battle.
                yield return AttemptRun();
            }

            // Return to ActionSelection.
            if (_state == BattleState.BattleOver)
            {
                yield break;
            }

            _state = BattleState.ActionSelection;
            _ui.ActionSelection();
        }

        private IEnumerator UseMove(MonsterObj attackingMonster, MonsterObj defendingMonster, MoveObj move)
        {
            bool isPlayerAttacker = attackingMonster == _animator.PlayerMonster.Monster;
            
            // Check for statuses like paralyze or sleep before trying to attack.
            bool canAttack = attackingMonster.CheckIfCanAttack();
            if (!canAttack)
            {
                yield return ShowStatusChanges(attackingMonster);
                if (isPlayerAttacker)
                {
                    yield return _ui.PlayerHud.UpdateHp();
                }
                else
                {
                    yield return _ui.EnemyHud.UpdateHp();
                }
                
                yield break;
            }

            // Clear the StatusChanges queue and decrease the move energy.
            yield return ShowStatusChanges(attackingMonster);
            move.Energy--;
            if (isPlayerAttacker)
            {
                yield return _ui.TypeDialog($"{attackingMonster.Base.Name} used {move.Base.Name}!");
            } 
            else
            {
                yield return _ui.TypeDialog($"Enemy {attackingMonster.Base.Name} used {move.Base.Name}!");
            }

            if (CheckIfMoveHits(move, attackingMonster, defendingMonster))
            {
                if (isPlayerAttacker)
                {
                    _animator.PlayerMonster.PlayAttackAnimation();
                    yield return YieldHelper.HALF_SECOND;
                    _animator.EnemyMonster.PlayHitAnimation();
                }
                else
                {
                    _animator.EnemyMonster.PlayAttackAnimation();
                    yield return YieldHelper.HALF_SECOND;
                    _animator.PlayerMonster.PlayHitAnimation();
                }

                // If status move then don't deal damage, switch to UseMoveEffects coroutine.
                if (move.Base.Category == MoveCategory.Status)
                {
                    yield return UseMoveEffects(move.Base.Effects, attackingMonster, defendingMonster, move.Base.Target);
                }
                else
                {
                    DamageDetails damageDetails = defendingMonster.TakeDamage(move, attackingMonster);
                    if (isPlayerAttacker)
                    {
                        yield return _ui.EnemyHud.UpdateHp();
                    }
                    else
                    {
                        yield return _ui.PlayerHud.UpdateHp();
                    }
                    
                    yield return ShowDamageDetails(damageDetails);
                }

                // Check for secondary move effects.
                if (move.Base.MoveSecondaryEffects != null &&
                    move.Base.MoveSecondaryEffects.Count > 0 &&
                    attackingMonster.CurrentHp > 0)
                {
                    foreach (MoveSecondaryEffects effect in move.Base.MoveSecondaryEffects)
                    {
                        int rng = Random.Range(1, 101);
                        if (rng <= effect.Chance)
                        {
                            yield return UseMoveEffects(effect, attackingMonster, defendingMonster, effect.Target);
                        }
                    }
                }

                // Handle downed monster and check if we should continue.
                if (defendingMonster.CurrentHp <= 0)
                {
                    yield return HandleDownedMonster(defendingMonster);
                }
            }
            else
            {
                // Handle a missed attack.
                if (attackingMonster == _animator.PlayerMonster.Monster)
                {
                    yield return _ui.TypeDialog($"{attackingMonster.Base.Name} missed their attack!");
                }  
                else
                {
                    yield return _ui.TypeDialog($"Enemy {attackingMonster.Base.Name} missed their attack!");
                }
            }
        }
        
        private IEnumerator UseMoveEffects(MoveEffects effects, MonsterObj attackingMonster, MonsterObj defendingMonster, 
                                           MoveTarget moveTarget)
        {
            // Handle any stat changes.
            if (effects.StatChanges != null)
            {
                if (moveTarget == MoveTarget.Self)
                {
                    attackingMonster.ApplyStatChanges(effects.StatChanges);
                }   
                else
                {
                    defendingMonster.ApplyStatChanges(effects.StatChanges);
                }  
            }

            // Handle any status conditions.
            if (effects.Status != ConditionID.None)
            {
                defendingMonster.SetStatus(effects.Status);
            }

            // Handle any volatile status conditions.
            if (effects.VolatileStatus != ConditionID.None)
            {
                defendingMonster.SetVolatileStatus(effects.VolatileStatus);
            }

            yield return ShowStatusChanges(attackingMonster);
            yield return ShowStatusChanges(defendingMonster);
        }

        private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
        {
            if (damageDetails.Critical > 1f)
            {
                yield return _ui.TypeDialog("That was a critical hit!");
            } 

            if (damageDetails.TypeEffectiveness > 1f)
            {
                yield return _ui.TypeDialog("That attack type is very strong!");
            } 
            else if (damageDetails.TypeEffectiveness < 1f)
            {
                yield return _ui.TypeDialog("That attack type is not very strong!");
            }   
        }

        private IEnumerator ShowStatusChanges(MonsterObj monster)
        {
            while (monster.StatusChanges.Count > 0)
            {
                string message = monster.StatusChanges.Dequeue();
                yield return _ui.TypeDialog(message);
            }
        }
        
        private IEnumerator HandleDownedMonster(MonsterObj downedMonster)
        {
            bool isPlayerDowned = downedMonster == _animator.PlayerMonster.Monster;
            _isMonsterDown = true;
            if (isPlayerDowned)
            {
                yield return _ui.TypeDialog($"{downedMonster.Base.Name} has been taken down!");
            }
            else
            {
                yield return _ui.TypeDialog($"Enemy {downedMonster.Base.Name} has been taken down!");
            }

            if (isPlayerDowned)
            {
                _animator.PlayerMonster.PlayDownedAnimation();
            }
            else
            {
                _animator.EnemyMonster.PlayDownedAnimation();
            }
    
            yield return YieldHelper.TWO_SECONDS;

            if (!isPlayerDowned)
            {
                // Setup exp gain variables.
                int expBase = downedMonster.Base.ExpGiven;
                int enemyLevel = downedMonster.Level;
                float battlerBonus = _isCharBattle ? 1.5f : 1f;

                // Handle exp gain.
                int expGain = Mathf.FloorToInt(expBase * enemyLevel * battlerBonus / 7);
                _playerMonster.Exp += expGain;
                yield return _ui.TypeDialog($"{_playerMonster.Base.Name} has gained {expGain.ToString()} experience!");
                yield return _ui.PlayerHud.SlideExp();

                // While loop in case the monster gains more than 1 level.
                while (_playerMonster.CheckForLevelUp())
                {
                    _ui.PlayerHud.SetLevel();
                    yield return _ui.TypeDialog($"{_playerMonster.Base.Name} has leveled up, they are now level {_playerMonster.Level.ToString()}!");
                    yield return _ui.PlayerHud.SlideExp(true);

                    // Learn a new move.
                    LearnableMove newMove = _playerMonster.GetLearnableMove();
                    if (newMove == null)
                    {
                        continue;
                    }

                    if (_playerMonster.Moves.Count < MonsterBase.MaxNumberOfMoves)
                    {
                        _playerMonster.LearnMove(newMove);
                        _ui.GetMoveList(_playerMonster.Moves);
                        GameSignals.BATTLE_MOVE_UPDATE.Dispatch(true);
                        yield return _ui.TypeDialog($"{_playerMonster.Base.Name} has learned {newMove.Base.Name}!");
                    }
                    else
                    {
                        // Forget an existing move first.
                        //TODO - fix forget move selection
                        //yield return ForgetMoveSelection(_playerMonster, newMove);
                    }
                }

                yield return YieldHelper.ONE_SECOND;
            }

            // Wait until move learning is finished before calling CheckIfBattleIsOver.
            yield return new WaitUntil(() => _state == BattleState.ExecutingTurn);
            CheckIfBattleIsOver(downedMonster);
        }

        private IEnumerator ForgetMove(MonsterObj monster, MoveObj oldMove)
        {
            //TODO - fix bug where new move is lost if monster gains more than one level
            LearnableMove newMove = _playerMonster.GetLearnableMove();
            monster.ForgetMove(oldMove);
            monster.LearnMove(newMove);
            _ui.GetMoveList(monster.Moves);
            GameSignals.BATTLE_MOVE_UPDATE.Dispatch(true);
            
            yield return _ui.TypeDialog($"{_playerMonster.Base.Name} has forgotten {oldMove.Base.Name}!");
            yield return _ui.TypeDialog($"{_playerMonster.Base.Name} has learned {newMove.Base.Name}!");
            _state = BattleState.ExecutingTurn;
        }
        
        private IEnumerator SwitchMonster(MonsterObj newMonster)
        {
            if (_playerMonster.CurrentHp > 0)
            {
                yield return _ui.TypeDialog($"{_playerMonster.Base.Name}, fall back!");
                _animator.PlayerMonster.PlayDownedAnimation(); //TODO - create animation for returning to party
                yield return YieldHelper.TWO_SECONDS;
            }
            
            _animator.PlayerMonster.Setup(newMonster);
            _ui.GetMoveList(newMonster.Moves);
            GameSignals.BATTLE_MOVE_UPDATE.Dispatch(true);
            yield return _ui.TypeDialog($"It's your turn now, {newMonster.Base.Name}!");

            // Allow player to switch monsters after enemy replaces a downed monster.
            if (_prevState == null)
            {
                _state = BattleState.ExecutingTurn;
            }
            else if (_prevState == BattleState.ChoiceSelection)
            {
                _prevState = null;
                StartCoroutine(SwitchEnemyMonster());
            }
        }

        private IEnumerator SwitchEnemyMonster()
        {
            _state = BattleState.Busy;
            MonsterObj nextMonster = _battlerParty.GetHealthyMonster();
            _animator.EnemyMonster.Setup(nextMonster);
            
            yield return _ui.TypeDialog($"{_battler.Name} has deployed {nextMonster.Base.Name} to the battle!");
            _state = BattleState.ExecutingTurn;
        }
        
        private IEnumerator ActivateCrystal()
        {
            _state = BattleState.Busy;
            if (_isCharBattle)
            {
                _state = BattleState.ExecutingTurn;
                yield return _ui.TypeDialog($"You can't steal {_enemyMonster.Base.Name} from {_battler.Name}!");
                yield break;
            }

            yield return _ui.TypeDialog($"{_player.Name} activated a Capture Crystal!");
            int beamCount = AttemptCapture(_enemyMonster);
            yield return _animator.PlayCrystalAnimation(_animator.PlayerMonster, _animator.EnemyMonster, beamCount);
            
            if (beamCount == 4)
            {
                // Capture the monster.
                //TODO - finish capture animation
                yield return _ui.TypeDialog($"{_enemyMonster.Base.Name} was captured!");
                _playerParty.AddMonster(_enemyMonster);
                yield return _ui.TypeDialog($"{_enemyMonster.Base.Name} was added to your team!");
                
                yield return YieldHelper.TWO_SECONDS;
                _animator.CleanUp();
                BattleOver(BattleResult.Won);
            }
            else
            {
                // Fail to capture the monster and give the player some feedback.
                //TODO - finish failed animation
                if (beamCount < 2)
                {
                    yield return _ui.TypeDialog($"{_enemyMonster.Base.Name} broke away easily!");
                }  
                else
                {
                    yield return _ui.TypeDialog($"{_enemyMonster.Base.Name} was almost captured!");
                }

                _animator.PlayFailAnimation();
                _state = BattleState.ExecutingTurn;
            }

            // Wait until the animation finishes to clean up.
            yield return YieldHelper.TWO_AND_CHANGE_SECONDS;
            _animator.CleanUp();
        }
        
        private IEnumerator AttemptRun()
        {
            _state = BattleState.Busy;
            if (_isCharBattle)
            {
                yield return _ui.TypeDialog($"You can't run away from enemy Battlers!");
                _state = BattleState.ExecutingTurn;
                yield break;
            }

            ++_escapeAttempts;

            // Algo is from g3/4.
            int playerSpeed = _playerMonster.Speed;
            int enemySpeed = _enemyMonster.Speed;

            if (playerSpeed > enemySpeed)
            {
                yield return _ui.TypeDialog($"You ran away from the enemy {_enemyMonster.Base.Name}!");
                BattleOver(BattleResult.Won);
            }
            else
            {
                float f = playerSpeed * 128 / enemySpeed + 30 * _escapeAttempts;
                f %= 256;
                int rng = Random.Range(0, 256);

                if (rng < f)
                {
                    yield return _ui.TypeDialog($"You ran away from the enemy {_enemyMonster.Base.Name}!");
                    BattleOver(BattleResult.Won);
                }
                else
                {
                    yield return _ui.TypeDialog($"You couldn't run from the enemy {_enemyMonster.Base.Name}!");
                    _state = BattleState.ExecutingTurn;
                }
            }
        }
        
        private IEnumerator CleanUpTurn(MonsterObj attackingMonster)
        {
            // Skip if battle is over.
            if (_state == BattleState.BattleOver)
            {
                yield break;
            }
            // Skip if monster is downed.
            if (_isMonsterDown)
            {
                _isMonsterDown = false;
                yield return new WaitUntil(() => _state == BattleState.ExecutingTurn);
                yield break;
            }
            // Wait for monster switch, etc.
            yield return new WaitUntil(() => _state == BattleState.ExecutingTurn);

            // Check for status changes like poison and update Monster/HUD.
            attackingMonster.CheckForStatusDamage();
            yield return ShowStatusChanges(attackingMonster);
            if (attackingMonster == _animator.PlayerMonster.Monster)
            {
                yield return _ui.PlayerHud.UpdateHp();
            }
            else
            {
                yield return _ui.EnemyHud.UpdateHp();
            }
            
            if (attackingMonster.CurrentHp > 0)
            {
                yield break;
            }

            // Attacking monster was downed from status effects.
            yield return HandleDownedMonster(attackingMonster);
            yield return new WaitUntil(() => _state == BattleState.ExecutingTurn);
        }
        
        private int AttemptCapture(MonsterObj monster)
        {
            // Algo is from g3/4.
            float a = (3 * monster.MaxHp - 2 * monster.CurrentHp) * monster.Base.CatchRate * 
                ConditionDB.GetStatusBonus(monster.Status) / (3 * monster.MaxHp);

            if (a >= 255)
            {
                return 4;
            }
                
            float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

            var beamCount = 0;
            while (beamCount < 4)
            {
                if (Random.Range(0, 65535) >= b)
                {
                    break;
                }

                ++beamCount;
            }

            return beamCount;
        }
        
        private bool CheckIfMoveHits(MoveObj move, MonsterObj attackingMonster, MonsterObj defendingMonster)
        {
            if (move.Base.AlwaysHits)
            {
                return true;
            }

            // Stat changes based on original game's formula.
            float moveAccuracy = move.Base.Accuracy;
            int accuracy = attackingMonster.StatsChanged[MonsterStat.Accuracy];
            int evasion = defendingMonster.StatsChanged[MonsterStat.Evasion];
            float[] changeVals = { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

            if (accuracy > 0)
            {
                moveAccuracy *= changeVals[accuracy];
            }
            else if (accuracy < 0)
            {
                moveAccuracy /= changeVals[-accuracy];
            }

            if (evasion > 0)
            {
                moveAccuracy /= changeVals[evasion];
            }
            else if (evasion < 0)
            {
                moveAccuracy *= changeVals[evasion];
            }

            int rng = Random.Range(1, 101);
            return rng <= moveAccuracy;
        }

        private void CheckIfBattleIsOver(MonsterObj downedMonster)
        {
            if (downedMonster == _animator.PlayerMonster.Monster)
            {
                MonsterObj nextMonster = _playerParty.GetHealthyMonster();
                if (nextMonster != null)
                {
                    GameSignals.PARTY_OPEN.Dispatch(true);
                }
                else
                {
                    BattleOver(BattleResult.Lost);
                }
            }
            else
            {
                if (!_isCharBattle)
                {
                    BattleOver(BattleResult.Won);
                }
                else
                {
                    MonsterObj nextEnemyMonster = _battlerParty.GetHealthyMonster();
                    if (nextEnemyMonster != null)
                    {
                        //TODO - fix choice selection
                        //StartCoroutine(ChoiceSelection(nextEnemyMonster));
                    }
                    else
                    {
                        BattleOver(BattleResult.Won);
                    } 
                }
            }
        }
        
        private void BattleOver(BattleResult result)
        {
            _state = BattleState.BattleOver;
            _isMonsterDown = false;
            _playerParty.Monsters.ForEach(p => p.CleanUpMonster());
            //TODO - handle battle over
            //OnBattleOver?.Invoke(result, _isCharBattle);
        }
    }
}