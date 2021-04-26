using Itsdits.Ravar.Core;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Data;
using Itsdits.Ravar.UI.Localization;
using UnityEngine;
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

        [Header("UI Elements")]
        [Tooltip("Canvas used to hold the other UI elements.")]
        [SerializeField] private Canvas _canvas;
        [Tooltip("Prefab for the save question notification pop-up.")]
        [SerializeField] private GameObject _saveAskPrefab;
        [Tooltip("Prefab for the save success notification pop-up.")]
        [SerializeField] private GameObject _saveSuccessPrefab;
        [Tooltip("Anchor for save success notification pop-up. There is where it will be displayed.")]
        [SerializeField] private GameObject _savePopupAnchor;

        private GameObject _savePopup;
        private Button _saveAskYesButton;
        private Button _saveAskNoButton;
        private Button _saveSuccessCloseButton;
        private bool _isLeavingMenu;
        
        private void OnEnable()
        {
            _saveGameButton.onClick.AddListener(SaveGame);
            _loadGameButton.onClick.AddListener(LoadGame);
            _settingsButton.onClick.AddListener(SettingsMenu);
            _mainMenuButton.onClick.AddListener(MainMenu);
            _returnButton.onClick.AddListener(ReturnToGame);
            _exitButton.onClick.AddListener(ExitGame);
            PlayerPrefs.SetString("previousMenu", "UI.Popup.Pause");
        }

        private void OnDisable()
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
            string saveId = GameData.PlayerData.id;
            GameSignals.GAME_SAVE.Dispatch(saveId);
            OpenSaveSuccessPopup();
        }
        
        private void LoadGame()
        {
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Load"));
        }

        private void SettingsMenu()
        {
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Settings"));
        }
        
        private void MainMenu()
        {
            _isLeavingMenu = true;
            OpenSaveAskPopup();
        }

        private void ReturnToGame()
        {
            GameSignals.GAME_RESUME.Dispatch(true);
        }
        
        private void ExitGame()
        {
            //TODO - handle this nicely
            Application.Quit();
        }

        private void OpenSaveAskPopup()
        {
            _canvas.GetComponent<CanvasGroup>().interactable = false;
            _savePopup = Instantiate(_saveAskPrefab, _savePopupAnchor.transform);
            AssignListenersOnSaveAskPopup();
        }

        private void AssignListenersOnSaveAskPopup()
        {
            Button[] buttons = _savePopup.GetComponentsInChildren<Button>();
            for (var i = 0; i < buttons.Length; i++)
            {
                string text = buttons[i].GetComponentInChildren<TextLocalizer>().Key;
                if (text == "UI_YES")
                {
                    _saveAskYesButton = buttons[i];
                }
                else if (text == "UI_NO")
                {
                    _saveAskNoButton = buttons[i];
                }
            }
            
            _saveAskYesButton.onClick.AddListener(HandleYes);
            _saveAskNoButton.onClick.AddListener(HandleNo);
        }

        private void HandleYes()
        {
            CloseSaveAskPopup();
            SaveGame();
        }

        private void HandleNo()
        {
            CloseSaveAskPopup();
            GameSignals.GAME_QUIT.Dispatch(true);
        }
        
        private void CloseSaveAskPopup()
        {
            _saveAskYesButton.onClick.RemoveListener(HandleYes);;
            _saveAskNoButton.onClick.RemoveListener(HandleNo);
            _saveAskYesButton = null;
            _saveAskNoButton = null;
            Destroy(_savePopup);
        }
        
        private void OpenSaveSuccessPopup()
        {
            _canvas.GetComponent<CanvasGroup>().interactable = false;
            _savePopup = Instantiate(_saveSuccessPrefab, _savePopupAnchor.transform);
            _saveSuccessCloseButton = _savePopup.GetComponentInChildren<Button>();
            _saveSuccessCloseButton.onClick.AddListener(CloseSaveSuccessPopup);
        }

        private void CloseSaveSuccessPopup()
        {
            _saveSuccessCloseButton.onClick.RemoveListener(CloseSaveSuccessPopup);
            _saveSuccessCloseButton = null;
            Destroy(_savePopup);

            if (_isLeavingMenu)
            {
                GameSignals.GAME_QUIT.Dispatch(true);
            }
            else
            {
                _canvas.GetComponent<CanvasGroup>().interactable = true;
            }
        }
    }
}