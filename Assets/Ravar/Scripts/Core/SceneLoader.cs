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
            if (IsSceneAlreadyLoaded(nextScene))
            {
                yield break;
            }
            
            _currentScene = SceneManager.GetActiveScene().name;
            yield return SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
            yield return YieldHelper.EndOfFrame;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
            SceneManager.UnloadSceneAsync(_currentScene);
            _currentScene = nextScene;
        }

        /// <summary>
        /// Loads a temporary scene without unloading the current scene.
        /// </summary>
        /// <param name="tempScene">Temporary scene to load.</param>
        /// <returns>The temporary scene.</returns>
        public IEnumerator LoadSceneNoUnload(string tempScene)
        {
            if (IsSceneAlreadyLoaded(tempScene))
            {
                yield break;
            }
            
            yield return SceneManager.LoadSceneAsync(tempScene, LoadSceneMode.Additive);
            yield return YieldHelper.EndOfFrame;
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
            var activeScenes = new List<string>(_scenes.Length);
            activeScenes.AddRange(_scenes.Where(scene => SceneManager.GetSceneByName(scene).isLoaded));

            var activeWorldScenes = new List<string>(activeScenes.Count);
            activeWorldScenes.AddRange(activeScenes.Where(scene => scene.Contains("World")));

            foreach (string scene in activeWorldScenes)
            {
                yield return Instance.UnloadScene(scene);
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

        private bool IsSceneAlreadyLoaded(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
    }
}