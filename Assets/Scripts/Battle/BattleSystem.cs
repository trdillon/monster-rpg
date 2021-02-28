using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleMonster playerMonster;
    [SerializeField] BattleMonster enemyMonster;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleDialogBox dialogBox;
    BattleState state;
    int currentAction;
    int currentMove;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    private void Update()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    public IEnumerator SetupBattle()
    {
        playerMonster.Setup();
        enemyMonster.Setup();
        playerHUD.SetData(playerMonster.Monster);
        enemyHUD.SetData(enemyMonster.Monster);
        dialogBox.SetMoveList(playerMonster.Monster.Moves);

        yield return dialogBox.TypeDialog($"You have encountered an enemy {enemyMonster.Monster.Base.Name}!");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Select an action:"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableDialogText(false);
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove()
    {
        var move = playerMonster.Monster.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name} used {move.Base.Name}!");

        playerMonster.PlayAttackAnimation();
        yield return new WaitForSeconds(0.5f);
        enemyMonster.PlayHitAnimation();

        //TODO - fix bug where player can attack repeatedly out of turn
        var damageDetails = enemyMonster.Monster.TakeDamage(move, playerMonster.Monster);
        yield return enemyHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Downed)
        {
            enemyMonster.PlayDownedAnimation();
            yield return dialogBox.TypeDialog($"{enemyMonster.Monster.Base.Name} has been taken down!");
        }
        else
        {
            StartCoroutine(PerformEnemyMove());
        }
    }

    IEnumerator PerformEnemyMove()
    {
        state = BattleState.EnemyMove;
        var move = enemyMonster.Monster.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyMonster.Monster.Base.Name} used {move.Base.Name}!");

        enemyMonster.PlayAttackAnimation();
        yield return new WaitForSeconds(0.5f);
        playerMonster.PlayHitAnimation();

        var damageDetails = playerMonster.Monster.TakeDamage(move, enemyMonster.Monster);
        yield return playerHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Downed)
        {
            playerMonster.PlayDownedAnimation();
            yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name} has been taken down!");
        }
        else
        {
            PlayerAction();
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

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                // Run
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerMonster.Monster.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerMonster.Monster.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerMonster.Monster.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}
