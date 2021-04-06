using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// Static data manager class that handles data persistence and file system IO.
    /// </summary>
    public static class GameData
    {
        public static SaveData saveData;
        public static PlayerData playerData;
        public static List<MonsterData> partyMonsters = new List<MonsterData>();

        /// <summary>
        /// Saves the current saved data to file. The save file is named after the player Id.
        /// </summary>
        /// <param name="player">The current player data to save.</param>
        /// <param name="party">The player's monster party data to save.</param>
        public static void SaveGameData(PlayerData player, List<MonsterData> party)
        {
            playerData = player;
            partyMonsters.Clear();
            partyMonsters.AddRange(party);
            saveData = new SaveData(player, party);

            if (!Directory.Exists(Application.persistentDataPath + "/save/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/save/");
            }

            string SaveDataJson = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(Application.persistentDataPath + $"/save/{playerData.id}.ravar", SaveDataJson);
            Debug.Log($"Save game: {playerData.id} saved successfully.");
            //TODO - Show user feedback about the save.
        }

        /// <summary>
        /// Loads a saved game from file.
        /// </summary>
        /// <param name="playerId">The Id of the save file to load.</param>
        public static SaveData LoadGameData(string playerId)
        {
            string LoadDataJson = File.ReadAllText(Application.persistentDataPath + $"/save/{playerId}.ravar");
            JsonUtility.FromJsonOverwrite(LoadDataJson, saveData);
            playerData = saveData.playerData;
            partyMonsters.Clear();
            partyMonsters.AddRange(saveData.partyData);
            Debug.Log($"Load game: {playerData.id} loaded successfully.");
            //TODO - Show user feedback about the load.
            return saveData;
        }
    }
}