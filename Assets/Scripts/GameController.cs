using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    GameState state;

    private void Awake()
    {
        ConditionDB.Init();
    }

    private void Start()
    {
        playerController.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        DialogController.Instance.OnShowDialog += StartDialog;
        DialogController.Instance.OnCloseDialog += EndDialog;
    }

    private void Update()
    {
        if (state == GameState.World)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogController.Instance.HandleUpdate();
        }
        else if (state == GameState.Menu)
        {
            // Menu controller
        }
        else if (state == GameState.Pause)
        {
            // Pause controller
        }
    }

    //
    // BATTLE
    //
    void StartBattle()
    {
        state = GameState.Battle;

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<MonsterParty>();
        var wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomMonster();

        battleSystem.StartBattle(playerParty, wildMonster);
    }

    void EndBattle(bool won)
    {
        state = GameState.World;

        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    //
    // DIALOG
    //
    void StartDialog()
    {
        state = GameState.Dialog;
    }

    void EndDialog()
    {
        if (state == GameState.Dialog)
            state = GameState.World;
        //TODO - handle character dialog segue to battle
    }

    //
    // MENU
    //

    //
    // PAUSE
    //
}
