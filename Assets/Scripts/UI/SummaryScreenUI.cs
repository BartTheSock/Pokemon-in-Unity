using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using BTS.GenericSelectionUI;

public class SummaryScreenUI : SelectionUI<TextSlot>
{
    [Header("Basic Details")]
    [SerializeField] Image image;
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text typeText;
    [SerializeField] Text natureText;
    [SerializeField] Image genderSymbol;
    [SerializeField] Sprite maleSymbol;
    [SerializeField] Sprite femaleSymbol;
    [SerializeField] Image shinySymbol;

    [Header("Pages")]
    [SerializeField] Text pageNameText;
    [SerializeField] GameObject skillsPage;
    [SerializeField] GameObject movesPage;

    [Header("Pokemon Skills")]
    [SerializeField] Text hpText;
    [SerializeField] Text attackText;
    [SerializeField] Text defenseText;
    [SerializeField] Text spAttackText;
    [SerializeField] Text spDefenseText;
    [SerializeField] Text speedText;
    [SerializeField] Color boostedColor;
    [SerializeField] Color loweredColor;
    [SerializeField] Color defaultColor;
    [SerializeField] Text abilityName;
    [SerializeField] Text abilityText;
    [SerializeField] Text expPointsText;
    [SerializeField] Text nextLevelExpText;
    [SerializeField] Transform expBar;

    [Header("Pokemon Moves")]
    [SerializeField] List<Text> moveTypeTexts;
    [SerializeField] List<Text> moveNameTexts;
    [SerializeField] List<Text> movePPTexts;
    [SerializeField] Text moveDescriptionText;
    [SerializeField] Text movePowerText;
    [SerializeField] Text moveAccuracyText;
    [SerializeField] Text moveCategoryText;
    [SerializeField] GameObject moveEffectsUI;

    List<TextSlot> moveSlots;
    
    private void Start()
    {
        moveSlots = moveNameTexts.Select(m => m.GetComponent<TextSlot>()).ToList();
        moveEffectsUI.SetActive(false);
        moveDescriptionText.text = "";
    }

    bool inMoveSelection;

    public bool InMoveSelection
    {
        get => inMoveSelection;
        set
        {
            inMoveSelection = value;
            if (inMoveSelection == true)
            {
                moveEffectsUI.SetActive(true);
                SetItems(moveSlots.Take(pokemon.Moves.Count).ToList());
            }
            else
            {
                ClearItems();
                moveEffectsUI.SetActive(false);
                moveDescriptionText.text = "";
            }
        }
    }

    Pokemon pokemon;

    public void SetBasicDetails(Pokemon pokemon)
    {
        this.pokemon = pokemon;

        if (pokemon.IsShiny)
            image.sprite = pokemon.Base.ShinyFrontSprite;
        else
            image.sprite = pokemon.Base.FrontSprite;

        nameText.text = pokemon.Base.PokemonName;

        if (pokemon.Gender == Genders.Male || pokemon.Gender == Genders.Female)
        {
            genderSymbol.gameObject.SetActive(true);
            if (pokemon.Gender == Genders.Male)
                genderSymbol.sprite = maleSymbol;
            else
                genderSymbol.sprite = femaleSymbol;
        }
        else
            genderSymbol.gameObject.SetActive(false);

        levelText.text = $"Lv. {pokemon.Level}";

        if (pokemon.Base.PokemonType1 == PokemonType.None)
            typeText.text = "None";
        else
        {
            if (pokemon.Base.PokemonType2 == PokemonType.None)
                typeText.text = pokemon.Base.PokemonType1.ToString();
            else
                typeText.text = pokemon.Base.PokemonType1.ToString() + " / " + pokemon.Base.PokemonType2.ToString();
        }

        shinySymbol.gameObject.SetActive(pokemon.IsShiny);
        natureText.text = $"Nature: {pokemon.Nature}";
    }

    public void ShowPage(int pageNumber)
    {
        if (pageNumber == 0)
        {
            pageNameText.text = "Pokémon Skills";
            skillsPage.SetActive(true);
            movesPage.SetActive(false);
            SetSkills();
        }
        else if (pageNumber == 1)
        {
            pageNameText.text = "Pokémon Moves";
            skillsPage.SetActive(false);
            movesPage.SetActive(true);
            SetMoves();
        }
    }

