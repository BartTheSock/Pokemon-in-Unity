using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] Text text;
    [SerializeField] bool useDefaultHighlightedColor = true;
    Color originalColor;

    public void Initialize()
    {
        originalColor = text.color;
    }

    public void Clear()
    {
        text.color = originalColor;
    }

    public void OnSelectionChanged(bool selected)
    {
        if (useDefaultHighlightedColor)
            text.color = (selected) ? GlobalSettings.i.HighlightedColorWhiteBackground : Color.black;
        else
            text.color = (selected) ? GlobalSettings.i.HighlightedColorDarkBackground : Color.black;
    }

    public void SetText(string textContents)
    {
        text.text = textContents;
    }

    public Text Text => text;
}