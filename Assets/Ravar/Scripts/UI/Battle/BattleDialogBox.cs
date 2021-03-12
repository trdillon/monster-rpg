using Itsdits.Ravar.Monster.Move;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI.Battle { 
    public class BattleDialogBox : MonoBehaviour
    {
        #region config
        [SerializeField] int lettersPerSecond;
        [SerializeField] Color highlightColor;
        [SerializeField] Text dialogText;
        [SerializeField] Text energyText;
        [SerializeField] Text typeText;
        [SerializeField] Text yesText;
        [SerializeField] Text noText;
        [SerializeField] GameObject actionSelector;
        [SerializeField] GameObject moveSelector;
        [SerializeField] GameObject choiceSelector;
        [SerializeField] GameObject moveDetails;
        [SerializeField] List<Text> actionTexts;
        [SerializeField] List<Text> moveTexts;
        #endregion

        /// <summary>
        /// Type the dialog character by character
        /// </summary>
        /// <param name="dialog">Dialog to type</param>
        /// <returns>Dialog</returns>
        public IEnumerator TypeDialog(string dialog)
        {
            if (dialog == null)
            {
                dialogText.text = "";
                Debug.LogError("BDB001: TypeDialog was passed a null value.");
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
        /// <param name="selectedAction">Index of action selected</param>
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
                    actionTexts[i].color = Color.black;
                }
            }
        }

        /// <summary>
        /// Handle move selection.
        /// </summary>
        /// <param name="selectedMove">Index of move selected</param>
        /// <param name="move">Move selected</param>
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
                    moveTexts[i].color = Color.black;
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
                energyText.color = Color.black;
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
                noText.color = Color.black;
            }
            else
            {
                noText.color = highlightColor;
                yesText.color = Color.black;
            }
            
        }

        /// <summary>
        /// Set the dialog to display.
        /// </summary>
        /// <param name="dialog">Dialog to display</param>
        public void SetDialog(string dialog)
        {
            if (dialog == null)
            {
                dialogText.text = "";
                Debug.LogError("BDB002: SetDialog was passed a null value.");
            }
            else
            {
                dialogText.text = dialog;
            }
        }

        /// <summary>
        /// Set the move list.
        /// </summary>
        /// <param name="moves">Available moves</param>
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
        /// <param name="enabled"></param>
        public void EnableDialogText(bool enabled)
        {
            dialogText.enabled = enabled;
        }

        /// <summary>
        /// Show the action selector.
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableActionSelector(bool enabled)
        {
            actionSelector.SetActive(enabled);
        }

        /// <summary>
        /// Show the move selector.
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableMoveSelector(bool enabled)
        {
            moveSelector.SetActive(enabled);
            moveDetails.SetActive(enabled);
        }

        /// <summary>
        /// Show the choice selector.
        /// </summary>
        /// <param name="enabled"></param>
        public void EnableChoiceSelector(bool enabled)
        {
            choiceSelector.SetActive(enabled);
        }
    }
}