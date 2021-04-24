using Itsdits.Ravar.Animation;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Util;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Monster.Condition;
using Itsdits.Ravar.Monster.Move;
using Itsdits.Ravar.UI.Battle;
using Itsdits.Ravar.UI.Party;

namespace Itsdits.Ravar.Battle
{
    /// <summary>
    /// Battle manager class. Handles turn logic, status and stat changes, monster changes and win/loss cases.
    /// </summary>
    /// <remarks>This class was large and clunky so it's been broken down into separate functionalities since 1.0.8.</remarks>
    /// <seealso cref="BattleController"/><seealso cref="BattleUIController"/><seealso cref="BattleAnimator"/>
    [Obsolete]
    public class BattleSystem : MonoBehaviour
    {
        [Header("Monsters")]
        [Tooltip("The player's monster in the battle.")]
        [SerializeField] private BattleMonster _playerMonster;
        [Tooltip("The enemy's monster in the battle.")]
        [SerializeField] private BattleMonster _enemyMonster;

        [Header("Characters")]
        [Tooltip("The player's character image.")]
        [SerializeField] private Image _playerImage;
        [Tooltip("The enemy's character image.")]
        [SerializeField] private Image _battlerImage;

        [Header("UI")]
        [Tooltip("The BattleAnimator GameObject.")]
        [SerializeField] private BattleAnimator _battleAnimator;
        [Tooltip("The BattleDialogBox GameObject.")]
        [SerializeField] private BattleDialogBox _dialogBox;
        [Tooltip("The PartyScreen GameObject.")]
        [SerializeField] private PartyScreen _partyScreen;

        private PlayerController _player;
        private BattlerController _battler;
        private MonsterParty _playerParty;
        private MonsterParty _battlerParty;
        private MonsterObj _wildMonster;
        private BattleState _state;
        private BattleState? _prevState;
        private int _currentAction;
        private int _currentMove;
        private int _currentMember;
        private int _escapeAttempts;
        private bool _isChoiceYes = true;
        private bool _isCharBattle;
        private bool _isMonsterDown;

        public event Action<BattleResult, bool> OnBattleOver;

        /// <summary>
        /// Starts a battle with a wild monster.
        /// </summary>
        /// <param name="playerParty">Player's party of monsters.</param>
        /// <param name="wildMonster">Copy of the wild monster generated from the MapLayer.</param>
        public void StartWildBattle(MonsterParty playerParty, MonsterObj wildMonster)
        {
            _playerParty = playerParty;
            _wildMonster = wildMonster;
            _isCharBattle = false;
            _player = playerParty.GetComponent<PlayerController>();
            StartCoroutine(SetupBattle());
        }

        /// <summary>
        /// Starts a battle with an enemy character.
        /// </summary>
        /// <param name="playerParty">Player's party of monsters.</param>
        /// <param name="battlerParty">Battler's party of monsters.</param>
        public void StartCharBattle(MonsterParty playerParty, MonsterParty battlerParty)
        {
            _playerParty = playerParty;
            _battlerParty = battlerParty;
            _isCharBattle = true;
            _player = playerParty.GetComponent<PlayerController>();
            _battler = battlerParty.GetComponent<BattlerController>();
            StartCoroutine(SetupBattle());
        }
        
