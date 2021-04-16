using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Main Menu scene. Inherits from <see cref="MenuController"/>.
    /// </summary>
    public class MainMenuController : MenuController
    {
        protected override void HandleSelection(string selection)
        {
            if (selection == "UI_NEW_GAME")
            {
                StartCoroutine(NewGame());
            }
            else if (selection == "UI_LOAD_GAME")
            {
                SceneManager.LoadScene("UI.Menu.Load");
            }
            else if (selection == "UI_SETTINGS")
            {
                // Open settings scene
            }
            else if (selection == "UI_INFO")
            {
                // Open info scene
            }
            else if (selection == "UI_EXIT")
            {
                //TODO - implement an exit handler to clean up before quitting
                Application.Quit();
            }
        }
        
        private IEnumerator NewGame()
        {
            SceneManager.LoadScene("Game.Core", LoadSceneMode.Additive);
            yield return SceneManager.LoadSceneAsync("World.Fornwest.Main", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("World.Fornwest.Main"));
            SceneManager.UnloadSceneAsync("Game.Preload");
            SceneManager.UnloadSceneAsync("UI.Menu.Main");
        }
    }
}