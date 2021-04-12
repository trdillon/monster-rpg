using TMPro;
using UnityEngine;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Localizes text elements based on the <see cref="Language"/> set in <see cref="Localization"/>.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextLocalizer : MonoBehaviour
    {
        private TextMeshProUGUI _textField;
        public string _key;

        private void Start()
        {
            _textField = GetComponent<TextMeshProUGUI>();
            string value = Localization.GetLocalizedValue(_key);
            _textField.text = value;
        }
    }
}
