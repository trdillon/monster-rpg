using System.Collections.Generic;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core;
using Itsdits.Ravar.Data;
using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Settings;
using Itsdits.Ravar.UI.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the in-game pause menu.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [Header("Pause Menu")]
        [Tooltip("GameObject that holds the Pause Menu.")]
        [SerializeField] private GameObject _pauseMenu;
        [Tooltip("List of Text elements that display on the Pause Menu screen.")]
        [SerializeField] private List<TextMeshProUGUI> _pauseTexts;
        
        [Header("Save Menu")]
        [Tooltip("GameObject that holds the Save Menu.")]
        [SerializeField] private GameObject _saveMenu;
        [Tooltip("List of Text elements that display on the Save Menu screen.")]
        [SerializeField] private List<TextMeshProUGUI> _saveTexts;

        [Header("Load Menu")]
        [Tooltip("GameObject that holds the Load Menu.")]
        [SerializeField] private GameObject _loadMenu;
        [Tooltip("List of Text elements that display on the Load Menu screen.")]
        [SerializeField] private List<TextMeshProUGUI> _loadTexts;
        
        [Header("Settings Menu")]
        [Tooltip("GameObject that holds the Settings Menu.")]
        [SerializeField] private GameObject _settingsMenu;
        [Tooltip("List of Text elements that display on the Settings Menu screen.")]
        [SerializeField] private List<TextMeshProUGUI> _settingsTexts;
        
        [Header("Variables")]
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] private TMP_ColorGradient _highlightGradient;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] private TMP_ColorGradient _standardGradient;
        
        // Each TextMeshProUGUI will have a TextLocalizer component which contains a Localization string key.
        // We will use this key to determine which menu item the player is selecting instead of an int index.
        // This way we don't break menu selection if we change the order of the text elements.
        // We can't use TextMeshProUGUI.text because it will change to localized text on TextLocalizer.Awake.
        private readonly List<string> _pauseKeys = new List<string>();
        private readonly List<string> _saveKeys = new List<string>();
        private readonly List<string> _loadKeys = new List<string>();
        private readonly List<string> _settingsKeys = new List<string>();

        private int _pauseIndex;
        private int _saveIndex;
        private int _loadIndex;
        private int _settingsIndex;

        private PauseState _state;
        private PlayerControls _controls;
        private InputAction _select;
        private InputAction _back;
        private InputAction _move;
        
        private int _parsedYInput;
        private int _parsedXInput;

        private void Start()
        {
            BuildKeyLists();
            ShowPause();
        }
        
        private void OnEnable()
        {
            _controls = new PlayerControls();
            _controls.Enable();
            _select = _controls.UI.Select;
            _back = _controls.UI.Back;
            _move = _controls.UI.Move;
            _move.performed += OnMove;
        }

        private void OnDisable()
        {
            _move.performed -= OnMove;
            _controls.Disable();
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            // Reads the composite binding input from keyboard and d-pad only.
            HandleInput(context.ReadValue<Vector2>());
        }

        private void Update()
        {
            if (_state == PauseState.Main)
            {
                HandleInputPause();
            }
            else if (_state == PauseState.Save)
            {
                HandleInputSave();
            }
            else if (_state == PauseState.Load)
            {
                HandleInputLoad();
            }
            else if (_state == PauseState.Settings)
            {
                HandleInputSettings();
            }
        }
        
        private void BuildKeyLists()
        {
            // Calling GetComponent within a bunch of loops isn't very performant, but considering we're only
            // doing it once on Start() I think we can live with it.
            foreach (TextMeshProUGUI t in _pauseTexts)
            {
                _pauseKeys.Add(t.GetComponent<TextLocalizer>().Key);
            }
            
            foreach (TextMeshProUGUI t in _saveTexts)
            {
                _saveKeys.Add(t.GetComponent<TextLocalizer>().Key);
            }
            
            foreach (TextMeshProUGUI t in _loadTexts)
            {
                _loadKeys.Add(t.GetComponent<TextLocalizer>().Key);
            }
            
            foreach (TextMeshProUGUI t in _settingsTexts)
            {
                _settingsKeys.Add(t.GetComponent<TextLocalizer>().Key);
            }
        }

        private void SaveGame()
        {
            PlayerController player = GameController.Instance.CurrentPlayer;
            PlayerData playerData = player.SavePlayerData();
            List<MonsterData> partyData = player.GetComponent<MonsterParty>().SaveMonsterParty();

            GameData.SaveGameData(playerData, partyData);
        }

        private void LoadGame()
        {
            PlayerController player = GameController.Instance.CurrentPlayer;
            var playerParty = player.GetComponent<MonsterParty>();
            //TODO - change this to the id of the save game the user selects in the UI
            SaveData saveData = GameData.LoadGameData(player.Id);
            player.LoadPlayerData(saveData.playerData);
            playerParty.LoadMonsterParty(saveData.partyData);
        }
        
        private void HandleInput(Vector2 inputVector)
        {
            // Only x or y should have a non-zero value to prevent diagonal movement.
            if (inputVector.x != 0)
            {
                inputVector.y = 0;
            }
            
            // Normalize the Vector2 because the composite mode on the input bindings can cause != 1f inputs.
            Vector2 parsedVector = inputVector.normalized; 
            _parsedYInput = Mathf.FloorToInt(parsedVector.y);
            _parsedXInput = Mathf.FloorToInt(parsedVector.x);
        }

        private void HandleInputPause()
        {
            // First we check if there is any Y input to navigate the menu.
            if (_parsedYInput == -1)
            {
                _pauseIndex += 1;
                _parsedYInput = 0;
            }
            else if (_parsedYInput == 1)
            {
                _pauseIndex -= 1;
                _parsedYInput = 0;
            }

            // Clamp to avoid index out of bounds. Then call UpdateIndex to give visual feedback to the player.
            _pauseIndex = Mathf.Clamp(_pauseIndex, 0, _pauseTexts.Count - 1);
            UpdateIndex(_pauseIndex, "pause");

            // Return if the player hasn't triggered a selection.
            if (!_select.triggered)
            {
                return;
            }

            // Check what the player has selected and then do something about it.
            if (_pauseKeys[_pauseIndex] == "UI_SAVE_GAME")
            {
                _state = PauseState.Save;
                _pauseMenu.SetActive(false);
                _saveMenu.SetActive(true);
            }
            else if (_pauseKeys[_pauseIndex] == "UI_LOAD_GAME")
            {
                _state = PauseState.Load;
                _pauseMenu.SetActive(false);
                _loadMenu.SetActive(true);
            }
            else if (_pauseKeys[_pauseIndex] == "UI_SETTINGS")
            {
                _state = PauseState.Settings;
                _pauseMenu.SetActive(false);
                _settingsMenu.SetActive(true);
            }
            else if (_pauseKeys[_pauseIndex] == "UI_MAIN_MENU")
            {
                //TODO - ensure a save check/option is handled here
            }
            else if (_pauseKeys[_pauseIndex] == "UI_RETURN")
            {
                StartCoroutine(GameController.Instance.PauseGame(false));
            }
            else if (_pauseKeys[_pauseIndex] == "UI_EXIT")
            {
                //TODO - implement an exit handler to clean up before quitting
                Application.Quit();
            }
        }

        private void HandleInputSave()
        {
            if (_parsedYInput == -1)
            {
                _saveIndex += 1;
                _parsedYInput = 0;
            }
            else if (_parsedYInput == 1)
            {
                _saveIndex -= 1;
                _parsedYInput = 0;
            }

            _loadIndex = Mathf.Clamp(_saveIndex, 0, _saveTexts.Count - 1);
            UpdateIndex(_saveIndex, "save");

            if (_back.triggered)
            {
                ShowPause();
            }
            
            if (!_select.triggered)
            {
                return;
            }
            
            if (_saveKeys[_saveIndex] == "UI_SAVE")
            {
                SaveGame();
            }
            else if (_saveKeys[_saveIndex] == "UI_BACK" || _back.triggered)
            {
                ShowPause();
            }
        }

        private void HandleInputLoad()
        {
            if (_parsedYInput == -1)
            {
                _loadIndex += 1;
                _parsedYInput = 0;
            }
            else if (_parsedYInput == 1)
            {
                _loadIndex -= 1;
                _parsedYInput = 0;
            }

            _loadIndex = Mathf.Clamp(_loadIndex, 0, _loadTexts.Count - 1);
            UpdateIndex(_loadIndex, "load");

            if (_back.triggered)
            {
                ShowPause();
            }
            
            if (!_select.triggered)
            {
                return;
            }
            
            if (_loadKeys[_loadIndex] == "UI_LOAD")
            {
                LoadGame();
            } 
            else if (_loadKeys[_loadIndex] == "UI_BACK" || _back.triggered)
            {
                ShowPause();
            }
        }

        private void HandleInputSettings()
        {
            if (_parsedYInput == -1)
            {
                _settingsIndex += 1;
                _parsedYInput = 0;
            }
            else if (_parsedYInput == 1)
            {
                _settingsIndex -= 1;
                _parsedYInput = 0;
            }

            _settingsIndex = Mathf.Clamp(_settingsIndex, 0, _settingsTexts.Count - 1);
            UpdateIndex(_settingsIndex, "settings");

            if (_back.triggered)
            {
                ShowPause();
            }
            
            if (!_select.triggered)
            {
                return;
            }
            
            if (_settingsKeys[_settingsIndex] == "UI_SAVE")
            {
                //TODO - implement settings save
                ShowPause();
            } 
            else if (_settingsKeys[_settingsIndex] == "UI_BACK" || _back.triggered)
            {
                ShowPause();
            }
        }

        private void UpdateIndex(int index, string screen)
        {
            // There should be a better way to handle this. The previous way was separate functions for each index
            // but I think because they're so closely related they should be handled in 1.
            switch (screen)
            {
                case "pause":
                    for (var i = 0; i < _pauseTexts.Count; ++i)
                    {
                        _pauseTexts[i].colorGradientPreset = i == index ? _highlightGradient : _standardGradient;
                    }
                    break;
                case "save":
                    for (var i = 0; i < _saveTexts.Count; ++i)
                    {
                        _saveTexts[i].colorGradientPreset = i == index ? _highlightGradient : _standardGradient;
                    }
                    break;
                case "load":
                    for (var i = 0; i < _loadTexts.Count; ++i)
                    {
                        _loadTexts[i].colorGradientPreset = i == index ? _highlightGradient : _standardGradient;
                    }
                    break;
                case "settings":
                    for (var i = 0; i < _settingsTexts.Count; ++i)
                    {
                        _settingsTexts[i].colorGradientPreset = i == index ? _highlightGradient : _standardGradient;
                    }
                    break;
            }
        }

        private void ShowPause()
        {
            _state = PauseState.Main;
            _saveMenu.SetActive(false);
            _loadMenu.SetActive(false);
            _settingsMenu.SetActive(false);
            _pauseMenu.SetActive(true);
        }
    }
}