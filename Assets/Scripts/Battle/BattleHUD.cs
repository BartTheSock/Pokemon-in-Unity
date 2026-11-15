using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] Image statusBackground;
    [SerializeField] Image statusBorder;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject experience;

    [SerializeField] Image genderSymbol;
    [SerializeField] Sprite maleSymbol;
    [SerializeField] Sprite femaleSymbol;
    [SerializeField] Image shinySymbol;

    [SerializeField] Color poisonColor;
    [SerializeField] Color burnColor;
    [SerializeField] Color sleepColor;
    [SerializeField] Color paralyzeColor;
    [SerializeField] Color freezeColor;

    Dictionary<StatusConditionID, Color> statusColors;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        ClearData();

        _pokemon = pokemon;

        nameText.text = pokemon.Base.PokemonName;
        shinySymbol.gameObject.SetActive(pokemon.IsShiny);
        SetGender();
        SetLevel();
        SetHPBar();
        SetEXP();

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
        _pokemon.OnStatusChanged += SetStatusText;
        _pokemon.OnHPChanged += UpdateHP;
        _pokemon.OnLevelUp += SetLevel;
    }

    void SetStatusText()
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

    public void SetLevel()
    {
        levelText.text = "Lv. " + _pokemon.Level;
    }

    public void SetGender()
    {
        if (_pokemon.Gender == Genders.Male || _pokemon.Gender == Genders.Female)
        {
            genderSymbol.gameObject.SetActive(true);
            if (_pokemon.Gender == Genders.Male)
                genderSymbol.sprite = maleSymbol;
            else
                genderSymbol.sprite = femaleSymbol;
        }
        else
            genderSymbol.gameObject.SetActive(false);
    }

    public void SetHPBar()
    {
        hpBar.SetHP(((float)_pokemon.HP / (float)_pokemon.MaxHP), _pokemon.MaxHP);
    }

    public void SetEXP()
    {
        if (experience == null) return;

        float normalizedExp = _pokemon.GetNormalizedEXP();
        experience.transform.localScale = new Vector3(normalizedExp, 1f, 1f);
    }

    public IEnumerator SetEXPSmooth(bool reset = false)
    {
        if (experience == null) yield break;

        if (reset)
            experience.transform.localScale = new Vector3(0f, 1f, 1f);

        float normalizedExp = _pokemon.GetNormalizedEXP();
        yield return experience.transform.DOScaleX(normalizedExp, 1f).WaitForCompletion();
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth(((float)_pokemon.HP / (float)_pokemon.MaxHP), _pokemon.MaxHP);
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }

    public void ClearData()
    {
        if (_pokemon != null)
        {
            _pokemon.OnStatusChanged -= SetStatusText;
            _pokemon.OnHPChanged -= UpdateHP;
            _pokemon.OnLevelUp -= SetLevel;
        }
    }
}