using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BTSUtilities.StateMachine;
using UnityEngine.InputSystem;

public class TargetSelectionState : State<BattleSystem>
{
    int selectedTarget = 0;
    
    // Output
    public bool SelectionMade { get; private set; }
    public int SelectedTarget => selectedTarget;

    public static TargetSelectionState i { get; private set; }

    BattleSystem battleSystem;

    private MainGameControls gamecontrols;
    private InputAction selectAction;
    private InputAction confirmAction;
    private InputAction cancelAction;

    private void Awake()
    {
        i = this;
        gamecontrols = new MainGameControls();
    }
    private void OnEnable()
    {
        selectAction = gamecontrols.TargetSelection.Select;
        confirmAction = gamecontrols.TargetSelection.Confirm;
        cancelAction = gamecontrols.TargetSelection.Cancel;
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
    public override void Enter(BattleSystem owner)
    {
        battleSystem = owner;

        selectedTarget = 0;
        SelectionMade = false;
        UpdateSelectionInUI();
    }

    public override void Execute()
    {
        int previousSelection = selectedTarget;

        float select = selectAction.ReadValue<float>();

        if (Mathf.Abs(select) == 1f && selectAction.WasPressedThisFrame())
        {
            if (select == 1f)
                selectedTarget++;
            else if (select == -1f)
                selectedTarget--;
        }

        selectedTarget = Mathf.Clamp(selectedTarget, 0, battleSystem.EnemyUnits.Count - 1);

        if (selectedTarget != previousSelection)
            UpdateSelectionInUI();

        if (confirmAction.WasPressedThisFrame())
        {
            SelectionMade = true;
            battleSystem.StateMachine.Pop();
        }
        else if (cancelAction.WasPressedThisFrame())
        {
            SelectionMade = false;
            battleSystem.StateMachine.Pop();
        }
    }

    public override void Exit()
    {
        battleSystem.EnemyUnits[selectedTarget].SetSelected(false);
    }

    void UpdateSelectionInUI()
    {
        for (int i = 0; i < battleSystem.EnemyUnits.Count; i++)
        {
            battleSystem.EnemyUnits[i].SetSelected(i == selectedTarget);
        }
    }
}