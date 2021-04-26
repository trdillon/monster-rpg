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
            DisablePlayer();
            GameSignals.GAME_NEW.AddListener(LoadGame);
            GameSignals.GAME_LOAD.AddListener(LoadGame);
            GameSignals.GAME_PAUSE.AddListener(OnPause);
            GameSignals.GAME_RESUME.AddListener(OnResume);
            GameSignals.DIALOG_OPEN.AddListener(OnDialogOpen);
            GameSignals.DIALOG_CLOSE.AddListener(OnDialogClose);
            GameSignals.BATTLE_START.AddListener(OnBattleStart);
        }

        private void OnDestroy()
        {
            GameSignals.GAME_NEW.RemoveListener(LoadGame);
            GameSignals.GAME_LOAD.RemoveListener(LoadGame);
            GameSignals.GAME_PAUSE.RemoveListener(OnPause);
            GameSignals.GAME_RESUME.RemoveListener(OnResume);
            GameSignals.DIALOG_OPEN.RemoveListener(OnDialogOpen);
            GameSignals.DIALOG_CLOSE.RemoveListener(OnDialogClose);
            GameSignals.BATTLE_START.RemoveListener(OnBattleStart);
        }

        private void LoadGame(string sceneName)
        {
            EnablePlayer();
            string sceneToLoad = GameData.PlayerData.currentScene;
            string previousScene = PlayerPrefs.GetString("previousMenu");
            if (previousScene == "UI.Menu.Main")
            {
                StartCoroutine(SceneLoader.Instance.LoadScene(sceneToLoad));
            }
            else if (previousScene == "UI.Popup.Pause")
            {
                StartCoroutine(SceneLoader.Instance.UnloadScene("UI.Menu.Load", false));
                GameSignals.GAME_RESUME.Dispatch(true);
            }
        }
        
        private void EnablePlayer()
        {
            //_eventSystem.enabled = true;
            //_playerCamera.enabled = true;
            _playerController.GetComponent<SpriteRenderer>().enabled = true;
        }

        private void DisablePlayer()
        {
            //_eventSystem.enabled = false;
            //_playerCamera.enabled = false;
            _playerController.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnPause(bool paused)
        {
            //_eventSystem.enabled = false;
        }

        private void OnResume(bool resumed)
        {
            //_eventSystem.enabled = true;
        }

        private void OnDialogOpen(DialogItem dialogItem)
        {
            //_eventSystem.enabled = false;
        }

        private void OnDialogClose(string speakerName)
        {
            //_eventSystem.enabled = true;
        }

        private void OnBattleStart(BattlerEncounter battler)
        {
            DisablePlayer();
        }
        
        private void OnBattleFinish()
        {
            EnablePlayer();
        }
    }
}