using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BTSUtilities.StateMachine;

public class UseItemState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    // Output
    public bool ItemUsed { get; private set; }

    public static UseItemState i { get; private set; }
    Inventory inventory;

    private void Awake()
    {
        i = this;
        inventory = Inventory.GetInventory();
    }

    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;
        ItemUsed = false;
        StartCoroutine(UseItem());
    }

    IEnumerator UseItem()
    {
        var item = inventoryUI.SelectedItem;
        var pokemon = partyScreen.SelectedPartyMember;

        if (item is TMItem)
            yield return HandleTMItems();
        else if (item is BattleItem)
            yield return HandleBattleItems();
        else if (item is OtherItems)
            yield return HandleOtherItems();
        else if (item is EvolutionItem)
        {
            var evolution = pokemon.CheckForEvolution(item);
            if (evolution != null)
                yield return EvolutionState.i.Evolve(pokemon, evolution);
            else
            {
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
                gameController.StateMachine.Pop();
                yield break;
            }
        }

        var usedItem = inventory.UseItem(item, partyScreen.SelectedPartyMember);
        if (usedItem != null)
        {
            ItemUsed = true;

            if (usedItem is RecoveryItem || usedItem is BerryItem)
            {
                yield return DialogManager.Instance.ShowDialogText($"playerName used {usedItem.ItemName}!");
                yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName}{usedItem.ItemUseMessage}");
            }
        }
        else
        {
            if (inventoryUI.SelectedCategory == (int)ItemCategory.RecoveryItems)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
        }

        gameController.StateMachine.Pop();
    }

    IEnumerator HandleTMItems()
    {
        var tmItem = inventoryUI.SelectedItem as TMItem;
        if (tmItem == null)
            yield break;

        var pokemon = partyScreen.SelectedPartyMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} already knows {tmItem.Move.MoveName}!");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} can't learn {tmItem.Move.MoveName}!");
            yield break;
        }

        if (pokemon.Moves.Count < PokemonBase.MaxNumberOfMoves)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} learned {tmItem.Move.MoveName}!");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} is trying to learn {tmItem.Move.MoveName}.");
            yield return DialogManager.Instance.ShowDialogText($"But it already knows {PokemonBase.MaxNumberOfMoves} moves!");

            yield return DialogManager.Instance.ShowDialogText($"Choose a move you want {pokemon.Base.PokemonName} to forget.", true, false);

            MoveToForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
            MoveToForgetState.i.NewMove = tmItem.Move;

            yield return gameController.StateMachine.PushAndWait(MoveToForgetState.i);

            int moveIndex = MoveToForgetState.i.Selection;

            if (moveIndex == PokemonBase.MaxNumberOfMoves || moveIndex == -1)
                yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} did not learn {tmItem.Move.MoveName}.");
            else
            {
                var selectedMove = pokemon.Moves[moveIndex].Base;
                yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} forgot {selectedMove.MoveName} and learned {tmItem.Move.MoveName}!");
                pokemon.Moves[moveIndex] = new Move(tmItem.Move);
            }
        }
    }

    IEnumerator HandleBattleItems()
    {
        var battleItem = inventoryUI.SelectedItem as BattleItem;
        if (battleItem == null)
            yield break;

        var pokemon = BattleSystem.FindFirstObjectByType<BattleUnit>(FindObjectsInactive.Exclude).Pokemon;
    }

    IEnumerator HandleOtherItems()
    {
        var otherItem = inventoryUI.SelectedItem as OtherItems;
        var pokemon = partyScreen.SelectedPartyMember;
        var player = FindFirstObjectByType<PlayerController>();

        if (otherItem == null || (otherItem.IncreaseLevel && pokemon.Level >= player.pokemonLevelCap))
            yield break;

        if (otherItem.IncreaseLevel)
        {
            int expGain = (pokemon.Base.GetEXPForLevel(pokemon.Level + 1) - pokemon.EXP);
            pokemon.EXP += expGain;

            if (pokemon.CheckForLevelUp())
            {
                yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} grew to Level {pokemon.Level}!");
                var newMove = pokemon.GetLearnableMoveAtCurrentLevel();
                if (newMove != null && !pokemon.HasMove(newMove.Base))
                {
                    if (pokemon.Moves.Count < PokemonBase.MaxNumberOfMoves)
                    {
                        pokemon.LearnMove(newMove.Base);
                        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} learned {newMove.Base.MoveName}!");
                    }
                    else
                    {
                        yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} is trying to learn {newMove.Base.MoveName}.");
                        yield return DialogManager.Instance.ShowDialogText($"But it already knows {PokemonBase.MaxNumberOfMoves} moves!");

                        yield return DialogManager.Instance.ShowDialogText($"Choose a move you want {pokemon.Base.PokemonName} to forget.", true, false);

                        MoveToForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
                        MoveToForgetState.i.NewMove = newMove.Base;

                        yield return gameController.StateMachine.PushAndWait(MoveToForgetState.i);

                        int moveIndex = MoveToForgetState.i.Selection;

                        if (moveIndex == PokemonBase.MaxNumberOfMoves || moveIndex == -1)
                            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} did not learn {newMove.Base.MoveName}.");
                        else
                        {
                            var selectedMove = pokemon.Moves[moveIndex].Base;
                            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.PokemonName} forgot {selectedMove.MoveName} and learned {newMove.Base.MoveName}!");
                            pokemon.Moves[moveIndex] = new Move(newMove.Base);
                        }
                    }
                }

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
    }
}