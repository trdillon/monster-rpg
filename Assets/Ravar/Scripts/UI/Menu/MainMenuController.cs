using System.Collections.Generic;
using Itsdits.Ravar.UI.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Main Menu scene. Handles switching between submenus and calling menu functions.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Main Menu")]
        [Tooltip("GameObject that holds the Main Menu Group.")]
        [SerializeField] private GameObject _mainMenu;
        [Tooltip("List of Text elements that display on the Main Menu screen.")]
        [SerializeField] private List<TextMeshProUGUI> _mainTexts;
        
        [Header("Load Menu")]
        [Tooltip("GameObject that holds the Load Menu Group.")]
        [SerializeField] private GameObject _loadMenu;
        [Tooltip("List of Text elements that display on the Load Menu screen.")]
        [SerializeField] private List<TextMeshProUGUI> _loadTexts;
        
        [Header("Settings Menu")]
        [Tooltip("GameObject that holds the Settings Menu Group.")]
        [SerializeField] private GameObject _settingsMenu;
        [Tooltip("List of Text elements that display on the Settings screen.")]
        [SerializeField] private List<TextMeshProUGUI> _settingsTexts;
        
        [Header("Info Menu")]
        [Tooltip("GameObject that holds the Info screen Group.")]
        [SerializeField] private GameObject _infoMenu;
        [Tooltip("List of Text elements that display on the Info screen.")]
        [SerializeField] private List<TextMeshProUGUI> _infoTexts;
        
        [Header("Variables")]
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] private TMP_ColorGradient _highlightGradient;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] private TMP_ColorGradient _standardGradient;

        private List<string> _mainKeys = new List<string>();
        private List<string> _loadKeys = new List<string>();
        private List<string> _settingsKeys = new List<string>();
        private List<string> _infoKeys = new List<string>();
        
        private int _mainIndex;
        private int _loadIndex;
        private int _settingsIndex;
        private int _infoIndex;

        private MenuState _state;

        private void Start()
        {
            BuildKeyLists();
            ShowMain();
        }

        private void Update()
        {
            if (_state == MenuState.Main)
            {
                HandleInputMain();
            }
            else if (_state == MenuState.Load)
            {
                HandleInputLoad();
            }
            else if (_state == MenuState.Settings)
            {
                HandleInputSettings();
            }
            else if (_state == MenuState.Info)
            {
                HandleInputInfo();
            }
        }

        private void BuildKeyLists()
        {
            foreach (TextMeshProUGUI t in _mainTexts)
            {
                _mainKeys.Add(t.GetComponent<TextLocalizer>().Key);
            }

            foreach (TextMeshProUGUI t in _loadTexts)
            {
                _loadKeys.Add(t.GetComponent<TextLocalizer>().Key);
            }
            
            foreach (TextMeshProUGUI t in _settingsTexts)
            {
                _settingsKeys.Add(t.GetComponent<TextLocalizer>().Key);
            }
            
            foreach (TextMeshProUGUI t in _infoTexts)
            {
                _infoKeys.Add(t.GetComponent<TextLocalizer>().Key);
            }
        }
        
        private void HandleInputMain()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame || 
                Gamepad.current.dpad.down.wasPressedThisFrame)
            {
                _mainIndex += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame ||
                     Gamepad.current.dpad.up.wasPressedThisFrame)
            {
                _mainIndex -= 1;
            }
            
            _mainIndex = Mathf.Clamp(_mainIndex, 0, _mainTexts.Count - 1);
            UpdateMainSelector(_mainIndex);
            
            if (Keyboard.current.zKey.wasPressedThisFrame ||
                Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                if (_mainKeys[_mainIndex] == "UI_NEW_GAME")
                {
                    Debug.Log("It worked boss.");
                }
                else if (_mainKeys[_mainIndex] == "UI_LOAD_GAME")
                {
                    ShowLoad();
                }
                else if (_mainKeys[_mainIndex] == "UI_SETTINGS")
                {
                    ShowSettings();
                }
                else if (_mainKeys[_mainIndex] == "UI_INFO")
                {
                    ShowInfo();
                }
                else if (_mainKeys[_mainIndex] == "UI_EXIT")
                {
                    //TODO - implement an exit handler to clean up before quitting
                    Application.Quit();
                }
            }
        }

        private void HandleInputLoad()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame || 
                Gamepad.current.dpad.down.wasPressedThisFrame)
            {
                _loadIndex += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame ||
                     Gamepad.current.dpad.up.wasPressedThisFrame)
            {
                _loadIndex -= 1;
            }

            _loadIndex = Mathf.Clamp(_loadIndex, 0, _loadTexts.Count - 1);
            UpdateLoadSelector(_loadIndex);

            if (Keyboard.current.zKey.wasPressedThisFrame ||
                Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                if (_loadKeys[_loadIndex] == "UI_BACK")
                {
                    ShowMain();
                }
            }
        }

        private void HandleInputSettings()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame || 
                Gamepad.current.dpad.down.wasPressedThisFrame)
            {
                _settingsIndex += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame ||
                     Gamepad.current.dpad.up.wasPressedThisFrame)
            {
                _settingsIndex -= 1;
            }

            _settingsIndex = Mathf.Clamp(_settingsIndex, 0, _settingsTexts.Count - 1);
            UpdateSettingsSelector(_settingsIndex);

            if (Keyboard.current.zKey.wasPressedThisFrame ||
                Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                if (_settingsKeys[_settingsIndex] == "UI_BACK")
                {
                    ShowMain();
                }
            }
        }

        private void HandleInputInfo()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame || 
                Gamepad.current.dpad.down.wasPressedThisFrame)
            {
                _infoIndex += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame ||
                     Gamepad.current.dpad.up.wasPressedThisFrame)
            {
                _infoIndex -= 1;
            }

            _infoIndex = Mathf.Clamp(_infoIndex, 0, _infoTexts.Count - 1);
            UpdateInfoSelector(_infoIndex);

            if (Keyboard.current.zKey.wasPressedThisFrame ||
                Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                if (_infoKeys[_infoIndex] == "UI_BACK")
                {
                    ShowMain();
                }
            }
        }
        
        private void UpdateMainSelector(int selected)
        {
            for (var i = 0; i < _mainTexts.Count; ++i)
            {
                _mainTexts[i].colorGradientPreset = i == selected ? _highlightGradient : _standardGradient;
            }
        }
        
        private void UpdateLoadSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (var i = 0; i < _loadTexts.Count; ++i)
            {
                _loadTexts[i].colorGradientPreset = i == selected ? _highlightGradient : _standardGradient;
            }
        }
        
        private void UpdateSettingsSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (var i = 0; i < _settingsTexts.Count; ++i)
            {
                _settingsTexts[i].colorGradientPreset = i == selected ? _highlightGradient : _standardGradient;
            }
        }
        
        private void UpdateInfoSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (var i = 0; i < _infoTexts.Count; ++i)
            {
                _infoTexts[i].colorGradientPreset = i == selected ? _highlightGradient : _standardGradient;
            }
        }
        
        private void ShowMain()
        {
            _state = MenuState.Main;
            _loadMenu.SetActive(false);
            _settingsMenu.SetActive(false);
            _infoMenu.SetActive(false);
            _mainMenu.SetActive(true);
        }

        private void ShowLoad()
        {
            _state = MenuState.Load;
            _mainMenu.SetActive(false);
            _loadMenu.SetActive(true);
        }

        private void ShowSettings()
        {
            _state = MenuState.Settings;
            _mainMenu.SetActive(false);
            _settingsMenu.SetActive(true);
        }

        private void ShowInfo()
        {
            _state = MenuState.Info;
            _mainMenu.SetActive(false);
            _infoMenu.SetActive(true);
        }
    }
}