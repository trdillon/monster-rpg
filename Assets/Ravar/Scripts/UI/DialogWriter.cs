using System.Collections;
using Itsdits.Ravar.Util;
using TMPro;
using UnityEngine;

namespace Itsdits.Ravar.UI
{
    public class DialogWriter : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshPro;

        private void Start()
        {
            _textMeshPro = gameObject.GetComponent<TextMeshProUGUI>() ?? gameObject.AddComponent<TextMeshProUGUI>();
        }

        public IEnumerator TypeDialog(string textToType)
        {
            _textMeshPro.SetText(textToType);
            int totalVisibleCharacters = _textMeshPro.textInfo.characterCount;
            var counter = 0;

            while (true)
            {
                int visibleCount = counter % (totalVisibleCharacters + 1);
                _textMeshPro.maxVisibleCharacters = visibleCount;

                if (visibleCount >= totalVisibleCharacters)
                {
                    yield return YieldHelper.TwoSeconds;
                    yield break;
                }
                
                counter += 1;
                yield return YieldHelper.TypingTime;
            }
        }
    }
}
