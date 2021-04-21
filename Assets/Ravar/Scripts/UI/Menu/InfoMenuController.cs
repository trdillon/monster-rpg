using Itsdits.Ravar.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Info Menu scene. <seealso cref="MenuController"/>
    /// </summary>
    public class InfoMenuController : MenuController
    {
        [Header("UI Buttons")]
        [Tooltip("Back button to return to the Main Menu.")]
        [SerializeField] private Button _backButton;

        [Tooltip("Text element that displays the current application version.")]
        [SerializeField] private TextMeshProUGUI _versionNumber;
        
        private void OnEnable()
        {
            EnableSceneManagement();
            _backButton.onClick.AddListener(ReturnToMenu);
            _versionNumber.text = Application.version;
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(ReturnToMenu);
        }

        private void ReturnToMenu()
        {
            DisableSceneManagement();
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Main"));
        }
    }
}