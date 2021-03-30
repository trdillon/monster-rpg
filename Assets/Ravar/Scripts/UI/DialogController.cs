using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Itsdits.Ravar.UI 
{
    /// <summary>
    /// Controller class for displaying <see cref="Dialog"/>.
    /// </summary>
    public class DialogController : MonoBehaviour
    {
        public static DialogController Instance { get; private set; }

        [Header("Dialog Box")]
        [SerializeField] GameObject dialogBox;
        [SerializeField] Text dialogText;

        [Header("Name Plate")]
        [SerializeField] GameObject namePlate;
        [SerializeField] Text nameText;

        [Header("Type Speed")]
        [Tooltip("How fast the dialog is typed on screen, default is 45.")]
        [SerializeField] int lettersPerSecond = 45;

        private Dialog dialog;
        private int currentString = 0;
        private bool isTyping;
        
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
        /// <param name="dialog">Dialog to show.</param>
        /// <param name="name">Name of character to display on name plate.</param>
        /// <param name="onFinished">What to do after showing.</param>
        public IEnumerator ShowDialog(Dialog dialog, string name, Action onFinished = null)
        {
            if (dialog.Strings.Count > 0)
            {
                // Wait so the Z key isn't immediately counted as pressed,
                // otherwise we might skip the first string.
                yield return new WaitForEndOfFrame();
                OnShowDialog?.Invoke();
                this.dialog = dialog;
                onDialogFinished = onFinished;
                dialogBox.SetActive(true);
                SetNamePlate(name);
                StartCoroutine(TypeDialog(dialog.Strings[0]));
            }
            else
            {
                // Error if dialog was null.
                Debug.LogError("Null dialog was passed to ShowDialog.");
                GameController.Instance.ReleasePlayer();
            }
        }

        /// <summary>
        /// Handles Update lifecycle when GameState.Dialog.
        /// </summary>
        public void HandleUpdate()
        {
            if (Keyboard.current.zKey.wasPressedThisFrame && !isTyping)
            {
                ++currentString;
                if (currentString < dialog.Strings.Count)
                {
                    StartCoroutine(TypeDialog(dialog.Strings[currentString]));
                }
                else
                {
                    currentString = 0;
                    dialogBox.SetActive(false);
                    onDialogFinished?.Invoke();
                    OnCloseDialog?.Invoke();
                }
            }
        }

        private IEnumerator TypeDialog(string dialog)
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

        private void SetNamePlate(string name)
        {
            if (name != null && name.Length > 1)
            {
                nameText.text = name;
                namePlate.SetActive(true);
            }
            else
            {
                nameText.text = "MISSING";
                namePlate.SetActive(true);
            }
        }
    }
}