using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BTSUtilities.StateMachine;

public class MoveSelectionState : State<BattleSystem>
{
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] GameObject moveDetailsUI;

    // Input
    public List<Move> Moves { get; set; }

    public static MoveSelectionState i { get; private set; }

    BattleSystem battleSystem;

    private void Awake()
    {
        i = this;
    }

    public override void Enter(BattleSystem owner)
    {
        battleSystem = owner;

        moveSelectionUI.SetMoves(Moves);

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.OnSelected += OnMoveSelected;
        moveSelectionUI.OnBack += OnBack;

        moveDetailsUI.SetActive(true);
        battleSystem.DialogBox.EnableDialogText(false);
    }

    public override void Execute()
    {
        moveSelectionUI.HandleUpdate();
    }

    public override void Exit()
    {
        moveSelectionUI.gameObject.SetActive(false);
        moveSelectionUI.OnSelected -= OnMoveSelected;
        moveSelectionUI.OnBack -= OnBack;

        moveDetailsUI.SetActive(false);
        battleSystem.DialogBox.EnableDialogText(true);
    }

    void OnMoveSelected(int selection)
    {
        StartCoroutine(OnMoveSelectedAsync(selection));
    }

    IEnumerator OnMoveSelectedAsync(int selection)
    {
        if (Moves[selection].PP > 0)
        {
            int moveTarget = 0;
            if (battleSystem.ActiveEnemyUnitsCount > 1)
            {
                yield return battleSystem.StateMachine.PushAndWait(TargetSelectionState.i);
                if (!TargetSelectionState.i.SelectionMade)
                    yield break;
                else
                    moveTarget = TargetSelectionState.i.SelectedTarget;
            }

            battleSystem.AddBattleAction(new BattleAction()
            {
                Type = BattleActionType.Move,
                SelectedMove = Moves[selection],
                Target = battleSystem.EnemyUnits[moveTarget]
            });
        }
    }

    void OnBack()
    {
        battleSystem.StateMachine.ChangeState(ActionSelectionState.i);
    }
}