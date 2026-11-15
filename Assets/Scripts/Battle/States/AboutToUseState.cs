using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BTSUtilities.StateMachine;

public class AboutToUseState : State<BattleSystem>
{
    // Input
    public Pokemon NewPokemon { get; set; }

    bool aboutToUseChoice;

    public static AboutToUseState i { get; private set; }

    BattleSystem battleSystem;

    private MainGameControls gameControls;
    private InputAction chooseAction;
    private InputAction confirmAction;
    private InputAction cancelAction;

    private void Awake()
    {
        i = this;
        gameControls = new MainGameControls();
    }

    private void OnEnable()
    {
        chooseAction = gameControls.CountSelector.Select;
        confirmAction = gameControls.GenericSelector.Confirm;
        cancelAction = gameControls.GenericSelector.Cancel;
        chooseAction.Enable();
        confirmAction.Enable();
        cancelAction.Enable();
    }

    private void OnDisable()
    {
        chooseAction.Disable();
        confirmAction.Disable();
        cancelAction.Disable();
    }

    public override void Enter(BattleSystem owner)
    {
        battleSystem = owner;
        StartCoroutine(StartState());
    }

    IEnumerator StartState()
    {
        if (battleSystem.IsTrainerBattle)
            yield return battleSystem.DialogBox.TypeDialog($"{battleSystem.Trainer.TrainerClass} {battleSystem.Trainer.TrainerName} is about to use {NewPokemon.Base.PokemonName}. Do you want to switch your Pokémon?");
        else if (battleSystem.IsGymLeaderBattle)
            yield return battleSystem.DialogBox.TypeDialog($"Gym Leader {battleSystem.GymLeader.TrainerName} is about to use {NewPokemon.Base.PokemonName}. Do you want to switch your Pokémon?");

        battleSystem.DialogBox.EnableChoiceBox(true);
    }

    public override void Execute()
    {
        if (!battleSystem.DialogBox.IsChoiceBoxEnabled)
            return;
                        
        if (chooseAction.WasPressedThisFrame())
            aboutToUseChoice = !aboutToUseChoice;

        battleSystem.DialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (confirmAction.WasPressedThisFrame())
        {
            battleSystem.DialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice == true) // Yes
                StartCoroutine(SwitchAndContinueBattle());
            else // No
                StartCoroutine(ContinueBattle());
        }
        else if (cancelAction.WasPressedThisFrame())
        {
            battleSystem.DialogBox.EnableChoiceBox(false);
            StartCoroutine(ContinueBattle());
        }
    }

    IEnumerator SwitchAndContinueBattle()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var selectedPokemon = PartyState.i.SelectedPokemon;
        if (selectedPokemon != null)
            yield return battleSystem.SwitchPokemon(selectedPokemon, battleSystem.PlayerUnits[0]);

        yield return ContinueBattle();
    }

    IEnumerator ContinueBattle()
    {
        if (battleSystem.IsTrainerBattle)
            yield return battleSystem.SendNextTrainerPokemon(battleSystem.EnemyUnits[0]);
        else
            yield return battleSystem.SendNextGymLeaderPokemon(battleSystem.EnemyUnits[0]);

        battleSystem.StateMachine.Pop();
    }
}