    public void SetSkills()
    {
        hpText.text = $"{pokemon.HP} / {pokemon.MaxHP}";
        attackText.text = $"{pokemon.Attack}";
        defenseText.text = $"{pokemon.Defense}";
        spAttackText.text = $"{pokemon.SpAttack}";
        spDefenseText.text = $"{pokemon.SpDefense}";
        speedText.text = $"{pokemon.Speed}";

        if (pokemon.Nature == Natures.Lonely || pokemon.Nature == Natures.Brave || pokemon.Nature == Natures.Adamant || pokemon.Nature == Natures.Naughty)
            attackText.color = boostedColor;
        else if (pokemon.Nature == Natures.Bold || pokemon.Nature == Natures.Timid || pokemon.Nature == Natures.Modest || pokemon.Nature == Natures.Calm)
            attackText.color = loweredColor;
        else
            attackText.color = defaultColor;

        if (pokemon.Nature == Natures.Bold || pokemon.Nature == Natures.Relaxed || pokemon.Nature == Natures.Impish || pokemon.Nature == Natures.Lax)
            defenseText.color = boostedColor;
        else if (pokemon.Nature == Natures.Lonely || pokemon.Nature == Natures.Hasty || pokemon.Nature == Natures.Mild || pokemon.Nature == Natures.Gentle)
            defenseText.color = loweredColor;
        else
            defenseText.color = defaultColor;

        if (pokemon.Nature == Natures.Modest || pokemon.Nature == Natures.Mild || pokemon.Nature == Natures.Quiet || pokemon.Nature == Natures.Rash)
            spAttackText.color = boostedColor;
        else if (pokemon.Nature == Natures.Adamant || pokemon.Nature == Natures.Impish || pokemon.Nature == Natures.Jolly || pokemon.Nature == Natures.Careful)
            spAttackText.color = loweredColor;
        else
            spAttackText.color = defaultColor;

        if (pokemon.Nature == Natures.Calm || pokemon.Nature == Natures.Gentle || pokemon.Nature == Natures.Sassy || pokemon.Nature == Natures.Careful)
            spDefenseText.color = boostedColor;
        else if (pokemon.Nature == Natures.Naughty || pokemon.Nature == Natures.Lax || pokemon.Nature == Natures.Naive || pokemon.Nature == Natures.Rash)
            spDefenseText.color = loweredColor;
        else
            spDefenseText.color = defaultColor;

        if (pokemon.Nature == Natures.Timid || pokemon.Nature == Natures.Hasty || pokemon.Nature == Natures.Jolly || pokemon.Nature == Natures.Naive)
            speedText.color = boostedColor;
        else if (pokemon.Nature == Natures.Brave || pokemon.Nature == Natures.Relaxed || pokemon.Nature == Natures.Quiet || pokemon.Nature == Natures.Sassy)
            speedText.color = loweredColor;
        else
            speedText.color = defaultColor;

        if (pokemon.Ability?.Id == pokemon.Base.HiddenAbilityID)
            abilityName.text = $"HIDDEN ABILITY - {pokemon.Ability.AbilityName.ToUpper()}";
        else
            abilityName.text = $"ABILITY - {pokemon.Ability.AbilityName.ToUpper()}";
        abilityText.text = $"{pokemon.Ability.AbilityDescription}";

        expPointsText.text = $"{pokemon.EXP}";
        nextLevelExpText.text = $"{pokemon.Base.GetEXPForLevel(pokemon.Level + 1) - pokemon.EXP}";

        expBar.localScale = new Vector2(pokemon.GetNormalizedEXP(), 1);
    }

    public void SetMoves()
    {
        for (int i = 0; i < moveNameTexts.Count; i++)
        {
            if (i < pokemon.Moves.Count)
            {
                var move = pokemon.Moves[i];
                moveTypeTexts[i].text = move.Base.MoveType.ToString();
                moveNameTexts[i].text = move.Base.MoveName;
                movePPTexts[i].text = $"PP {move.PP} / {move.Base.PP}";
            }
            else
            {
                moveTypeTexts[i].text = "-";
                moveNameTexts[i].text = "-";
                movePPTexts[i].text = $"-";
            }
        }
    }

    public override void HandleUpdate()
    {
        if (InMoveSelection)
            base.HandleUpdate();
    }

    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();

        var move = pokemon.Moves[selectedItem];
        moveDescriptionText.text = $"{move.Base.MoveDescription}";
        
        if (move.Base.Power == 0)
            movePowerText.text = "-";
        else
            movePowerText.text = $"{move.Base.Power}";

        if (move.Base.AlwaysHits)
            moveAccuracyText.text = "-";
        else
            moveAccuracyText.text = $"{move.Base.Accuracy}";
        
        moveCategoryText.text = $"{move.Base.Category}";
    }
}