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
        [SerializeField] private List<Text> _mainTexts;
        [Tooltip("GameObject that holds the Main Menu.")]
        [SerializeField] private GameObject _mainMenu;

        [Header("Loader Menu")]
        [Tooltip("List of Text elements that display on the Load Menu screen.")]
        [SerializeField] private List<Text> _loaderTexts;
        [Tooltip("GameObject that holds the Load Menu.")]
        [SerializeField] private GameObject _loaderMenu;

        [Header("Settings Menu")]
        [Tooltip("List of Text elements that display on the Settings screen.")]
        [SerializeField] private List<Text> _settingsTexts;
        [Tooltip("GameObject that holds the Settings Menu.")]
        [SerializeField] private GameObject _settingsMenu;

        [Header("Info Menu")]
        [Tooltip("List of Text elements that display on the Info screen.")]
        [SerializeField] private List<Text> _infoTexts;
        [Tooltip("GameObject that holds the Info screen.")]
        [SerializeField] private GameObject _infoMenu;

        [Header("Variables")]
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] private Color _highlightColor;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] private Color _standardColor;

        /// <summary>
        /// List of Text elements that display on the Main Menu screen.
        /// </summary>
        public List<Text> MainTexts => _mainTexts;
        /// <summary>
        /// List of Text elements that display on the Load Menu screen.
        /// </summary>
        public List<Text> LoaderTexts => _loaderTexts;
        /// <summary>
        /// List of Text elements that display on the Settings screen.
        /// </summary>
        public List<Text> SettingsTexts => _settingsTexts;
        /// <summary>
        /// List of Text elements that display on the Info screen.
        /// </summary>
        public List<Text> InfoTexts => _infoTexts;

        /// <summary>
        /// Handle updates to the main screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateMainSelector(int selected)
        {
            for (var i = 0; i < _mainTexts.Count; ++i)
            {
                _mainTexts[i].color = i == selected ? _highlightColor : _standardColor;
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
        /// Handle updates to the info screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateInfoSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (var i = 0; i < _infoTexts.Count; ++i)
            {
                _infoTexts[i].color = i == selected ? _highlightColor : _standardColor;
            }
        }

        /// <summary>
        /// Enable or disable the main menu.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnableMainMenu(bool isEnabled)
        {
            _mainMenu.SetActive(isEnabled);
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

        /// <summary>
        /// Enable or disable the info screen.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnableInfo(bool isEnabled)
        {
            _infoMenu.SetActive(isEnabled);
        }
    }
}