        private IEnumerator SetupBattle()
        {
            //TODO - handle case with no healthy monsters
            _playerMonster.HideHud();
            _enemyMonster.HideHud();
            if (!_isCharBattle)
            {
                _playerMonster.Setup(_playerParty.GetHealthyMonster()); 
                _enemyMonster.Setup(_wildMonster);
                _dialogBox.SetMoveList(_playerMonster.Monster.Moves);
                yield return _dialogBox.TypeDialog($"You have encountered an enemy {_enemyMonster.Monster.Base.Name}!");
            }
            else
            {
                ShowCharacterSprites();
                yield return _dialogBox.TypeDialog($"{_battler.Name} has challenged you to a battle!");

                // Deploy enemy monster.
                _battlerImage.gameObject.SetActive(false); //TODO - animate this
                _enemyMonster.gameObject.SetActive(true);
                MonsterObj enemyLeadMonster = _battlerParty.GetHealthyMonster();
                _enemyMonster.Setup(enemyLeadMonster);
                yield return _dialogBox.TypeDialog($"{_battler.Name} has deployed {enemyLeadMonster.Base.Name} to the battle!");

                // Deploy player monster.
                _playerImage.gameObject.SetActive(false); //TODO - animate this too
                _playerMonster.gameObject.SetActive(true);
                MonsterObj playerLeadMonster = _playerParty.GetHealthyMonster();
                _playerMonster.Setup(playerLeadMonster);
                _dialogBox.SetMoveList(_playerMonster.Monster.Moves);
                yield return _dialogBox.TypeDialog($"You have deployed {playerLeadMonster.Base.Name} to the battle!");
            }

            _escapeAttempts = 0;
            _partyScreen.Init();
            ActionSelection();
        }
        
        private IEnumerator ExecuteTurn(BattleAction playerAction)
        {
            _state = BattleState.ExecutingTurn;
            if (playerAction == BattleAction.Move)
            {
                // Get the monster moves.
                _playerMonster.Monster.CurrentMove = _playerMonster.Monster.Moves[_currentMove];
                _enemyMonster.Monster.CurrentMove = _enemyMonster.Monster.GetRandomMove();
                if (_playerMonster.Monster.CurrentMove == null || _enemyMonster.Monster.CurrentMove == null)
                {
                    BattleOver(BattleResult.Error);
                    yield break;
                }

                int playerPriority = _playerMonster.Monster.CurrentMove.Base.Priority;
                int enemyPriority = _enemyMonster.Monster.CurrentMove.Base.Priority;

                // Check who goes first.
                var isPlayerFirst = true;
                if (enemyPriority > playerPriority)
                {
                    isPlayerFirst = false;
                }   
                else if (playerPriority == enemyPriority)
                {
                    isPlayerFirst = _playerMonster.Monster.Speed >= _enemyMonster.Monster.Speed;
                }

                BattleMonster firstMonster = (isPlayerFirst) ? _playerMonster : _enemyMonster;
                BattleMonster secondMonster = (isPlayerFirst) ? _enemyMonster : _playerMonster;

                // Store in case it gets downed and switched out before its move.
                MonsterObj lastMonster = secondMonster.Monster;

                // Execute the first move.
                yield return UseMove(firstMonster, secondMonster, firstMonster.Monster.CurrentMove);
                yield return CleanUpTurn(firstMonster);
                if (_state == BattleState.BattleOver)
                {
                    yield break;
                }  

                // Execute the second move.
                if (lastMonster.CurrentHp > 0)
                {
                    yield return UseMove(secondMonster, firstMonster, secondMonster.Monster.CurrentMove);
                    yield return CleanUpTurn(secondMonster);
                    if (_state == BattleState.BattleOver)
                    {
                        yield break;
                    }   
                }
            }
            else if (playerAction == BattleAction.SwitchMonster)
            {
                // Switch the monster.
                MonsterObj selectedMember = _playerParty.Monsters[_currentMember];
                _state = BattleState.Busy;
                yield return SwitchMonster(selectedMember);

                // Now it's the enemy's turn.
                MoveObj enemyMove = _enemyMonster.Monster.GetRandomMove();
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
                _dialogBox.EnableActionSelector(false);
                yield return ActivateCrystal();
            }
            else if (playerAction == BattleAction.Run)
            {
                // Run from the battle.
                yield return AttemptRun();
            }

            // Return to ActionSelection.
            if (_state != BattleState.BattleOver)
            {
                ActionSelection();
            }
        }

