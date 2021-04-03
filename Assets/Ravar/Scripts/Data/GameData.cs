namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// Static data manager class that handles data persistence between scenes.
    /// </summary>
    public static class GameData
    {
        public static PlayerData playerData;

        /// <summary>
        /// Adds the player data to the data manager.
        /// </summary>
        /// <param name="newData">New player data to add.</param>
        public static void AddPlayerData(PlayerData newData)
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
        public static void RemovePlayerData()
        {
            playerData = null;
        }
    }
}
