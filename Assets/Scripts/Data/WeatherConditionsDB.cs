using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherConditionsDB
{
    public static void Initialize()
    {
        foreach (var keyValuePair in Conditions)
        {
            var conditionID = keyValuePair.Key;
            var condition = keyValuePair.Value;

            condition.Id = conditionID;
        }
    }

    public static Dictionary<WeatherConditionID, WeatherCondition> Conditions = new Dictionary<WeatherConditionID, WeatherCondition>()
    {
        {
            WeatherConditionID.HarshSunlight,
            new WeatherCondition()
            {
                Name = "Harsh Sunlight",
                StartMessage = "The sunlight is strong.",
                StartByMoveMessage = "The sunlight turned harsh!",
                EffectMessage = "The sunlight is strong.",
                EndMessage = "The harsh sunlight faded.",
                OnDamageModify = (Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Fire)
                        return 1.5f;
                    else if (move.Base.MoveType == PokemonType.Water)
                        return 0.5f;

                    return 1f;
                }
            }
        },
        {
            WeatherConditionID.Rain,
            new WeatherCondition()
            {
                Name = "Heavy Rain",
                StartMessage = "It is raining.",
                StartByMoveMessage = "It started to rain!",
                EffectMessage = "Rain continues to fall.",
                EndMessage = "It stopped raining.",
                OnDamageModify = (Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Water)
                        return 1.5f;
                    else if (move.Base.MoveType == PokemonType.Fire)
                        return 0.5f;

                    return 1f;
                }
            }
        },
        {
            WeatherConditionID.Sandstorm,
            new WeatherCondition()
            {
                Name = "Sandstorm",
                StartMessage = "The sandstorm is raging!",
                StartByMoveMessage = "A sandstorm kicked up!",
                EffectMessage = "The sandstorm rages.",
                EndMessage = "The sandstorm subsided.",
                OnWeatherEffect = (Pokemon pokemon) =>
                {
                    if (pokemon.IsOfType(PokemonType.Ground) || pokemon.IsOfType(PokemonType.Rock) || pokemon.IsOfType(PokemonType.Steel)) return;

                    pokemon.DecreaseHP(pokemon.MaxHP / 16);
                    pokemon.AddStatusEvent(StatusEventType.Damage, $"{pokemon.Base.PokemonName} is buffeted by the sandstorm!");
                }
            }
        },
        {
            WeatherConditionID.Hail,
            new WeatherCondition()
            {
                Name = "Hail",
                StartMessage = "It's hailing!",
                StartByMoveMessage = "It started to hail!",
                EffectMessage = "Hail continues to fall.",
                EndMessage = "The hail stopped.",
                OnWeatherEffect = (Pokemon pokemon) =>
                {
                    if (pokemon.IsOfType(PokemonType.Ice)) return;

                    pokemon.DecreaseHP(pokemon.MaxHP / 16);
                    pokemon.AddStatusEvent(StatusEventType.Damage, $"{pokemon.Base.PokemonName} is buffeted by the hail!");
                }
            }
        }
    };
}

public class WeatherCondition
{
    public WeatherConditionID Id { get; set; }
    public string Name { get; set; }
    public string StartMessage { get; set; }
    public string StartByMoveMessage { get; set; }
    public string EffectMessage { get; set; }
    public string EndMessage { get; set; }

    public Action<Pokemon> OnWeatherEffect { get; set; }
    public Func<Move, float> OnDamageModify { get; set; }
}

public enum WeatherConditionID
{
    None, HarshSunlight, Rain, Sandstorm, Hail
}
