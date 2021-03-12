using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI { 
    public class DialogController : MonoBehaviour
    {
        public static DialogController Instance { get; private set; }

        [SerializeField] GameObject dialogBox;
        [SerializeField] Text dialogText;
        [SerializeField] int lettersPerSecond;

        private Dialog dialog;
        private int currentString = 0;
        private bool isTyping;

        public bool IsShowing { get; private set; }
        
        private Action onDialogFinished;
        public event Action OnShowDialog;
        public event Action OnCloseDialog;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Show the dialog box on the screen.
        /// </summary>
        /// <param name="dialog">Dialog to show</param>
        /// <param name="onFinished">What to do after showing</param>
        /// <returns>onFinished</returns>
        public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null)
        {
            if (dialog.Strings.Count > 0)
            {
                // Wait so the Z key isn't immediately counted as pressed,
                // otherwise we might skip the first string.
                yield return new WaitForEndOfFrame();
                OnShowDialog?.Invoke();
                IsShowing = true;
                this.dialog = dialog;
                onDialogFinished = onFinished;
                dialogBox.SetActive(true);
                StartCoroutine(TypeDialog(dialog.Strings[0]));
            }
            else
            {
                // Error if dialog was null.
                Debug.LogError("DC001: Null dialog was passed to the controller.");
                GameController.Instance.ReleasePlayer();
            }
            
        }

        /// <summary>
        /// Type the dialog character by character.
        /// </summary>
        /// <param name="dialog">Dialog to type</param>
        /// <returns></returns>
        public IEnumerator TypeDialog(string dialog)
        {
            isTyping = true;
            dialogText.text = "";

            foreach (var letter in dialog.ToCharArray())
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(1f / lettersPerSecond);
            }

            isTyping = false;
        }

        /// <summary>
        /// Handle user input during the dialog.
        /// </summary>
        public void HandleUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
            {
                ++currentString;
                if (currentString < dialog.Strings.Count)
                {
                    StartCoroutine(TypeDialog(dialog.Strings[currentString]));
                }
                else
                {
                    currentString = 0;
                    IsShowing = false;
                    dialogBox.SetActive(false);
                    onDialogFinished?.Invoke();
                    OnCloseDialog?.Invoke();
                }
            }
        }
    }
}