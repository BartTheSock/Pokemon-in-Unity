using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Add new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea]
    [SerializeField] string moveDescription;

    [SerializeField] PokemonType moveType;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] bool highCriticalHitRatio;
    [SerializeField] bool alwaysCriticalHit;
    [SerializeField] int recoilPercent;

    [SerializeField] DamageType damageType;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;
    [SerializeField] bool isMultistrikeMove = false;
    [SerializeField] Vector2Int strikeRange;
    [SerializeField] AudioClip sound;
    [SerializeField] List<MoveFlag> flags;

    public static void InitializeMoves()
    {
        var movesList = Resources.LoadAll<MoveBase>("");
        foreach (var move in movesList)
        {
            move.effects.Source = EffectSource.Move;
        }
    }

    public bool HasFlag(MoveFlag flag)
    {
        return flags.Contains(flag);
    }

    public int GetStrikeTimes()
    {
        if (strikeRange == Vector2Int.zero || !isMultistrikeMove)
            return 1;

        int strikeCount = 1;
        if (strikeRange.y == 0)
        {
            strikeCount = strikeRange.x;
        }
        else
        {
            if (strikeRange.x == 2 && strikeRange.y == 5)
            {
                if (Random.Range(0, 100) < 70)
                    strikeCount = Random.Range(2, 4);
                else
                    strikeCount = Random.Range(4, 6);
            }
            else
                strikeCount = Random.Range(strikeRange.x, strikeRange.y + 1);
        }

        return strikeCount;
    }

    public string MoveName {
        get { return moveName; }
    }

    public string MoveDescription {
        get { return moveDescription; }
    }

    public PokemonType MoveType {
        get { return moveType; }
    }

    public int Power {
        get { return power; }
    }

    public int Accuracy {
        get { return accuracy; }
    }

    public bool AlwaysHits {
        get { return alwaysHits; }
    }

    public int PP {
        get { return pp; }
    }

    public int Priority {
        get { return priority; }
    }

    public bool HighCriticalHitRatio {
        get { return highCriticalHitRatio; }
    }

    public bool AlwaysCriticalHit {
        get { return alwaysCriticalHit; }
    }

    public DamageType DamageType {
        get { return damageType; }
    }

    public int RecoilPercent {
        get { return recoilPercent; }
    }

    public MoveCategory Category {
        get { return category; }
    }

    public MoveEffects Effects {
        get { return effects; }
    }

    public List<SecondaryEffects> SecondaryEffects {
        get { return secondaryEffects; }
    }

    public MoveTarget Target {
        get { return target; }
    }

    public bool IsMultistrikeMove => isMultistrikeMove;

    public AudioClip Sound {
        get { return sound; }
    }

    public List<MoveFlag> Flags {
        get { return flags; }
    }
}

public class EffectData
{
    public EffectSource Source { get; set; }
    public int SourceId { get; set; }
}

[System.Serializable]
public class MoveEffects : EffectData
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] StatusConditionID status;
    [SerializeField] StatusConditionID volatileStatus;
    [SerializeField] WeatherConditionID weather;
    [SerializeField] bool healUser;
    [SerializeField] bool recoil;

    public List<StatBoost> Boosts {
        get { return boosts; }
    }

    public StatusConditionID Status {
        get { return status; }
    }

    public StatusConditionID VolatileStatus {
        get { return volatileStatus; }
    }

    public WeatherConditionID Weather => weather;

    public bool HealUser {
        get { return healUser; }
    }

    public bool Recoil {
        get { return recoil; }
    }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance {
        get { return chance; }
    }

    public MoveTarget Target {
        get { return target; }
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical, Special, Status
}

public enum DamageType
{
    Normal, Fixed, Level, Psywave
}

public enum MoveTarget
{
    Foe, Self
}

public enum EffectSource
{
    Ability, Item, Move, Condition
}

public enum MoveFlag
{
    Contact, Punch, Bite, Pulse, Sound, Slice, BallBomb, Explode, Wind
}
