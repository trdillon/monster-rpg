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
        else if (state == GameState.Menu)
        {
            // Menu controller
        }
        else if (state == GameState.Pause)
        {
            // Pause controller
        }
    }

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
}
