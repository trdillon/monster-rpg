using Itsdits.Ravar.Monster;
using Itsdits.Ravar.Util;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Class for managing the dialog box displayed during battles.
    /// </summary>
    public class BattleDialogBox : MonoBehaviour
    {
        [Header("Dialog Box")]
        [Tooltip("The Text element that displays the current dialog.")]
        [SerializeField] private TMP_Text _dialogText;

        [Header("Action Selector")]
        [Tooltip("List of Text elements that display on the Action Selection screen.")]
        [SerializeField] private List<Text> _actionTexts;
        [Tooltip("GameObject that holds the Action Selector.")]
        [SerializeField] private GameObject _actionSelector;

        [Header("Move Selector")]
        [Tooltip("List of Text elements that display on the Move Selection screen.")]
        [SerializeField] private List<Text> _moveTexts;
        [Tooltip("GameObject that holds the Move Selector.")]
        [SerializeField] private GameObject _moveSelector;

        [Header("Move Details")]
        [Tooltip("GameObject that holds the Move Details.")]
        [SerializeField] private GameObject _moveDetails;
        [Tooltip("Text element that displays the move's energy.")]
        [SerializeField] private Text _energyText;
        [Tooltip("Text element that displays the move's type.")]
        [SerializeField] private Text _typeText;

        [Header("Choice Selector")]
        [Tooltip("GameObject that holds the Choice Selector.")]
        [SerializeField] private GameObject _choiceSelector;
        [Tooltip("Text element that holds the Yes text.")]
        [SerializeField] private Text _yesText;
        [Tooltip("Text element that holds the No text.")]
        [SerializeField] private Text _noText;

        [Header("Variables")]
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] private Color _highlightColor;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] private Color _standardColor;

        private StringBuilder _dialogBuilder = new StringBuilder();
        
        /// <summary>
        /// Display the dialog one character at a time to appear as if it's being typed.
        /// </summary>
        /// <param name="dialog">Dialog to type.</param>
        /// <returns>Dialog typed out.</returns>
        public IEnumerator TypeDialog(string dialog)
        {
            if (dialog == null)
            {
                _dialogText.text = "";
            }
            else
            {
                _dialogText.text = "";
                _dialogBuilder.Clear();
                foreach (char letter in dialog)
                {
                    _dialogBuilder.Append(letter);
                    //_dialogText.text += letter;
                    yield return YieldHelper.TypingTime;
                }

                _dialogText.text = _dialogBuilder.ToString();
                // Give the player time to read the dialog.
                yield return YieldHelper.OneSecond;
            }
        }

        /// <summary>
        /// Handle action selection.
        /// </summary>
        /// <param name="selectedAction">Index of action selected.</param>
        public void UpdateActionSelection(int selectedAction)
        {
            for (var i = 0; i < _actionTexts.Count; ++i)
            {
                _actionTexts[i].color = i == selectedAction ? _highlightColor : _standardColor;
            }
        }

        /// <summary>
        /// Handle move selection.
        /// </summary>
        /// <param name="selectedMove">Index of move selected.</param>
        /// <param name="move">Move selected.</param>
        public void UpdateMoveSelection(int selectedMove, MoveObj move)
        {
            for (var i = 0; i < _moveTexts.Count; ++i)
            {
                _moveTexts[i].color = i == selectedMove ? _highlightColor : _standardColor;
            }

            _energyText.text = $"Energy: {move.Energy.ToString()}/{move.Base.Energy.ToString()}";
            _typeText.text = "Type: " + move.Base.Type;

            _energyText.color = move.Energy == 0 ? Color.red : _standardColor;   
        }

        /// <summary>
        /// Handle choice selection.
        /// </summary>
        /// <param name="yes">Yes or No</param>
        public void UpdateChoiceSelection(bool yes)
        {
            if (yes)
            {
                _yesText.color = _highlightColor;
                _noText.color = _standardColor;
            }
            else
            {
                _noText.color = _highlightColor;
                _yesText.color = _standardColor;
            }
            
        }

        /// <summary>
        /// Set the dialog to display.
        /// </summary>
        /// <param name="dialog">Dialog to display.</param>
        public void SetDialog(string dialog)
        {
            _dialogText.text = dialog ?? "";
        }

        /// <summary>
        /// Set the move list.
        /// </summary>
        /// <param name="moves">Available moves.</param>
        public void SetMoveList(List<MoveObj> moves)
        {
            for (var i = 0; i < _moveTexts.Count; ++i)
            {
                _moveTexts[i].text = i < moves.Count ? moves[i].Base.Name : "-";
            }
        }

        /// <summary>
        /// Show the dialog box.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnableDialogText(bool isEnabled)
        {
            _dialogText.enabled = isEnabled;
        }

        /// <summary>
        /// Show the action selector.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnableActionSelector(bool isEnabled)
        {
            _actionSelector.SetActive(isEnabled);
        }

        /// <summary>
        /// Show the move selector.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnableMoveSelector(bool isEnabled)
        {
            _moveSelector.SetActive(isEnabled);
            _moveDetails.SetActive(isEnabled);
        }

        /// <summary>
        /// Show the choice selector.
        /// </summary>
        /// <param name="isEnabled">True for enabled, false for disabled.</param>
        public void EnableChoiceSelector(bool isEnabled)
        {
            _choiceSelector.SetActive(isEnabled);
        }
    }
}