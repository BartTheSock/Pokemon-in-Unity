using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BTSUtilities.StateMachine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] ActionSelectionUI actionSelectionUI;

    public static ActionSelectionState i { get; private set; }

    BattleSystem battleSystem;

    private void Awake()
    {
        i = this;
    }

    public override void Enter(BattleSystem owner)
    {
        battleSystem = owner;
        actionSelectionUI.SetAction();
        actionSelectionUI.gameObject.SetActive(true);
        actionSelectionUI.OnSelected += OnActionSelected;
        battleSystem.UnitInSelection.Pokemon.CalculateStats();
        battleSystem.DialogBox.SetDialog($"What should {battleSystem.UnitInSelection.Pokemon.Base.PokemonName} do?");
    }

    public override void Execute()
    {
        actionSelectionUI.HandleUpdate();
    }

    public override void Exit()
    {
        actionSelectionUI.gameObject.SetActive(false);
        actionSelectionUI.OnSelected -= OnActionSelected;
    }

    void OnActionSelected(int selection)
    {
        if (selection == 0)
        {   // Fight
            MoveSelectionState.i.Moves = battleSystem.UnitInSelection.Pokemon.Moves;
            var movesWithPP = battleSystem.UnitInSelection.Pokemon.Moves.Where(x => x.PP > 0).ToList();
            if (movesWithPP.Count == 0)
            {
                battleSystem.AddBattleAction(new BattleAction()
                {
                    Type = BattleActionType.Move,
                    SelectedMove = new Move(GlobalSettings.i.BackupMove),
                    User = battleSystem.UnitInSelection,
                    Target = battleSystem.EnemyUnits[UnityEngine.Random.Range(0, battleSystem.EnemyUnits.Count)]
                });
            }
            else
                battleSystem.StateMachine.ChangeState(MoveSelectionState.i);
        }
        else if (selection == 1)
        {   // Bag
            StartCoroutine(GoToInventoryState());
        }
        else if (selection == 2)
        {   // Pokemon
            StartCoroutine(GoToPartyState());
        }
        else if (selection == 3)
        {   // Run
            battleSystem.AddBattleAction(new BattleAction()
            {
                Type = BattleActionType.Run,
                Target = battleSystem.EnemyUnits[0],
                SelectedMove = new Move(GlobalSettings.i.BackupMove)
            });
        }
    }

    IEnumerator GoToPartyState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
        var selectedPokemon = PartyState.i.SelectedPokemon;
        if (selectedPokemon != null)
        {
            battleSystem.AddBattleAction(new BattleAction()
            {
                Type = BattleActionType.SwitchPokemon,
                Target = battleSystem.EnemyUnits[0],
                SelectedPokemon = selectedPokemon,
                SelectedMove = new Move(GlobalSettings.i.BackupMove)
            });
        }
    }

    IEnumerator GoToInventoryState()
    {
        yield return GameController.Instance.StateMachine.PushAndWait(InventoryState.i);
        var selectedItem = InventoryState.i.SelectedItem;
        if (selectedItem != null)
        {
            battleSystem.AddBattleAction(new BattleAction()
            {
                Type = BattleActionType.UseItem,
                Target = battleSystem.EnemyUnits[0],
                SelectedItem = selectedItem,
                SelectedMove = new Move(GlobalSettings.i.BackupMove)
            });
        }
    }
}