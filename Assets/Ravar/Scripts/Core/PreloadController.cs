using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar
{
    public class PreloadController : MonoBehaviour
    {
        private List<string> _sceneList = new List<string>();
        private void Awake()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneByBuildIndex(i);
                _sceneList.Add(scene.name);
            }
            
            LoadSceneIfNotLoadedAlready("UI.Menu.Main");
            LoadSceneIfNotLoadedAlready("World.Fornwest.Main");
        }

        public static void LoadSceneIfNotLoadedAlready(string sceneName)
        {
            if (IsSceneLoadedAlready(sceneName))
            {
                return;
            }

            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        private static bool IsSceneLoadedAlready(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
    }
}
