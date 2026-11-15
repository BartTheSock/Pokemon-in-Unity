using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Add new Pokemon")]
public class PokemonBase : ScriptableObject
{
    [Header("Name and Description")]
    [SerializeField] string pokemonName;
    [TextArea]
    [SerializeField] string pokemonDescription;

    [Header("Images")]
    [SerializeField] Sprite frontSprite;
    [SerializeField] float battleOffsetY = 0f;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite iconSprite;
    [SerializeField] Sprite shinyFrontSprite;
    [SerializeField] Sprite shinyBackSprite;

    [Header("Type and Ability")]
    [SerializeField] PokemonType pokemonType1;
    [SerializeField] PokemonType pokemonType2;
    [SerializeField] AbilityID ability1;
    [SerializeField] AbilityID ability2;
    [SerializeField] AbilityID hiddenAbility;

    [Header("Base Stats")]
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [Header("Effort Value Yield")]
    [SerializeField] int HPEVYield = 0;
    [SerializeField] int attackEVYield = 0;
    [SerializeField] int defenseEVYield = 0;
    [SerializeField] int spAttackEVYield = 0;
    [SerializeField] int spDefenseEVYield = 0;
    [SerializeField] int speedEVYield = 0;

    [Header("Experience")]
    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    [Header("Other Stuff")]
    [SerializeField] int catchRate = 255;
    [SerializeField] PokemonStatus pokemonStatus;
    [SerializeField] GenderRatio genderRate;
    [SerializeField] int baseFriendship = 50;
    [SerializeField] MoveBase moveLearnedUponEvolution;

    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> learnableByItems;
    [SerializeField] List<Evolution> evolutions;

    public static int MaxNumberOfMoves { get; set; } = 4;

    public int GetEXPForLevel(int level)
    {
        if (level == 1)
            return 0;
        else
        {
            if (growthRate == GrowthRate.Fast)
                return Mathf.FloorToInt(4 * (level * level * level) / 5f);
            else if (growthRate == GrowthRate.MediumFast)
                return level * level * level;
            else if (growthRate == GrowthRate.MediumSlow)
                return Mathf.FloorToInt((6 * (level * level * level) / 5f) - (15 * (level * level)) + (100 * level) - 140);
            else if (growthRate == GrowthRate.Slow)
                return Mathf.FloorToInt(5 * (level * level * level) / 4f);
            else if (growthRate == GrowthRate.Fluctuating)
            {
                if (level < 15)
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor((level + 1) / 3f) + 24) / 50f));
                else if (level >= 15 && level < 36)
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((level + 14) / 50f));
                else
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor(level / 2f) + 32) / 50f));
            }
            else if (growthRate == GrowthRate.Erratic)
            {
                if (level < 50)
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((100 - level) / 50f));
                else if (level >= 50 && level < 68)
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((150 - level) / 100f));
                else if (level >= 68 && level < 98)
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((Mathf.Floor(1911 - (level * 10)) / 3f) / 500f));
                else if (level >= 98 && level <= 100)
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * ((160 - level) / 100f));
            }
        }
        return -1;
    }

    public string PokemonName {
        get { return pokemonName; }
    }

    public string PokemonDescription {
        get { return pokemonDescription; }
    }

    public Sprite FrontSprite {
        get { return frontSprite; }
    }

    public float BattleOffsetY {
        get { return battleOffsetY; }
    }

    public Sprite BackSprite {
        get { return backSprite; }
    }

    public Sprite IconSprite {
        get { return iconSprite; }
    }

    public Sprite ShinyFrontSprite {
        get { return shinyFrontSprite; }
    }

    public Sprite ShinyBackSprite {
        get { return shinyBackSprite; }
    }

    public PokemonType PokemonType1 {
        get { return pokemonType1; }
    }

    public PokemonType PokemonType2 {
        get { return pokemonType2; }
    }

    public AbilityID AbilityID1 {
        get { return ability1; }
    }

    public AbilityID AbilityID2 {
        get { return ability2; }
    }

    public AbilityID HiddenAbilityID {
        get { return hiddenAbility; }
    }

    public int MaxHP {
        get { return maxHP; }
    }

    public int Attack {
        get { return attack; }
    }

    public int Defense {
        get { return defense; }
    }

    public int SpAttack {
        get { return spAttack; }
    }

    public int SpDefense {
        get { return spDefense; }
    }

    public int Speed {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves {
        get { return learnableMoves; }
    }

    public List<MoveBase> LearnableByItems => learnableByItems;
    public List<Evolution> Evolutions => evolutions;
    public int CatchRate => catchRate;
    public int ExpYield => expYield;
    public GrowthRate GrowthRate => growthRate;
    public PokemonStatus PokemonStatus => pokemonStatus;
    public GenderRatio GenderRate => genderRate;
    public int BaseFriendship => baseFriendship;
    public MoveBase MoveLearnedUponEvolution => moveLearnedUponEvolution;
    public Dictionary<Stat, int> EVYields => new Dictionary<Stat, int>()
    {
        { Stat.HP, HPEVYield },
        { Stat.Attack, attackEVYield },
        { Stat.Defense, defenseEVYield },
        { Stat.SpAttack, spAttackEVYield },
        { Stat.SpDefense, spDefenseEVYield },
        { Stat.Speed, speedEVYield }
    };

    public Dictionary<Stat, int> DebugEVYields => new Dictionary<Stat, int>()
    {
        { Stat.HP, HPEVYield * 2},
        { Stat.Attack, attackEVYield * 2},
        { Stat.Defense, defenseEVYield * 2},
        { Stat.SpAttack, spAttackEVYield * 2},
        { Stat.SpDefense, spDefenseEVYield * 2},
        { Stat.Speed, speedEVYield * 2}
    };
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base {
        get { return moveBase; }
    }

    public int Level {
        get { return level; }
    }
}

