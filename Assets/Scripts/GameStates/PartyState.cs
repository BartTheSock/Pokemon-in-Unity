using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BTSUtilities.StateMachine;

public class PartyState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;

    public Pokemon SelectedPokemon { get; private set; }

    bool isSwitchingPosition;
    int selectedIndexForSwitching = 0;

    public static PartyState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    PokemonParty playerParty;

    private void Start()
    {
        playerParty = PlayerController.i.GetComponent<PokemonParty>();
    }

    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;
        SelectedPokemon = null;
        
        var previousState = gameController.StateMachine.GetPreviousState();
        if (previousState == BattleState.i)
            partyScreen.Initialize();
        
        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;

        partyScreen.SetPartyData();
    }

    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }

    public override void Exit()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }

    void OnPokemonSelected(int selection)
    {
        SelectedPokemon = partyScreen.SelectedPartyMember;

        StartCoroutine(PokemonSelectedAction(selection));
    }

    IEnumerator PokemonSelectedAction(int selectedPokemonIndex)
    {
        var previousState = gameController.StateMachine.GetPreviousState();
        if (previousState == InventoryState.i)
        {
            StartCoroutine(GoToUseItemState());
        }
        else if (previousState == BattleState.i)
        {
            var battleState = previousState as BattleState;

            DynamicMenuState.i.MenuItems = new List<string> { "Switch", "Summary", "Cancel" };
            yield return gameController.StateMachine.PushAndWait(DynamicMenuState.i);

            if (DynamicMenuState.i.SelectedItem == 0)
            {
                if (SelectedPokemon.HP <= 0)
                {
                    partyScreen.SetMessageText("You can't send out a fainted Pokémon!");
                    yield break;
                }

                if (battleState.BattleSystem.PlayerUnits.Any(u => u.Pokemon == SelectedPokemon))
                {
                    partyScreen.SetMessageText("That Pokémon is already out!");
                    yield break;
                }

                if (battleState.BattleSystem.UnitCount > 1 && battleState.BattleSystem.IsPokemonSelectedToSwitch(SelectedPokemon))
                {
                    partyScreen.SetMessageText("That Pokémon is already selected to switch!");
                    yield break;
                }

                gameController.StateMachine.Pop();
            }
            else if (DynamicMenuState.i.SelectedItem == 1)
            {
                SummaryState.i.SelectedPokemonIndex = selectedPokemonIndex;
                yield return gameController.StateMachine.PushAndWait(SummaryState.i);
            }
            else
                yield break;
        }
        else
        {
            if (isSwitchingPosition)
            {
                if (selectedIndexForSwitching == selectedPokemonIndex)
                {
                    partyScreen.SetMessageText("You can't switch with the same Pokémon!");
                    yield break;
                }

                isSwitchingPosition = false;

                var temporaryPokemon = playerParty.PokemonList[selectedIndexForSwitching];
                playerParty.PokemonList[selectedIndexForSwitching] = playerParty.PokemonList[selectedPokemonIndex];
                playerParty.PokemonList[selectedPokemonIndex] = temporaryPokemon;

                playerParty.PartyUpdated();
                yield break;
            }

            DynamicMenuState.i.MenuItems = new List<string> { "Summary", "Switch", "Cancel" };
            yield return gameController.StateMachine.PushAndWait(DynamicMenuState.i);
            
            if (DynamicMenuState.i.SelectedItem == 0)
            {
                SummaryState.i.SelectedPokemonIndex = selectedPokemonIndex;
                yield return gameController.StateMachine.PushAndWait(SummaryState.i);
            }
            else if (DynamicMenuState.i.SelectedItem == 1)
            {
                isSwitchingPosition = true;
                selectedIndexForSwitching = selectedPokemonIndex;
                partyScreen.SetMessageText("Choose a Pokémon to switch positions with.");
            }
            else
                yield break;
        }
    }

    IEnumerator GoToUseItemState()
    {
        yield return gameController.StateMachine.PushAndWait(UseItemState.i);
        gameController.StateMachine.Pop();
    }

    void OnBack()
    {
        SelectedPokemon = null;
        var previousState = gameController.StateMachine.GetPreviousState();
        if (previousState == BattleState.i)
        {
            var battleState = previousState as BattleState;
            if (battleState.BattleSystem.PlayerUnits.Any(u => u.Pokemon != null && u.Pokemon.HP <= 0))
            {
                partyScreen.SetMessageText("You have to choose a Pokémon to continue!");
                return;
            }
        }

        gameController.StateMachine.Pop();
    }
}