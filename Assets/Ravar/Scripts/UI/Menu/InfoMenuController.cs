using Itsdits.Ravar.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Menu
{
    /// <summary>
    /// Controller class for the Info Menu scene.
    /// </summary>
    public class InfoMenuController : MonoBehaviour
    {
        [Header("UI Buttons")]
        [Tooltip("Back button to return to the Main Menu.")]
        [SerializeField] private Button _backButton;

        [Tooltip("Text element that displays the current application version.")]
        [SerializeField] private TextMeshProUGUI _versionNumber;
        
        private void OnEnable()
        {
            _backButton.onClick.AddListener(ReturnToMenu);
            _versionNumber.text = Application.version;
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(ReturnToMenu);
        }

        private void ReturnToMenu()
        {
            StartCoroutine(SceneLoader.Instance.LoadScene("UI.Menu.Main"));
        }
    }
}