        private IEnumerator UseMove(BattleMonster attackingMonster, BattleMonster defendingMonster, MoveObj move)
        {
            // Check for statuses like paralyze or sleep before trying to attack.
            bool canAttack = attackingMonster.Monster.CheckIfCanAttack();
            if (!canAttack)
            {
                yield return ShowStatusChanges(attackingMonster.Monster);
                yield return attackingMonster.Hud.UpdateHp();
                yield break;
            }

            // Clear the StatusChanges queue and decrease the move energy.
            yield return ShowStatusChanges(attackingMonster.Monster);
            move.Energy--;
            if (attackingMonster.IsPlayerMonster)
            {
                yield return _dialogBox.TypeDialog($"{attackingMonster.Monster.Base.Name} used {move.Base.Name}!");
            } 
            else
            {
                yield return _dialogBox.TypeDialog($"Enemy {attackingMonster.Monster.Base.Name} used {move.Base.Name}!");
            }

            if (CheckIfMoveHits(move, attackingMonster.Monster, defendingMonster.Monster))
            {
                attackingMonster.PlayAttackAnimation();
                yield return YieldHelper.HalfSecond;
                defendingMonster.PlayHitAnimation();

                // If status move then don't deal damage, switch to UseMoveEffects coroutine.
                if (move.Base.Category == MoveCategory.Status)
                {
                    yield return UseMoveEffects(move.Base.Effects, attackingMonster.Monster, defendingMonster.Monster, 
                                                move.Base.Target);
                }
                else
                {
                    DamageDetails damageDetails = defendingMonster.Monster.TakeDamage(move, attackingMonster.Monster);
                    yield return defendingMonster.Hud.UpdateHp();
                    yield return ShowDamageDetails(damageDetails);
                }

                // Check for secondary move effects.
                if (move.Base.MoveSecondaryEffects != null &&
                    move.Base.MoveSecondaryEffects.Count > 0 &&
                    attackingMonster.Monster.CurrentHp > 0)
                {
                    foreach (MoveSecondaryEffects effect in move.Base.MoveSecondaryEffects)
                    {
                        int rng = UnityEngine.Random.Range(1, 101);
                        if (rng <= effect.Chance)
                        {
                            yield return UseMoveEffects(effect, attackingMonster.Monster, defendingMonster.Monster, 
                                                        effect.Target);
                        }
                    }
                }

                // Handle downed monster and check if we should continue.
                if (defendingMonster.Monster.CurrentHp <= 0)
                {
                    yield return HandleDownedMonster(defendingMonster);
                }
            }
            else
            {
                // Handle a missed attack.
                if (attackingMonster.IsPlayerMonster)
                {
                    yield return _dialogBox.TypeDialog($"{attackingMonster.Monster.Base.Name} missed their attack!");
                }  
                else
                {
                    yield return _dialogBox.TypeDialog($"Enemy {attackingMonster.Monster.Base.Name} missed their attack!");
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
                yield return _dialogBox.TypeDialog("That was a critical hit!");
            } 

            if (damageDetails.TypeEffectiveness > 1f)
            {
                yield return _dialogBox.TypeDialog("That attack type is very strong!");
            } 
            else if (damageDetails.TypeEffectiveness < 1f)
            {
                yield return _dialogBox.TypeDialog("That attack type is not very strong!");
            }   
        }

        private IEnumerator ShowStatusChanges(MonsterObj monster)
        {
            while (monster.StatusChanges.Count > 0)
            {
                string message = monster.StatusChanges.Dequeue();
                yield return _dialogBox.TypeDialog(message);
            }
        }

        private IEnumerator HandleDownedMonster(BattleMonster downedMonster)
        {
            _isMonsterDown = true;
            if (downedMonster.IsPlayerMonster)
            {
                yield return _dialogBox.TypeDialog($"{downedMonster.Monster.Base.Name} has been taken down!");
            }
            else
            {
                yield return _dialogBox.TypeDialog($"Enemy {downedMonster.Monster.Base.Name} has been taken down!");
            }

            downedMonster.PlayDownedAnimation();
            yield return YieldHelper.TwoSeconds;

            if (!downedMonster.IsPlayerMonster)
            {
                // Setup exp gain variables.
                int expBase = downedMonster.Monster.Base.ExpGiven;
                int enemyLevel = downedMonster.Monster.Level;
                float battlerBonus = _isCharBattle ? 1.5f : 1f;

                // Handle exp gain.
                int expGain = Mathf.FloorToInt(expBase * enemyLevel * battlerBonus / 7);
                _playerMonster.Monster.Exp += expGain;
                yield return _dialogBox.TypeDialog($"{_playerMonster.Monster.Base.Name} has gained {expGain.ToString()} experience!");
                yield return _playerMonster.Hud.SlideExp();

                // While loop in case the monster gains more than 1 level.
                while (_playerMonster.Monster.CheckForLevelUp())
                {
                    _playerMonster.Hud.SetLevel();
                    yield return _dialogBox.TypeDialog($"{_playerMonster.Monster.Base.Name} has leveled up, they are now level {_playerMonster.Monster.Level.ToString()}!");
                    yield return _playerMonster.Hud.SlideExp(true);

                    // Learn a new move.
                    LearnableMove newMove = _playerMonster.Monster.GetLearnableMove();
                    if (newMove == null)
                    {
                        continue;
                    }

                    if (_playerMonster.Monster.Moves.Count < MonsterBase.MaxNumberOfMoves)
                    {
                        _playerMonster.Monster.LearnMove(newMove);
                        _dialogBox.SetMoveList(_playerMonster.Monster.Moves);
                        yield return _dialogBox.TypeDialog($"{_playerMonster.Monster.Base.Name} has learned {newMove.Base.Name}!");
                    }
                    else
                    {
                        // Forget an existing move first.
                        yield return ForgetMoveSelection(_playerMonster.Monster, newMove);
                    }
                }

                yield return YieldHelper.OneSecond;
            }

            // Wait until move learning is finished before calling CheckIfBattleIsOver.
            yield return new WaitUntil(() => _state == BattleState.ExecutingTurn);
            CheckIfBattleIsOver(downedMonster);
        }

        private IEnumerator ForgetMove(MonsterObj monster, MoveObj oldMove)
        {
            //TODO - fix bug where new move is lost if monster gains more than one level
            LearnableMove newMove = _playerMonster.Monster.GetLearnableMove();
            monster.ForgetMove(oldMove);
            monster.LearnMove(newMove);
            _dialogBox.SetMoveList(monster.Moves);
            
            yield return _dialogBox.TypeDialog($"{_playerMonster.Monster.Base.Name} has forgotten {oldMove.Base.Name}!");
            yield return _dialogBox.TypeDialog($"{_playerMonster.Monster.Base.Name} has learned {newMove.Base.Name}!");
            _state = BattleState.ExecutingTurn;
        }

        private IEnumerator SwitchMonster(MonsterObj newMonster)
        {
            if (_playerMonster.Monster.CurrentHp > 0)
            {
                yield return _dialogBox.TypeDialog($"{_playerMonster.Monster.Base.Name}, fall back!");
                _playerMonster.PlayDownedAnimation(); //TODO - create animation for returning to party
                yield return YieldHelper.TwoSeconds;
            }

            _playerMonster.Setup(newMonster);
            _dialogBox.SetMoveList(newMonster.Moves);
            yield return _dialogBox.TypeDialog($"It's your turn now, {newMonster.Base.Name}!");

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
            _enemyMonster.Setup(nextMonster);
            
            yield return _dialogBox.TypeDialog($"{_battler.Name} has deployed {nextMonster.Base.Name} to the battle!");
            _state = BattleState.ExecutingTurn;
        }

        private IEnumerator ActivateCrystal()
        {
            _state = BattleState.Busy;
            if (_isCharBattle)
            {
                _state = BattleState.ExecutingTurn;
                yield return _dialogBox.TypeDialog($"You can't steal {_enemyMonster.Monster.Base.Name} from {_battler.Name}!");
                yield break;
            }

            yield return _dialogBox.TypeDialog($"{_player.Name} activated a Capture Crystal!");
            int beamCount = AttemptCapture(_enemyMonster.Monster);
            yield return _battleAnimator.PlayCrystalAnimation(_playerMonster, _enemyMonster, beamCount);
            
            if (beamCount == 4)
            {
                // Capture the monster.
                //TODO - finish capture animation
                yield return _dialogBox.TypeDialog($"{_enemyMonster.Monster.Base.Name} was captured!");
                _playerParty.AddMonster(_enemyMonster.Monster);
                yield return _dialogBox.TypeDialog($"{_enemyMonster.Monster.Base.Name} was added to your team!");
                
                yield return YieldHelper.TwoSeconds;
                _battleAnimator.CleanUp();
                BattleOver(BattleResult.Won);
            }
            else
            {
                // Fail to capture the monster and give the player some feedback.
                //TODO - finish failed animation
                if (beamCount < 2)
                {
                    yield return _dialogBox.TypeDialog($"{_enemyMonster.Monster.Base.Name} broke away easily!");
                }  
                else
                {
                    yield return _dialogBox.TypeDialog($"{_enemyMonster.Monster.Base.Name} was almost captured!");
                }

                _battleAnimator.PlayFailAnimation();
                _state = BattleState.ExecutingTurn;
            }

            // Wait until the animation finishes to clean up.
            yield return YieldHelper.TwoAndChangeSeconds;
            _battleAnimator.CleanUp();
        }

        private IEnumerator AttemptRun()
        {
            _state = BattleState.Busy;
            if (_isCharBattle)
            {
                yield return _dialogBox.TypeDialog($"You can't run away from enemy Battlers!");
                _state = BattleState.ExecutingTurn;
                yield break;
            }

            ++_escapeAttempts;

            // Algo is from g3/4.
            int playerSpeed = _playerMonster.Monster.Speed;
            int enemySpeed = _enemyMonster.Monster.Speed;

            if (playerSpeed > enemySpeed)
            {
                yield return _dialogBox.TypeDialog($"You ran away from the enemy {_enemyMonster.Monster.Base.Name}!");
                BattleOver(BattleResult.Won);
            }
            else
            {
                float f = playerSpeed * 128 / enemySpeed + 30 * _escapeAttempts;
                f %= 256;
                int rng = UnityEngine.Random.Range(0, 256);

                if (rng < f)
                {
                    yield return _dialogBox.TypeDialog($"You ran away from the enemy {_enemyMonster.Monster.Base.Name}!");
                    BattleOver(BattleResult.Won);
                }
                else
                {
                    yield return _dialogBox.TypeDialog($"You couldn't run from the enemy {_enemyMonster.Monster.Base.Name}!");
                    _state = BattleState.ExecutingTurn;
                }
            }
        }

        private IEnumerator CleanUpTurn(BattleMonster attackingMonster)
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
            attackingMonster.Monster.CheckForStatusDamage();
            yield return ShowStatusChanges(attackingMonster.Monster);
            yield return attackingMonster.Hud.UpdateHp();
            if (attackingMonster.Monster.CurrentHp > 0)
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
                if (UnityEngine.Random.Range(0, 65535) >= b)
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

            int rng = UnityEngine.Random.Range(1, 101);
            return rng <= moveAccuracy;
        }

