using System;
using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleMonster playerMonster;
    [SerializeField] BattleMonster enemyMonster;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    MonsterParty playerParty;
    Monster wildMonster;

    BattleState state;
    BattleState? prevState;

    // Indices for UI and monster selection
    int currentAction;
    int currentMove;
    int currentMember;

    public event Action<bool> OnBattleOver;

    public void StartBattle(MonsterParty playerParty, Monster wildMonster)
    {
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;
        StartCoroutine(SetupBattle());
    }

    //
    // BATTLE COROUTINES
    //
    public IEnumerator SetupBattle()
    {
        playerMonster.Setup(playerParty.GetHealthyMonster()); //TODO - handle setup with no healthy monsters
        enemyMonster.Setup(wildMonster); //TODO - handle setup with enemy character
        partyScreen.Init();
        dialogBox.SetMoveList(playerMonster.Monster.Moves);

        yield return dialogBox.TypeDialog($"You have encountered an enemy {enemyMonster.Monster.Base.Name}!");
        ActionSelection();
    }

    IEnumerator ExecuteTurn(BattleAction playerAction)
    {
        state = BattleState.ExecutingTurn;

        if (playerAction == BattleAction.Move)
        {
            // Get monster moves
            playerMonster.Monster.CurrentMove = playerMonster.Monster.Moves[currentMove];
            enemyMonster.Monster.CurrentMove = enemyMonster.Monster.GetRandomMove();
            int playerPriority = playerMonster.Monster.CurrentMove.Base.Priority;
            int enemyPriority = enemyMonster.Monster.CurrentMove.Base.Priority;

            // Check who goes first
            bool isPlayerFirst = true;

            if (enemyPriority > playerPriority)
                isPlayerFirst = false;
            else if (playerPriority == enemyPriority)
                isPlayerFirst = playerMonster.Monster.Speed >= enemyMonster.Monster.Speed;

            var firstMonster = (isPlayerFirst) ? playerMonster : enemyMonster;
            var secondMonster = (isPlayerFirst) ? enemyMonster : playerMonster;

            // Store incase it gets downed and switched before its move
            var lastMonster = secondMonster.Monster;

            // First turn
            yield return UseMove(firstMonster, secondMonster, firstMonster.Monster.CurrentMove);
            yield return CleanUpTurn(firstMonster);
            if (state == BattleState.BattleOver) yield break;

            // Second turn
            if (lastMonster.CurrentHp > 0)
            {
                yield return UseMove(secondMonster, firstMonster, secondMonster.Monster.CurrentMove);
                yield return CleanUpTurn(secondMonster);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else if (playerAction == BattleAction.SwitchMonster)
        {
            // Switch monster
            var selectedMember = playerParty.Monsters[currentMember];
            state = BattleState.Busy;
            yield return SwitchMonster(selectedMember);

            // Enemy turn
            var enemyMove = enemyMonster.Monster.GetRandomMove();
            yield return UseMove(enemyMonster, playerMonster, enemyMove);
            yield return CleanUpTurn(enemyMonster);
            if (state == BattleState.BattleOver) yield break;
        }
        else if (playerAction == BattleAction.UseItem)
        {
            // Use item
        }
        else if (playerAction == BattleAction.Run)
        {
            // Run
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    IEnumerator UseMove(BattleMonster attackingMonster, BattleMonster defendingMonster, Move move)
    {
        // Check for statuses like paralyze or sleep before trying to attack
        bool canAttack = attackingMonster.Monster.OnTurnBegin();

        if (!canAttack)
        {
            yield return ShowStatusChanges(attackingMonster.Monster);
            yield return attackingMonster.Hud.UpdateHP();
            yield break;
        }

        yield return ShowStatusChanges(attackingMonster.Monster);
        yield return dialogBox.TypeDialog($"{attackingMonster.Monster.Base.Name} used {move.Base.Name}!");
        move.Energy--;

        if (CheckIfMoveHits(move, attackingMonster.Monster, defendingMonster.Monster))
        {
            attackingMonster.PlayAttackAnimation();
            yield return new WaitForSeconds(0.5f);
            defendingMonster.PlayHitAnimation();

            // If status move then don't deal damage, switch to UseMoveEffects coroutine
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

            // Check for secondary move effects
            if (move.Base.MoveSecondaryEffects != null && 
                move.Base.MoveSecondaryEffects.Count > 0 &&
                attackingMonster.Monster.CurrentHp > 0)
            {
                foreach (var effect in move.Base.MoveSecondaryEffects)
                {
                    var rng = UnityEngine.Random.Range(1, 101);
                    if (rng <= effect.Chance)
                        yield return UseMoveEffects(effect, attackingMonster.Monster, defendingMonster.Monster, effect.Target);
                }
            }

            // Handle downed monster and check if we continue
            if (defendingMonster.Monster.CurrentHp <= 0)
            {
                defendingMonster.PlayDownedAnimation();
                yield return dialogBox.TypeDialog($"{defendingMonster.Monster.Base.Name} has been taken down!");
                yield return new WaitForSeconds(2f);

                CheckIfBattleIsOver(defendingMonster);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{attackingMonster.Monster.Base.Name} missed their attack!");
        }        
    }

    IEnumerator UseMoveEffects(MoveEffects effects, Monster attackingMonster, Monster defendingMonster, MoveTarget moveTarget)
    {
        // Stat changes
        if (effects.StatChanges != null)
        {
            if (moveTarget == MoveTarget.Self)
                attackingMonster.ApplyStatChanges(effects.StatChanges);
            else
                defendingMonster.ApplyStatChanges(effects.StatChanges);
        }

        // Status conditions
        if (effects.Status != ConditionID.NON)
        {
            defendingMonster.SetStatus(effects.Status);
        }

        // Volatile status conditions
        if (effects.VolatileStatus != ConditionID.NON)
        {
            defendingMonster.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(attackingMonster);
        yield return ShowStatusChanges(defendingMonster);
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("That was a critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("That attack type is very strong!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("That attack type is not very strong!");
    }

    IEnumerator ShowStatusChanges(Monster monster)
    {
        while (monster.StatusChanges.Count > 0)
        {
            var message = monster.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator SwitchMonster(Monster newMonster)
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

        state = BattleState.ExecutingTurn;
    }

    IEnumerator CleanUpTurn(BattleMonster attackingMonster)
    {
        // Skip if battle is over
        if (state == BattleState.BattleOver) yield break;
        // Wait for monster switch, etc
        yield return new WaitUntil(() => state == BattleState.ExecutingTurn);

        // Check for status changes like poison and update Monster/HUD
        attackingMonster.Monster.OnTurnOver();
        yield return ShowStatusChanges(attackingMonster.Monster);
        yield return attackingMonster.Hud.UpdateHP();

        // Attacking monster can be downed from status effects
        if (attackingMonster.Monster.CurrentHp <= 0)
        {
            attackingMonster.PlayDownedAnimation();
            yield return dialogBox.TypeDialog($"{attackingMonster.Monster.Base.Name} has been taken down!");
            yield return new WaitForSeconds(2f);

            CheckIfBattleIsOver(attackingMonster);
        }
    }

    //
    // HELPER FUNCTIONS
    //
    bool CheckIfMoveHits(Move move, Monster attackingMonster, Monster defendingMonster)
    {
        if (move.Base.AlwaysHits)
            return true;

        // Stat changes based on original game's formula
        float moveAccuracy = move.Base.Accuracy;
        int accuracy = attackingMonster.StatsChanged[MonsterStat.Accuracy];
        int evasion = defendingMonster.StatsChanged[MonsterStat.Evasion];
        var changeVals = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= changeVals[accuracy];
        else if (accuracy < 0)
            moveAccuracy /= changeVals[-accuracy];

        if (evasion > 0)
            moveAccuracy /= changeVals[evasion];
        else if (evasion < 0)
            moveAccuracy *= changeVals[evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    void CheckIfBattleIsOver(BattleMonster downedMonster)
    {
        if (downedMonster.IsPlayerMonster)
        {
            var nextMonster = playerParty.GetHealthyMonster();
            if (nextMonster != null)
                OpenPartyScreen();
            else
                BattleOver(false); // false for lost battle
        }
        else
            BattleOver(true); // true for won battle
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;

        playerParty.Monsters.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    // 
    // SELECTOR FUNCTIONS
    //
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
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;

        dialogBox.SetDialog("Select an action:");
        dialogBox.EnableActionSelector(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;

        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;

        partyScreen.SetPartyData(playerParty.Monsters);
        partyScreen.gameObject.SetActive(true);
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Items
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
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove+= 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove-= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerMonster.Monster.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerMonster.Monster.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(ExecuteTurn(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 3;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 3;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Monsters.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Monsters[currentMember];
            if (selectedMember.CurrentHp <= 0)
            {
                partyScreen.SetMessageText("That monster is downed and cannot be used!");
                return;
            }
            if (selectedMember == playerMonster.Monster)
            {
                partyScreen.SetMessageText("That monster is already being used!");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            // If player switched monster voluntarily it should count as a turn move
            // If monster was downed and forced switch then it should trigger a new turn
            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(ExecuteTurn(BattleAction.SwitchMonster));
            } else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchMonster(selectedMember));
            }  
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }
}
