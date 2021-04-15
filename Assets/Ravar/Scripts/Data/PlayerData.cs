// ReSharper disable InconsistentNaming
namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// Data class that holds data for the Player such as current scene and position.
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        public string id;
        public string currentScene;
        public int[] currentPosition;

        /// <summary>
        /// Constructor for a PlayerData data object.
        /// </summary>
        /// <param name="newId">ID of the player in this instance.</param>
        /// <param name="scene">Current scene the player is in.</param>
        /// <param name="position">Current position the player is in.</param>
        public PlayerData(string newId, string scene, int[] position)
        {
            id = newId;
            currentScene = scene;
            currentPosition = position;
        }
    }
}
