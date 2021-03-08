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
        playerController.OnEncounter += StartWildBattle;
        playerController.OnLoS += StartCharBattle;
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
    void StartWildBattle()
    {
        state = GameState.Battle;

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<MonsterParty>();
        var wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomMonster(); //TODO - refactor this, it seems dangerous
        battleSystem.StartBattle(playerParty, wildMonster);
    }

    void StartCharBattle(Collider2D battlerCollider)
    {
        var battler = battlerCollider.GetComponentInParent<BattlerController>();
        if (battler != null)
        {
            state = GameState.Cutscene;

            StartCoroutine(battler.TriggerBattle(playerController));
        }
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
    }

    //
    // MENU
    //

    //
    // PAUSE
    //
}
