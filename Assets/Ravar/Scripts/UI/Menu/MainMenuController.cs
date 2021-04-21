using Itsdits.Ravar.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Main Menu scene. <seealso cref="MenuController"/>
    /// </summary>
    public class MainMenuController : MenuController
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

        private void OnEnable()
        {
            EnableSceneManagement();
            _newGameButton.onClick.AddListener(NewGame);
            _loadGameButton.onClick.AddListener(LoadGame);
            _settingsButton.onClick.AddListener(SettingsMenu);
            _infoButton.onClick.AddListener(InfoMenu);
            _exitButton.onClick.AddListener(ExitGame);
            PlayerPrefs.SetString("previousMenu", "UI.Menu.Main");
        }

        private void OnDisable()
        {
            _newGameButton.onClick.RemoveListener(NewGame);
            _loadGameButton.onClick.RemoveListener(LoadGame);
            _settingsButton.onClick.RemoveListener(SettingsMenu);
            _infoButton.onClick.RemoveListener(InfoMenu);
            _exitButton.onClick.RemoveListener(ExitGame);
        }
        
        private void NewGame()
        {
            DisableSceneManagement();
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.NewGame"));
        }

        private void LoadGame()
        {
            DisableSceneManagement();
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Load"));
        }

        private void SettingsMenu()
        {
            DisableSceneManagement();
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Settings"));
        }

        private void InfoMenu()
        {
            DisableSceneManagement();
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Info"));
        }

        private void ExitGame()
        {
            //TODO - handle this nicely
            Application.Quit();
        }
    }
}