[System.Serializable]
public class Evolution
{
    [SerializeField] PokemonBase evolvesInto;
    [SerializeField] int requiredLevel;
    [SerializeField] EvolutionItem requiredItem;
    [SerializeField] MoveBase requiredMove;
    [SerializeField] bool friendshipRequired;

    public PokemonBase EvolvesInto => evolvesInto;
    public int RequiredLevel => requiredLevel;
    public EvolutionItem RequiredItem => requiredItem;
    public MoveBase RequiredMove => requiredMove;
    public bool FriendshipRequired => friendshipRequired;
}

public enum PokemonType
{
    None,
    Normal,
    Fighting,
    Flying,
    Poison,
    Ground,
    Rock,
    Bug,
    Ghost,
    Steel,
    Fire,
    Water,
    Grass,
    Electric,
    Psychic,
    Ice,
    Dragon,
    Dark,
    Fairy
}

public enum GrowthRate
{
    Erratic,
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Fluctuating
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    // These three aren't actual stats. They are used to calculate a move's avccuracy and chance of a critical hit.
    accuracy,
    evasion,
    critical,

    HP
}

public enum PokemonStatus
{
    Standard,
    Rare,
    FirstPartner,
    PseudoLegendary,
    Legendary,
    Mythical
}

public enum GenderRatio
{
    GenderUnknown,
    AllMale,
    SevenEighthsMale,
    ThreeFourthsMale,
    HalfMaleHalfFemale,
    OneFourthMale,
    OneEighthMale,
    AllFemale
}

public class TypeChart
{
    static float[][] chart =
    {
//                          NORML FIGHT FLYNG POISN GROND ROCK  BUG   GHOST STEEL FIRE  WATER GRASS ELCTR PSYCH ICE   DRAGN DARK  FAIRY
/*NORMAL*/   new float [] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, },
/*FIGHTING*/ new float [] { 2.0f, 1.0f, 0.5f, 0.5f, 1.0f, 2.0f, 0.5f, 0.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 0.5f, },
/*FLYING*/   new float [] { 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, },
/*POISON*/   new float [] { 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, },
/*GROUND*/   new float [] { 1.0f, 1.0f, 0.0f, 2.0f, 1.0f, 2.0f, 0.5f, 1.0f, 2.0f, 2.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, },
/*ROCK*/     new float [] { 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, },
/*BUG*/      new float [] { 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 0.5f, },
/*GHOST*/    new float [] { 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, },
/*STEEL*/    new float [] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, },
/*FIRE*/     new float [] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, },
/*WATER*/    new float [] { 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, },
/*GRASS*/    new float [] { 1.0f, 1.0f, 0.5f, 0.5f, 2.0f, 2.0f, 0.5f, 1.0f, 0.5f, 0.5f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, },
/*ELECTRIC*/ new float [] { 1.0f, 1.0f, 2.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, },
/*PSYCHIC*/  new float [] { 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.0f, 1.0f, },
/*ICE*/      new float [] { 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, },
/*DRAGON*/   new float [] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.0f, },
/*DARK*/     new float [] { 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 0.5f, },
/*FAIRY*/    new float [] { 1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, },
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;

        int row = (int)attackType - 1;
        int column = (int)defenseType - 1;

        return chart[row][column];
    }
}