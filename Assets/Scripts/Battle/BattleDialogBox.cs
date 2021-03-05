using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightColor;
    [SerializeField] Text dialogText;
    [SerializeField] Text energyText;
    [SerializeField] Text typeText;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        // give the player time to read the dialog
        yield return new WaitForSeconds(1f);
    }

    //
    // SELECTOR FUNCTIONS
    //
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightColor;
            else
                moveTexts[i].color = Color.black;
        }

        energyText.text = $"Energy: {move.Energy}/{move.Base.Energy}";
        typeText.text = "Type: " + move.Base.Type.ToString();

        if (move.Energy == 0)
            energyText.color = Color.red;
        else
            energyText.color = Color.black;
    }

    //
    // HELPER FUNCTIONS
    //
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public void SetMoveList(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "-";
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }
}
