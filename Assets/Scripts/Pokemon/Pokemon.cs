using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isOwnedByTrainer;
    int shinyThreshold = 512;

    public Pokemon(PokemonBase pBase, int pLevel, bool ownedByTrainer)
    {
        _base = pBase;
        level = pLevel;
        isOwnedByTrainer = ownedByTrainer;
        Initialize(isOwnedByTrainer);
    }

    public PokemonBase Base {
        get { return _base; }
    }

    public int Level {
        get { return level; }
    }

    public int HPIV { get; set; }
    public int AttackIV { get; set; }
    public int DefenseIV { get; set; }
    public int SpAttackIV { get; set; }
    public int SpDefenseIV { get; set; }
    public int SpeedIV { get; set; }
    public int BoostedStat { get; set; }
    public int LoweredStat { get; set; }
    public Natures Nature { get; set; }
    public int GenderNumber { get; set; }
    public GenderRatio GenderRatio { get; set; }
    public Genders Gender { get; set; }
    public int Friendship { get; set; }
    public int EXP { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Dictionary<Stat, int> StatEffortValues { get; private set; }
    public StatusCondition Status { get; private set; }
    public int StatusTime { get; set; }
    public StatusCondition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }
    public Ability Ability { get; private set; }
    public int AbilityNumber { get; set; }
    public int ShinyNumber { get; set; }
    public bool IsShiny { get; set; }
    public int TurnCount { get; set; }
    public float CriticalThreshold { get; set; }
    public float CriticalDamage { get; set; }
    public int MagnitudeStrength { get; set; } = -1;
    public float PsywaveStrength { get; set; } = -1f;
    public float MoveTypeEffectiveness { get; set; } = 1f;
    public Queue<StatusEvent> StatusChanges { get; private set; } = new Queue<StatusEvent>();

    public event System.Action OnStatusChanged;
    public event System.Action OnHPChanged;
    public event System.Action OnLevelUp;

    public void Initialize(bool trainerOwned)
    {
        // Generate Moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if ((move.Level <= Level) && !HasMove(move.Base))
                Moves.Add(new Move(move.Base));

            if (Moves.Count > PokemonBase.MaxNumberOfMoves)
                Moves.RemoveAt(0);
        }

        StatEffortValues = new Dictionary<Stat, int>()
        {
            { Stat.HP, 0 },
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefense, 0 },
            { Stat.Speed, 0 }
        };

        EXP = Base.GetEXPForLevel(level);

        if (trainerOwned)
        {
            HPIV = 15;
            AttackIV = 15;
            DefenseIV = 15;
            SpAttackIV = 15;
            SpDefenseIV = 15;
            SpeedIV = 15;
        }
        else
        {
            HPIV = UnityEngine.Random.Range(0, 32);
            AttackIV = UnityEngine.Random.Range(0, 32);
            DefenseIV = UnityEngine.Random.Range(0, 32);
            SpAttackIV = UnityEngine.Random.Range(0, 32);
            SpDefenseIV = UnityEngine.Random.Range(0, 32);
            SpeedIV = UnityEngine.Random.Range(0, 32);
        }

        BoostedStat = UnityEngine.Random.Range(0, 5);

        if (trainerOwned)
            LoweredStat = BoostedStat;
        else
            LoweredStat = UnityEngine.Random.Range(0, 5);

        DetermineNature();
        AbilityNumber = UnityEngine.Random.Range(0, 256);
        DetermineAbility();
        GenderNumber = UnityEngine.Random.Range(0, 256);
        GenderRatio = Base.GenderRate;
        DetermineGender();
        CalculateStats();
        InitializeShininess();
        Friendship = Base.BaseFriendship;
        HP = MaxHP;

        StatusChanges = new Queue<StatusEvent>();
        ResetStatBoost();
        CureStatus();
        CureVolatileStatus();
        TurnCount = 0;
    }

    public Pokemon(PokemonSaveData saveData)
    {
        _base = PokemonDB.GetObjectByName(saveData.pokemonName);
        HP = saveData.hp;
        level = saveData.level;
        EXP = saveData.exp;
        HPIV = saveData.hpIV;
        AttackIV = saveData.attackIV;
        DefenseIV = saveData.defenseIV;
        SpAttackIV = saveData.spAttackIV;
        SpDefenseIV = saveData.spDefenseIV;
        SpeedIV = saveData.speedIV;
        BoostedStat = saveData.boostedStat;
        LoweredStat = saveData.loweredStat;
        DetermineNature();
        GenderNumber = saveData.genderNumber;
        Friendship = saveData.friendship;
        GenderRatio = saveData.genderRatio;
        DetermineGender();
        AbilityNumber = saveData.abilityNumber;
        DetermineAbility();
        ShinyNumber = saveData.shinyNumber;
        DetermineShininess();

        if (saveData.statusID != null)
            Status = StatusConditionsDB.StatusConditions[saveData.statusID.Value];
        else
            Status = null;

        StatEffortValues = saveData.statEffortValues;
        Moves = saveData.moves.Select(s => new Move(s)).ToList();

        CalculateStats();
        StatusChanges = new Queue<StatusEvent>();
        ResetStatBoost();
        VolatileStatus = null;
    }

    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            pokemonName = Base.name,
            hp = HP,
            level = Level,
            exp = EXP,
            hpIV = HPIV,
            attackIV = AttackIV,
            defenseIV = DefenseIV,
            spAttackIV = SpAttackIV,
            spDefenseIV = SpDefenseIV,
            speedIV = SpeedIV,
            boostedStat = BoostedStat,
            loweredStat = LoweredStat,
            genderNumber = GenderNumber,
            friendship = Friendship,
            genderRatio = GenderRatio,
            abilityNumber = AbilityNumber,
            shinyNumber = ShinyNumber,
            statusID = Status?.Id,
            statEffortValues = StatEffortValues,
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }

    void DetermineNature()
    {
        Nature = (Natures)((BoostedStat * 5) + LoweredStat);
    }

    void DetermineAbility()
    {
        if (AbilityNumber >= 240)
            Ability = AbilitiesDB.Abilities[Base.HiddenAbilityID];
        else if(AbilityNumber >= 120)
            Ability = AbilitiesDB.Abilities[Base.AbilityID2];
        else
            Ability = AbilitiesDB.Abilities[Base.AbilityID1];
    }

    void InitializeShininess()
    {
        if (isOwnedByTrainer)
        {
            ShinyNumber = 65536;
            IsShiny = false;
        }
        else
        {
            IsShiny = false;
            int shinyRolls = 1;

            if (PlayerController.i.GetComponent<Inventory>().HasItem(GlobalSettings.i.ShinyCharmDX))
                shinyRolls = 8;
            else if (PlayerController.i.GetComponent<Inventory>().HasItem(GlobalSettings.i.ShinyCharm))
                shinyRolls = 3;

            for (int i = 0; i < shinyRolls; i++)
            {
                ShinyNumber = UnityEngine.Random.Range(0, 65536);
                if (ShinyNumber < shinyThreshold)
                    break;
            }
            DetermineShininess();
        }
    }

    void DetermineShininess()
    {
        if (ShinyNumber >= shinyThreshold)
            ShinyNumber = 65536;
        else
            ShinyNumber = -1;
        IsShiny = (ShinyNumber <= -1);
    }

    void DetermineGender()
    {
        if (GenderRatio == GenderRatio.AllMale)
            Gender = Genders.Male;
        else if (GenderRatio == GenderRatio.SevenEighthsMale)
        {
            if (GenderNumber >= 32)
                Gender = Genders.Male;
            else
                Gender = Genders.Female;
        }
        else if (GenderRatio == GenderRatio.ThreeFourthsMale)
        {
            if (GenderNumber >= 64)
                Gender = Genders.Male;
            else
                Gender = Genders.Female;
        }
        else if (GenderRatio == GenderRatio.HalfMaleHalfFemale)
        {
            if (GenderNumber >= 128)
                Gender = Genders.Male;
            else
                Gender = Genders.Female;
        }
        else if (GenderRatio == GenderRatio.OneFourthMale)
        {
            if (GenderNumber >= 192)
                Gender = Genders.Male;
            else
                Gender = Genders.Female;
        }
        else if (GenderRatio == GenderRatio.OneEighthMale)
        {
            if (GenderNumber >= 224)
                Gender = Genders.Male;
            else
                Gender = Genders.Female;
        }
        else if (GenderRatio == GenderRatio.AllFemale)
            Gender = Genders.Female;
        else
            Gender = Genders.Genderless;
    }

    public void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();

        if (Nature == Natures.Lonely || Nature == Natures.Brave || Nature == Natures.Adamant || Nature == Natures.Naughty)
            Stats.Add(Stat.Attack, Mathf.FloorToInt((((((2 * Base.Attack) + AttackIV + (StatEffortValues[Stat.Attack] / 4f)) * Level) / 100f) + 5) * 1.1f));
        else if (Nature == Natures.Bold || Nature == Natures.Timid || Nature == Natures.Modest || Nature == Natures.Calm)
            Stats.Add(Stat.Attack, Mathf.FloorToInt((((((2 * Base.Attack) + AttackIV + (StatEffortValues[Stat.Attack] / 4f)) * Level) / 100f) + 5) * 0.9f));
        else
            Stats.Add(Stat.Attack, Mathf.FloorToInt((((2 * Base.Attack) + AttackIV + (StatEffortValues[Stat.Attack] / 4f)) * Level) / 100f) + 5);

        if (Nature == Natures.Bold || Nature == Natures.Relaxed || Nature == Natures.Impish || Nature == Natures.Lax)
            Stats.Add(Stat.Defense, Mathf.FloorToInt((((((2 * Base.Defense) + DefenseIV + (StatEffortValues[Stat.Defense] / 4f)) * Level) / 100f) + 5) * 1.1f));
        else if (Nature == Natures.Lonely || Nature == Natures.Hasty || Nature == Natures.Mild || Nature == Natures.Gentle)
            Stats.Add(Stat.Defense, Mathf.FloorToInt((((((2 * Base.Defense) + DefenseIV + (StatEffortValues[Stat.Defense] / 4f)) * Level) / 100f) + 5) * 0.9f));
        else
            Stats.Add(Stat.Defense, Mathf.FloorToInt((((2 * Base.Defense) + DefenseIV + (StatEffortValues[Stat.Defense] / 4f)) * Level) / 100f) + 5);

        if (Nature == Natures.Modest || Nature == Natures.Mild || Nature == Natures.Quiet || Nature == Natures.Rash)
            Stats.Add(Stat.SpAttack, Mathf.FloorToInt((((((2 * Base.SpAttack) + SpAttackIV + (StatEffortValues[Stat.SpAttack] / 4f)) * Level) / 100f) + 5) * 1.1f));
        else if (Nature == Natures.Adamant || Nature == Natures.Impish || Nature == Natures.Jolly || Nature == Natures.Careful)
            Stats.Add(Stat.SpAttack, Mathf.FloorToInt((((((2 * Base.SpAttack) + SpAttackIV + (StatEffortValues[Stat.SpAttack] / 4f)) * Level) / 100f) + 5) * 0.9f));
        else
            Stats.Add(Stat.SpAttack, Mathf.FloorToInt((((2 * Base.SpAttack) + SpAttackIV + (StatEffortValues[Stat.SpAttack] / 4f)) * Level) / 100f) + 5);

        if (Nature == Natures.Calm || Nature == Natures.Gentle || Nature == Natures.Sassy || Nature == Natures.Careful)
            Stats.Add(Stat.SpDefense, Mathf.FloorToInt((((((2 * Base.SpDefense) + SpDefenseIV + (StatEffortValues[Stat.SpDefense] / 4f)) * Level) / 100f) + 5) * 1.1f));
        else if (Nature == Natures.Naughty || Nature == Natures.Lax || Nature == Natures.Naive || Nature == Natures.Rash)
            Stats.Add(Stat.SpDefense, Mathf.FloorToInt((((((2 * Base.SpDefense) + SpDefenseIV + (StatEffortValues[Stat.SpDefense] / 4f)) * Level) / 100f) + 5) * 0.9f));
        else
            Stats.Add(Stat.SpDefense, Mathf.FloorToInt((((2 * Base.SpDefense) + SpDefenseIV + (StatEffortValues[Stat.SpDefense] / 4f)) * Level) / 100f) + 5);

        if (Nature == Natures.Timid || Nature == Natures.Hasty || Nature == Natures.Jolly || Nature == Natures.Naive)
        {
            if (Status?.Id == StatusConditionID.Paralyze && Ability?.Id != AbilityID.QuickFeet)
                Stats.Add(Stat.Speed, Mathf.FloorToInt((((((2 * Base.Speed) + SpeedIV + (StatEffortValues[Stat.Speed] / 4f)) * Level) / 100f) + 5) * 0.275f));
            else
                Stats.Add(Stat.Speed, Mathf.FloorToInt((((((2 * Base.Speed) + SpeedIV + (StatEffortValues[Stat.Speed] / 4f)) * Level) / 100f) + 5) * 1.1f));
        }
        else if (Nature == Natures.Brave || Nature == Natures.Relaxed || Nature == Natures.Quiet || Nature == Natures.Sassy)
        {
            if (Status?.Id == StatusConditionID.Paralyze && Ability?.Id != AbilityID.QuickFeet)
                Stats.Add(Stat.Speed, Mathf.FloorToInt((((((2 * Base.Speed) + SpeedIV + (StatEffortValues[Stat.Speed] / 4f)) * Level) / 100f) + 5) * 0.225f));
            else
                Stats.Add(Stat.Speed, Mathf.FloorToInt((((((2 * Base.Speed) + SpeedIV + (StatEffortValues[Stat.Speed] / 4f)) * Level) / 100f) + 5) * 0.9f));
        }
        else
        {
            if (Status?.Id == StatusConditionID.Paralyze && Ability?.Id != AbilityID.QuickFeet)
                Stats.Add(Stat.Speed, Mathf.FloorToInt((((((2 * Base.Speed) + SpeedIV + (StatEffortValues[Stat.Speed] / 4f)) * Level) / 100f) + 5) * 0.25f));
            else
                Stats.Add(Stat.Speed, Mathf.FloorToInt((((((2 * Base.Speed) + SpeedIV + (StatEffortValues[Stat.Speed] / 4f)) * Level) / 100f) + 5)));
        }

        int oldMaxHP = MaxHP;
        if (Base.PokemonName == "Shedinja")
            MaxHP = Base.MaxHP;
        else
            MaxHP = Mathf.FloorToInt((((2 * Base.MaxHP) + HPIV + (StatEffortValues[Stat.HP] / 4f)) * Level) / 100f) + Level + 10;

        if (oldMaxHP != 0)
            HP += MaxHP - oldMaxHP;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.accuracy, 0},
            {Stat.evasion, 0},
            {Stat.critical, 0},
        };
    }

    int GetStat(Stat stat)
    {
        int statValue = Stats[stat];

        // Apply stat boosts
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        else
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);

        return statValue;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts, Pokemon source)
    {
        var statsDictionary = statBoosts.ToDictionary(x => x.stat, x => x.boost);
        Ability?.OnBoost?.Invoke(statsDictionary, source, this);

        foreach (var keyValuePair in statsDictionary)
        {
            var stat = keyValuePair.Key;
            var boost = keyValuePair.Value;
            var statName = stat.ToString();

            if (stat == Stat.SpAttack)
                statName = "Sp. Atk";
            else if (stat == Stat.SpDefense)
                statName = "Sp. Def";

            if (stat == Stat.critical)
            {
                if (StatBoosts[stat] >= 2)
                    AddStatusEvent($"But it failed!");
                else
                {
                    if (boost >= 1)
                        AddStatusEvent($"{Base.PokemonName} is getting pumped!");
                    else
                        AddStatusEvent($"But it failed!");
                }

                StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, 0, 6);
            }
            else
            {
                if (boost > 0)
                {
                    if (StatBoosts[stat] == 6)
                        AddStatusEvent(StatusEventType.StatBoost, $"{Base.PokemonName}'s {statName} won't go higher!");
                    else
                    {
                        if (boost >= 3)
                            AddStatusEvent(StatusEventType.StatBoost, $"{Base.PokemonName}'s {statName} rose drastically!");
                        else if (boost == 2)
                            AddStatusEvent(StatusEventType.StatBoost, $"{Base.PokemonName}'s {statName} rose sharply!");
                        else
                            AddStatusEvent(StatusEventType.StatBoost, $"{Base.PokemonName}'s {statName} rose!");
                    }
                }
                else if (boost < 0)
                {
                    if (StatBoosts[stat] == -6)
                        AddStatusEvent(StatusEventType.StatBoost, $"{Base.PokemonName}'s {statName} won't go lower!");
                    else
                    {
                        if (boost <= -3)
                            AddStatusEvent(StatusEventType.StatBoost, $"{Base.PokemonName}'s {statName} drastically fell!");
                        else if (boost == -2)
                            AddStatusEvent(StatusEventType.StatBoost, $"{Base.PokemonName}'s {statName} harshly fell!");
                        else
                            AddStatusEvent(StatusEventType.StatBoost, $"{Base.PokemonName}'s {statName} fell!");
                    }
                }
                else if (boost == 0)
                    AddStatusEvent($"But nothing happened!");

                StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            }
        }
    }

    public bool CheckForLevelUp()
    {
        if (EXP >= Base.GetEXPForLevel(level + 1))
        {
            level++;
            ChangeFriendship(8, 5, 3);
            CalculateStats();
            OnLevelUp?.Invoke();
            return true;
        }

        return false;
    }

    public void GainEVs(Dictionary<Stat, int> evGained)
    {
        foreach (var statEffortValues in StatEffortValues.ToArray())
        {
            if (statEffortValues.Value < GlobalSettings.i.MaxEvPerStat && GetTotalEvs() < GlobalSettings.i.MaxEvs)
            {
                evGained[statEffortValues.Key] = Mathf.Clamp(evGained[statEffortValues.Key], 0, (GlobalSettings.i.MaxEvs - GetTotalEvs()));
                StatEffortValues[statEffortValues.Key] = Mathf.Clamp((StatEffortValues[statEffortValues.Key] += evGained[statEffortValues.Key]), 0, GlobalSettings.i.MaxEvPerStat);
            }
        }
    }

    public int GetTotalEvs()
    {
        return StatEffortValues.Values.Sum();
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if (Moves.Count >= PokemonBase.MaxNumberOfMoves)
            return;

        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }

    public Evolution CheckForEvolution(string evolutionName)
    {
        var evolvesIntoPokemon = Base.Evolutions.Where(e => (e.RequiredLevel <= level && e.FriendshipRequired == false) || (e.FriendshipRequired == true && Friendship >= 160) || (HasMove(e.RequiredMove) && e.RequiredMove != null && e.RequiredLevel <= level)).ToList();
        
        if (evolutionName == "Hitmonlee" || evolutionName == "Hitmonchan" || evolutionName == "Hitmontop" || evolutionName == "Silcoon" || evolutionName == "Cascoon")
            evolvesIntoPokemon = evolvesIntoPokemon.Where(p => p.EvolvesInto.PokemonName == evolutionName).ToList();
        
        return evolvesIntoPokemon.FirstOrDefault();
    }

    public Evolution CheckForEvolution(ItemBase item)
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredItem == item);
    }

    public void Evolve(Evolution evolution)
    {
        int oldMaxHP = MaxHP;
        _base = evolution.EvolvesInto;
        GenderRatio = Base.GenderRate;
        DetermineGender();
        DetermineAbility();
        CalculateStats();
        HP = Mathf.Clamp(HP + (MaxHP - oldMaxHP), 0, MaxHP);
    }

    public void Heal()
    {
        HP = MaxHP;
        OnHPChanged?.Invoke();

        CureStatus();
        foreach (Move move in Moves)
        {
            move.PP = move.Base.PP;
        }
        CalculateStats();
    }

    public bool IsOfType(PokemonType type)
    {
        return type == Base.PokemonType1 || type == Base.PokemonType2;
    }

    public float GetNormalizedEXP()
    {
        int currentLevelExp = Base.GetEXPForLevel(Level);
        int nextLevelExp = Base.GetEXPForLevel(Level + 1);

        float normalizedExp = (float)(EXP - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public int Attack {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHP { get; private set; }

    public int CalculateDamage(Move move, Pokemon attacker, Pokemon defender, float criticalThreshold, int magnitudeStrength, float weatherModifier = 1f, bool heal = false, bool recoil = false)
    {
        defender = this;

        float criticalChance;

        int critical = attacker.StatBoosts[Stat.critical];
        if (move.Base.AlwaysCriticalHit)
            critical = 6;
        else
        {
            if (move.Base.HighCriticalHitRatio)
                critical++;

            critical = Mathf.Clamp(critical, 0, 6);
        }

        if (critical == 1)
            criticalChance = 12.5f;
        else if (critical == 2)
            criticalChance = 50f;
        else if (critical >= 3)
            criticalChance = 100f;
        else
            criticalChance = 4.1666f;

        if (criticalThreshold <= criticalChance)
            CriticalDamage = 1.5f;
        else
            CriticalDamage = 1f;

        float stab;
        float power = move.Base.Power;
        float attack;
        float defense;
        float modifiers;
        float randomness = UnityEngine.Random.Range(0.85f, 1f);
        float statusModifier;
        float a;
        float d;
        int damage;

        MoveTypeEffectiveness = TypeChart.GetEffectiveness(move.Base.MoveType, defender.Base.PokemonType1) * TypeChart.GetEffectiveness(move.Base.MoveType, defender.Base.PokemonType2);

        if (move.Base.MoveType == PokemonType.Ground && defender.Ability?.Id == AbilityID.Levitate)
            MoveTypeEffectiveness = 0;

        if (move.Base.MoveType == attacker.Base.PokemonType1 || move.Base.MoveType == attacker.Base.PokemonType2)
        {
            if (attacker.Ability?.Id == AbilityID.Adaptability)
                stab = 2f;
            else
                stab = 1.5f;
        }
        else
            stab = 1f;

        if (move.Base.MoveName == "Facade" && (attacker.Status?.Id == StatusConditionID.Poison || attacker.Status?.Id == StatusConditionID.BadPoison || attacker.Status?.Id == StatusConditionID.Burn || attacker.Status?.Id == StatusConditionID.Paralyze))
            statusModifier = 2f;
        else if (move.Base.MoveName == "Hex" && defender.Status?.Id != StatusConditionID.None)
            statusModifier = 2f;
        else if (move.Base.MoveName == "Wake-Up Slap" && defender.Status?.Id == StatusConditionID.Sleep)
            statusModifier = 2f;
        else
        {
            if (attacker.Status?.Id == StatusConditionID.Burn && move.Base.Category == MoveCategory.Physical && attacker.Ability?.Id != AbilityID.Guts)
                statusModifier = 0.5f;
            else
                statusModifier = 1f;
        }

        if (move.Base.MoveName == "Frustration")
            power = Mathf.Max(1, Mathf.FloorToInt((float)(255 - Friendship) / 2.5f));
        else if (move.Base.MoveName == "Return")
            power = Mathf.Max(1, Mathf.FloorToInt((float)Friendship / 2.5f));
        else if (move.Base.MoveName == "Magnitude")
        {
            if (magnitudeStrength < 5)
                power = 10f;
            else if (magnitudeStrength >= 5 && magnitudeStrength < 15)
                power = 30f;
            else if (magnitudeStrength >= 15 && magnitudeStrength < 35)
                power = 50f;
            else if (magnitudeStrength >= 35 && magnitudeStrength < 65)
                power = 70f;
            else if (magnitudeStrength >= 65 && magnitudeStrength < 85)
                power = 90f;
            else if (magnitudeStrength >= 85 && magnitudeStrength < 95)
                power = 110f;
            else
                power = 150f;
        }
        else if (move.Base.MoveName == "Flail" || move.Base.MoveName == "Reversal")
        {
            var healthRemainingPercent = ((float)HP / (float)MaxHP) * 100f;
            if (healthRemainingPercent >= 68.8f)
                power = 20f;
            else if (healthRemainingPercent >= 35.4f && healthRemainingPercent < 68.8f)
                power = 40f;
            else if (healthRemainingPercent >= 20.8f && healthRemainingPercent < 35.4f)
                power = 80f;
            else if (healthRemainingPercent >= 10.4f && healthRemainingPercent < 20.8f)
                power = 100f;
            else if (healthRemainingPercent >= 4.2f && healthRemainingPercent < 10.4f)
                power = 150f;
            else if (healthRemainingPercent < 4.2f)
                power = 200f;
        }

        power = attacker.OnBasePower(power, this, move);
        
        if (move.Base.Category == MoveCategory.Special)
        {
            attack = attacker.SpAttack;
            defense = SpDefense;

            // Abilites & Held Items might modify the stats
            attack = attacker.ModifySpAtk(attack, defender, move);
            defense = ModifySpDef(defense, attacker, move);
        }
        else
        {
            attack = attacker.Attack;
            defense = Defense;

            // Abilites & Held might modify the stats
            attack = attacker.ModifyAtk(attack, defender, move);
            defense = ModifyDef(defense, attacker, move);
        }

        modifiers = randomness * MoveTypeEffectiveness * CriticalDamage * weatherModifier * stab * statusModifier;
        a = ((2 * attacker.Level) / 5) + 2;
        d = ((a * power * ((float)attack / defense)) / 50) + 2;

        if ((defender.Ability?.Id == AbilityID.WonderGuard && MoveTypeEffectiveness <= 1) || (defender.Ability?.Id == AbilityID.Damp && move.Base.HasFlag(MoveFlag.Explode)) || (defender.Ability?.Id == AbilityID.Bulletproof && move.Base.HasFlag(MoveFlag.BallBomb)) || (defender.Ability?.Id == AbilityID.WindRider && move.Base.HasFlag(MoveFlag.Wind)) || (defender.Ability?.Id == AbilityID.Soundproof && move.Base.HasFlag(MoveFlag.Sound)))
        {
            MoveTypeEffectiveness = 0;
            damage = 0;
        }
        else
        {
            if (move.Base.DamageType == DamageType.Fixed)
                damage = move.Base.Power;
            else if (move.Base.DamageType == DamageType.Level)
                damage = Level;
            else if (move.Base.DamageType == DamageType.Psywave)
                damage = (int)PsywaveStrength;
            else
                damage = Mathf.FloorToInt(d * modifiers);
        }

        return damage;
    }
    public DamageDetails TakeDamage(Move move, Pokemon attacker, Pokemon defender, float weatherModifier = 1f, bool heal = false, bool recoil = false)
    {
        if (move.Base.MoveName == "Magnitude")
            MagnitudeStrength = UnityEngine.Random.Range(0, 100);

        CriticalThreshold = UnityEngine.Random.Range(0, 100);

        int damage = CalculateDamage(move, attacker, defender, CriticalThreshold, MagnitudeStrength, weatherModifier, heal, recoil);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = MoveTypeEffectiveness,
            Critical = CriticalDamage,
            Fainted = false,
            Damage = damage,
            Attacker = attacker
        };

        if (move.Base.Power > 0 && MoveTypeEffectiveness > 0)
        {
            if (heal)
                attacker.IncreaseHP(Mathf.Max(1, Mathf.FloorToInt((float)damage / 2f)));
            else if (recoil)
                DecreaseHP(Mathf.Max(1, Mathf.FloorToInt((float)damage * ((float)move.Base.RecoilPercent / 100f))));
            else
            {
                if ((move.Base.MoveName == "False Swipe" && damage >= HP) || (Ability?.Id == AbilityID.Sturdy && damage >= MaxHP && HP == MaxHP))
                    DecreaseHP(HP - 1);
                else
                    DecreaseHP(Mathf.Max(1, damage));
            }
        }

        if (damage > 0)
            Ability?.OnDamagingHit?.Invoke(damage, attacker, defender, move);

        return damageDetails;
    }

    public void IncreaseHP(int healAmount)
    {
        HP = Mathf.Clamp(HP + healAmount, 0, MaxHP);
        OnHPChanged?.Invoke();
    }

    public void DecreaseHP(int damage, bool callUpdateEvent = false)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        
        if (callUpdateEvent)
            OnHPChanged?.Invoke();
    }

    public void ChangeFriendship(int lowFriendshipBoost, int medFriendshipBoost, int highFriendshipBoost)
    {
        if (Friendship < 100)
            Friendship = Mathf.Clamp(Friendship + lowFriendshipBoost, 0, 255);
        else if (Friendship >= 100 && Friendship < 200)
            Friendship = Mathf.Clamp(Friendship + medFriendshipBoost, 0, 255);
        else if (Friendship >= 200)
            Friendship = Mathf.Clamp(Friendship + highFriendshipBoost, 0, 255);
    }

    public void SetStatus(StatusConditionID conditionID, EffectSource effect = EffectSource.Move)
    {
        if (Status != null) return;

        bool canSet = Ability?.OnTrySetStatus?.Invoke(conditionID, this, effect) ?? true;
        if (!canSet) return;

        Status = StatusConditionsDB.StatusConditions[conditionID];

        if ((Status == StatusConditionsDB.StatusConditions[StatusConditionID.Poison] || Status == StatusConditionsDB.StatusConditions[StatusConditionID.BadPoison]) && (IsOfType(PokemonType.Poison) || IsOfType(PokemonType.Steel)))
        {
            AddStatusEvent($"{Base.PokemonName}'s immune to poison!");
            Status = null;
            return;
        }
        else if (Status == StatusConditionsDB.StatusConditions[StatusConditionID.Burn] && IsOfType(PokemonType.Fire))
        {
            AddStatusEvent($"{Base.PokemonName}'s immune to burning!");
            Status = null;
            return;
        }
        else if (Status == StatusConditionsDB.StatusConditions[StatusConditionID.Paralyze] && (IsOfType(PokemonType.Electric) || IsOfType(PokemonType.Ground)))
        {
            AddStatusEvent($"{Base.PokemonName}'s immune to paralysis!");
            Status = null;
            return;
        }
        else if (Status == StatusConditionsDB.StatusConditions[StatusConditionID.Freeze] && IsOfType(PokemonType.Ice))
        {
            AddStatusEvent($"{Base.PokemonName}'s immune to freezing!");
            Status = null;
            return;
        }

        Status?.OnStart?.Invoke(this);
        AddStatusEvent($"{Base.PokemonName} {Status.StatusConditionStartMessage}");
        CalculateStats();
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        CalculateStats();
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(StatusConditionID conditionID, Pokemon attacker, int playerPriority, int attackerPriority, EffectSource effect = EffectSource.Move)
    {
        if (VolatileStatus != null) return;

        bool canSet = Ability?.OnTrySetVolatile?.Invoke(conditionID, this, effect) ?? true;
        if (!canSet) return;

        VolatileStatus = StatusConditionsDB.StatusConditions[conditionID];
        if (VolatileStatus == StatusConditionsDB.StatusConditions[StatusConditionID.Flinch])
        {
            if (attackerPriority < playerPriority || (attacker.Speed < Speed && attackerPriority == playerPriority))
            {
                CureVolatileStatus();
                return;
            }
        }

        VolatileStatus?.OnStart?.Invoke(this);

        AddStatusEvent($"{Base.PokemonName} {VolatileStatus.StatusConditionStartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move ChooseMove(Pokemon targetPokemon, TrainerController trainerController = null, GymLeaderController gymLeaderController = null, DoubleTrainerController doubleTrainerController = null)
    {
        int[] moveWeights = new int[4];
        var trainer = trainerController;

        if (doubleTrainerController != null)
            trainer = doubleTrainerController;
        else if (gymLeaderController != null)
            trainer = gymLeaderController;
        else if (trainerController == null)
            trainer = null;


        moveWeights[0] = 20;
        if (Moves.Count >= 2)
        {
            moveWeights[1] = 20;
            if (Moves.Count >= 3)
            {
                moveWeights[2] = 20;
                if (Moves.Count == 4)
                    moveWeights[3] = 20;
                else
                    moveWeights[3] = 0;
            }
            else
            {
                moveWeights[2] = 0;
                moveWeights[3] = 0;
            }

            if (trainer != null ? trainer.basicAI : false)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    if ((Moves[i].Base.Effects.Status == StatusConditionID.Poison || Moves[i].Base.Effects.Status == StatusConditionID.Burn || Moves[i].Base.Effects.Status == StatusConditionID.Paralyze || Moves[i].Base.Effects.Status == StatusConditionID.Sleep || Moves[i].Base.Effects.Status == StatusConditionID.Freeze) && targetPokemon.Status != null)
                        moveWeights[i] -= 10;
                }
            }

            if (trainer != null ? trainer.setupAI : false)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    if (Moves[i].Base.Category == MoveCategory.Status && Moves[i].Base.Effects.Boosts != null)
                    {
                        if (TurnCount == 0)
                            moveWeights[i] += 18;
                        else
                            moveWeights[i] -= 6;
                    }
                }
            }

            if (trainer != null ? trainer.typesAI : false)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    if (Moves[i].Base.Category != MoveCategory.Status)
                    {
                        float effectiveness = TypeChart.GetEffectiveness(Moves[i].Base.MoveType, targetPokemon.Base.PokemonType1) * TypeChart.GetEffectiveness(Moves[i].Base.MoveType, targetPokemon.Base.PokemonType2);
                        if (effectiveness == 4f)
                            moveWeights[i] += 9;
                        else if (effectiveness == 2f)
                            moveWeights[i] += 6;
                        else if (effectiveness == 0.5f)
                            moveWeights[i] -= 3;
                        else if (effectiveness == 0.25f)
                            moveWeights[i] -= 5;
                        else if (effectiveness == 0f)
                            moveWeights[i] = -20;
                    }
                }
            }

            if (trainer != null ? trainer.offensiveAI : false)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    if (Moves[i].Base.Category == MoveCategory.Status)
                        moveWeights[i] -= 10;
                }
            }

            if (trainer != null ? trainer.aggressiveAI : false)
            {
                int power = 0;
                for (int i = 0; i < Moves.Count; i++)
                {
                    if (CalculateDamage(Moves[i], this, targetPokemon, CriticalThreshold, MagnitudeStrength) > power)
                        power = CalculateDamage(Moves[i], this, targetPokemon, CriticalThreshold, MagnitudeStrength);
                }
                for (int i = 0; i < Moves.Count; i++)
                {
                    if (CalculateDamage(Moves[i], this, targetPokemon, CriticalThreshold, MagnitudeStrength) == power)
                        moveWeights[i] += 12;
                    else if (Moves[i].Base.IsMultistrikeMove)
                        moveWeights[i] += 8;
                }
            }

            if (trainer != null ? trainer.cautiousAI : false)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    if ((Moves[i].Base.Effects.Status == StatusConditionID.Poison || Moves[i].Base.Effects.Status == StatusConditionID.Burn || Moves[i].Base.Effects.Status == StatusConditionID.Paralyze) || Moves[i].Base.MoveName == "Leech Seed" || Moves[i].Base.MoveName == "Focus Energy")
                    {
                        if (TurnCount == 0)
                            moveWeights[i] += 15;
                        else
                            moveWeights[i] -= 3;
                    }
                }
            }

            if (trainer != null ? trainer.riskyAI : false)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    if (Moves[i].Base.Effects.Recoil && this.HP < this.MaxHP)
                    {
                        moveWeights[i] += 4;
                        if (CalculateDamage(Moves[i], this, targetPokemon, CriticalThreshold, MagnitudeStrength) >= targetPokemon.HP)
                            moveWeights[i] += 8;
                    }
                }
            }

            if (trainer != null ? trainer.opportunistAI : false)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    if (HP / MaxHP <= 0.5f)
                    {
                        if (HP / MaxHP > 0.25f)
                        {
                            if (Moves[i].Base.Category == MoveCategory.Status)
                                moveWeights[i] -= 3;
                        }
                        else
                        {
                            if (Moves[i].Base.Category == MoveCategory.Status)
                                moveWeights[i] -= 5;
                        }
                    }
                }
            }

            if (trainer != null ? trainer.statusAI : false)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    if (Moves[i].Base.SecondaryEffects != null)
                    {
                        foreach (var secondary in Moves[i].Base.SecondaryEffects)
                        {
                            if (secondary.Status == StatusConditionID.Poison && (targetPokemon.IsOfType(PokemonType.Poison) || targetPokemon.IsOfType(PokemonType.Steel)))
                                moveWeights[i] -= 3;
                            else if (secondary.Status == StatusConditionID.Burn && targetPokemon.IsOfType(PokemonType.Fire))
                                moveWeights[i] -= 3;
                            else if (secondary.Status == StatusConditionID.Paralyze && (targetPokemon.IsOfType(PokemonType.Electric) || targetPokemon.IsOfType(PokemonType.Ground)))
                                moveWeights[i] -= 3;
                            else if (secondary.Status == StatusConditionID.Freeze && targetPokemon.IsOfType(PokemonType.Ice))
                                moveWeights[i] -= 3;
                        }
                    }
                    else if (Moves[i].Base.Effects.Status == StatusConditionID.Poison && (targetPokemon.IsOfType(PokemonType.Poison) || targetPokemon.IsOfType(PokemonType.Steel)))
                        moveWeights[i] -= 5;
                    else if (Moves[i].Base.Effects.Status == StatusConditionID.Burn && targetPokemon.IsOfType(PokemonType.Fire))
                        moveWeights[i] -= 5;
                    else if (Moves[i].Base.Effects.Status == StatusConditionID.Paralyze && (targetPokemon.IsOfType(PokemonType.Electric) || targetPokemon.IsOfType(PokemonType.Ground)))
                        moveWeights[i] -= 5;
                    else if (Moves[i].Base.Effects.Status == StatusConditionID.Freeze && targetPokemon.IsOfType(PokemonType.Ice))
                        moveWeights[i] -= 5;
                }
            }

            if (moveWeights[0] < 0)
                moveWeights[0] = 0;

            if (moveWeights[1] < 0)
                moveWeights[1] = 0;
            
            if (moveWeights[2] < 0)
                moveWeights[2] = 0;
            
            if (moveWeights[3] < 0)
                moveWeights[3] = 0;

            if (moveWeights[0] == 0 && moveWeights[1] == 0 && moveWeights[2] == 0 && moveWeights[3] == 0)
            {
                moveWeights[0] = 20;
                if (Moves.Count >= 2)
                {
                    moveWeights[1] = 20;
                    if (Moves.Count >= 3)
                    {
                        moveWeights[2] = 20;
                        if (Moves.Count == 4)
                            moveWeights[3] = 20;
                        else
                            moveWeights[3] = 0;
                    }
                }
                else
                {
                    moveWeights[1] = 0;
                    moveWeights[2] = 0;
                    moveWeights[3] = 0;
                }
            }
        }
        else
        {
            moveWeights[1] = 0;
            moveWeights[2] = 0;
            moveWeights[3] = 0;
        }

        int weightTotal = 0;
        foreach (int w in moveWeights)
        {
            weightTotal += w;
        }

        int result;
        int total = 0;
        int randomMove = UnityEngine.Random.Range(0, weightTotal);
        for (result = 0; result < moveWeights.Length; result++)
        {
            total += moveWeights[result];
            if (total > randomMove) break;
        }
        return Moves[result];
    }
    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }

        TurnCount++;
        CriticalThreshold = UnityEngine.Random.value * 100f;
        MagnitudeStrength = UnityEngine.Random.Range(0, 100);
        PsywaveStrength = UnityEngine.Random.Range(0, Mathf.FloorToInt((float)Level * 1.5f)) + 1;
        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnAfterTurnSeeded(Pokemon attacker)
    {
        VolatileStatus?.OnAfterTurnSeeded?.Invoke(this, attacker);
    }

    public void OnAfterTurnSeeder(Pokemon attacker)
    {
        VolatileStatus?.OnAfterTurnSeeder?.Invoke(attacker, this);
    }

    public void OnBattleOver()
    {
        CureVolatileStatus();
        ResetStatBoost();
        CalculateStats();
        TurnCount = 0;
        CriticalThreshold = 100f;
        MagnitudeStrength = -1;
        PsywaveStrength = -1f;
    }

    public void AddStatusEvent(StatusEventType type, string message)
    {
        StatusChanges.Enqueue(new StatusEvent(type, message));
    }

    public void AddStatusEvent(string message)
    {
        AddStatusEvent(StatusEventType.Text, message);
    }

    public float OnBasePower(float basePower, Pokemon defender, Move move)
    {
        if (Ability?.OnBasePower != null)
            basePower = Ability.OnBasePower(basePower, this, defender, move);

        return basePower;
    }

    public float ModifyAtk(float atk, Pokemon defender, Move move)
    {
        if (Ability?.OnModifyAtk != null)
            return Ability.OnModifyAtk(atk, this, defender, move);

        return atk;
    }

    public float ModifySpAtk(float atk, Pokemon defender, Move move)
    {
        if (Ability?.OnModifySpAtk != null)
            return Ability.OnModifySpAtk(atk, this, defender, move);

        return atk;
    }

    public float ModifyDef(float def, Pokemon attacker, Move move)
    {
        if (Ability?.OnModifyDef != null)
            return Ability.OnModifyDef(def, attacker, this, move);

        return def;
    }

    public float ModifySpDef(float def, Pokemon attacker, Move move)
    {
        if (Ability?.OnModifySpDef != null)
            return Ability.OnModifySpDef(def, attacker, this, move);

        return def;
    }

    public float ModifySpd(float spd, Pokemon defender, Move move)
    {
        if (Ability?.OnModifySpd != null)
            return Ability.OnModifySpd(spd, this, defender, move);

        return spd;
    }

    public float ModifyAcc(float acc, Pokemon defender, Move move)
    {
        if (Ability?.OnModifyAcc != null)
            return Ability.OnModifyAcc(acc, this, defender, move);

        return acc;
    }
}