        private void CheckIfBattleIsOver(BattleMonster downedMonster)
        {
            if (downedMonster.IsPlayerMonster)
            {
                MonsterObj nextMonster = _playerParty.GetHealthyMonster();
                if (nextMonster != null)
                {
                    OpenPartyScreen();
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
                        StartCoroutine(ChoiceSelection(nextEnemyMonster));
                    }
                    else
                    {
                        BattleOver(BattleResult.Won);
                    } 
                }
            }
        }

        private void ShowCharacterSprites()
        {
            _playerMonster.gameObject.SetActive(false);
            _enemyMonster.gameObject.SetActive(false);
            _playerImage.gameObject.SetActive(true);
            _battlerImage.gameObject.SetActive(true);
            _playerImage.sprite = _player.BattleSprite;
            _battlerImage.sprite = _battler.Sprite;
        }

        private void BattleOver(BattleResult result)
        {
            _state = BattleState.BattleOver;
            _isMonsterDown = false;
            _playerParty.Monsters.ForEach(p => p.CleanUpMonster());
            OnBattleOver?.Invoke(result, _isCharBattle);
        }

        /// <summary>
        /// Handle updates to the BattleState.
        /// </summary>
        public void HandleUpdate()
        {
            if (_state == BattleState.ActionSelection)
            {
                HandleActionSelection();
            }
            else if (_state == BattleState.MoveSelection)
            {
                HandleMoveSelection();
            }
            else if (_state == BattleState.ForgetSelection)
            {
                HandleMoveSelection();
            }
            else if (_state == BattleState.ChoiceSelection)
            {
                HandleChoiceSelection();
            }
            else if (_state == BattleState.PartyScreen)
            {
                HandlePartySelection();
            }
        }

