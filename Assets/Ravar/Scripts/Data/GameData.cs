using System.Collections.Generic;
using System.IO;
using Itsdits.Ravar.Core.Signal;
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
        /// Exposes the player data for the current game instance.
        /// </summary>
        public static PlayerData PlayerData => _playerData;

        /// <summary>
        /// Exposes the player's monster party data for the current game instance.
        /// </summary>
        public static IEnumerable<MonsterData> MonsterData => _partyMonsters;
        
        /// <summary>
        /// Saves the current game data.
        /// </summary>
        /// <param name="player">Current player to save.</param>
        /// <param name="party">Current player's monster party.</param>
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
            Debug.Log($"Save game: {_playerData.id} saved successfully.");
        }
        
        /// <summary>
        /// Loads the game data from a saved game.
        /// </summary>
        /// <param name="saveGameId">Id of the saved game to load.</param>
        /// <returns>SaveData of the game being loaded.</returns>
        public static void LoadGameData(string saveGameId)
        {
            string loadDataJson = File.ReadAllText(Application.persistentDataPath + $"/save/{saveGameId}.ravar");
            if (_saveData == null)
            {
                _saveData = JsonUtility.FromJson<SaveData>(loadDataJson);
            }
            else
            {
                JsonUtility.FromJsonOverwrite(loadDataJson, _saveData);
            }
            
            _playerData = _saveData.playerData;
            _partyMonsters.Clear();
            _partyMonsters.AddRange(_saveData.partyData);
            GameSignals.LOAD_GAME.Dispatch(saveGameId);
            Debug.Log($"Load game: {_playerData.id} loaded successfully.");
        }
    }
}