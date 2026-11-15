using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
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

    [SerializeField] Image genderIcon;
    [SerializeField] Sprite maleSymbol;
    [SerializeField] Sprite femaleSymbol;
    [SerializeField] Image shinySymbol;

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
    [SerializeField] Color faintedColor;

    Dictionary<StatusConditionID, Color> statusColors;

    Pokemon _pokemon;

    public void Initialize(Pokemon pokemon)
    {
        _pokemon = pokemon;

        UpdateData();
        _pokemon.OnHPChanged += UpdateData;
        _pokemon.OnStatusChanged += SetStatusText;
        _pokemon.OnLevelUp += UpdateData;
    }

    void UpdateData()
    {
        nameText.text = _pokemon.Base.PokemonName;
        levelText.text = "Lv. " + _pokemon.Level;
        hpBar.SetHP(((float)_pokemon.HP / (float)_pokemon.MaxHP), _pokemon.MaxHP);
        icon.sprite = _pokemon.Base.IconSprite;

        shinySymbol.gameObject.SetActive(_pokemon.IsShiny);

        if (_pokemon.Gender == Genders.Genderless)
            genderIcon.gameObject.SetActive(false);
        else
        {
            genderIcon.gameObject.SetActive(true);

            if (_pokemon.Gender == Genders.Male)
                genderIcon.sprite = maleSymbol;
            else if (_pokemon.Gender == Genders.Female)
                genderIcon.sprite = femaleSymbol;
        }

        statusColors = new Dictionary<StatusConditionID, Color>()
        {
            {StatusConditionID.Poison, poisonColor},
            {StatusConditionID.BadPoison, poisonColor},
            {StatusConditionID.Burn, burnColor},
            {StatusConditionID.Sleep, sleepColor},
            {StatusConditionID.Paralyze, paralyzeColor},
            {StatusConditionID.Freeze, freezeColor}
        };

        SetStatusText();
    }

    void SetStatusText()
    {
        if (_pokemon.HP <= 0)
        {
            statusBorder.gameObject.SetActive(true);
            statusBackground.color = faintedColor;
            statusText.color = Color.white;
            statusText.text = "FNT";
        }
        else
        {
            if (_pokemon.Status == null)
            {
                statusText.text = "";
                statusBorder.gameObject.SetActive(false);
            }
            else
            {
                statusBorder.gameObject.SetActive(true);
                statusBackground.color = statusColors[_pokemon.Status.Id];
                statusText.color = Color.white;

                if (_pokemon.Status.Id == StatusConditionID.Poison || _pokemon.Status.Id == StatusConditionID.BadPoison)
                    statusText.text = "PSN";
                else if (_pokemon.Status.Id == StatusConditionID.Burn)
                    statusText.text = "BRN";
                else if (_pokemon.Status.Id == StatusConditionID.Sleep)
                    statusText.text = "SLP";
                else if (_pokemon.Status.Id == StatusConditionID.Paralyze)
                    statusText.text = "PAR";
                else if (_pokemon.Status.Id == StatusConditionID.Freeze)
                    statusText.text = "FRZ";
            }
        }
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            nameText.color = highlightedTextColor;
            levelText.color = highlightedTextColor;
            hpText.color = highlightedTextColor;

            if (_pokemon.HP == 0)
                background.color = highlightedFaintedColor;
            else
                background.color = highlightedBackgroundColor;
        }
        else
        {
            nameText.color = defaultTextColor;
            levelText.color = defaultTextColor;
            hpText.color = defaultTextColor;

            if (_pokemon.HP == 0)
                background.color = defaultFaintedColor;
            else
                background.color = defaultBackgroundColor;
        }
    }
}