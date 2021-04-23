using System.Collections;
using Itsdits.Ravar.Util;
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
            // Do initial setup
        }

        private void Start()
        {
            StartCoroutine(BootGame());
        }

        private IEnumerator BootGame()
        {
            SceneManager.LoadScene("Game.Core", LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync("UI.Dialog", LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync("UI.Menu.Main", LoadSceneMode.Additive);
            yield return YieldHelper.EndOfFrame;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("UI.Menu.Main"));
            SceneManager.UnloadSceneAsync("Game.Preload");
        }
    }
}