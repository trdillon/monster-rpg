using System.Collections;
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
            Debug.Log($"{_currentScene}");
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

        public IEnumerator DumpScene(string oldScene)
        {
            yield return SceneManager.UnloadSceneAsync(oldScene);
            yield return YieldHelper.EndOfFrame;
        }

        private IEnumerator LoadNextScene(string nextSceneName)
        {
            var _async = new AsyncOperation();
            _async = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
            _async.allowSceneActivation = false;
            
            // Wait for animation or something.
            
            _async.allowSceneActivation = true;
     
            while (!_async.isDone) 
            {
                yield return null;
            }
     
            Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
            if (!nextScene.IsValid())
            {
                yield break;
            }

            SceneManager.SetActiveScene(nextScene);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        }
        
        private bool IsSceneAlreadyLoaded(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
    }
}