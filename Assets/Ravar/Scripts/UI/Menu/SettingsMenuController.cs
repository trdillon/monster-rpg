using Itsdits.Ravar.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Settings Menu scene.
    /// </summary>
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("UI Buttons")]
        [Tooltip("Back button to return to the Main Menu.")]
        [SerializeField] private Button _backButton;
        [Tooltip("Save button to save the current settings.")]
        [SerializeField] private Button _saveButton;
        
        private void OnEnable()
        {
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
            // Save settings
            string previousScene = PlayerPrefs.GetString("previousMenu");
            StartCoroutine(SceneLoader.Instance.LoadScene(previousScene == "UI.Popup.Pause" ? 
                                                              "UI.Popup.Pause" : "UI.Menu.Main"));
        }

        private void ReturnToMenu()
        {
            string previousScene = PlayerPrefs.GetString("previousMenu");
            StartCoroutine(SceneLoader.Instance.LoadScene(previousScene == "UI.Popup.Pause" ? 
                                                              "UI.Popup.Pause" : "UI.Menu.Main"));
        }
    }
}