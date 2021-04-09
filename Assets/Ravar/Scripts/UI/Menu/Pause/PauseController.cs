using System.Collections.Generic;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core;
using Itsdits.Ravar.Data;
using Itsdits.Ravar.Monster;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Controller class for the in-game pause menu.
    /// </summary>
    public class PauseController : MonoBehaviour
    {
        [Tooltip("The canvas GameObject that holds the PauseBox and related UI elements.")]
        [SerializeField] private PauseBox _pauseBox;

        private int _currentPause;
        private int _currentSave;
        private int _currentLoad;
        private int _currentSetting;

        private PauseState _state;

        /// <summary>
        /// The current state of the pause menu.
        /// </summary>
        public PauseState State => _state;

        /// <summary>
        /// Handles Update lifecycle when GameState is Pause.
        /// </summary>
        public void HandleUpdate()
        {
            if (_state == PauseState.Main)
            {
                HandlePauseSelection();
            }
            else if (_state == PauseState.Save)
            {
                HandleSaveSelection();
            }
            else if (_state == PauseState.Load)
            {
                HandleLoadSelection();
            }
            else if (_state == PauseState.Settings)
            {
                HandleSettingsSelection();
            }
        }

        /// <summary>
        /// Enables or disables the PauseBox game object to show or hide the pause menu.
        /// </summary>
        public void EnablePauseBox(bool isEnabled)
        {
            _pauseBox.gameObject.SetActive(isEnabled);
            _pauseBox.EnablePauseMenu(isEnabled);
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

        private void PauseSelection()
        {
            _state = PauseState.Main;
            _pauseBox.EnableSave(false);
            _pauseBox.EnableLoader(false);
            _pauseBox.EnableSettings(false);
            _pauseBox.EnablePauseMenu(true);
        }

        private void SaveSelection()
        {
            _state = PauseState.Save;
            _pauseBox.EnablePauseMenu(false);
            _pauseBox.EnableSave(true);
        }

        private void LoaderSelection()
        {
            _state = PauseState.Load;
            _pauseBox.EnablePauseMenu(false);
            _pauseBox.EnableLoader(true);
        }

        private void SettingsSelection()
        {
            _state = PauseState.Settings;
            _pauseBox.EnablePauseMenu(false);
            _pauseBox.EnableSettings(true);
        }

        private void HandlePauseSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _currentPause += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentPause -= 1;
            }

            _currentPause = Mathf.Clamp(_currentPause, 0, _pauseBox.PauseTexts.Count - 1);
            _pauseBox.UpdatePauseSelector(_currentPause);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (_currentPause == 0)
                {
                    // Save Game
                    SaveSelection();
                }
                else if (_currentPause == 1)
                {
                    // Load Game
                    LoaderSelection();
                }
                else if (_currentPause == 2)
                {
                    // Settings
                    SettingsSelection();
                }
                else if (_currentPause == 3)
                {
                    // Exit to Main Menu
                    //TODO - ensure a save check/option is handled here
                }
                else if (_currentPause == 4)
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
                _currentSave += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentSave -= 1;
            }

            _currentLoad = Mathf.Clamp(_currentSave, 0, _pauseBox.SaveTexts.Count - 1);
            _pauseBox.UpdateSaveSelector(_currentSave);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (_currentSave == 0)
                {
                    // Back
                    PauseSelection();
                }
                else if (_currentSave == 1)
                {
                    // Save
                    SaveGame();
                }
            }
        }

        private void HandleLoadSelection()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _currentLoad += 1;
            }
            else if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _currentLoad -= 1;
            }

            _currentLoad = Mathf.Clamp(_currentLoad, 0, _pauseBox.LoaderTexts.Count - 1);
            _pauseBox.UpdateLoaderSelector(_currentLoad);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (_currentLoad == 0)
                {
                    // Back
                    PauseSelection();
                }
                else if (_currentLoad == 1)
                {
                    // Load
                    LoadGame();
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

            _currentSetting = Mathf.Clamp(_currentSetting, 0, _pauseBox.SettingsTexts.Count - 1);
            _pauseBox.UpdateSettingsSelector(_currentSetting);

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                if (_currentSetting == 0)
                {
                    // Back
                    PauseSelection();
                }
                else if (_currentSetting == 1)
                {
                    // Save
                    //TODO - implement settings save
                    PauseSelection();
                }
            }
        }
    }
}