        private void ActionSelection()
        {
            _state = BattleState.ActionSelection;
            _dialogBox.SetDialog("Select an action:");
            _dialogBox.EnableActionSelector(true);
        }

        private void MoveSelection()
        {
            if (_state != BattleState.ForgetSelection)
            {
                _state = BattleState.MoveSelection;
            }

            _dialogBox.EnableDialogText(false);
            _dialogBox.EnableActionSelector(false);
            _dialogBox.EnableMoveSelector(true);
        }

        private IEnumerator ForgetMoveSelection(MonsterObj monster, LearnableMove newMove)
        {
            _prevState = BattleState.ForgetSelection;
            _state = BattleState.Busy;
            yield return _dialogBox.TypeDialog($"{monster.Base.Name} can learn {newMove.Base.Name}, but it's move list is full. Forget a move to learn {newMove.Base.Name}?");
            _state = BattleState.ChoiceSelection;
            _dialogBox.EnableChoiceSelector(true);
        }

        private IEnumerator ChoiceSelection(MonsterObj nextMonster)
        {
            _state = BattleState.Busy;
            yield return _dialogBox.TypeDialog($"{_battler.Name} is about to deploy {nextMonster.Base.Name}! Do you want to switch your Battokuri too?");
            _state = BattleState.ChoiceSelection;
            _dialogBox.EnableChoiceSelector(true);
        }

