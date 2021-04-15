using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.Core
{
    /// <summary>
    /// Controller class for the preload scene that handles game setup and initialization.
    /// </summary>
    public class PreloadController : MonoBehaviour
    {
        private void Awake()
        {
            LoadSceneIfNotLoadedAlready("UI.Menu.Main");
        }
        
        private void LoadSceneIfNotLoadedAlready(string sceneName)
        {
            if (IsSceneLoadedAlready(sceneName))
            {
                return;
            }

            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        private bool IsSceneLoadedAlready(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
    }
}
