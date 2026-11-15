using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BTS.GenericSelectionUI;

public class ActionSelectionUI : SelectionUI<TextSlot>
{
    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
    }
    public void SetAction()
    {
        selectedItem = 0;
        SetSelectionSettings(SelectionType.Grid, 2);
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}