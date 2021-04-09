using System.Collections;
using System.Text;
using Itsdits.Ravar.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI
{
    public class DialogWriter : MonoBehaviour
    {
        [Tooltip("The Text element the dialog will be displayed in.")]
        [SerializeField] private Text _dialogText;
        
        private StringBuilder _dialogBuilder = new StringBuilder(250);
        
        public IEnumerator TypeDialog(string dialog)
        {
            _dialogText.text = "";
            _dialogBuilder.Clear();
            foreach (char letter in dialog)
            {
                _dialogBuilder.Append(letter);
                _dialogText.text = _dialogBuilder.ToString();
                yield return YieldHelper.TypingTime;
            }
            
        }
    }
}
