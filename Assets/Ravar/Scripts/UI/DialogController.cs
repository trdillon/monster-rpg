using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Itsdits.Ravar.Core;
using Itsdits.Ravar.Util;
using TMPro;

namespace Itsdits.Ravar.UI 
{
    /// <summary>
    /// Controller class for displaying <see cref="Dialog"/>.
    /// </summary>
    public class DialogController : MonoBehaviour
    {
        /// <summary>
        /// Static instance of the DialogController.
        /// </summary>
        public static DialogController Instance { get; private set; }

        [Header("Dialog Box")]
        [Tooltip("The GameObject holding the DialogBox.")]
        [SerializeField] private GameObject _dialogBox;
        [Tooltip("The Text element of the DialogBox.")]
        [SerializeField] private TextMeshProUGUI _dialogText;

        [Header("Name Plate")]
        [Tooltip("The nameplate of the character that is displaying dialog.")]
        [SerializeField] private GameObject _namePlate;
        [Tooltip("The Text element that displays the name.")]
        [SerializeField] private Text _nameText;

        private Dialog _dialog;
        private int _currentString;
        private bool _isTyping;

        private Action _onDialogFinished;
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
        /// <param name="speakerName">Name of character to display on name plate.</param>
        /// <param name="onFinished">What to do after showing.</param>
        public IEnumerator ShowDialog(Dialog dialog, string speakerName, Action onFinished = null)
        {
            if (dialog.Strings.Count > 0)
            {
                // Wait so the Z key isn't immediately counted as pressed,
                // otherwise we might skip the first string.
                yield return YieldHelper.EndOfFrame;
                OnShowDialog?.Invoke();
                _dialog = dialog;
                _onDialogFinished = onFinished;
                _dialogBox.SetActive(true);
                SetNamePlate(speakerName);
                StartCoroutine(TypeDialog(dialog.Strings[0]));
            }
            else
            {
                GameController.Instance.ReleasePlayer();
                throw new ArgumentException("Null dialog passed into ShowDialog method.");
            }
        }

        private IEnumerator TypeDialog(string textToType)
        {
            _isTyping = true;
            _dialogText.SetText(textToType);
            int totalVisibleCharacters = _dialogText.textInfo.characterCount;
            var counter = 0;

            while (true)
            {
                int visibleCount = counter % (totalVisibleCharacters + 1);
                _dialogText.maxVisibleCharacters = visibleCount;

                if (visibleCount >= totalVisibleCharacters)
                {
                    yield return YieldHelper.TwoSeconds;
                    _isTyping = false;
                    yield break;
                }
                
                counter += 1;
                yield return YieldHelper.TypingTime;
            }
        }

        /// <summary>
        /// Handles Update lifecycle when GameState.Dialog.
        /// </summary>
        public void HandleUpdate()
        {
            if (!Keyboard.current.zKey.wasPressedThisFrame || _isTyping)
            {
                return;
            }

            ++_currentString;
            if (_currentString < _dialog.Strings.Count)
            {
                StartCoroutine(TypeDialog(_dialog.Strings[_currentString]));
            }
            else
            {
                _currentString = 0;
                _dialogBox.SetActive(false);
                _onDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }

        private void SetNamePlate(string speakerName)
        {
            if (speakerName != null && speakerName.Length > 1)
            {
                _nameText.text = speakerName;
                _namePlate.SetActive(true);
            }
            else
            {
                _nameText.text = "MISSING";
                _namePlate.SetActive(true);
            }
        }
    }
}