using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Controller class for the core game scene. Controls player, camera and event functions.
    /// </summary>
    public class CoreController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Controller object for the player.")]
        [SerializeField] private PlayerController _playerController;
        [Tooltip("Camera attached to the player prefab.")]
        [SerializeField] private Camera _playerCamera;

        [Header("Event System")]
        [Tooltip("Event System for the core scene.")]
        [SerializeField] private EventSystem _eventSystem;

        private void Awake()
        {
            DisablePlayer(true);
            GameSignals.GAME_NEW.AddListener(LoadGame);
            GameSignals.GAME_LOAD.AddListener(LoadGame);
            GameSignals.GAME_PAUSE.AddListener(DisablePlayer);
            GameSignals.GAME_RESUME.AddListener(EnablePlayer);
            GameSignals.DIALOG_OPEN.AddListener(OnDialogOpen);
            GameSignals.DIALOG_CLOSE.AddListener(OnDialogClose);
            GameSignals.BATTLE_START.AddListener(OnBattleStart);
        }

        private void OnDestroy()
        {
            GameSignals.GAME_NEW.RemoveListener(LoadGame);
            GameSignals.GAME_LOAD.RemoveListener(LoadGame);
            GameSignals.GAME_PAUSE.RemoveListener(DisablePlayer);
            GameSignals.GAME_RESUME.RemoveListener(EnablePlayer);
            GameSignals.DIALOG_OPEN.RemoveListener(OnDialogOpen);
            GameSignals.DIALOG_CLOSE.RemoveListener(OnDialogClose);
            GameSignals.BATTLE_START.RemoveListener(OnBattleStart);
        }

        private void LoadGame(string sceneName)
        {
            EnablePlayer(true);
            string sceneToLoad = GameData.PlayerData.currentScene;
            string previousScene = PlayerPrefs.GetString("previousMenu");
            if (previousScene == "UI.Menu.Main")
            {
                StartCoroutine(SceneLoader.Instance.LoadScene(sceneToLoad));
            }
            else if (previousScene == "UI.Menu.Pause")
            {
                StartCoroutine(SceneLoader.Instance.UnloadScene("UI.Menu.Load"));
                GameSignals.GAME_RESUME.Dispatch(true);
            }
        }
        
        private void EnablePlayer(bool enable)
        {
            _eventSystem.enabled = true;
            _playerCamera.enabled = true;
            _playerController.GetComponent<SpriteRenderer>().enabled = true;
        }

        private void DisablePlayer(bool disable)
        {
            _eventSystem.enabled = false;
            _playerCamera.enabled = false;
            _playerController.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnDialogOpen(bool opened)
        {
            _eventSystem.enabled = false;
        }

        private void OnDialogClose(bool closed)
        {
            _eventSystem.enabled = true;
        }

        private void OnBattleStart(BattlerEncounter battler)
        {
            DisablePlayer(true);
        }
        
        private void OnBattleFinish()
        {
            EnablePlayer(true);
        }
    }
}