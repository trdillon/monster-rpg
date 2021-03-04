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
        CheckWhoIsFaster();
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.UseMove;

        var move = playerMonster.Monster.Moves[currentMove];
        yield return UseMove(playerMonster, enemyMonster, move);

        // UseMove checks if the battle is over and sets state to BattleOver if it is
        if (state == BattleState.UseMove)
            StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.UseMove;

        var move = enemyMonster.Monster.GetRandomMove();
        yield return UseMove(enemyMonster, playerMonster, move);

        // UseMove checks if the battle is over and sets state to BattleOver if it is
        if (state == BattleState.UseMove)
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
                yield return UseMoveEffects(move, attackingMonster.Monster, defendingMonster.Monster);
            }
            else
            {
                var damageDetails = defendingMonster.Monster.TakeDamage(move, attackingMonster.Monster);
                yield return defendingMonster.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
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

    IEnumerator UseMoveEffects(Move move, Monster attackingMonster, Monster defendingMonster)
    {
        var effects = move.Base.Effects;

        // Stat changes
        if (effects.StatChanges != null)
        {
            if (move.Base.Target == MoveTarget.Self)
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
        bool isSwitchForced = true;
        if (playerMonster.Monster.CurrentHp > 0)
        {
            isSwitchForced = false;
            yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name}, fall back!");
            playerMonster.PlayDownedAnimation(); //TODO - create animation for returning to party
            yield return new WaitForSeconds(2f);
        }
        playerMonster.Setup(newMonster);
        dialogBox.SetMoveList(newMonster.Moves);
        yield return dialogBox.TypeDialog($"You have switched to {newMonster.Base.Name}!");

        // If monster was downed, reset the turn. If monster was switched, forfeit the turn.
        if (isSwitchForced)
            CheckWhoIsFaster();
        else
            StartCoroutine(EnemyMove());
    }

    //
    // HELPER FUNCTIONS
    //
    void CheckWhoIsFaster()
    {
        if (playerMonster.Monster.Speed >= enemyMonster.Monster.Speed) //TODO - randomize attacker for equal speeds
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
    }

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
            StartCoroutine(PlayerMove());
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

            state = BattleState.Busy;

            partyScreen.gameObject.SetActive(false);
            StartCoroutine(SwitchMonster(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }
}
