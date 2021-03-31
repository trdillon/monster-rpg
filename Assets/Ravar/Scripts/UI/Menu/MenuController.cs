using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Controller class for menu input and selection.
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] MenuBox menuBox;

        private int currentMain;
        private int currentLoad;
        private int currentSetting;
        private int currentInfo;

        private MenuState state;

        public MenuState State => state;

        private void Awake()
        {
            MainSelection();
        }

        public void Update()
        {
            if (state == MenuState.Main)
            {
                HandleMainSelection();
            }
            else if (state == MenuState.Loader)
            {
                HandleLoaderSelection();
            }
            else if (state == MenuState.Settings)
            {
                HandleSettingsSelection();
            }
            else if (state == MenuState.Info)
            {
                HandleInfoSelection();
            }
        }

        private void MainSelection()
        {
            state = MenuState.Main;

            menuBox.EnableLoader(false);
            menuBox.EnableSettings(false);
            menuBox.EnableInfo(false);
            menuBox.EnableMainMenu(true);
        }

        private void LoaderSelection()
        {
            state = MenuState.Loader;

            menuBox.EnableMainMenu(false);
            menuBox.EnableLoader(true);
        }

        private void SettingsSelection()
        {
            state = MenuState.Settings;

            menuBox.EnableMainMenu(false);
            menuBox.EnableSettings(true);
        }

        private void InfoSelection()
        {
            state = MenuState.Info;

            menuBox.EnableMainMenu(false);
            menuBox.EnableInfo(true);
        }

        private void HandleMainSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                currentMain += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentMain -= 1;
            }
            
            currentMain = Mathf.Clamp(currentMain, 0, menuBox.MainTexts.Count - 1);
            menuBox.UpdateMainSelector(currentMain);
            
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (currentMain == 0)
                {
                    // New Game
                }
                else if (currentMain == 1)
                {
                    // Load Game
                    LoaderSelection();
                }
                else if (currentMain == 2)
                {
                    // Settings
                    SettingsSelection();
                }
                else if (currentMain == 3)
                {
                    // Info
                    InfoSelection();
                }
                else if (currentMain == 4)
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
                currentLoad += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentLoad -= 1;
            }

            currentLoad = Mathf.Clamp(currentLoad, 0, menuBox.LoaderTexts.Count - 1);
            menuBox.UpdateLoaderSelector(currentLoad);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (currentLoad == 0)
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
                currentSetting += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentSetting -= 1;
            }

            currentSetting = Mathf.Clamp(currentSetting, 0, menuBox.SettingsTexts.Count - 1);
            menuBox.UpdateSettingsSelector(currentSetting);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (currentSetting == 0)
                {
                    // Back
                    MainSelection();
                }
                else if (currentSetting == 1)
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
                currentInfo += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentInfo -= 1;
            }

            currentInfo = Mathf.Clamp(currentInfo, 0, menuBox.InfoTexts.Count - 1);
            menuBox.UpdateInfoSelector(currentInfo);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (currentInfo == 0)
                {
                    // Back
                    MainSelection();
                }
            }
        }
    }
}
