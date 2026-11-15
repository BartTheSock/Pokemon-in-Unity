using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounterRecord> wildPokemonListInGrass;
    [SerializeField] List<PokemonEncounterRecord> wildPokemonListInWater;
    [SerializeField] List<PokemonEncounterRecord> wildPokemonListInCaves;
    [SerializeField] List<PokemonEncounterRecord> wildPokemonListBreakRocks;
    [SerializeField] List<PokemonEncounterRecord> staticPokemonOne;
    [SerializeField] List<PokemonEncounterRecord> staticPokemonTwo;

    [SerializeField] WeatherConditionID weather;
    public WeatherConditionID Weather => weather;

    [HideInInspector]
    [SerializeField] float totalChanceGrass = -1f;
    [HideInInspector]
    [SerializeField] float totalChanceWater = -1f;
    [HideInInspector]
    [SerializeField] float totalChanceCave = -1f;
    [HideInInspector]
    [SerializeField] float totalChanceRock = -1f;
    [HideInInspector]
    [SerializeField] float totalStaticPokemonOne = -1f;
    [HideInInspector]
    [SerializeField] float totalStaticPokemonTwo = -1f;

    private void OnValidate()
    {
        CalculateChancePercentage();
        CalculateChancePercentageStatic();
    }

    private void Start()
    {
        CalculateChancePercentage();
        CalculateChancePercentageStatic();
    }

    void CalculateChancePercentage()
    {
        totalChanceGrass = -1;
        totalChanceWater = -1;
        totalChanceCave = -1;
        totalChanceRock = -1;

        if (wildPokemonListInGrass.Count > 0)
        {
            totalChanceGrass = 0;
            foreach (var record in wildPokemonListInGrass)
            {
                record.chanceLower = totalChanceGrass;
                record.chanceUpper = totalChanceGrass + record.chancePercent;

                totalChanceGrass = totalChanceGrass + record.chancePercent;
            }
        }

        if (wildPokemonListInWater.Count > 0)
        {
            totalChanceWater = 0;
            foreach (var record in wildPokemonListInWater)
            {
                record.chanceLower = totalChanceWater;
                record.chanceUpper = totalChanceWater + record.chancePercent;

                totalChanceWater = totalChanceWater + record.chancePercent;
            }
        }

        if (wildPokemonListInCaves.Count > 0)
        {
            totalChanceCave = 0;
            foreach (var record in wildPokemonListInCaves)
            {
                record.chanceLower = totalChanceCave;
                record.chanceUpper = totalChanceCave + record.chancePercent;
                
                totalChanceCave = totalChanceCave + record.chancePercent;
            }
        }
            
        if (wildPokemonListBreakRocks.Count > 0)
        {
            totalChanceRock = 0;
            foreach (var record in wildPokemonListBreakRocks)
            {
                record.chanceLower = totalChanceRock;
                record.chanceUpper = totalChanceRock + record.chancePercent;

                totalChanceRock = totalChanceRock + record.chancePercent;
            }
        }
    }

    void CalculateChancePercentageStatic()
    {
        totalStaticPokemonOne = -1;
        totalStaticPokemonTwo = -1;

        if (staticPokemonOne.Count > 0)
        {
            totalStaticPokemonOne = 0;
            foreach (var record in staticPokemonOne)
            {
                record.chanceLower = totalStaticPokemonOne;
                record.chanceUpper = totalStaticPokemonOne + record.chancePercent;

                totalStaticPokemonOne = totalStaticPokemonOne + record.chancePercent;
            }
        }

        if (staticPokemonTwo.Count > 0)
        {
            totalStaticPokemonTwo = 0;
            foreach (var record in staticPokemonTwo)
            {
                record.chanceLower = totalStaticPokemonTwo;
                record.chanceUpper = totalStaticPokemonTwo + record.chancePercent;

                totalStaticPokemonTwo = totalStaticPokemonTwo + record.chancePercent;
            }
        }
    }

    public Pokemon GetRandomWildPokemon(BattleTrigger trigger)
    {
        var pokemonList = wildPokemonListInGrass;

        if (trigger == BattleTrigger.PlainsRock || trigger == BattleTrigger.ForestRock || trigger == BattleTrigger.CaveRock || trigger == BattleTrigger.MountainRock || trigger == BattleTrigger.SandRock || trigger == BattleTrigger.DeepSandRock)
            pokemonList = wildPokemonListBreakRocks;
        else if (trigger == BattleTrigger.WaterEdge || trigger == BattleTrigger.Water || trigger == BattleTrigger.CaveWater)
            pokemonList = wildPokemonListInWater;
        else if (trigger == BattleTrigger.Mountain || trigger == BattleTrigger.Cave || trigger == BattleTrigger.BuildingA || trigger == BattleTrigger.BuildingB || trigger == BattleTrigger.BuildingC || trigger == BattleTrigger.Sand || trigger == BattleTrigger.DeepSand || trigger == BattleTrigger.Snow || trigger == BattleTrigger.CaveIce)
            pokemonList = wildPokemonListInCaves;
        else if (trigger == BattleTrigger.Omanyte || trigger == BattleTrigger.Aerodactyl || trigger == BattleTrigger.Lileep || trigger == BattleTrigger.Cranidos || trigger == BattleTrigger.Tirtouga || trigger == BattleTrigger.Tyrunt || trigger == BattleTrigger.Dracozolt || trigger == BattleTrigger.Arctozolt || trigger == BattleTrigger.Articuno || trigger == BattleTrigger.Zapdos || trigger == BattleTrigger.Moltres || trigger == BattleTrigger.Mewtwo || trigger == BattleTrigger.Mew || trigger == BattleTrigger.Raikou || trigger == BattleTrigger.Entei || trigger == BattleTrigger.Suicune || trigger == BattleTrigger.Lugia || trigger == BattleTrigger.HoOh || trigger == BattleTrigger.Celebi || trigger == BattleTrigger.Regirock || trigger == BattleTrigger.Regice || trigger == BattleTrigger.Registeel || trigger == BattleTrigger.Latias || trigger == BattleTrigger.Latios || trigger == BattleTrigger.Groudon || trigger == BattleTrigger.Kyogre || trigger == BattleTrigger.Rayquaza || trigger == BattleTrigger.Jirachi || trigger == BattleTrigger.Deoxys || trigger == BattleTrigger.Uxie || trigger == BattleTrigger.Mesprit || trigger == BattleTrigger.Azelf || trigger == BattleTrigger.Dialga || trigger == BattleTrigger.Palkia || trigger == BattleTrigger.Heatran || trigger == BattleTrigger.Regigigas || trigger == BattleTrigger.Giratina || trigger == BattleTrigger.Cresselia || trigger == BattleTrigger.Phione || trigger == BattleTrigger.Manaphy || trigger == BattleTrigger.Darkrai || trigger == BattleTrigger.Shaymin || trigger == BattleTrigger.Arceus || trigger == BattleTrigger.Victini || trigger == BattleTrigger.Cobalion || trigger == BattleTrigger.Terrakion || trigger == BattleTrigger.Virizion || trigger == BattleTrigger.Tornadus || trigger == BattleTrigger.Thundurus || trigger == BattleTrigger.Reshiram || trigger == BattleTrigger.Zekrom || trigger == BattleTrigger.Landorus || trigger == BattleTrigger.Kyurem || trigger == BattleTrigger.Keldeo || trigger == BattleTrigger.Meloetta || trigger == BattleTrigger.Genesect || trigger == BattleTrigger.Xerneas || trigger == BattleTrigger.Yveltal || trigger == BattleTrigger.Zygarde || trigger == BattleTrigger.Diancie || trigger == BattleTrigger.Hoopa || trigger == BattleTrigger.Volcanion || trigger == BattleTrigger.TypeNull || trigger == BattleTrigger.TapuKoko || trigger == BattleTrigger.TapuLele || trigger == BattleTrigger.TapuBulu || trigger == BattleTrigger.TapuFini || trigger == BattleTrigger.Cosmog1 || trigger == BattleTrigger.Cosmog2 || trigger == BattleTrigger.Nihilego || trigger == BattleTrigger.Buzzwole || trigger == BattleTrigger.Pheromosa || trigger == BattleTrigger.Xurkitree || trigger == BattleTrigger.Celesteela || trigger == BattleTrigger.Kartana || trigger == BattleTrigger.Guzzlord || trigger == BattleTrigger.Necrozma || trigger == BattleTrigger.Magearna || trigger == BattleTrigger.Marshadow || trigger == BattleTrigger.Poipole || trigger == BattleTrigger.Stakataka || trigger == BattleTrigger.Blacephalon || trigger == BattleTrigger.Zeraora || trigger == BattleTrigger.Meltan || trigger == BattleTrigger.Zacian || trigger == BattleTrigger.Zamazenta || trigger == BattleTrigger.Eternatus || trigger == BattleTrigger.Kubfu || trigger == BattleTrigger.Zarude || trigger == BattleTrigger.Regieleki || trigger == BattleTrigger.Regidrago || trigger == BattleTrigger.Glastrier || trigger == BattleTrigger.Spectrier || trigger == BattleTrigger.Calyrex || trigger == BattleTrigger.MissingNo)
            pokemonList = staticPokemonOne;
        else if (trigger == BattleTrigger.Kabuto || trigger == BattleTrigger.Anorith || trigger == BattleTrigger.Shieldon || trigger == BattleTrigger.Archen || trigger == BattleTrigger.Amaura || trigger == BattleTrigger.Dracovish || trigger == BattleTrigger.Arctovish)
            pokemonList = staticPokemonTwo;
        else
            pokemonList = wildPokemonListInGrass;

        float randomValue = Random.Range(0, 100);
        var pokemonRecord = pokemonList.First(p => randomValue >= p.chanceLower && randomValue <= p.chanceUpper);

        var levelRange = pokemonRecord.levelRange;
        int level = levelRange.y == 0 || levelRange.y == levelRange.x ? levelRange.x : Random.Range(levelRange.x, (levelRange.y + 1));

        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level, false);
        wildPokemon.Initialize(false);
        return wildPokemon;
    }
}

[System.Serializable]
public class PokemonEncounterRecord
{
    public PokemonBase pokemon;
    public Vector2Int levelRange;
    public float chancePercent;

    public float chanceLower { get; set; }
    public float chanceUpper { get; set; }
}