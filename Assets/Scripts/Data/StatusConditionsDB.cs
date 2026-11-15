using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusConditionsDB
{
    public static void Initialize()
    {
        foreach (var keyValuePair in StatusConditions)
        {
            var conditionId = keyValuePair.Key;
            var condition = keyValuePair.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<StatusConditionID, StatusCondition> StatusConditions { get; set; } = new Dictionary<StatusConditionID, StatusCondition>()
    {
        {
            StatusConditionID.None,
            new StatusCondition()
            {
                StatusConditionName = "None",
                StatusConditionStartMessage = "",
            }
        },
        {
            StatusConditionID.Poison,
            new StatusCondition()
            {
                StatusConditionName = "Poison",
                StatusConditionStartMessage = "was poisoned!",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.DecreaseHP(Mathf.Max(1, pokemon.MaxHP / 8));
                    pokemon.AddStatusEvent(StatusEventType.Damage, $"{pokemon.Base.PokemonName} was hurt by its poisoning!");
                }
            }
        },
        {
            StatusConditionID.BadPoison,
            new StatusCondition()
            {
                StatusConditionName = "Bad Poison",
                StatusConditionStartMessage = "was badly poisoned!",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = 1;
                },
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.DecreaseHP(Mathf.Max(1, (pokemon.MaxHP / 8) * pokemon.StatusTime));
                    pokemon.AddStatusEvent(StatusEventType.Damage, $"{pokemon.Base.PokemonName} was hurt by its poisoning!");
                    pokemon.StatusTime++;
                }
            }
        },
        {
            StatusConditionID.Burn,
            new StatusCondition()
            {
                StatusConditionName = "Burn",
                StatusConditionStartMessage = "was burned!",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.DecreaseHP(Mathf.Max(1, pokemon.MaxHP / 16));
                    pokemon.AddStatusEvent(StatusEventType.Damage, $"{pokemon.Base.PokemonName} was hurt by its burn!");
                }
            }
        },
        {
            StatusConditionID.Sleep,
            new StatusCondition()
            {
                StatusConditionName = "Sleep",
                StatusConditionStartMessage = "fell asleep!",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.StatusTime = Random.Range(1, 4);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName} woke up!");
                        return true;
                    }

                    pokemon.StatusTime--;
                    pokemon.AddStatusEvent($"{pokemon.Base.PokemonName} is fast asleep!");
                    return false;
                }
            }
        },
        {
            StatusConditionID.Paralyze,
            new StatusCondition()
            {
                StatusConditionName = "Paralyze",
                StatusConditionStartMessage = "was paralyzed! It may be unable to move!",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName} couldn't move because it's paralyzed!");
                        return false;
                    }

                    pokemon.AddStatusEvent($"{pokemon.Base.PokemonName} is paralyzed! It may be unable to move!");
                    return true;
                }
            }
        },
        {
            StatusConditionID.Freeze,
            new StatusCondition()
            {
                StatusConditionName = "Freeze",
                StatusConditionStartMessage = "was frozen solid!",
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (Random.Range(1, 6) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName} thawed out!");
                        return true;
                    }

                    pokemon.AddStatusEvent($"{pokemon.Base.PokemonName} is frozen solid! It can't move!");
                    return false;
                }
            }
        },

        // Volatile Statuses
        {
            StatusConditionID.Confuse,
            new StatusCondition()
            {
                StatusConditionName = "Confuse",
                StatusConditionStartMessage = "became confused!",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = Random.Range(1, 5);
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName} snapped out of confusion!");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    pokemon.AddStatusEvent($"{pokemon.Base.PokemonName} is confused!");

                    if (Random.Range(1, 3) == 1)
                        return true;
                    else
                    {
                        float a = ((2 * pokemon.Level) / 5) + 2;
                        float confusionDamage = ((a * 40 * ((float)pokemon.Attack / pokemon.Defense)) / 50) + 2;
                        pokemon.DecreaseHP(Mathf.Max(1, Mathf.FloorToInt(confusionDamage * Random.Range(0.85f, 1f))));
                        pokemon.AddStatusEvent("It hurt itself in its confusion!");
                        return false;
                    }
                }
            }
        },
        {
            StatusConditionID.Flinch,
            new StatusCondition()
            {
                StatusConditionName = "Flinch",
                StatusConditionStartMessage = "flinched!",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = 1;
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        return true;
                    }
                    pokemon.VolatileStatusTime--;
                    return false;
                }
            }
        },
        {
            StatusConditionID.Seeded,
            new StatusCondition()
            {
                StatusConditionName = "Seeded",
                StatusConditionStartMessage = "was seeded!",
                OnAfterTurnSeeded = (Pokemon pokemon, Pokemon attacker) =>
                {
                    pokemon.DecreaseHP(Mathf.Max(1, pokemon.MaxHP / 8));
                    pokemon.AddStatusEvent(StatusEventType.Damage, $"{pokemon.Base.PokemonName}'s health was sapped!");
                },
                OnAfterTurnSeeder = (Pokemon pokemon, Pokemon attacker) =>
                {
                    pokemon.IncreaseHP(Mathf.Max(1, attacker.MaxHP / 16));
                    pokemon.AddStatusEvent(StatusEventType.Damage, $"{pokemon.Base.PokemonName} regained health!");
                }
            }
        },
        {
            StatusConditionID.Drowsy,
            new StatusCondition()
            {
                StatusConditionName = "Drowsy",
                StatusConditionStartMessage = "became drowsy!",
                OnStart = (Pokemon pokemon) =>
                {
                    pokemon.VolatileStatusTime = 1;
                },
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.SetStatus(StatusConditionID.Sleep);
                        pokemon.CureVolatileStatus();
                        return false;
                    }
                    pokemon.VolatileStatusTime--;
                    return true;
                }
            }
        },
    };

    public static float GetStatusBonus(StatusCondition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.Id == StatusConditionID.Sleep || condition.Id == StatusConditionID.Freeze)
            return 2f;
        else if (condition.Id == StatusConditionID.Poison || condition.Id == StatusConditionID.Burn || condition.Id == StatusConditionID.Paralyze)
            return 1.5f;

        return 1f;
    }
}

public enum StatusConditionID
{
    // Status Effects
    None,
    Poison,
    Burn,
    Sleep,
    Paralyze,
    Freeze,
    BadPoison,

    // Volatile statuses
    Confuse,
    Flinch,
    Seeded,
    Drowsy
}