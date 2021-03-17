using Itsdits.Ravar.Animation;
using Itsdits.Ravar.Character.Battler;
using Itsdits.Ravar.Character.Player;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Monster.Condition;
using Itsdits.Ravar.Monster.Move;
using Itsdits.Ravar.UI.Battle;
using Itsdits.Ravar.UI.Party;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.Battle
{
    public class BattleSystem : MonoBehaviour
    {
        #region config
        [SerializeField] BattleAnimator battleAnimator;
        [SerializeField] BattleDialogBox dialogBox;
        [SerializeField] PartyScreen partyScreen;
        [SerializeField] BattleMonster playerMonster;
        [SerializeField] BattleMonster enemyMonster;
        [SerializeField] Image playerImage;
        [SerializeField] Image battlerImage;

        private PlayerController player;
        private BattlerController battler;
        private MonsterParty playerParty;
        private MonsterParty battlerParty;
        private MonsterObj wildMonster;
        private BattleState state;
        private BattleState? prevState;
        private int currentAction;
        private int currentMove;
        private int currentMember;
        private int escapeAttempts;
        private bool isChoiceYes = true;
        private bool isCharBattle = false;
        private bool isMonsterDown = false;
        #endregion
        
        /// <summary>
        /// Wild Monster encounter battle constructor.
        /// </summary>
        /// <param name="playerParty">Player's party of Monsters</param>
        /// <param name="wildMonster">Copy of the wildMonster pulled from the MapLayer</param>
        public void StartWildBattle(MonsterParty playerParty, MonsterObj wildMonster)
        {
            this.playerParty = playerParty;
            this.wildMonster = wildMonster;
            isCharBattle = false;
            player = playerParty.GetComponent<PlayerController>();
            StartCoroutine(SetupBattle());
        }

        /// <summary>
        /// Character battle constructor.
        /// </summary>
        /// <param name="playerParty">Player's party of Monsters</param>
        /// <param name="battlerParty">Battler's party of Monsters</param>
        public void StartCharBattle(MonsterParty playerParty, MonsterParty battlerParty)
        {
            this.playerParty = playerParty;
            this.battlerParty = battlerParty;
            isCharBattle = true;
            player = playerParty.GetComponent<PlayerController>();
            battler = battlerParty.GetComponent<BattlerController>();
            StartCoroutine(SetupBattle());
        }

        public event Action<BattleResult, bool> OnBattleOver;

        #region BattleCoroutines
        /// <summary>
        /// Setup the battle.
        /// </summary>
        /// <returns>ActionSelection coroutine</returns>
        public IEnumerator SetupBattle()
        {
            playerMonster.HideHud();
            enemyMonster.HideHud();

            if (!isCharBattle)
            {
                // Wild
                playerMonster.Setup(playerParty.GetHealthyMonster()); //TODO - handle setup with no healthy monsters
                enemyMonster.Setup(wildMonster);
                dialogBox.SetMoveList(playerMonster.Monster.Moves);
                yield return dialogBox.TypeDialog($"You have encountered an enemy {enemyMonster.Monster.Base.Name}!");
            }
            else
            {
                // Character
                ShowCharacterSprites();
                yield return dialogBox.TypeDialog($"{battler.Name} has challenged you to a battle!");

                // Deploy enemy monster.
                battlerImage.gameObject.SetActive(false); //TODO - animate this
                enemyMonster.gameObject.SetActive(true);
                var enemyLeadMonster = battlerParty.GetHealthyMonster();
                enemyMonster.Setup(enemyLeadMonster);
                yield return dialogBox.TypeDialog($"{battler.Name} has deployed {enemyLeadMonster.Base.Name} to the battle!");

                // Deploy player monster.
                playerImage.gameObject.SetActive(false); //TODO - animate this too
                playerMonster.gameObject.SetActive(true);
                var playerLeadMonster = playerParty.GetHealthyMonster();
                playerMonster.Setup(playerLeadMonster);
                dialogBox.SetMoveList(playerMonster.Monster.Moves);
                yield return dialogBox.TypeDialog($"You have deployed {playerLeadMonster.Base.Name} to the battle!");
            }

            escapeAttempts = 0;
            partyScreen.Init();
            ActionSelection();
        }

        private IEnumerator ExecuteTurn(BattleAction playerAction)
        {
            state = BattleState.ExecutingTurn;

            if (playerAction == BattleAction.Move)
            {
                // Get the monster moves.
                playerMonster.Monster.CurrentMove = playerMonster.Monster.Moves[currentMove];
                enemyMonster.Monster.CurrentMove = enemyMonster.Monster.GetRandomMove();
                if (playerMonster.Monster.CurrentMove == null || enemyMonster.Monster.CurrentMove == null)
                {
                    Debug.LogError("BS001: CurrentMove null. Escaping battle to attempt to recover.");

                    BattleOver(BattleResult.Error);
                }

                int playerPriority = playerMonster.Monster.CurrentMove.Base.Priority;
                int enemyPriority = enemyMonster.Monster.CurrentMove.Base.Priority;

                // Check who goes first.
                bool isPlayerFirst = true;

                if (enemyPriority > playerPriority)
                {
                    isPlayerFirst = false;
                }   
                else if (playerPriority == enemyPriority)
                {
                    isPlayerFirst = playerMonster.Monster.Speed >= enemyMonster.Monster.Speed;
                }

                var firstMonster = (isPlayerFirst) ? playerMonster : enemyMonster;
                var secondMonster = (isPlayerFirst) ? enemyMonster : playerMonster;

                // Store incase it gets downed and switched out before its move.
                var lastMonster = secondMonster.Monster;

                // Execute the first move.
                yield return UseMove(firstMonster, secondMonster, firstMonster.Monster.CurrentMove);
                yield return CleanUpTurn(firstMonster);
                if (state == BattleState.BattleOver)
                {
                    yield break;
                }  

                // Execute the second move.
                if (lastMonster.CurrentHp > 0)
                {
                    yield return UseMove(secondMonster, firstMonster, secondMonster.Monster.CurrentMove);
                    yield return CleanUpTurn(secondMonster);
                    if (state == BattleState.BattleOver)
                    {
                        yield break;
                    }   
                }
            }
            else if (playerAction == BattleAction.SwitchMonster)
            {
                // Switch the monster.
                var selectedMember = playerParty.Monsters[currentMember];
                state = BattleState.Busy;
                yield return SwitchMonster(selectedMember);

                // Now it's the enemy's turn.
                var enemyMove = enemyMonster.Monster.GetRandomMove();
                if (enemyMove == null)
                {
                    Debug.LogError("BS002: enemyMove null. Escaping battle to attempt to recover.");

                    BattleOver(BattleResult.Error);
                }

                yield return UseMove(enemyMonster, playerMonster, enemyMove);
                yield return CleanUpTurn(enemyMonster);
                if (state == BattleState.BattleOver)
                {
                    yield break;
                }
            }
            else if (playerAction == BattleAction.UseItem)
            {
                //TODO - refactor this when item system is implemented.
                // Use the item.
                dialogBox.EnableActionSelector(false);
                yield return ActivateCrystal();
            }
            else if (playerAction == BattleAction.Run)
            {
                // Run from the battle.
                yield return AttemptRun();
            }

            // Return to ActionSelection.
            if (state != BattleState.BattleOver)
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
                yield return attackingMonster.Hud.UpdateHP();
                yield break;
            }

            // Clear the StatusChanges queue and decrease the move energy.
            yield return ShowStatusChanges(attackingMonster.Monster);
            move.Energy--;

            if (attackingMonster.IsPlayerMonster)
            {
                yield return dialogBox.TypeDialog($"{attackingMonster.Monster.Base.Name} used {move.Base.Name}!");
            } 
            else
            {
                yield return dialogBox.TypeDialog($"Enemy {attackingMonster.Monster.Base.Name} used {move.Base.Name}!");
            }

            if (CheckIfMoveHits(move, attackingMonster.Monster, defendingMonster.Monster))
            {
                attackingMonster.PlayAttackAnimation();
                yield return new WaitForSeconds(0.5f);
                defendingMonster.PlayHitAnimation();

                // If status move then don't deal damage, switch to UseMoveEffects coroutine.
                if (move.Base.Category == MoveCategory.Status)
                {
                    yield return UseMoveEffects(move.Base.Effects, attackingMonster.Monster, defendingMonster.Monster, move.Base.Target);
                }
                else
                {
                    var damageDetails = defendingMonster.Monster.TakeDamage(move, attackingMonster.Monster);
                    yield return defendingMonster.Hud.UpdateHP();
                    yield return ShowDamageDetails(damageDetails);
                }

                // Check for secondary move effects.
                if (move.Base.MoveSecondaryEffects != null &&
                    move.Base.MoveSecondaryEffects.Count > 0 &&
                    attackingMonster.Monster.CurrentHp > 0)
                {
                    foreach (var effect in move.Base.MoveSecondaryEffects)
                    {
                        var rng = UnityEngine.Random.Range(1, 101);
                        if (rng <= effect.Chance)
                        {
                            yield return UseMoveEffects(effect, attackingMonster.Monster, defendingMonster.Monster, effect.Target);
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
                    yield return dialogBox.TypeDialog($"{attackingMonster.Monster.Base.Name} missed their attack!");
                }  
                else
                {
                    yield return dialogBox.TypeDialog($"Enemy {attackingMonster.Monster.Base.Name} missed their attack!");
                }
            }
        }

        private IEnumerator UseMoveEffects(MoveEffects effects, MonsterObj attackingMonster, MonsterObj defendingMonster, MoveTarget moveTarget)
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
            if (effects.Status != ConditionID.NON)
            {
                defendingMonster.SetStatus(effects.Status);
            }

            // Handle any volatile status conditions.
            if (effects.VolatileStatus != ConditionID.NON)
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
                yield return dialogBox.TypeDialog("That was a critical hit!");
            } 

            if (damageDetails.TypeEffectiveness > 1f)
            {
                yield return dialogBox.TypeDialog("That attack type is very strong!");
            } 
            else if (damageDetails.TypeEffectiveness < 1f)
            {
                yield return dialogBox.TypeDialog("That attack type is not very strong!");
            }   
        }

        private IEnumerator ShowStatusChanges(MonsterObj monster)
        {
            while (monster.StatusChanges.Count > 0)
            {
                var message = monster.StatusChanges.Dequeue();
                yield return dialogBox.TypeDialog(message);
            }
        }

        private IEnumerator HandleDownedMonster(BattleMonster downedMonster)
        {
            isMonsterDown = true;

            if (downedMonster.IsPlayerMonster)
            {
                yield return dialogBox.TypeDialog($"{downedMonster.Monster.Base.Name} has been taken down!");
            }
            else
            {
                yield return dialogBox.TypeDialog($"Enemy {downedMonster.Monster.Base.Name} has been taken down!");
            }

            downedMonster.PlayDownedAnimation();
            yield return new WaitForSeconds(2f);

            if (!downedMonster.IsPlayerMonster)
            {
                // Setup exp gain variables.
                int expBase = downedMonster.Monster.Base.ExpGiven;
                int enemyLevel = downedMonster.Monster.Level;
                float battlerBonus = (isCharBattle) ? 1.5f : 1f;

                // Handle exp gain.
                int expGain = Mathf.FloorToInt((expBase * enemyLevel * battlerBonus) / 7);
                playerMonster.Monster.Exp += expGain;
                yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name} has gained {expGain} experience!");
                yield return playerMonster.Hud.SlideExp();

                // While loop incase the monster gains more than 1 level.
                while (playerMonster.Monster.CheckForLevelUp())
                {
                    playerMonster.Hud.SetLevel();
                    yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name} has leveled up, they are now level {playerMonster.Monster.Level}!");
                    yield return playerMonster.Hud.SlideExp(true);

                    // Learn a new move.
                    var newMove = playerMonster.Monster.GetLearnableMove();
                    if (newMove != null)
                    {
                        if (playerMonster.Monster.Moves.Count < MonsterBase.MaxNumberOfMoves)
                        {
                            playerMonster.Monster.LearnMove(newMove);
                            dialogBox.SetMoveList(playerMonster.Monster.Moves);
                            yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name} has learned {newMove.Base.Name}!");
                        }
                        else
                        {
                            // Forget an existing move first.
                            yield return ForgetMoveSelection(playerMonster.Monster, newMove);
                        }
                    }
                }

                yield return new WaitForSeconds(1f);
            }

            // Wait until move learning is finished before CheckIfBattleIsOver.
            yield return new WaitUntil(() => state == BattleState.ExecutingTurn);
            CheckIfBattleIsOver(downedMonster);
        }

        private IEnumerator ForgetMove(MonsterObj monster, MoveObj oldMove)
        {
            var newMove = playerMonster.Monster.GetLearnableMove();
            monster.ForgetMove(oldMove);
            monster.LearnMove(newMove);
            dialogBox.SetMoveList(monster.Moves);
            yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name} has forgotten {oldMove.Base.Name}!");
            yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name} has learned {newMove.Base.Name}!");

            state = BattleState.ExecutingTurn;
        }

        private IEnumerator SwitchMonster(MonsterObj newMonster)
        {
            if (playerMonster.Monster.CurrentHp > 0)
            {
                yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name}, fall back!");
                playerMonster.PlayDownedAnimation(); //TODO - create animation for returning to party
                yield return new WaitForSeconds(2f);
            }

            playerMonster.Setup(newMonster);
            dialogBox.SetMoveList(newMonster.Moves);
            yield return dialogBox.TypeDialog($"It's your turn now, {newMonster.Base.Name}!");

            // Allow player to switch monsters after enemy replaces a downed monster.
            if (prevState == null)
            {
                state = BattleState.ExecutingTurn;
            }
            else if (prevState == BattleState.ChoiceSelection)
            {
                prevState = null;
                StartCoroutine(SwitchEnemyMonster());
            }
        }

        private IEnumerator SwitchEnemyMonster()
        {
            state = BattleState.Busy;

            var nextMonster = battlerParty.GetHealthyMonster();
            enemyMonster.Setup(nextMonster);
            yield return dialogBox.TypeDialog($"{battler.Name} has deployed {nextMonster.Base.Name} to the battle!");

            state = BattleState.ExecutingTurn;
        }

        private IEnumerator ActivateCrystal()
        {
            state = BattleState.Busy;

            if (isCharBattle)
            {
                state = BattleState.ExecutingTurn;

                yield return dialogBox.TypeDialog($"You can't steal {enemyMonster.Monster.Base.Name} from {battler.Name}!");
                yield break;
            }

            yield return dialogBox.TypeDialog($"{player.Name} activated a Capture Crystal!");
            int beamCount = AttemptCapture(enemyMonster.Monster);
            Debug.Log($"Beam count: {beamCount}");
            yield return battleAnimator.PlayCrystalAnimation(playerMonster, enemyMonster, beamCount);

            if (beamCount == 4)
            {
                // Capture the monster.
                //TODO - finish capture animation
                yield return dialogBox.TypeDialog($"{enemyMonster.Monster.Base.Name} was captured!");
                playerParty.AddMonster(enemyMonster.Monster);
                yield return dialogBox.TypeDialog($"{enemyMonster.Monster.Base.Name} was added to your team!");
                yield return new WaitForSeconds(2f);

                battleAnimator.CleanUp();
                BattleOver(BattleResult.Won);
            }
            else
            {
                // Fail to capture the monster and give the player some feedback.
                //TODO - finish failed animation
                if (beamCount < 2)
                {
                    yield return dialogBox.TypeDialog($"{enemyMonster.Monster.Base.Name} broke away easily!");
                }  
                else
                {
                    yield return dialogBox.TypeDialog($"{enemyMonster.Monster.Base.Name} was almost captured!");
                }

                battleAnimator.PlayFailAnimation();

                state = BattleState.ExecutingTurn;
            }

            // Wait until the animation finishes to clean up.
            yield return new WaitForSeconds(2.1f);
            battleAnimator.CleanUp();
        }

        private IEnumerator AttemptRun()
        {
            state = BattleState.Busy;

            if (isCharBattle)
            {
                yield return dialogBox.TypeDialog($"You can't run away from enemy Battlers!");

                state = BattleState.ExecutingTurn;

                yield break;
            }

            ++escapeAttempts;

            // Algo is from g3/4.
            int playerSpeed = playerMonster.Monster.Speed;
            int enemySpeed = enemyMonster.Monster.Speed;

            if (playerSpeed > enemySpeed)
            {
                yield return dialogBox.TypeDialog($"You ran away from the enemy {enemyMonster.Monster.Base.Name}!");
                BattleOver(BattleResult.Won);
            }
            else
            {
                float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
                f = f % 256;
                int rng = UnityEngine.Random.Range(0, 256);

                if (rng < f)
                {
                    yield return dialogBox.TypeDialog($"You ran away from the enemy {enemyMonster.Monster.Base.Name}!");
                    BattleOver(BattleResult.Won);
                }
                else
                {
                    yield return dialogBox.TypeDialog($"You couldn't run from the enemy {enemyMonster.Monster.Base.Name}!");

                    state = BattleState.ExecutingTurn;
                }
            }
        }

        private IEnumerator CleanUpTurn(BattleMonster attackingMonster)
        {
            // Skip if battle is over.
            if (state == BattleState.BattleOver)
            {
                yield break;
            }
            // Skip if monster is downed.
            if (isMonsterDown)
            {
                isMonsterDown = false;
                yield return new WaitUntil(() => state == BattleState.ExecutingTurn);
                yield break;
            }
            // Wait for monster switch, etc.
            yield return new WaitUntil(() => state == BattleState.ExecutingTurn);

            // Check for status changes like poison and update Monster/HUD.
            attackingMonster.Monster.CheckForStatusDamage();
            yield return ShowStatusChanges(attackingMonster.Monster);
            yield return attackingMonster.Hud.UpdateHP();

            // Attacking monster can be downed from status effects.
            if (attackingMonster.Monster.CurrentHp <= 0)
            {
                yield return HandleDownedMonster(attackingMonster);
                yield return new WaitUntil(() => state == BattleState.ExecutingTurn);
            }
        }
        #endregion

        #region Helper Functions
        private int AttemptCapture(MonsterObj monster)
        {
            // Algo is from g3/4.
            float a = (3 * monster.MaxHp - 2 * monster.CurrentHp) * monster.Base.CatchRate * ConditionDB.GetStatusBonus(monster.Status) / (3 * monster.MaxHp);

            if (a >= 255)
            {
                return 4;
            }
                
            float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

            int beamCount = 0;
            while (beamCount < 4)
            {
                if (UnityEngine.Random.Range(0, 65535) >= b)
                    break;

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
            var changeVals = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

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
                var nextMonster = playerParty.GetHealthyMonster();
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
                if (!isCharBattle)
                {
                    BattleOver(BattleResult.Won);
                }
                else
                {
                    var nextEnemyMonster = battlerParty.GetHealthyMonster();
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
            playerMonster.gameObject.SetActive(false);
            enemyMonster.gameObject.SetActive(false);
            playerImage.gameObject.SetActive(true);
            battlerImage.gameObject.SetActive(true);
            playerImage.sprite = player.BattleSprite;
            battlerImage.sprite = battler.Sprite;
        }

        private void BattleOver(BattleResult result)
        {
            state = BattleState.BattleOver;

            isMonsterDown = false;
            playerParty.Monsters.ForEach(p => p.CleanUpMonster());
            OnBattleOver(result, isCharBattle);
        }

        #endregion

        #region Selector Functions

        /// <summary>
        /// Handle updates to the BattleState.
        /// </summary>
        public void HandleUpdate()
        {
            if (state == BattleState.ActionSelection)
            {
                HandleActionSelection();
            }
            else if (state == BattleState.MoveSelection)
            {
                HandleMoveSelection();
            }
            else if (state == BattleState.ForgetSelection)
            {
                HandleMoveSelection();
            }
            else if (state == BattleState.ChoiceSelection)
            {
                HandleChoiceSelection();
            }
            else if (state == BattleState.PartyScreen)
            {
                HandlePartySelection();
            }
        }

        private void ActionSelection()
        {
            state = BattleState.ActionSelection;

            dialogBox.SetDialog("Select an action:");
            dialogBox.EnableActionSelector(true);
        }

        private void MoveSelection()
        {
            if (state != BattleState.ForgetSelection)
            {
                state = BattleState.MoveSelection;
            }

            dialogBox.EnableDialogText(false);
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableMoveSelector(true);
        }

        private IEnumerator ForgetMoveSelection(MonsterObj monster, LearnableMove newMove)
        {
            prevState = BattleState.ForgetSelection;
            state = BattleState.Busy;

            yield return dialogBox.TypeDialog($"{monster.Base.Name} can learn {newMove.Base.Name}, but it's move list is full. Forget a move to learn {newMove.Base.Name}?");

            state = BattleState.ChoiceSelection;

            dialogBox.EnableChoiceSelector(true);
        }

        private IEnumerator ChoiceSelection(MonsterObj nextMonster)
        {
            state = BattleState.Busy;

            yield return dialogBox.TypeDialog($"{battler.Name} is about to deploy {nextMonster.Base.Name}! Do you want to switch your Battokuri too?");

            state = BattleState.ChoiceSelection;

            dialogBox.EnableChoiceSelector(true);
        }

        private void OpenPartyScreen()
        {
            state = BattleState.PartyScreen;

            partyScreen.SetPartyData(playerParty.Monsters);
            partyScreen.gameObject.SetActive(true);
        }

        private void HandleActionSelection()
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                ++currentAction;
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                --currentAction;
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                currentAction += 2;
            }  
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentAction -= 2;
            }

            currentAction = Mathf.Clamp(currentAction, 0, 3);
            dialogBox.UpdateActionSelection(currentAction);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (currentAction == 0)
                {
                    // Fight
                    MoveSelection();
                }
                else if (currentAction == 1)
                {
                    // Items
                    StartCoroutine(ExecuteTurn(BattleAction.UseItem));
                }
                else if (currentAction == 2)
                {
                    // Monsters
                    prevState = state;

                    OpenPartyScreen();
                }
                else if (currentAction == 3)
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
                ++currentMove;
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                --currentMove;
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                currentMove += 2;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentMove -= 2;
            }

            currentMove = Mathf.Clamp(currentMove, 0, playerMonster.Monster.Moves.Count - 1);
            dialogBox.UpdateMoveSelection(currentMove, playerMonster.Monster.Moves[currentMove]);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                var move = playerMonster.Monster.Moves[currentMove];
                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);

                if (state == BattleState.ForgetSelection)
                {
                    prevState = null;

                    StartCoroutine(ForgetMove(playerMonster.Monster, move));
                }
                else
                {
                    if (move.Energy == 0)
                    {
                        Debug.Log($"No Energy for {move}.");
                        return;
                    }
                    else
                    {
                        StartCoroutine(ExecuteTurn(BattleAction.Move));
                    }
                }
            }
            else if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);
                ActionSelection();
            }
        }

        private void HandleChoiceSelection()
        {
            //TODO - refactor this function to provide clarity on what the player is choosing.
            if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                isChoiceYes = !isChoiceYes;
            }

            dialogBox.UpdateChoiceSelection(isChoiceYes);
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                dialogBox.EnableChoiceSelector(false);
                if (isChoiceYes)
                {
                    if (prevState == BattleState.ForgetSelection)
                    {
                        state = BattleState.ForgetSelection;

                        MoveSelection();
                    }
                    else
                    {
                        prevState = BattleState.ChoiceSelection;

                        OpenPartyScreen();
                    }
                }
                else
                {
                    if (prevState == BattleState.ForgetSelection)
                    {
                        prevState = null;

                        state = BattleState.ExecutingTurn;
                    }
                    else
                    {
                        StartCoroutine(SwitchEnemyMonster());
                    }
                }
            }
            else if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                dialogBox.EnableChoiceSelector(false);
                if (prevState == BattleState.ForgetSelection)
                {
                    prevState = null;

                    state = BattleState.ExecutingTurn;
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
                ++currentMember;
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                --currentMember;
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                currentMember += 3;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentMember -= 3;
            }

            currentMember = Mathf.Clamp(currentMember, 0, playerParty.Monsters.Count - 1);
            partyScreen.UpdateMemberSelection(currentMember);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                var selectedMember = playerParty.Monsters[currentMember];

                if (selectedMember.CurrentHp <= 0)
                {
                    partyScreen.SetMessageText("That Battokuri is downed and cannot be used!");
                    return;
                }

                if (selectedMember == playerMonster.Monster)
                {
                    partyScreen.SetMessageText("That Battokuri is already being used!");
                    return;
                }

                partyScreen.gameObject.SetActive(false);
                // If player switched monster voluntarily it should count as a turn move
                // If monster was downed and forced switch then it should trigger a new turn
                if (prevState == BattleState.ActionSelection)
                {
                    prevState = null;

                    StartCoroutine(ExecuteTurn(BattleAction.SwitchMonster));
                }
                else
                {
                    state = BattleState.Busy;

                    StartCoroutine(SwitchMonster(selectedMember));
                }
            }
            else if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                if (playerMonster.Monster.CurrentHp <= 0)
                {
                    partyScreen.SetMessageText("You must select a Battokuri!");
                    return;
                }

                partyScreen.gameObject.SetActive(false);
                if (prevState == BattleState.ChoiceSelection)
                {
                    prevState = null;

                    StartCoroutine(SwitchEnemyMonster());
                }
                else
                {
                    ActionSelection();
                }  
            }
        }
        #endregion
    }
}