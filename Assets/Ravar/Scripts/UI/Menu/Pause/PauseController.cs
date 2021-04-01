using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Controller class for the in-game pause menu.
    /// </summary>
    public class PauseController : MonoBehaviour
    {
        [SerializeField] PauseBox pauseBox;

        private int currentPause;
        private int currentSave;
        private int currentLoad;
        private int currentSetting;

        private PauseState state;

        public PauseState State => state;

        public void HandleUpdate()
        {
            if (state == PauseState.Main)
            {
                HandlePauseSelection();
            }
            else if (state == PauseState.Save)
            {
                HandleSaveSelection();
            }
            else if (state == PauseState.Load)
            {
                HandleLoadSelection();
            }
            else if (state == PauseState.Settings)
            {
                HandleSettingsSelection();
            }
        }

        /// <summary>
        /// Enables or disables the PauseBox game object to show or hide the pause menu.
        /// </summary>
        public void EnablePauseBox(bool enabled)
        {
            pauseBox.gameObject.SetActive(enabled);
            pauseBox.EnablePauseMenu(enabled);
        }

        private void PauseSelection()
        {
            state = PauseState.Main;

            pauseBox.EnableSave(false);
            pauseBox.EnableLoader(false);
            pauseBox.EnableSettings(false);
            pauseBox.EnablePauseMenu(true);
        }

        private void SaveSelection()
        {
            state = PauseState.Save;

            pauseBox.EnablePauseMenu(false);
            pauseBox.EnableSave(true);
        }

        private void LoaderSelection()
        {
            state = PauseState.Load;

            pauseBox.EnablePauseMenu(false);
            pauseBox.EnableLoader(true);
        }

        private void SettingsSelection()
        {
            state = PauseState.Settings;

            pauseBox.EnablePauseMenu(false);
            pauseBox.EnableSettings(true);
        }

        private void HandlePauseSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                currentPause += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentPause -= 1;
            }

            currentPause = Mathf.Clamp(currentPause, 0, pauseBox.PauseTexts.Count - 1);
            pauseBox.UpdatePauseSelector(currentPause);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (currentPause == 0)
                {
                    // Save Game
                    SaveSelection();
                }
                else if (currentPause == 1)
                {
                    // Load Game
                    LoaderSelection();
                }
                else if (currentPause == 2)
                {
                    // Settings
                    SettingsSelection();
                }
                else if (currentPause == 3)
                {
                    // Exit to Main Menu
                    //TODO - ensure a save check/option is handled here
                }
                else if (currentPause == 4)
                {
                    // Back to game
                    GameController.Instance.PauseGame(false);
                }
            }
        }

        private void HandleSaveSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                currentSave += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentSave -= 1;
            }

            currentLoad = Mathf.Clamp(currentSave, 0, pauseBox.SaveTexts.Count - 1);
            pauseBox.UpdateSaveSelector(currentSave);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (currentSave == 0)
                {
                    // Back
                    PauseSelection();
                }
            }
        }

        private void HandleLoadSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                currentLoad += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentLoad -= 1;
            }

            currentLoad = Mathf.Clamp(currentLoad, 0, pauseBox.LoaderTexts.Count - 1);
            pauseBox.UpdateLoaderSelector(currentLoad);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (currentLoad == 0)
                {
                    // Back
                    PauseSelection();
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

            currentSetting = Mathf.Clamp(currentSetting, 0, pauseBox.SettingsTexts.Count - 1);
            pauseBox.UpdateSettingsSelector(currentSetting);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (currentSetting == 0)
                {
                    // Back
                    PauseSelection();
                }
                else if (currentSetting == 1)
                {
                    // Save
                    //TODO - implement settings save
                    PauseSelection();
                }
            }
        }
    }
}
