using Itsdits.Ravar.Data;
using System.Collections.Generic;

namespace Itsdits.Ravar
{
    /// <summary>
    /// Data container that holds the various data objects that need to be saved.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public PlayerData playerData;
        public List<MonsterData> partyData;

        /// <summary>
        /// Constructor for a SaveData object that encapsulates the various data objects
        /// that make up a save game.
        /// </summary>
        /// <param name="player">Data about the current player.</param>
        /// <param name="party">Data about the current player's monster party.</param>
        public SaveData(PlayerData player, List<MonsterData> party)
        {
            playerData = player;
            partyData = party;
        }
    }
}
