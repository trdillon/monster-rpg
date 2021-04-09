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
        private static SaveData _saveData;
        private static PlayerData _playerData;
        private static List<MonsterData> _partyMonsters = new List<MonsterData>();

        /// <summary>
        /// Saves the current saved data to file. The save file is named after the player Id.
        /// </summary>
        /// <param name="player">The current player data to save.</param>
        /// <param name="party">The player's monster party data to save.</param>
        public static void SaveGameData(PlayerData player, List<MonsterData> party)
        {
            _playerData = player;
            _partyMonsters.Clear();
            _partyMonsters.AddRange(party);
            _saveData = new SaveData(player, party);

            if (!Directory.Exists(Application.persistentDataPath + "/save/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/save/");
            }

            string saveDataJson = JsonUtility.ToJson(_saveData, true);
            File.WriteAllText(Application.persistentDataPath + $"/save/{_playerData.id}.ravar", saveDataJson);
            //TODO - Show user feedback about the save.
            Debug.Log($"Save game: {_playerData.id} saved successfully.");
        }

        /// <summary>
        /// Loads a saved game from file.
        /// </summary>
        /// <param name="playerId">The Id of the save file to load.</param>
        public static SaveData LoadGameData(string playerId)
        {
            string loadDataJson = File.ReadAllText(Application.persistentDataPath + $"/save/{playerId}.ravar");
            JsonUtility.FromJsonOverwrite(loadDataJson, _saveData);
            _playerData = _saveData.playerData;
            _partyMonsters.Clear();
            _partyMonsters.AddRange(_saveData.partyData);
            //TODO - Show user feedback about the load.
            Debug.Log($"Load game: {_playerData.id} loaded successfully.");
            return _saveData;
        }
    }
}