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
using UnityEngine.EventSystems;

namespace Itsdits.Ravar.UI.Dialog
{
    /// <summary>
    /// Controller class for displaying <see cref="Dialog"/>.
    /// </summary>
    public class DialogController : MonoBehaviour
    {
        [Header("Scene Management")]
        [Tooltip("EventSystem for the Dialog scene.")]
        [SerializeField] private EventSystem _eventSystem;
        [Tooltip("Canvas that contains the UI elements in the scene.")]
        [SerializeField] private GameObject _canvas;
        
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
            DisableDialog(true);
            _interact = _controls.Player.Interact;
            _textLocalizer = _dialogText.GetComponent<TextLocalizer>();
            GameSignals.DIALOG_OPEN.AddListener(EnableDialog);
            GameSignals.DIALOG_CLOSE.AddListener(DisableDialog);
            GameSignals.DIALOG_SHOW.AddListener(OnDialogShow);
        }

        private void OnDisable()
        {
            _controls.Disable();
            GameSignals.DIALOG_OPEN.RemoveListener(EnableDialog);
            GameSignals.DIALOG_CLOSE.RemoveListener(DisableDialog);
            GameSignals.DIALOG_SHOW.RemoveListener(OnDialogShow);
        }

        private IEnumerator ShowDialog()
        {
            for (var i = 0; i < _dialog.Count; i++)
            {
                yield return TypeDialog(_dialog[i]);
            }
            
            GameSignals.DIALOG_FINISH.Dispatch(_nameText.text);
            GameSignals.DIALOG_CLOSE.Dispatch(true);
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
                yield return YieldHelper.TypingTime;
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
                GameSignals.DIALOG_CLOSE.Dispatch(true);
                throw new ArgumentException($"Null dialog passed with signal. Speaker: {dialogItem.Speaker}");
            }
        }

        private void EnableDialog(bool enable)
        {
            _eventSystem.enabled = true;
            _canvas.SetActive(true);
            _controls.Enable();
        }

        private void DisableDialog(bool disabled)
        {
            _eventSystem.enabled = false;
            _canvas.SetActive(false);
            _controls.Disable();
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