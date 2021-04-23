using System.Collections.Generic;
using System.IO;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Monster;
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
        /// Creates a new save game data.
        /// </summary>
        /// <param name="playerName">New player's name.</param>
        /// <param name="monsterChoice">New player's choice of starting monster.</param>
        public static void NewGameData(string playerName, string monsterChoice)
        {
            // Create a new player and place it in the starting spot.
            _playerData = new PlayerData(playerName, "World.Fornwest.Main", new[] {1, 1});
            
            // Create the starter monster and add it to the party.
            var monsterBase = Resources.Load<MonsterBase>($"Monsters/{monsterChoice}");
            var starterMonster = new MonsterObj(monsterBase, 5);
            _partyMonsters.Clear();
            _partyMonsters.Add(starterMonster.SaveMonsterData());
            
            // Data is ready so now we signal the new game.
            GameSignals.GAME_NEW.Dispatch(_playerData.id);
            Debug.Log($"New game: {_playerData.id} created.");
        }
        
        /// <summary>
        /// Saves the current game data.
        /// </summary>
        /// <param name="player">Current player to save.</param>
        /// <param name="party">Current player's monster party.</param>
        public static void SaveGameData(PlayerData player, List<MonsterData> party)
        {
            // Save the data fields locally.
            _playerData = player;
            _partyMonsters.Clear();
            _partyMonsters.AddRange(party);
            
            // Package the data.
            _saveData = new SaveData(player, party);
            
            // Create save directory if one doesn't already exist.
            if (!Directory.Exists(Application.persistentDataPath + "/save/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/save/");
            }

            // Save the data to file.
            string saveDataJson = JsonUtility.ToJson(_saveData, true);
            File.WriteAllText(Application.persistentDataPath + $"/save/{_playerData.id}.ravar", saveDataJson);
        }
        
        /// <summary>
        /// Loads the game data from a saved game.
        /// </summary>
        /// <param name="saveGameId">Id of the saved game to load.</param>
        /// <returns>SaveData of the game being loaded.</returns>
        public static void LoadGameData(string saveGameId)
        {
            // Read in the data from file.
            string loadDataJson = File.ReadAllText(Application.persistentDataPath + $"/save/{saveGameId}.ravar");
            
            // FromJsonOverwrite is cheaper so we do that if we can.
            if (_saveData == null)
            {
                _saveData = JsonUtility.FromJson<SaveData>(loadDataJson);
            }
            else
            {
                JsonUtility.FromJsonOverwrite(loadDataJson, _saveData);
            }
            
            // Load the data locally.
            _playerData = _saveData.playerData;
            _partyMonsters.Clear();
            _partyMonsters.AddRange(_saveData.partyData);
            
            // Data is ready so now we signal the load.
            GameSignals.GAME_LOAD.Dispatch(saveGameId);
        }
    }
}