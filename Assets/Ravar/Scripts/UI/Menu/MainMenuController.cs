using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Main Menu scene.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Buttons")]
        [Tooltip("Button for starting a new game.")]
        [SerializeField] private Button _newGameButton;
        [Tooltip("Button for loading a saved game.")]
        [SerializeField] private Button _loadGameButton;
        [Tooltip("Button for the settings menu.")]
        [SerializeField] private Button _settingsButton;
        [Tooltip("Button for the info screen.")]
        [SerializeField] private Button _infoButton;
        [Tooltip("Button for exiting the game.")]
        [SerializeField] private Button _exitButton;

        private void Start()
        {
            _newGameButton.onClick.AddListener(() => StartCoroutine(NewGame()));
            _loadGameButton.onClick.AddListener(LoadGame);
            _settingsButton.onClick.AddListener(SettingsMenu);
            _infoButton.onClick.AddListener(InfoMenu);
            _exitButton.onClick.AddListener(ExitGame);
        }

        private void OnDestroy()
        {
            _newGameButton.onClick.RemoveListener(()=> StartCoroutine(NewGame()));
            _loadGameButton.onClick.RemoveListener(LoadGame);
            _settingsButton.onClick.RemoveListener(SettingsMenu);
            _infoButton.onClick.RemoveListener(InfoMenu);
            _exitButton.onClick.RemoveListener(ExitGame);
        }
        
        private IEnumerator NewGame()
        {
            SceneManager.LoadScene("Game.Core", LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync("World.Fornwest.Main", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("World.Fornwest.Main"));
            SceneManager.UnloadSceneAsync("Game.Preload");
            SceneManager.UnloadSceneAsync("UI.Menu.Main");
        }

        private void LoadGame()
        {
            SceneManager.LoadScene("UI.Menu.Load");
        }

        private void SettingsMenu()
        {
            SceneManager.LoadScene("UI.Menu.Settings");
        }

        private void InfoMenu()
        {
            SceneManager.LoadScene("UI.Menu.Info");
        }

        private void ExitGame()
        {
            //TODO - handle this nicely
            Application.Quit();
        }
    }
}