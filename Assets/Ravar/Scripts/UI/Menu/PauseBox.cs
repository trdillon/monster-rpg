using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Container class that holds references to the pause menu objects and screens.
    /// </summary>
    public class PauseBox : MonoBehaviour
    {
        [Header("Pause Menu")]
        [Tooltip("List of Text elements that display on the Pause Menu screen.")]
        [SerializeField] private List<Text> _pauseTexts;
        [Tooltip("GameObject that holds the Pause Menu.")]
        [SerializeField] private GameObject _pauseMenu;

        [Header("Save Menu")]
        [Tooltip("List of Text elements that display on the Save Menu screen.")]
        [SerializeField] private List<Text> _saveTexts;
        [Tooltip("GameObject that holds the Save Menu.")]
        [SerializeField] private GameObject _saveMenu;

        [Header("Loader Menu")]
        [Tooltip("List of Text elements that display on the Load Menu screen.")]
        [SerializeField] private List<Text> _loaderTexts;
        [Tooltip("GameObject that holds the Load Menu.")]
        [SerializeField] private GameObject _loaderMenu;

        [Header("Settings Menu")]
        [Tooltip("List of Text elements that display on the Settings Menu screen.")]
        [SerializeField] private List<Text> _settingsTexts;
        [Tooltip("GameObject that holds the Settings Menu.")]
        [SerializeField] private GameObject _settingsMenu;

        [Header("Variables")]
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] private Color _highlightColor;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] private Color _standardColor;

        /// <summary>
        /// List of Text elements that display on the Pause Menu screen.
        /// </summary>
        public List<Text> PauseTexts => _pauseTexts;
        /// <summary>
        /// List of Text elements that display on the Save Menu screen.
        /// </summary>
        public List<Text> SaveTexts => _saveTexts;
        /// <summary>
        /// List of Text elements that display on the Load Menu screen.
        /// </summary>
        public List<Text> LoaderTexts => _loaderTexts;
        /// <summary>
        /// List of Text elements that display on the Settings Menu screen.
        /// </summary>
        public List<Text> SettingsTexts => _settingsTexts;

        /// <summary>
        /// Handle updates to the pause screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdatePauseSelector(int selected)
        {
            for (var i = 0; i < _pauseTexts.Count; ++i)
            {
                _pauseTexts[i].color = i == selected ? _highlightColor : _standardColor;
            }
        }

        /// <summary>
        /// Handle updates to the save screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateSaveSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (var i = 0; i < _saveTexts.Count; ++i)
            {
                _saveTexts[i].color = i == selected ? _highlightColor : _standardColor;
            }
        }

        /// <summary>
        /// Handle updates to the load screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateLoaderSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (var i = 0; i < _loaderTexts.Count; ++i)
            {
                _loaderTexts[i].color = i == selected ? _highlightColor : _standardColor;
            }
        }

        /// <summary>
        /// Handle updates to the settings screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateSettingsSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (var i = 0; i < _settingsTexts.Count; ++i)
            {
                _settingsTexts[i].color = i == selected ? _highlightColor : _standardColor;
            }
        }

        /// <summary>
        /// Enable or disable the pause menu.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnablePauseMenu(bool isEnabled)
        {
            _pauseMenu.SetActive(isEnabled);
        }

        /// <summary>
        /// Enable or disable the save menu.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnableSave(bool isEnabled)
        {
            _saveMenu.SetActive(isEnabled);
        }

        /// <summary>
        /// Enable or disable the load menu.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnableLoader(bool isEnabled)
        {
            _loaderMenu.SetActive(isEnabled);
        }

        /// <summary>
        /// Enable or disable the settings menu.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnableSettings(bool isEnabled)
        {
            _settingsMenu.SetActive(isEnabled);
        }
    }
}
