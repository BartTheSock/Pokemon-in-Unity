using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BTSUtilities.StateMachine;

public class FreeRoamState : State<GameController>
{
    public static FreeRoamState i { get; private set; }

    private MainGameControls gameControls;
    private InputAction openMenuAction;

    private void Awake()
    {
        i = this;
        gameControls = new MainGameControls();
    }
    private void OnEnable()
    {
        openMenuAction = gameControls.OpenMenu.OpenMenu;
        openMenuAction.Enable();
    }

    private void OnDisable()
    {
        openMenuAction.Disable();
    }

    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;
    }

    public override void Execute()
    {
        PlayerController.i.HandleUpdate();

        if (openMenuAction.WasReleasedThisFrame())
        {
            PlayerController.i.Character.Animator.IsMoving = false;
            PlayerController.i.Character.Animator.IsRunning = false;
            gameController.StateMachine.Push(GameMenuState.i);
        }
    }
}