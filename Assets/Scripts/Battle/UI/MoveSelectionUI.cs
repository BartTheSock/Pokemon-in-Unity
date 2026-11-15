using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BTS.GenericSelectionUI;

public class MoveSelectionUI : SelectionUI<TextSlot>
{
    [SerializeField] List<TextSlot> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    List<Move> _moves;

    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
    }

    public void SetMoves(List<Move> moves)
    {
        _moves = moves;

        selectedItem = 0;

        SetSelectionSettings(SelectionType.Grid, 2);
        SetItems(moveTexts.Take(moves.Count).ToList());

        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
                moveTexts[i].SetText(moves[i].Base.MoveName);
            else
                moveTexts[i].SetText("-");
        }
    }

    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();

        var move = _moves[selectedItem];

        ppText.text = $"PP: {move.PP} / {move.Base.PP}";
        typeText.text = move.Base.MoveType.ToString();

        if (((float)move.PP / (float)move.Base.PP) > 0.5f)
            ppText.color = Color.black;
        else if (((float)move.PP / (float)move.Base.PP) > 0.2f)
            ppText.color = new Color(1f, 0.625f, 0f);
        else if (move.PP > 0)
            ppText.color = new Color(1f, 0.25f, 0f);
        else
            ppText.color = new Color(0.75f, 0f, 0f);
    }
}