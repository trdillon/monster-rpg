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
        [SerializeField] DialogController dialogController;
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
            if (playerParty == null)
            {
                Debug.LogError("GC001: Player party null. Failed to get party from playerController.");
            }

            var wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomMonster();
            if (wildMonster == null)
            {
                Debug.LogError("GC002: wildMonster null. Failed to GetRandomMonster from MapArea.");
            }

            var enemyMonster = new MonsterObj(wildMonster.Base, wildMonster.Level);

            battleSystem.StartWildBattle(playerParty, enemyMonster);
        }

        private void StartCharEncounter(Collider2D battlerCollider)
        {
            var battler = battlerCollider.GetComponentInParent<BattlerController>();
            if (battler == null)
            {
                Debug.LogError($"GC003: battlerCollider encountered, but failed to get BattlerController from it. Escaping battle sequence to attempt recovery.");
            }
            else
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
            if (playerParty == null || battlerParty == null)
            {
                Debug.LogError("GC004: MonsterParty null. Failed to get party during StartCharBattle.");
            }
            else
            {
                battleSystem.StartCharBattle(playerParty, battlerParty);
            }
        }

        /// <summary>
        /// Sets the GameState to World, used to release player from error conditions, etc.
        /// </summary>
        public void ReleasePlayer()
        {
            state = GameState.World;
        }

        private void EndBattle(BattleResult result, bool isCharBattle)
        {
            state = GameState.World;

            if (isCharBattle && battler == null)
            {
                Debug.LogError("GC005: EndBattle called but battler was null.");
            }

            if (battler != null && result == BattleResult.Won)
            {
                battler.SetBattlerState(BattlerState.Defeated);
                battler = null;
            }
            else if (battler != null && result == BattleResult.Lost)
            {
                //TODO - handle a loss
            }
            else
            {
                //TODO - handle error
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