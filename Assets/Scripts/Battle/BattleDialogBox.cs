using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Text dialogText;
    [SerializeField] Color highlightedColor;

    [SerializeField] Color highPPColor;
    [SerializeField] Color mediumPPColor;
    [SerializeField] Color lowPPColor;
    [SerializeField] Color emptyPPColor;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    [SerializeField] Text yesText;
    [SerializeField] Text noText;

    int lettersPerSecond = 60;

    private void Start()
    {
        if (GlobalSettings.i.TextSpeed == 0)
            lettersPerSecond = 60;
        else if (GlobalSettings.i.TextSpeed == 1)
            lettersPerSecond = 120;
        else
            lettersPerSecond = 240;
    }

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        for (int i = 0; i < dialog.ToCharArray().Length; i++)
        {
            if (GlobalSettings.i.TextSpeed == 2)
            {
                dialogText.text += dialog.ToCharArray()[i];
                i++;
                if (i < dialog.ToCharArray().Length)
                    dialogText.text += dialog.ToCharArray()[i];
                yield return new WaitForEndOfFrame();
            }
            else
            {
                dialogText.text += dialog.ToCharArray()[i];
                yield return new WaitForSeconds(1f / (float)lettersPerSecond);
            }
        }
        yield return new WaitForSeconds(1f);
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

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    public bool IsChoiceBoxEnabled => choiceBox.activeSelf;

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP: {move.PP} / {move.Base.PP}";
        typeText.text = move.Base.MoveType.ToString();

        if (((float)move.PP / (float)move.Base.PP) > 0.5f)
            ppText.color = highPPColor;
        else if (((float)move.PP / (float)move.Base.PP) > 0.2f)
            ppText.color = mediumPPColor;
        else if (move.PP > 0)
            ppText.color = lowPPColor;
        else
            ppText.color = emptyPPColor;
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.MoveName;
            else
                moveTexts[i].text = "-";
        }
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (yesSelected)
            {
                yesText.color = highlightedColor;
                noText.color = Color.black;
            }
            else
            {
                yesText.color = Color.black;
                noText.color = highlightedColor;
            }
        }
    }
}