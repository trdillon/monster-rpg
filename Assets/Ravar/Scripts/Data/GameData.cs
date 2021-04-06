using System.Collections.Generic;
using UnityEngine;

namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// Static data manager class that handles data persistence between scenes.
    /// </summary>
    public static class GameData
    {
        public static PlayerData playerData;
        public static List<MonsterData> partyMonsters = new List<MonsterData>();

        /// <summary>
        /// Saves the player data to the data manager.
        /// </summary>
        /// <param name="newData">New player data to add.</param>
        public static void SavePlayerData(PlayerData newData)
        {
            playerData = newData;
        }

        /// <summary>
        /// Loads the player data from the data manager to the caller.
        /// </summary>
        /// <returns>PlayerData that is saved in this instance.</returns>
        public static PlayerData LoadPlayerData()
        {
            return playerData;
        }

        /// <summary>
        /// Removes the player data from the data manager.
        /// </summary>
        public static void ClearPlayerData()
        {
            playerData = null;
        }

        /// <summary>
        /// Saves the list of monster data to the data manager.
        /// </summary>
        /// <param name="newMonsters">Monsters to add.</param>
        public static void SaveMonsterPartyData(List<MonsterData> newMonsters)
        {
            partyMonsters.Clear();
            partyMonsters.AddRange(newMonsters);

            // Put this into the filesave class.
            //var debugData = JsonHelper.ToJson(partyMonsters.ToArray(), true);
        }

        /// <summary>
        /// Loads the list of monster data to the caller.
        /// </summary>
        /// <returns></returns>
        public static List<MonsterData> LoadMonsterPartyData()
        {
            return partyMonsters;
        }
    }
}
