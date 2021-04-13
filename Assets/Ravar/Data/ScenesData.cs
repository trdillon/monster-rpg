using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// Database of <see cref="GameScene"/> classes.
    /// </summary>
    [CreateAssetMenu(fileName = "sceneDB", menuName = "Scene Data/Database")]
    public class ScenesData : ScriptableObject
    {
        [Header("Database Contents")]
        [Tooltip("List of Level scenes in the database.")]
        [SerializeField] private List<Level> _levels = new List<Level>();
        [Tooltip("List of Menu scenes in the database.")]
        [SerializeField] private List<Menu> _menus = new List<Menu>();
        [Tooltip("The index of the current scene the player is in.")]
        [SerializeField] private string _currentScene;

        /// <summary>
        /// Starts a new game.
        /// </summary>
        public void NewGame()
        {
            SceneManager.LoadScene("Game.Core");
            SceneManager.LoadSceneAsync("World.Fornwest.Main", LoadSceneMode.Additive);
        }
        
        /// <summary>
        /// Loads a <see cref="Level"/> scene by name.
        /// </summary>
        /// <param name="levelName">Name of the level scene to load.</param>
        public void LoadLevelByName(string levelName)
        {
            if (!IsSceneLoaded("Game.Core"))
            {
                SceneManager.LoadSceneAsync("Game.Core");
            }

            SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Loads the Main <see cref="Menu"/>.
        /// </summary>
        public void LoadMainMenu()
        {
            SceneManager.LoadSceneAsync("UI.Menu.Main");
        }

        /// <summary>
        /// Loads the Pause <see cref="Menu"/>.
        /// </summary>
        public void LoadPauseMenu()
        {
            SceneManager.LoadSceneAsync("UI.Menu.Pause", LoadSceneMode.Additive);
        }
        
        private bool IsSceneLoaded(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
    }
}
