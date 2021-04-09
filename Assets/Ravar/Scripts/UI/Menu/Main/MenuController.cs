using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Controller class for menu input and selection.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [Tooltip("GameObject that holds the MenuBox canvas.")]
        [SerializeField] private MenuBox _menuBox;

        private int _currentMain;
        private int _currentLoad;
        private int _currentSetting;
        private int _currentInfo;

        private MenuState _state;

        /// <summary>
        /// Current state of the main menu.
        /// </summary>
        public MenuState State => _state;

        private void Awake()
        {
            MainSelection();
        }

        private void Update()
        {
            if (_state == MenuState.Main)
            {
                HandleMainSelection();
            }
            else if (_state == MenuState.Loader)
            {
                HandleLoaderSelection();
            }
            else if (_state == MenuState.Settings)
            {
                HandleSettingsSelection();
            }
            else if (_state == MenuState.Info)
            {
                HandleInfoSelection();
            }
        }

        private void MainSelection()
        {
            _state = MenuState.Main;
            _menuBox.EnableLoader(false);
            _menuBox.EnableSettings(false);
            _menuBox.EnableInfo(false);
            _menuBox.EnableMainMenu(true);
        }

        private void LoaderSelection()
        {
            _state = MenuState.Loader;
            _menuBox.EnableMainMenu(false);
            _menuBox.EnableLoader(true);
        }

        private void SettingsSelection()
        {
            _state = MenuState.Settings;
            _menuBox.EnableMainMenu(false);
            _menuBox.EnableSettings(true);
        }

        private void InfoSelection()
        {
            _state = MenuState.Info;
            _menuBox.EnableMainMenu(false);
            _menuBox.EnableInfo(true);
        }

        private void HandleMainSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _currentMain += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentMain -= 1;
            }
            
            _currentMain = Mathf.Clamp(_currentMain, 0, _menuBox.MainTexts.Count - 1);
            _menuBox.UpdateMainSelector(_currentMain);
            
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (_currentMain == 0)
                {
                    // New Game
                }
                else if (_currentMain == 1)
                {
                    // Load Game
                    LoaderSelection();
                }
                else if (_currentMain == 2)
                {
                    // Settings
                    SettingsSelection();
                }
                else if (_currentMain == 3)
                {
                    // Info
                    InfoSelection();
                }
                else if (_currentMain == 4)
                {
                    // Exit
                    //TODO - implement an exit handler to clean up before quitting
                    Application.Quit();
                }
            }
        }

        private void HandleLoaderSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _currentLoad += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentLoad -= 1;
            }

            _currentLoad = Mathf.Clamp(_currentLoad, 0, _menuBox.LoaderTexts.Count - 1);
            _menuBox.UpdateLoaderSelector(_currentLoad);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (_currentLoad == 0)
                {
                    // Back
                    MainSelection();
                }
            }
        }

        private void HandleSettingsSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _currentSetting += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentSetting -= 1;
            }

            _currentSetting = Mathf.Clamp(_currentSetting, 0, _menuBox.SettingsTexts.Count - 1);
            _menuBox.UpdateSettingsSelector(_currentSetting);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (_currentSetting == 0)
                {
                    // Back
                    MainSelection();
                }
                else if (_currentSetting == 1)
                {
                    // Save
                    //TODO - implement settings save
                    MainSelection();
                }
            }
        }

        private void HandleInfoSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _currentInfo += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentInfo -= 1;
            }

            _currentInfo = Mathf.Clamp(_currentInfo, 0, _menuBox.InfoTexts.Count - 1);
            _menuBox.UpdateInfoSelector(_currentInfo);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (_currentInfo == 0)
                {
                    // Back
                    MainSelection();
                }
            }
        }
    }
}
