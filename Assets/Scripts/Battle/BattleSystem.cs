using System;
using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleMonster playerMonster;
    [SerializeField] BattleMonster enemyMonster;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;

    BattleState state;
    MonsterParty playerParty;
    Monster wildMonster;
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

    public IEnumerator SetupBattle()
    {
        playerMonster.Setup(playerParty.GetHealthyMonster()); //TODO - handle setup with no healthy monsters
        enemyMonster.Setup(wildMonster);
        partyScreen.Init();
        dialogBox.SetMoveList(playerMonster.Monster.Moves);

        yield return dialogBox.TypeDialog($"You have encountered an enemy {enemyMonster.Monster.Base.Name}!");
        CheckWhoIsFaster();
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

    IEnumerator PlayerMove()
    {
        state = BattleState.UseMove;

        var move = playerMonster.Monster.Moves[currentMove];
        yield return UseMove(playerMonster, enemyMonster, move);

        if (state == BattleState.UseMove)
            StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.UseMove;

        var move = enemyMonster.Monster.GetRandomMove();
        yield return UseMove(enemyMonster, playerMonster, move);

        if (state == BattleState.UseMove)
            ActionSelection();
    }

    IEnumerator UseMove(BattleMonster attackingMonster, BattleMonster defendingMonster, Move move)
    {
        move.Energy--;
        yield return dialogBox.TypeDialog($"{attackingMonster.Monster.Base.Name} used {move.Base.Name}!");

        attackingMonster.PlayAttackAnimation();
        yield return new WaitForSeconds(0.5f);
        defendingMonster.PlayHitAnimation();

        if (move.Base.Category == MoveCategory.Status)
        {
            var effects = move.Base.Effects;
            if (effects.StatChanges != null)
            {
                if (move.Base.Target == MoveTarget.Self)
                    attackingMonster.Monster.ApplyStatChanges(effects.StatChanges);
                else
                    defendingMonster.Monster.ApplyStatChanges(effects.StatChanges);
            }

            yield return ShowStatusChanges(attackingMonster.Monster);
            yield return ShowStatusChanges(defendingMonster.Monster);

        }
        else
        {
            var damageDetails = defendingMonster.Monster.TakeDamage(move, attackingMonster.Monster);
            yield return defendingMonster.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        if (defendingMonster.Monster.CurrentHp <= 0)
        {
            defendingMonster.PlayDownedAnimation();
            yield return dialogBox.TypeDialog($"{defendingMonster.Monster.Base.Name} has been taken down!");
            yield return new WaitForSeconds(2f);

            CheckIfBattleIsOver(defendingMonster);
        }
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

        if (isSwitchForced)
            CheckWhoIsFaster();
        else
            StartCoroutine(EnemyMove());
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

    void CheckWhoIsFaster()
    {
        if (playerMonster.Monster.Speed >= enemyMonster.Monster.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyMove());   
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;

        playerParty.Monsters.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
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
