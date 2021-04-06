using Itsdits.Ravar.Monster;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] Text dialogText;

        [Header("Action Selector")]
        [Tooltip("List of Text elements that display on the Action Selection screen.")]
        [SerializeField] List<Text> actionTexts;
        [Tooltip("GameObject that holds the Action Selector.")]
        [SerializeField] GameObject actionSelector;

        [Header("Move Selector")]
        [Tooltip("List of Text elements that display on the Move Selection screen.")]
        [SerializeField] List<Text> moveTexts;
        [Tooltip("GameObject that holds the Move Selector.")]
        [SerializeField] GameObject moveSelector;

        [Header("Move Details")]
        [Tooltip("GameObject that holds the Move Details.")]
        [SerializeField] GameObject moveDetails;
        [Tooltip("Text element that displays the move's energy.")]
        [SerializeField] Text energyText;
        [Tooltip("Text element that displays the move's type.")]
        [SerializeField] Text typeText;

        [Header("Choice Selector")]
        [Tooltip("GameObject that holds the Choice Selector.")]
        [SerializeField] GameObject choiceSelector;
        [Tooltip("Text element that holds the Yes text.")]
        [SerializeField] Text yesText;
        [Tooltip("Text element that holds the No text.")]
        [SerializeField] Text noText;

        [Header("Variables")]
        [Tooltip("How fast the dialog is typed on screen, default is 45.")]
        [SerializeField] int lettersPerSecond;
        [Tooltip("The color to change the text to when highlighted.")]
        [SerializeField] Color highlightColor;
        [Tooltip("The color to display when the text is not highlighted.")]
        [SerializeField] Color standardColor;


        /// <summary>
        /// Display the dialog one character at a time to appear as if it's being typed.
        /// </summary>
        /// <param name="dialog">Dialog to type.</param>
        /// <returns>Dialog typed out.</returns>
        public IEnumerator TypeDialog(string dialog)
        {
            if (dialog == null)
            {
                dialogText.text = "";
            }
            else
            {
                dialogText.text = "";
                foreach (var letter in dialog.ToCharArray())
                {
                    dialogText.text += letter;
                    yield return new WaitForSeconds(1f / lettersPerSecond);
                }
                // Give the player time to read the dialog.
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// Handle action selection.
        /// </summary>
        /// <param name="selectedAction">Index of action selected.</param>
        public void UpdateActionSelection(int selectedAction)
        {
            for (int i = 0; i < actionTexts.Count; ++i)
            {
                if (i == selectedAction)
                {
                    actionTexts[i].color = highlightColor;
                }
                else
                {
                    actionTexts[i].color = standardColor;
                }
            }
        }

        /// <summary>
        /// Handle move selection.
        /// </summary>
        /// <param name="selectedMove">Index of move selected.</param>
        /// <param name="move">Move selected.</param>
        public void UpdateMoveSelection(int selectedMove, MoveObj move)
        {
            for (int i = 0; i < moveTexts.Count; ++i)
            {
                if (i == selectedMove)
                {
                    moveTexts[i].color = highlightColor;
                } 
                else
                {
                    moveTexts[i].color = standardColor;
                }
            }

            energyText.text = $"Energy: {move.Energy}/{move.Base.Energy}";
            typeText.text = "Type: " + move.Base.Type.ToString();

            if (move.Energy == 0)
            {
                energyText.color = Color.red;
            }  
            else
            {
                energyText.color = standardColor;
            }   
        }

        /// <summary>
        /// Handle choice selection.
        /// </summary>
        /// <param name="yes">Yes or No</param>
        public void UpdateChoiceSelection(bool yes)
        {
            if (yes)
            {
                yesText.color = highlightColor;
                noText.color = standardColor;
            }
            else
            {
                noText.color = highlightColor;
                yesText.color = standardColor;
            }
            
        }

        /// <summary>
        /// Set the dialog to display.
        /// </summary>
        /// <param name="dialog">Dialog to display.</param>
        public void SetDialog(string dialog)
        {
            if (dialog == null)
            {
                dialogText.text = "";
            }
            else
            {
                dialogText.text = dialog;
            }
        }

        /// <summary>
        /// Set the move list.
        /// </summary>
        /// <param name="moves">Available moves.</param>
        public void SetMoveList(List<MoveObj> moves)
        {
            for (int i = 0; i < moveTexts.Count; ++i)
            {
                if (i < moves.Count)
                {
                    moveTexts[i].text = moves[i].Base.Name;
                } 
                else
                {
                    moveTexts[i].text = "-";
                }    
            }
        }

        /// <summary>
        /// Show the dialog box.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnableDialogText(bool enabled)
        {
            dialogText.enabled = enabled;
        }

        /// <summary>
        /// Show the action selector.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnableActionSelector(bool enabled)
        {
            actionSelector.SetActive(enabled);
        }

        /// <summary>
        /// Show the move selector.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnableMoveSelector(bool enabled)
        {
            moveSelector.SetActive(enabled);
            moveDetails.SetActive(enabled);
        }

        /// <summary>
        /// Show the choice selector.
        /// </summary>
        /// <param name="enabled">True for enabled, false for disabled.</param>
        public void EnableChoiceSelector(bool enabled)
        {
            choiceSelector.SetActive(enabled);
        }
    }
}