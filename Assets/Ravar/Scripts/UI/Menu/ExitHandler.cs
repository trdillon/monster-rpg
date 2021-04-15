using UnityEngine;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Handler class for the Exit button.
    /// </summary>
    public class ExitHandler
    {
        /// <summary>
        /// Exits the game.
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
