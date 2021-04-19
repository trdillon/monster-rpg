using System.Collections;
using Itsdits.Ravar.Battle;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Levels;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Monster.Condition;
using Itsdits.Ravar.UI.Dialog;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private PlayerController _playerController;
        [Tooltip("GameObject that holds the BattleSystem component.")]
        [SerializeField] private BattleSystem _battleSystem;
        [Tooltip("The world camera that is attached to the Player GameObject.")]
        [SerializeField] private Camera _worldCamera;
        [SerializeField] private EventSystem _eventSystem;

        private BattlerController _battler;
        private GameState _state;
        private GameState _prevState;
        private string _currentSceneName;
        private string _previousSceneName;
        private int _currentSceneIndex;

        /// <summary>
        /// The current <see cref="GameState"/>.
        /// </summary>
        public GameState State => _state;
        /// <summary>
        /// The previous <see cref="GameState"/>.
        /// </summary>
        public GameState PrevState => _prevState;
        /// <summary>
        /// The name of the scene the player is currently in.
        /// </summary>
        public string CurrentSceneName => _currentSceneName;
        /// <summary>
        /// The index of the scene the player is currently in.
        /// </summary>
        public int CurrentSceneIndex => _currentSceneIndex;
        /// <summary>
        /// The current player in this game instance.
        /// </summary>
        public PlayerController CurrentPlayer => _playerController;

        private void Awake()
        {
            Instance = this;
            ConditionDB.Init();
        }

        private void Start()
        {
            GameSignals.PAUSE_GAME.AddListener(OnPause);
            GameSignals.RESUME_GAME.AddListener(OnResume);
            _battleSystem.OnBattleOver += EndBattle;
            DialogController.Instance.OnShowDialog += StartDialog;
            DialogController.Instance.OnCloseDialog += EndDialog;
            UpdateCurrentScene();
        }

        private void Update()
        {
            if (_state == GameState.World)
            {
                _playerController.HandleUpdate();
            }
            else if (_state == GameState.Battle)
            {
                _battleSystem.HandleUpdate();
            }
            else if (_state == GameState.Dialog)
            {
                DialogController.Instance.HandleUpdate();
            }
        }

        private void OnDestroy()
        {
            GameSignals.PAUSE_GAME.RemoveListener(OnPause);
            GameSignals.RESUME_GAME.RemoveListener(OnResume);
            _battleSystem.OnBattleOver -= EndBattle;
            DialogController.Instance.OnShowDialog -= StartDialog;
            DialogController.Instance.OnCloseDialog -= EndDialog;
        }

        private void OnPause(bool pause)
        {
            StartCoroutine(PauseGame());
        }
        
        private void OnResume(bool resume)
        {
            StartCoroutine(ResumeGame());
        }

        /// <summary>
        /// Starts a battle with a wild monster after Encounter collider is triggered.
        /// </summary>
        public void StartWildBattle()
        {
            _state = GameState.Battle;
            _battleSystem.gameObject.SetActive(true);
            _worldCamera.gameObject.SetActive(false);

            var playerParty = _playerController.GetComponent<MonsterParty>();
            MonsterObj wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomMonster();
            var enemyMonster = new MonsterObj(wildMonster.Base, wildMonster.Level);

            _battleSystem.StartWildBattle(playerParty, enemyMonster);
        }

        /// <summary>
        /// Starts an encounter with a character after LoS collider is triggered.
        /// </summary>
        /// <param name="battler">Character that was encountered.</param>
        public void StartCharEncounter(BattlerController battler)
        {
            _state = GameState.Cutscene;
            StartCoroutine(battler.TriggerBattle(_playerController));
        }

        /// <summary>
        /// Start a battle with enemy character.
        /// </summary>
        /// <param name="battler">Character to do battle with.</param>
        public void StartCharBattle(BattlerController battler)
        {
            _state = GameState.Battle;
            _battleSystem.gameObject.SetActive(true);
            _worldCamera.gameObject.SetActive(false);

            _battler = battler;
            var playerParty = _playerController.GetComponent<MonsterParty>();
            var battlerParty = battler.GetComponent<MonsterParty>();

            _battleSystem.StartCharBattle(playerParty, battlerParty);
        }
        
        private IEnumerator PauseGame()
        {
            _prevState = _state;
            _previousSceneName = SceneManager.GetActiveScene().name;
            _eventSystem.enabled = false;
            yield return LoadSceneAsyncWithCheck("UI.Menu.Pause");
            _state = GameState.Pause;
        }

        private IEnumerator ResumeGame()
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_previousSceneName));
            _state = _prevState;
            yield return SceneManager.UnloadSceneAsync("UI.Menu.Pause");
            _eventSystem.enabled = true;
            _previousSceneName = null;
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
                _prevState = _state;
                _state = GameState.Cutscene;
            }
            else
            {
                _state = _prevState;
            }
        }

        /// <summary>
        /// Loads the game into a different scene.
        /// </summary>
        /// <remarks>Used for changing scenes on game loading.</remarks>
        /// <param name="sceneName">Name of the scene to load.</param>
        public IEnumerator LoadScene(string sceneName)
        {
            enabled = false;
            if (_currentSceneIndex > 0)
            {
                yield return SceneManager.UnloadSceneAsync(_currentSceneIndex);
            }
            
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            enabled = true;
        }

        public IEnumerator LoadSceneAsyncWithCheck(string sceneName)
        {
            if (IsSceneLoadedAlready(sceneName))
            {
                yield break;
            }

            AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (scene.progress < 0.9f)
            {
                // Show loading bar if we want.
                yield return null;
            }

            while (!scene.isDone)
            {
                // Wait until the scene really is loaded.
                yield return null;
            }
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        /// <summary>
        /// Updates the currentSceneName and currentSceneIndex.
        /// </summary>
        public void UpdateCurrentScene()
        {
            _currentSceneName = SceneManager.GetActiveScene().name;
            _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary>
        /// Sets the GameState to World.
        /// </summary>
        /// <remarks>Used to release player from error conditions, etc.
        /// This is a debug function that usually indicates a function calling this is buggy or incomplete.</remarks>
        public void ReleasePlayer()
        {
            _state = GameState.World;
        }

        private void EndBattle(BattleResult result, bool isCharBattle)
        {
            _state = GameState.World;
            if (_battler != null && result == BattleResult.Won)
            {
                _battler.SetBattlerState(BattlerState.Defeated);
                _battler = null;
            }
            else if (_battler != null && result == BattleResult.Lost)
            {
                //TODO - handle a loss
            }
            else
            {
                //TODO - handle error
            }

            _battleSystem.gameObject.SetActive(false);
            _worldCamera.gameObject.SetActive(true);
        }
        
        private void StartDialog()
        {
            _state = GameState.Dialog;
        }

        private void EndDialog()
        {
            if (_state == GameState.Dialog)
            {
                _state = GameState.World;
            }
        }
        
        private bool IsSceneLoadedAlready(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
    }
}