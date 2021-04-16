using System.Collections.Generic;
using Itsdits.Ravar.Character;
using Itsdits.Ravar.Core;
using Itsdits.Ravar.Data;
using Itsdits.Ravar.Monster;
using UnityEngine;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Pause menu. Inherits from <see cref="MenuController"/>.
    /// </summary>
    public class PauseMenuController : MenuController
    {
        protected override void HandleSelection(string selection)
        {
            if (selection == "UI_SAVE_GAME")
            {
                // Open save game scene
            }
            else if (selection == "UI_LOAD_GAME")
            {
                // Open load game scene
            }
            else if (selection == "UI_SETTINGS")
            {
                // Open settings scene
            }
            else if (selection == "UI_MAIN_MENU")
            {
                //TODO - ensure a save check/option is handled here
                // Quit to main menu after save check
            }
            else if (selection == "UI_RETURN")
            {
                //TODO - find a better way to implement this
                StartCoroutine(GameController.Instance.PauseGame(false));
            }
            else if (selection == "UI_EXIT")
            {
                //TODO - implement an exit handler to clean up before quitting
                Application.Quit();
            }
        }
        
        private void SaveGame()
        {
            PlayerController player = GameController.Instance.CurrentPlayer;
            PlayerData playerData = player.SavePlayerData();
            List<MonsterData> partyData = player.GetComponent<MonsterParty>().SaveMonsterParty();

            GameData.SaveGameData(playerData, partyData);
        }

        
    }
}