        private void OpenPartyScreen()
        {
            _state = BattleState.PartyScreen;
            _partyScreen.SetPartyData(_playerParty.Monsters);
            _partyScreen.gameObject.SetActive(true);
        }

        private void HandleActionSelection()
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                ++_currentAction;
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                --_currentAction;
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _currentAction += 2;
            }  
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentAction -= 2;
            }

            _currentAction = Mathf.Clamp(_currentAction, 0, 3);
            _dialogBox.UpdateActionSelection(_currentAction);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (_currentAction == 0)
                {
                    // Fight
                    MoveSelection();
                }
                else if (_currentAction == 1)
                {
                    // Items
                    StartCoroutine(ExecuteTurn(BattleAction.UseItem));
                }
                else if (_currentAction == 2)
                {
                    // Monsters
                    _prevState = _state;
                    OpenPartyScreen();
                }
                else if (_currentAction == 3)
                {
                    // Run
                    StartCoroutine(ExecuteTurn(BattleAction.Run));
                }
            }
        }

        private void HandleMoveSelection()
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                ++_currentMove;
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                --_currentMove;
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _currentMove += 2;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentMove -= 2;
            }

            _currentMove = Mathf.Clamp(_currentMove, 0, _playerMonster.Monster.Moves.Count - 1);
            _dialogBox.UpdateMoveSelection(_currentMove, _playerMonster.Monster.Moves[_currentMove]);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                MoveObj move = _playerMonster.Monster.Moves[_currentMove];
                _dialogBox.EnableMoveSelector(false);
                _dialogBox.EnableDialogText(true);

                if (_state == BattleState.ForgetSelection)
                {
                    _prevState = null;
                    StartCoroutine(ForgetMove(_playerMonster.Monster, move));
                }
                else
                {
                    if (move.Energy == 0)
                    {
                        return;
                    }

                    StartCoroutine(ExecuteTurn(BattleAction.Move));
                }
            }
            else if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                _dialogBox.EnableMoveSelector(false);
                _dialogBox.EnableDialogText(true);
                ActionSelection();
            }
        }

        private void HandleChoiceSelection()
        {
            //TODO - refactor this function to provide clarity on what the player is choosing.
            if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _isChoiceYes = !_isChoiceYes;
            }

            _dialogBox.UpdateChoiceSelection(_isChoiceYes);
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                _dialogBox.EnableChoiceSelector(false);
                if (_isChoiceYes)
                {
                    if (_prevState == BattleState.ForgetSelection)
                    {
                        _state = BattleState.ForgetSelection;
                        MoveSelection();
                    }
                    else
                    {
                        _prevState = BattleState.ChoiceSelection;
                        OpenPartyScreen();
                    }
                }
                else
                {
                    if (_prevState == BattleState.ForgetSelection)
                    {
                        _prevState = null;
                        _state = BattleState.ExecutingTurn;
                    }
                    else
                    {
                        StartCoroutine(SwitchEnemyMonster());
                    }
                }
            }
            else if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                _dialogBox.EnableChoiceSelector(false);
                if (_prevState == BattleState.ForgetSelection)
                {
                    _prevState = null;
                    _state = BattleState.ExecutingTurn;
                }
                else
                {
                    StartCoroutine(SwitchEnemyMonster());
                }
            }
        }

        private void HandlePartySelection()
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                ++_currentMember;
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                --_currentMember;
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _currentMember += 3;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentMember -= 3;
            }

            _currentMember = Mathf.Clamp(_currentMember, 0, _playerParty.Monsters.Count - 1);
            _partyScreen.UpdateMemberSelection(_currentMember);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                MonsterObj selectedMember = _playerParty.Monsters[_currentMember];
                if (selectedMember.CurrentHp <= 0)
                {
                    _partyScreen.SetMessageText("That Battokuri is downed and cannot be used!");
                    return;
                }

                if (selectedMember == _playerMonster.Monster)
                {
                    _partyScreen.SetMessageText("That Battokuri is already being used!");
                    return;
                }

                _partyScreen.gameObject.SetActive(false);
                // If player switched monster voluntarily it should count as a turn move
                // If monster was downed and forced switch then it should trigger a new turn
                if (_prevState == BattleState.ActionSelection)
                {
                    _prevState = null;
                    StartCoroutine(ExecuteTurn(BattleAction.SwitchMonster));
                }
                else
                {
                    _state = BattleState.Busy;
                    StartCoroutine(SwitchMonster(selectedMember));
                }
            }
            else if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                if (_playerMonster.Monster.CurrentHp <= 0)
                {
                    _partyScreen.SetMessageText("You must select a Battokuri!");
                    return;
                }

                _partyScreen.gameObject.SetActive(false);
                if (_prevState == BattleState.ChoiceSelection)
                {
                    _prevState = null;
                    StartCoroutine(SwitchEnemyMonster());
                }
                else
                {
                    ActionSelection();
                }  
            }
        }
    }
}