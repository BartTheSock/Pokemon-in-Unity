using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] Image background;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text hpText;
    [SerializeField] Image icon;
    [SerializeField] HPBar hpBar;
    [SerializeField] Text statusText;
    [SerializeField] Image statusBackground;
    [SerializeField] Image statusBorder;

    [SerializeField] Color highlightedTextColor;
    [SerializeField] Color defaultTextColor;
    [SerializeField] Color highlightedBackgroundColor;
    [SerializeField] Color defaultBackgroundColor;
    [SerializeField] Color highlightedFaintedColor;
    [SerializeField] Color defaultFaintedColor;

    [SerializeField] Color poisonColor;
    [SerializeField] Color burnColor;
    [SerializeField] Color sleepColor;
    [SerializeField] Color paralyzeColor;
    [SerializeField] Color freezeColor;

    public void OnSelectionChanged(bool selected)
    {
        nameText.color = (selected) ? highlightedTextColor : defaultTextColor;
        levelText.color = (selected) ? highlightedTextColor : defaultTextColor;
        hpText.color = (selected) ? highlightedTextColor : defaultTextColor;
        
        if (hpBar.Health.transform.localScale.x <= 0f)
            background.color = (selected) ? highlightedFaintedColor : defaultFaintedColor;
        else
            background.color = (selected) ? highlightedBackgroundColor : defaultBackgroundColor;
    }
    
    public void Initialize()
    {
        nameText.color = defaultTextColor;
        levelText.color = defaultTextColor;
        hpText.color = defaultTextColor;
        background.color = defaultBackgroundColor;
        background.color = (hpBar.Health.transform.localScale.x <= 0f) ? defaultFaintedColor : defaultBackgroundColor;
    }

    public void Clear()
    {
        nameText.color = defaultTextColor;
        levelText.color = defaultTextColor;
        hpText.color = defaultTextColor;
        background.color = defaultBackgroundColor;
    }
}