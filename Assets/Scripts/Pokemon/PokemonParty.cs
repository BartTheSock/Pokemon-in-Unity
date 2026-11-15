using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemonList;

    public event Action OnUpdated;

    public bool ownedByTrainer { get; set; } = false;

    public List<Pokemon> PokemonList {
        get { return pokemonList; }
        set { pokemonList = value; OnUpdated?.Invoke(); }
    }

    PokemonStorageBoxes storageBoxes;

    private void Awake()
    {
        storageBoxes = GetComponent<PokemonStorageBoxes>();

        foreach (var pokemon in pokemonList)
        {
            pokemon.Initialize(GetComponent<TrainerController>() != null || GetComponent<GymLeaderController>() != null);
        }
    }

    private void Start()
    {
        
    }

    public Pokemon GetHealthyPokemon(List<Pokemon> dontInclude = null)
    {
        var healthyPokemonList = pokemonList.Where(x => x.HP > 0).ToList();
        if (dontInclude != null)
            healthyPokemonList = healthyPokemonList.Where(p => !dontInclude.Contains(p)).ToList();

        return healthyPokemonList.FirstOrDefault();
    }

    public List<Pokemon> GetMultipleHealthyPokemon(int unitCount)
    {
        return pokemonList.Where(x => x.HP > 0).Take(unitCount).ToList();
    }
    
    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemonList.Count < 6)
        {
            pokemonList.Add(newPokemon);
            OnUpdated?.Invoke();
        }
        else
            storageBoxes.AddPokemonToEmptySlot(newPokemon);
    }

    public void ClearAllPokemon()
    {
        pokemonList.Clear();
        storageBoxes.RemoveAllPokemon();
        OnUpdated?.Invoke();
    }

    public bool CheckForEvolutions()
    {
        return pokemonList.Any(p => p.CheckForEvolution("") != null);
    }

    public IEnumerator RunEvolutions()
    {
        foreach (var pokemon in pokemonList)
        {
            var evolution = pokemon.CheckForEvolution("");
            
            if (pokemon.Base.PokemonName == "Tyrogue")
            {
                if (pokemon.Attack > pokemon.Defense)
                    evolution = pokemon.CheckForEvolution("Hitmonlee");
                else if (pokemon.Attack < pokemon.Defense)
                    evolution = pokemon.CheckForEvolution("Hitmonchan");
                else if (pokemon.Attack == pokemon.Defense)
                    evolution = pokemon.CheckForEvolution("Hitmontop");
            }
            else if (pokemon.Base.PokemonName == "Wurmple")
            {
                if (pokemon.GenderNumber % 2 == 0)
                    evolution = pokemon.CheckForEvolution("Silcoon");
                else
                    evolution = pokemon.CheckForEvolution("Cascoon");
            }

            if (evolution != null)
                yield return EvolutionState.i.Evolve(pokemon, evolution);
        }
    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindFirstObjectByType<PlayerController>(FindObjectsInactive.Exclude).GetComponent<PokemonParty>();
    }
}