using Itsdits.Ravar.Battle;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Levels;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Static controller for game state management.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        /// <summary>
        /// Static instance of the game controller.
        /// </summary>
        public static GameController Instance { get; private set; }

        [Tooltip("GameObject that holds the PlayerController component.")]
        [SerializeField] PlayerController playerController;
        [Tooltip("GameObject that holds the DialogController component.")]
        [SerializeField] DialogController dialogController;
        [Tooltip("GameObject that holds the PauseController component.")]
        [SerializeField] PauseController pauseController;
        [Tooltip("GameObject that holds the BattleSystem component.")]
        [SerializeField] BattleSystem battleSystem;
        [Tooltip("The world camera that is attached to the Player GameObject.")]
        [SerializeField] Camera worldCamera;

        private BattlerController battler;
        private GameState state;
        private GameState prevState;
        private int currentScene;

        /// <summary>
        /// The current <see cref="GameState"/>.
        /// </summary>
        public GameState State => state;
        /// <summary>
        /// The previous <see cref="GameState"/>.
        /// </summary>
        public GameState PrevState => prevState;
        /// <summary>
        /// The index of the scene the player is currently in.
        /// </summary>
        public int CurrentScene => currentScene;
        /// <summary>
        /// The current player in this game instance.
        /// </summary>
        public PlayerController CurrentPlayer => playerController;

        private void Awake()
        {
            Instance = this;
            ConditionDB.Init();
        }

        private void Start()
        {
            battleSystem.OnBattleOver += EndBattle;
            DialogController.Instance.OnShowDialog += StartDialog;
            DialogController.Instance.OnCloseDialog += EndDialog;
            UpdateCurrentScene();
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
            else if (state == GameState.Pause)
            {
                pauseController.HandleUpdate();
            }
        }

        /// <summary>
        /// Starts a battle with a wild monster after Encounter collider is triggered.
        /// </summary>
        public void StartWildBattle()
        {
            state = GameState.Battle;

            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);

            var playerParty = playerController.GetComponent<MonsterParty>();
            var wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomMonster();
            var enemyMonster = new MonsterObj(wildMonster.Base, wildMonster.Level);

            battleSystem.StartWildBattle(playerParty, enemyMonster);
        }

        /// <summary>
        /// Starts an encounter with a character after LoS collider is triggered.
        /// </summary>
        /// <param name="battlerCollider">Character that was encountered.</param>
        public void StartCharEncounter(BattlerController battler)
        {
                state = GameState.Cutscene;

                StartCoroutine(battler.TriggerBattle(playerController));
        }

        /// <summary>
        /// Start a battle with enemy character.
        /// </summary>
        /// <param name="battler">Character to do battle with.</param>
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

        /// <summary>
        /// Pause and unpause the game.
        /// </summary>
        /// <param name="pause">True for pause, false for unpause.</param>
        public void PauseGame(bool pause)
        {
            if (pause)
            {
                prevState = state;
                pauseController.EnablePauseBox(true);
                state = GameState.Pause;
            }
            else
            {
                pauseController.EnablePauseBox(false);
                state = prevState;
            }
        }

        /// <summary>
        /// Stops the character and prevents player input.
        /// </summary>
        /// <remarks>Used in scene switching and cutscenes.</remarks>
        /// <param name="frozen">True for freeze, false for unfreeze.</param>
        public void FreezePlayer(bool frozen)
        {
            if (frozen)
            {
                prevState = state;
                state = GameState.Cutscene;
            }
            else
            {
                state = prevState;
            }
        }

        /// <summary>
        /// Loads the game into a different scene.
        /// </summary>
        /// <remarks>Used for changing scenes on game loading.</remarks>
        /// <param name="sceneIndex"></param>
        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadSceneAsync(sceneIndex);
        }

        /// <summary>
        /// Updates the currentScene index.
        /// </summary>
        public void UpdateCurrentScene()
        {
            currentScene = SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary>
        /// Sets the GameState to World.
        /// </summary>
        /// <remarks>Used to release player from error conditions, etc.
        /// This is a debug function that usually indicates a function calling this is buggy or incomplete.</remarks>
        public void ReleasePlayer()
        {
            state = GameState.World;
        }

        private void EndBattle(BattleResult result, bool isCharBattle)
        {
            state = GameState.World;

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