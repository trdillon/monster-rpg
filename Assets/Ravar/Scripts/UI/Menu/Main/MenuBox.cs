using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Container class that holds references to the menu objects and screens. Also handles input and selection.
    /// </summary>
    public class MenuBox : MonoBehaviour
    {
        [Header("Main Menu")]
        [Tooltip("List of Text elements that display on the Main Menu screen.")]
        [SerializeField] List<Text> mainTexts;
        [Tooltip("GameObject that holds the Main Menu.")]
        [SerializeField] GameObject mainMenu;

        [Header("Loader Menu")]
        [Tooltip("List of Text elements that display on the Load Menu screen.")]
        [SerializeField] List<Text> loaderTexts;
        [Tooltip("GameObject that holds the Load Menu.")]
        [SerializeField] GameObject loaderMenu;

        [Header("Settings Menu")]
        [Tooltip("List of Text elements that display on the Settings screen.")]
        [SerializeField] List<Text> settingsTexts;
        [Tooltip("GameObject that holds the Settings Menu.")]
        [SerializeField] GameObject settingsMenu;

        [Header("Info Menu")]
        [Tooltip("List of Text elements that display on the Info screen.")]
        [SerializeField] List<Text> infoTexts;
        [Tooltip("GameObject that holds the Info screen.")]
        [SerializeField] GameObject infoMenu;

        [Header("Variables")]
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] Color highlightColor;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] Color standardColor;

        /// <summary>
        /// List of Text elements that display on the Main Menu screen.
        /// </summary>
        public List<Text> MainTexts => mainTexts;
        /// <summary>
        /// List of Text elements that display on the Load Menu screen.
        /// </summary>
        public List<Text> LoaderTexts => loaderTexts;
        /// <summary>
        /// List of Text elements that display on the Settings screen.
        /// </summary>
        public List<Text> SettingsTexts => settingsTexts;
        /// <summary>
        /// List of Text elements that display on the Info screen.
        /// </summary>
        public List<Text> InfoTexts => infoTexts;

        /// <summary>
        /// Handle updates to the main screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateMainSelector(int selected)
        {
            for (int i = 0; i < mainTexts.Count; ++i)
            {
                if (i == selected)
                {
                    mainTexts[i].color = highlightColor;
                }
                else
                {
                    mainTexts[i].color = standardColor;
                }
            }
        }

        /// <summary>
        /// Handle updates to the load screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateLoaderSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (int i = 0; i < loaderTexts.Count; ++i)
            {
                if (i == selected)
                {
                    loaderTexts[i].color = highlightColor;
                }
                else
                {
                    loaderTexts[i].color = standardColor;
                }
            }
        }

        /// <summary>
        /// Handle updates to the settings screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateSettingsSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (int i = 0; i < settingsTexts.Count; ++i)
            {
                if (i == selected)
                {
                    settingsTexts[i].color = highlightColor;
                }
                else
                {
                    settingsTexts[i].color = standardColor;
                }
            }
        }

        /// <summary>
        /// Handle updates to the info screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateInfoSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (int i = 0; i < infoTexts.Count; ++i)
            {
                if (i == selected)
                {
                    infoTexts[i].color = highlightColor;
                }
                else
                {
                    infoTexts[i].color = standardColor;
                }
            }
        }

        /// <summary>
        /// Enable or disable the main menu.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnableMainMenu(bool enabled)
        {
            mainMenu.SetActive(enabled);
        }

        /// <summary>
        /// Enable or disable the load menu.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnableLoader(bool enabled)
        {
            loaderMenu.SetActive(enabled);
        }

        /// <summary>
        /// Enable or disable the settings menu.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnableSettings(bool enabled)
        {
            settingsMenu.SetActive(enabled);
        }

        /// <summary>
        /// Enable or disable the info screen.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnableInfo(bool enabled)
        {
            infoMenu.SetActive(enabled);
        }
    }
}
