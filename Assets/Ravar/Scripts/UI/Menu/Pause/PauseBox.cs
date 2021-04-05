using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Container class that holds references to the pause menu objects and screens.
    /// </summary>
    public class PauseBox : MonoBehaviour
    {
        [Header("Pause Menu")]
        [Tooltip("List of Text elements that display on the Pause Menu screen.")]
        [SerializeField] List<Text> pauseTexts;
        [Tooltip("GameObject that holds the Pause Menu.")]
        [SerializeField] GameObject pauseMenu;

        [Header("Save Menu")]
        [Tooltip("List of Text elements that display on the Save Menu screen.")]
        [SerializeField] List<Text> saveTexts;
        [Tooltip("GameObject that holds the Save Menu.")]
        [SerializeField] GameObject saveMenu;

        [Header("Loader Menu")]
        [Tooltip("List of Text elements that display on the Load Menu screen.")]
        [SerializeField] List<Text> loaderTexts;
        [Tooltip("GameObject that holds the Load Menu.")]
        [SerializeField] GameObject loaderMenu;

        [Header("Settings Menu")]
        [Tooltip("List of Text elements that display on the Settings Menu screen.")]
        [SerializeField] List<Text> settingsTexts;
        [Tooltip("GameObject that holds the Settings Menu.")]
        [SerializeField] GameObject settingsMenu;

        [Header("Variables")]
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] Color highlightColor;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] Color standardColor;

        /// <summary>
        /// List of Text elements that display on the Pause Menu screen.
        /// </summary>
        public List<Text> PauseTexts => pauseTexts;
        /// <summary>
        /// List of Text elements that display on the Save Menu screen.
        /// </summary>
        public List<Text> SaveTexts => saveTexts;
        /// <summary>
        /// List of Text elements that display on the Load Menu screen.
        /// </summary>
        public List<Text> LoaderTexts => loaderTexts;
        /// <summary>
        /// List of Text elements that display on the Settings Menu screen.
        /// </summary>
        public List<Text> SettingsTexts => settingsTexts;

        /// <summary>
        /// Handle updates to the pause screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdatePauseSelector(int selected)
        {
            for (int i = 0; i < pauseTexts.Count; ++i)
            {
                if (i == selected)
                {
                    pauseTexts[i].color = highlightColor;
                }
                else
                {
                    pauseTexts[i].color = standardColor;
                }
            }
        }

        /// <summary>
        /// Handle updates to the save screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateSaveSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (int i = 0; i < saveTexts.Count; ++i)
            {
                if (i == selected)
                {
                    saveTexts[i].color = highlightColor;
                }
                else
                {
                    saveTexts[i].color = standardColor;
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
        /// Enable or disable the pause menu.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnablePauseMenu(bool enabled)
        {
            pauseMenu.SetActive(enabled);
        }

        /// <summary>
        /// Enable or disable the save menu.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnableSave(bool enabled)
        {
            saveMenu.SetActive(enabled);
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
    }
}
