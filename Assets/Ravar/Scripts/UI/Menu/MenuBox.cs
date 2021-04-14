using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Container class that holds references to the menu objects and screens. Also handles input and selection.
    /// </summary>
    public class MenuBox : MonoBehaviour
    {
        [Header("Main Menu")]
        [Tooltip("List of Text elements that display on the Main Menu screen.")]
        [SerializeField] private List<TextMeshProUGUI> _mainTexts;
        [Tooltip("GameObject that holds the Main Menu.")]
        [SerializeField] private GameObject _mainMenu;

        [Header("Load Menu")]
        [Tooltip("List of Text elements that display on the Load Menu screen.")]
        [SerializeField] private List<TextMeshProUGUI> _loadTexts;
        [Tooltip("GameObject that holds the Load Menu.")]
        [SerializeField] private GameObject _loadMenu;

        [Header("Settings Menu")]
        [Tooltip("List of Text elements that display on the Settings screen.")]
        [SerializeField] private List<TextMeshProUGUI> _settingsTexts;
        [Tooltip("GameObject that holds the Settings Menu.")]
        [SerializeField] private GameObject _settingsMenu;

        [Header("Info Menu")]
        [Tooltip("List of Text elements that display on the Info screen.")]
        [SerializeField] private List<TextMeshProUGUI> _infoTexts;
        [Tooltip("GameObject that holds the Info screen.")]
        [SerializeField] private GameObject _infoMenu;

        [Header("Variables")]
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] private TMP_ColorGradient _highlightGradient;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] private TMP_ColorGradient _standardGradient;

        /// <summary>
        /// List of Text elements that display on the Main Menu screen.
        /// </summary>
        public List<TextMeshProUGUI> MainTexts => _mainTexts;
        /// <summary>
        /// List of Text elements that display on the Load Menu screen.
        /// </summary>
        public List<TextMeshProUGUI> LoadTexts => _loadTexts;
        /// <summary>
        /// List of Text elements that display on the Settings screen.
        /// </summary>
        public List<TextMeshProUGUI> SettingsTexts => _settingsTexts;
        /// <summary>
        /// List of Text elements that display on the Info screen.
        /// </summary>
        public List<TextMeshProUGUI> InfoTexts => _infoTexts;

        /// <summary>
        /// Handle updates to the main screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateMainSelector(int selected)
        {
            for (var i = 0; i < _mainTexts.Count; ++i)
            {
                _mainTexts[i].colorGradientPreset = i == selected ? _highlightGradient : _standardGradient;
            }
        }

        /// <summary>
        /// Handle updates to the load screen based on player inputs.
        /// </summary>
        /// <param name="selected">Item that is selected.</param>
        public void UpdateLoadSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (var i = 0; i < _loadTexts.Count; ++i)
            {
                _loadTexts[i].colorGradientPreset = i == selected ? _highlightGradient : _standardGradient;
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
                _settingsTexts[i].colorGradientPreset = i == selected ? _highlightGradient : _standardGradient;
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
                _infoTexts[i].colorGradientPreset = i == selected ? _highlightGradient : _standardGradient;
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
        public void EnableLoad(bool isEnabled)
        {
            _loadMenu.SetActive(isEnabled);
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
