using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itsdits.Ravar.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Singleton class that handles scene loading.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// Static instance of the Scene Loader.
        /// </summary>
        public static SceneLoader Instance { get; private set; }

        private string[] _scenes;
        private string _currentScene;
        private string _currentWorldScene;

        /// <summary>
        /// Exposes the current World.[Level] scene it can be saved with the player data.
        /// </summary>
        public string CurrentWorldScene => _currentWorldScene;

        private void Awake() 
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            } 
            else 
            {
                Instance = this;
            }

            BuildSceneList();
        }

        /// <summary>
        /// Loads the next scene additively and then unloads the current scene.
        /// </summary>
        /// <param name="nextScene">Next scene to load.</param>
        /// <returns>The next scene.</returns>
        public IEnumerator LoadScene(string nextScene)
        {
            // Don't load multiple instances of the same scene.
            if (IsSceneAlreadyLoaded(nextScene))
            {
                yield break;
            }
            
            // Update the current scene tracker.
            _currentScene = SceneManager.GetActiveScene().name;

            // Load the next scene then wait for a frame to set it active.
            yield return SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
            yield return YieldHelper.EndOfFrame;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
            
            // Update the current world scene in case the player has changed levels.
            UpdateCurrentWorldScene();

            // Unload the previous scene and update the current scene tracker.
            SceneManager.UnloadSceneAsync(_currentScene);
            _currentScene = nextScene;
        }

        /// <summary>
        /// Loads a temporary scene without unloading the current scene.
        /// </summary>
        /// <param name="tempScene">Temporary scene to load.</param>
        /// <param name="setActive">Set the temporary scene as the active scene or not.</param>
        /// <returns>The temporary scene.</returns>
        public IEnumerator LoadSceneNoUnload(string tempScene, bool setActive)
        {
            // Don't load multiple instances of the same scene.
            if (IsSceneAlreadyLoaded(tempScene))
            {
                yield break;
            }
            
            // Update the current world scene because the player might quit the game from the temp scene.
            UpdateCurrentWorldScene();
            
            // Load the temp scene and wait a frame for it to finish.
            yield return SceneManager.LoadSceneAsync(tempScene, LoadSceneMode.Additive);
            yield return YieldHelper.EndOfFrame;

            // If the temp scene shouldn't be the active scene then break, otherwise set it active.
            if (!setActive)
            {
                yield break;
            }
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(tempScene));
        }

        /// <summary>
        /// Unloads a scene that is no longer needed.
        /// </summary>
        /// <param name="oldScene">Scene to unload.</param>
        /// <returns></returns>
        public IEnumerator UnloadScene(string oldScene)
        {
            yield return SceneManager.UnloadSceneAsync(oldScene);
            yield return YieldHelper.EndOfFrame;
        }

        /// <summary>
        /// Finds and unloads the current World scenes in the game.
        /// </summary>
        /// <returns></returns>
        public IEnumerator UnloadWorldScenes()
        {
            // Build a list of the currently loaded scenes.
            var activeScenes = new List<string>(_scenes.Length);
            activeScenes.AddRange(_scenes.Where(scene => SceneManager.GetSceneByName(scene).isLoaded));

            // Check if the loaded scenes have World in their names.
            var activeWorldScenes = new List<string>(activeScenes.Count);
            activeWorldScenes.AddRange(activeScenes.Where(scene => scene.Contains("World")));

            // Traverse the list of scenes and unload them all. Usually this should only be one scene but we want to
            // be safe and make sure we didn't miss anything.
            for (var i = 0; i < activeWorldScenes.Count; i++)
            {
                yield return Instance.UnloadScene(activeWorldScenes[i]);
            }
        }

        private void BuildSceneList()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            _scenes = new string[sceneCount];
            for (var i = 0; i < sceneCount; i++)
            {
                // For some reason using SceneManager.GetSceneByBuildIndex().name was returning null strings for scenes
                // that had not been loaded yet. So we use this work around to get all the scene names.
                string pathToScene = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = Path.GetFileNameWithoutExtension(pathToScene);
                _scenes[i] = sceneName;
            }
        }

        private void UpdateCurrentWorldScene()
        {
            // This should be called after loading a new scene to check if the player has changed levels. We track this
            // so we know what level the player is in when they save the game.
            string scene = SceneManager.GetActiveScene().name;
            if (!scene.Contains("World"))
            {
                return;
            }

            _currentWorldScene = scene;
        }

        private bool IsSceneAlreadyLoaded(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
    }
}