public enum Natures
{
    Hardy,
    Lonely,
    Brave,
    Adamant,
    Naughty,
    Bold,
    Docile,
    Relaxed,
    Impish,
    Lax,
    Timid,
    Hasty,
    Serious,
    Jolly,
    Naive,
    Modest,
    Mild,
    Quiet,
    Bashful,
    Rash,
    Calm,
    Gentle,
    Sassy,
    Careful,
    Quirky
}

public enum Genders
{
    Male, Female, Genderless
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
    public int Damage { get; set; }
    public Pokemon Attacker { get; set; }
}

[System.Serializable]
public class PokemonSaveData
{
    public string pokemonName;
    public int hp;
    public int level;
    public int exp;
    public int hpIV;
    public int attackIV;
    public int defenseIV;
    public int spAttackIV;
    public int spDefenseIV;
    public int speedIV;
    public int boostedStat;
    public int loweredStat;
    public int genderNumber;
    public int friendship;
    public GenderRatio genderRatio;
    public int abilityNumber;
    public int shinyNumber;
    public AbilityID abilityID;
    public Dictionary<Stat, int> statEffortValues;
    public StatusConditionID? statusID;
    public List<MoveSaveData> moves;
}

public enum StatusEventType { Text, Damage, StatBoost }

public class StatusEvent
{
    public StatusEventType Type { get; private set; }
    public string Message { get; private set; }

    public StatusEvent(StatusEventType type, string message)
    {
        Type = type;
        Message = message;
    }
}