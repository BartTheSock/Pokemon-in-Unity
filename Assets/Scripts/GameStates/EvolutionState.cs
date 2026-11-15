using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using E7.Introloop;
using BTSUtilities.StateMachine;

public class EvolutionState : State<GameController>
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;

    [SerializeField] IntroloopAudio evolutionStart;
    [SerializeField] IntroloopAudio evolutionMusic;
    [SerializeField] IntroloopAudio evolutionComplete;

    [SerializeField] Pokemon shedinja;
    [SerializeField] ItemBase pokeBall;

    PlayerController player;
    PokemonParty playerParty;
    Inventory inventory;
    int pokemonInActiveParty;

    public static EvolutionState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        playerParty = PokemonParty.GetPlayerParty();
        inventory = Inventory.GetInventory();
        pokemonInActiveParty = playerParty.PokemonList.Count;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution)
    {
        GameController.Instance.StateMachine.Push(this);
        evolutionUI.SetActive(true);

        AudioManager.i.PlayMusic(evolutionStart, true);

        pokemonImage.sprite = pokemon.Base.FrontSprite;

        yield return new WaitForSeconds(evolutionStart.ClipLength);
        AudioManager.i.PlayMusic(evolutionMusic, true);

        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText($"What? {pokemon.Base.PokemonName} is evolving! Do you want {pokemon.Base.PokemonName} to evolve?",
            waitForInput: false,
            choices: new List<string>() { "Yes", "No" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            var oldPokemon = pokemon.Base;
            pokemon.Evolve(evolution);
            
            float animationWaitTime = 0.4f;
            for (int i = 0; i < 6; i++)
            {
                pokemonImage.sprite = pokemon.Base.FrontSprite;
                yield return new WaitForSeconds(0.05f);
                pokemonImage.sprite = oldPokemon.FrontSprite;
                yield return new WaitForSeconds(animationWaitTime);
                animationWaitTime -= 0.05f;
            }
            for (int i = 0; i < 4; i++)
            {
                pokemonImage.sprite = pokemon.Base.FrontSprite;
                yield return new WaitForSeconds(0.05f);
                pokemonImage.sprite = oldPokemon.FrontSprite;
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < 6; i++)
            {
                pokemonImage.sprite = pokemon.Base.FrontSprite;
                yield return new WaitForSeconds(0.05f);
                pokemonImage.sprite = oldPokemon.FrontSprite;
                yield return new WaitForSeconds(0.05f);
            }
            for (int i = 0; i < 10; i++)
            {
                pokemonImage.sprite = pokemon.Base.FrontSprite;
                yield return new WaitForSeconds(0.03f);
                pokemonImage.sprite = oldPokemon.FrontSprite;
                yield return new WaitForSeconds(0.03f);
            }
            for (int i = 0; i < 20; i++)
            {
                pokemonImage.sprite = pokemon.Base.FrontSprite;
                yield return new WaitForSeconds(0.02f);
                pokemonImage.sprite = oldPokemon.FrontSprite;
                yield return new WaitForSeconds(0.02f);
            }

            pokemonImage.sprite = pokemon.Base.FrontSprite;

            AudioManager.i.PlayMusic(evolutionComplete, false);
            yield return DialogManager.Instance.ShowDialogText($"Congratulations! Your {oldPokemon.PokemonName} evolved into {pokemon.Base.PokemonName}!");

            if (oldPokemon.PokemonName == "Nincada" && pokemon.Base.PokemonName == "Ninjask" && pokemonInActiveParty < 6 && inventory.HasItem(pokeBall))
            {
                shedinja.Initialize(false);
                playerParty.AddPokemon(shedinja);
                inventory.RemoveItem(pokeBall);
                yield return DialogManager.Instance.ShowDialogText($"What? Ninjask's newly shed exoskeleton turned into Shedinja!");
            }

            if (oldPokemon.PokemonName == "Azurill" && pokemon.Base.PokemonName == "Marill" && (pokemon.GenderNumber > 127 && pokemon.GenderNumber < 192))
                pokemon.GenderNumber = 127;

            var evolutionMove = pokemon.Base.MoveLearnedUponEvolution;

            if (evolutionMove != null && !pokemon.HasMove(evolutionMove))
            {
                if (pokemon.Moves.Count < PokemonBase.MaxNumberOfMoves)
                {
                    pokemon.LearnMove(evolutionMove);
                    yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} learned {evolutionMove.MoveName}!");
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} is trying to learn {evolutionMove.MoveName}.");
                    yield return DialogManager.Instance.ShowDialogText($"But it already knows {PokemonBase.MaxNumberOfMoves} moves!");
                    yield return DialogManager.Instance.ShowDialogText($"Choose a move you want {pokemon.Base.PokemonName} to forget.", true, false);

                    MoveToForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
                    MoveToForgetState.i.NewMove = evolutionMove;
                    yield return GameController.Instance.StateMachine.PushAndWait(MoveToForgetState.i);

                    int moveIndex = MoveToForgetState.i.Selection;

                    if (moveIndex == PokemonBase.MaxNumberOfMoves || moveIndex == -1)
                        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} did not learn {evolutionMove.MoveName}.");
                    else
                    {
                        var selectedMove = pokemon.Moves[moveIndex].Base;
                        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} forgot {selectedMove.MoveName} and learned {evolutionMove.MoveName}!");
                        pokemon.Moves[moveIndex] = new Move(evolutionMove);
                    }
                }
            }

            var levelMove = pokemon.GetLearnableMoveAtCurrentLevel();

            if (levelMove != null && !pokemon.HasMove(levelMove.Base))
            {
                if (pokemon.Moves.Count < PokemonBase.MaxNumberOfMoves)
                {
                    pokemon.LearnMove(levelMove.Base);
                    yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} learned {levelMove.Base.MoveName}!");
                }
                else
                {
                    yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} is trying to learn {levelMove.Base.MoveName}.");
                    yield return DialogManager.Instance.ShowDialogText($"But it already knows {PokemonBase.MaxNumberOfMoves} moves!");
                    yield return DialogManager.Instance.ShowDialogText($"Choose a move you want {pokemon.Base.PokemonName} to forget.", true, false);

                    MoveToForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
                    MoveToForgetState.i.NewMove = levelMove.Base;
                    yield return GameController.Instance.StateMachine.PushAndWait(MoveToForgetState.i);

                    int moveIndex = MoveToForgetState.i.Selection;

                    if (moveIndex == PokemonBase.MaxNumberOfMoves || moveIndex == -1)
                        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} did not learn {levelMove.Base.MoveName}.");
                    else
                    {
                        var selectedMove = pokemon.Moves[moveIndex].Base;
                        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} forgot {selectedMove.MoveName} and learned {levelMove.Base.MoveName}!");
                        pokemon.Moves[moveIndex] = new Move(levelMove.Base);
                    }
                }
            }
        }
        else if (selectedChoice == 1)
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} did not evolve.");

        GameController.Instance.PartyScreen.SetPartyData();
        AudioManager.i.PlayMusic(GameController.Instance.CurrentScene.SceneMusic, true);
        evolutionUI.SetActive(false);
        GameController.Instance.StateMachine.Pop();
    }
}