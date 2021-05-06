using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Data;
using UnityEngine;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Controller class for the core game scene.
    /// </summary>
    public class CoreController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Controller object for the player.")]
        [SerializeField] private PlayerController _playerController;

        private void Awake()
        {
            HidePlayer();
            GameSignals.GAME_NEW.AddListener(LoadGame);
            GameSignals.GAME_LOAD.AddListener(LoadGame);
            GameSignals.BATTLE_OPEN.AddListener(OnBattleStart);
        }

        private void OnDestroy()
        {
            GameSignals.GAME_NEW.RemoveListener(LoadGame);
            GameSignals.GAME_LOAD.RemoveListener(LoadGame);
            GameSignals.BATTLE_OPEN.RemoveListener(OnBattleStart);
        }

        private void LoadGame(string sceneName)
        {
            ShowPlayer();
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
        
        private void ShowPlayer()
        {
            _playerController.GetComponent<SpriteRenderer>().enabled = true;
        }

        private void HidePlayer()
        {
            _playerController.GetComponent<SpriteRenderer>().enabled = false;
        }

        private void OnBattleStart(BattlerEncounter battler)
        {
            HidePlayer();
        }
        
        private void OnBattleFinish()
        {
            ShowPlayer();
        }
    }
}