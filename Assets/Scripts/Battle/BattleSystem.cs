using System;
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
    [SerializeField] PartyScreen partyScreen;
    public event Action<bool> OnBattleFinish;
    MonsterParty playerParty;
    Monster wildMonster;
    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    public void StartBattle(MonsterParty playerParty, Monster wildMonster)
    {
        this.playerParty = playerParty;
        this.wildMonster = wildMonster;
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
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
        playerHUD.SetData(playerMonster.Monster);
        enemyHUD.SetData(enemyMonster.Monster);
        partyScreen.Init();
        dialogBox.SetMoveList(playerMonster.Monster.Moves);

        yield return dialogBox.TypeDialog($"You have encountered an enemy {enemyMonster.Monster.Base.Name}!");
        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        dialogBox.SetDialog("Select an action:");
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
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

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerMonster.Monster.Moves[currentMove];
        move.Energy--;
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
            yield return dialogBox.TypeDialog($"You have won the battle!");
            yield return new WaitForSeconds(2f);
            OnBattleFinish(true); // true for won battle
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
        move.Energy--;
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
            yield return new WaitForSeconds(2f);

            var nextMonster = playerParty.GetHealthyMonster();
            if (nextMonster != null)
            {
                OpenPartyScreen();
            }
            else
            {
                yield return dialogBox.TypeDialog($"You have lost the battle!");
                yield return new WaitForSeconds(2f);
                OnBattleFinish(false); // false for lost battle
            }
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

    IEnumerator SwitchMonster(Monster newMonster)
    {
        if (playerMonster.Monster.CurrentHp > 0)
        {
            yield return dialogBox.TypeDialog($"{playerMonster.Monster.Base.Name}, fall back!");
            playerMonster.PlayDownedAnimation(); //TODO - create animation for returning to party
            yield return new WaitForSeconds(2f);
        }
        playerMonster.Setup(newMonster);
        playerHUD.SetData(newMonster);
        dialogBox.SetMoveList(newMonster.Moves);
        yield return dialogBox.TypeDialog($"You have switched to {newMonster.Base.Name}!");

        StartCoroutine(PerformEnemyMove());
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
                PlayerMove();
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
            StartCoroutine(PerformPlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            PlayerAction();
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
            PlayerAction();
        }
    }
}
