using BTSUtilities.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SummaryState : State<GameController>
{
    [SerializeField] SummaryScreenUI summaryScreen;

    int selectedPage = 0;

    // Input
    public int SelectedPokemonIndex { get; set; }

    public static SummaryState i { get; private set; }

    private MainGameControls gameControls;
    private InputAction selectAction;
    private InputAction confirmAction;
    private InputAction cancelAction;

    private void Awake()
    {
        i = this;
        gameControls = new MainGameControls();
    }

    private void OnEnable()
    {
        selectAction = gameControls.GenericSelector.GridSelect;
        confirmAction = gameControls.GenericSelector.Confirm;
        cancelAction = gameControls.GenericSelector.Cancel;
        selectAction.Enable();
        confirmAction.Enable();
        cancelAction.Enable();
    }

    private void OnDisable()
    {
        selectAction.Disable();
        confirmAction.Disable();
        cancelAction.Disable();
    }

    List<Pokemon> playerParty;
    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;

        playerParty = PlayerController.i.GetComponent<PokemonParty>().PokemonList;
        summaryScreen.gameObject.SetActive(true);
        summaryScreen.SetBasicDetails(playerParty[SelectedPokemonIndex]);
        summaryScreen.ShowPage(selectedPage);
    }

    public override void Execute()
    {
        if (!summaryScreen.InMoveSelection)
        {
            int previousPage = selectedPage;
            int previousSelection = SelectedPokemonIndex;
            
            var selection = selectAction.ReadValue<Vector2>();

            if (selectAction.WasPressedThisFrame())
            {
                if (selection.x == 1f)
                {
                    selectedPage++;

                    if (selectedPage > 1)
                        selectedPage = 0;
                }
                else if (selection.x == -1f)
                {
                    selectedPage--;

                    if (selectedPage < 0)
                        selectedPage = 1;
                }
                else if (selection.y == 1f)
                {
                    SelectedPokemonIndex--;

                    if (SelectedPokemonIndex < 0)
                        SelectedPokemonIndex = playerParty.Count - 1;
                }
                else if (selection.y == -1f)
                {
                    SelectedPokemonIndex++;

                    if (SelectedPokemonIndex > playerParty.Count - 1)
                        SelectedPokemonIndex = 0;
                }
            }

            if (selectedPage != previousPage)
                summaryScreen.ShowPage(selectedPage);

            if (SelectedPokemonIndex != previousSelection)
            {
                summaryScreen.SetBasicDetails(playerParty[SelectedPokemonIndex]);
                summaryScreen.ShowPage(selectedPage);
            }
        }

        if (confirmAction.WasPressedThisFrame())
        {
            if (selectedPage == 1 && !summaryScreen.InMoveSelection)
                summaryScreen.InMoveSelection = true;
        }
        else if (cancelAction.WasPressedThisFrame())
        {
            if (summaryScreen.InMoveSelection)
                summaryScreen.InMoveSelection = false;
            else
            {
                gameController.StateMachine.Pop();
                return;
            }
        }

        summaryScreen.HandleUpdate();
    }

    public override void Exit()
    {
        summaryScreen.gameObject.SetActive(false);
    }
}