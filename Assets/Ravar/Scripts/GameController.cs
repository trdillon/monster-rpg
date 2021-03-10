using Itsdits.Ravar.Battle;
using Itsdits.Ravar.Character.Battler;
using Itsdits.Ravar.Character.Player;
using Itsdits.Ravar.Levels;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Monster.Condition;
using Itsdits.Ravar.UI;
using UnityEngine;

namespace Itsdits.Ravar
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }


        [SerializeField] PlayerController playerController;
        [SerializeField] BattleSystem battleSystem;
        [SerializeField] Camera worldCamera;

        private BattlerController battler;
        private GameState state;

        private void Awake()
        {
            Instance = this;
            ConditionDB.Init();
        }

        private void Start()
        {
            playerController.OnEncounter += StartWildBattle;
            playerController.OnLoS += StartCharEncounter;
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
        }

        private void StartWildBattle()
        {
            state = GameState.Battle;

            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);

            var playerParty = playerController.GetComponent<MonsterParty>();
            var wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomMonster();
            var enemyMonster = new MonsterObj(wildMonster.Base, wildMonster.Level);

            battleSystem.StartWildBattle(playerParty, enemyMonster);
        }

        private void StartCharEncounter(Collider2D battlerCollider)
        {
            var battler = battlerCollider.GetComponentInParent<BattlerController>();
            if (battler != null)
            {
                state = GameState.Cutscene;

                StartCoroutine(battler.TriggerBattle(playerController));
            }
        }

        /// <summary>
        /// Start a battle with enemy character.
        /// </summary>
        /// <param name="battler">Character to battle</param>
        public void StartCharBattle(BattlerController battler)
        {
            state = GameState.Battle;

            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);

            this.battler = battler;
            var playerParty = playerController.GetComponent<MonsterParty>();
            var battlerParty = battler.GetComponent<MonsterParty>();
            battleSystem.StartCharBattle(playerParty, battlerParty);
        }

        private void EndBattle(bool won)
        {
            state = GameState.World;

            if (battler != null && won == true)
            {
                battler.SetDefeated();
                battler = null;
            }

            battleSystem.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
        }
        
        private void StartDialog()
        {
            state = GameState.Dialog;
        }

        private void EndDialog()
        {
            if (state == GameState.Dialog)
            {
                state = GameState.World;
            }
        }
    }
}