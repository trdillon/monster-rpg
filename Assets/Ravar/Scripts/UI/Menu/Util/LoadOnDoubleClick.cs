using System;
using Itsdits.Ravar.UI.Menu;
using TMPro;

namespace Itsdits.Ravar
{
    /// <summary>
    /// Component that attaches to buttons which access save game instances. Double clicking these buttons allow the
    /// player to load a saved game.
    /// </summary>
    public class LoadOnDoubleClick : DoubleClick
    {
        public event Action<string> OnDoubleClicked;
        
        protected override void OnDoubleClick()
        {
            string selectedGame = GetComponentInChildren<TextMeshProUGUI>().text;
            if (selectedGame == null)
            {
                return;
            }

            OnDoubleClicked?.Invoke(selectedGame);
        }
    }
}