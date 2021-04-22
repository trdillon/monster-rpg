using TMPro;
using UnityEngine;

namespace Itsdits.Ravar.UI.Localization
{
    /// <summary>
    /// Localizes text elements based on the <see cref="Language"/> set in <see cref="Localizer"/>.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextLocalizer : MonoBehaviour
    {
        [Tooltip("Key string of the text to be displayed from the Locale file.")]
        [SerializeField] private string _key;
        
        private TextMeshProUGUI _textField;

        /// <summary>
        /// Get the key of the text this element displays.
        /// </summary>
        public string Key => _key;

        private void Awake()
        {
            _textField = GetComponent<TextMeshProUGUI>();
            string value = Localizer.GetLocalizedValue(_key);
            _textField.text = value;
        }

        /// <summary>
        /// Changes the key field to display a different localized string.
        /// </summary>
        /// <param name="newKey">New key to change this field to.</param>
        public void ChangeKey(string newKey)
        {
            _key = newKey;
            string value = Localizer.GetLocalizedValue(_key);
            _textField.text = value;
        }
    }
}