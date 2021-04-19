using Itsdits.Ravar.Core.Signal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Pause Menu scene.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [Header("UI Buttons")]
        [Tooltip("Button for saving the current game.")]
        [SerializeField] private Button _saveGameButton;
        [Tooltip("Button for loading a saved game.")]
        [SerializeField] private Button _loadGameButton;
        [Tooltip("Button for the settings menu.")]
        [SerializeField] private Button _settingsButton;
        [Tooltip("Button for quitting to the Main Menu.")]
        [SerializeField] private Button _mainMenuButton;
        [Tooltip("Button for returning to the game.")]
        [SerializeField] private Button _returnButton;
        [Tooltip("Button for exiting the game.")]
        [SerializeField] private Button _exitButton;

        private void Start()
        {
            _saveGameButton.onClick.AddListener(SaveGame);
            _loadGameButton.onClick.AddListener(LoadGame);
            _settingsButton.onClick.AddListener(SettingsMenu);
            _mainMenuButton.onClick.AddListener(MainMenu);
            _returnButton.onClick.AddListener(ReturnToGame);
            _exitButton.onClick.AddListener(ExitGame);
        }

        private void OnDestroy()
        {
            _saveGameButton.onClick.RemoveListener(SaveGame);
            _loadGameButton.onClick.RemoveListener(LoadGame);
            _settingsButton.onClick.RemoveListener(SettingsMenu);
            _mainMenuButton.onClick.RemoveListener(MainMenu);
            _returnButton.onClick.RemoveListener(ReturnToGame);
            _exitButton.onClick.RemoveListener(ExitGame);
        }

        private void SaveGame()
        {
            SceneManager.LoadScene("UI.Menu.Save");
        }
        
        private void LoadGame()
        {
            SceneManager.LoadScene("UI.Menu.Load");
        }

        private void SettingsMenu()
        {
            SceneManager.LoadScene("UI.Menu.Settings");
        }
        
        private void MainMenu()
        {
            //TODO - do a save check here so the player doesn't lose their progress on accident
            SceneManager.LoadScene("UI.Menu.Main");
        }

        private void ReturnToGame()
        {
            GameSignals.RESUME_GAME.Dispatch(true);
        }
        
        private void ExitGame()
        {
            //TODO - handle this nicely
            Application.Quit();
        }
        /*
           PlayerController player = GameController.Instance.CurrentPlayer;
           PlayerData playerData = player.SavePlayerData();
           List<MonsterData> partyData = player.GetComponent<MonsterParty>().SaveMonsterParty();

           GameData.SaveGameData(playerData, partyData);
           */
    }
}