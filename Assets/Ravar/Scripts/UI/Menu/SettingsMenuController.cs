using Itsdits.Ravar.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Settings Menu scene. <seealso cref="MenuController"/>
    /// </summary>
    public class SettingsMenuController : MenuController
    {
        [Header("UI Buttons")]
        [Tooltip("Back button to return to the Main Menu.")]
        [SerializeField] private Button _backButton;
        [Tooltip("Save button to save the current settings.")]
        [SerializeField] private Button _saveButton;
        
        private void OnEnable()
        {
            EnableSceneManagement();
            _backButton.onClick.AddListener(ReturnToMenu);
            _saveButton.onClick.AddListener(SaveSettings);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(ReturnToMenu);
            _saveButton.onClick.RemoveListener(SaveSettings);
        }
        
        //TODO - implement localization settings
        
        //TODO - implement audio settings

        private void SaveSettings()
        {
            // Save shit
            DisableSceneManagement();
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Main"));
        }

        private void ReturnToMenu()
        {
            DisableSceneManagement();
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Main"));
        }
    }
}