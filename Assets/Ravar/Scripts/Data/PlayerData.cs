using UnityEngine;

namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// Data class that holds data for the Player such as current scene and position.
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        public string id;
        public int currentScene;
        public Vector2 currentPosition;

        public PlayerData(string newId, int scene, Vector2 position)
        {
            id = newId;
            currentScene = scene;
            currentPosition = position;
        }
    }
}
