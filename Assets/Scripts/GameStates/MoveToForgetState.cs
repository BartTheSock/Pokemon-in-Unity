using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTSUtilities.StateMachine;

public class MoveToForgetState : State<GameController>
{
    [SerializeField] MoveToForgetSelectionUI moveToForgetSelectionUI;

    // Inputs
    public List<MoveBase> CurrentMoves { get; set; }
    public MoveBase NewMove { get; set; }

    // Output
    public int Selection { get; set; }

    public static MoveToForgetState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;
        Selection = 0;
        moveToForgetSelectionUI.gameObject.SetActive(true);
        moveToForgetSelectionUI.SetMoveData(CurrentMoves, NewMove);
        moveToForgetSelectionUI.OnSelected += OnMoveSelected;
        moveToForgetSelectionUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        moveToForgetSelectionUI.HandleUpdate();
    }

    public override void Exit()
    {
        moveToForgetSelectionUI.gameObject.SetActive(false);
        moveToForgetSelectionUI.OnSelected -= OnMoveSelected;
        moveToForgetSelectionUI.OnBack -= OnBack;
    }

    void OnMoveSelected(int selection)
    {
        Selection = selection;
        gameController.StateMachine.Pop();
    }

    void OnBack()
    {
        Selection = -1;
        gameController.StateMachine.Pop();
    }
}