using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Itsdits.Ravar.Core.Signal;
using Itsdits.Ravar.Settings;
using Itsdits.Ravar.UI.Localization;
using Itsdits.Ravar.Util;
using TMPro;

namespace Itsdits.Ravar.UI.Dialog
{
    /// <summary>
    /// Controller class for displaying <see cref="Dialog"/>.
    /// </summary>
    public class DialogController : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("The Text element of the Dialog Box.")]
        [SerializeField] private TextMeshProUGUI _dialogText;
        [Tooltip("The Text element that displays the name.")]
        [SerializeField] private TextMeshProUGUI _nameText;

        private TextLocalizer _textLocalizer;
        private PlayerControls _controls;
        private InputAction _interact; 

        private List<string> _dialog = new List<string>();
        
        private void OnEnable()
        {
            _controls = new PlayerControls();
            _controls.Enable();
            _interact = _controls.Player.Interact;
            _textLocalizer = _dialogText.GetComponent<TextLocalizer>();
            GameSignals.DIALOG_SHOW.AddListener(OnDialogShow);
        }

        private void OnDisable()
        {
            _controls.Disable();
            GameSignals.DIALOG_SHOW.RemoveListener(OnDialogShow);
        }

        private IEnumerator ShowDialog()
        {
            for (var i = 0; i < _dialog.Count; i++)
            {
                yield return TypeDialog(_dialog[i]);
            }
            
            GameSignals.DIALOG_CLOSE.Dispatch(_nameText.text);
        }

        private IEnumerator TypeDialog(string dialog)
        {
            _textLocalizer.ChangeKey(dialog);
            _dialogText.ForceMeshUpdate();
            int totalVisibleCharacters = _dialogText.textInfo.characterCount;
            var counter = 0;

            while (true)
            {
                int visibleCount = counter % (totalVisibleCharacters + 1);
                _dialogText.maxVisibleCharacters = visibleCount;

                if (visibleCount >= totalVisibleCharacters)
                {
                    // Finished displaying all dialog. Wait until the player has finished reading to break out.
                    yield return new WaitUntil(() => _interact.triggered);
                    yield break;
                }
                
                counter += 1;
                yield return YieldHelper.TYPING_TIME;
            }
        }

        private void OnDialogShow(DialogItem dialogItem)
        {
            if (dialogItem.Dialog.Length > 0)
            {
                _dialog.Clear();
                _dialog.AddRange(dialogItem.Dialog);
                SetNamePlate(dialogItem.Speaker);
                StartCoroutine(ShowDialog());
            }
            else
            {
                GameSignals.DIALOG_CLOSE.Dispatch(null);
                throw new ArgumentException($"Null dialog passed with signal. Speaker: {dialogItem.Speaker}");
            }
        }

        private void SetNamePlate(string speakerName)
        {
            if (speakerName != null && speakerName.Length > 1)
            {
                _nameText.SetText(speakerName);
            }
            else
            {
                _nameText.SetText("MISSING");
            }
        }
    }
}