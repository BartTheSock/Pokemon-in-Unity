using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTSUtilities.StateMachine;

public class InventoryState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;

    // Output
    public ItemBase SelectedItem { get; private set; }

    public static InventoryState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;

        SelectedItem = null;
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        inventoryUI.HandleUpdate();
    }

    public override void Exit()
    {
        inventoryUI.gameObject.SetActive(false);
        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnBack;
    }

    void OnItemSelected(int selection)
    {
        SelectedItem = inventoryUI.SelectedItem;
        if (gameController.StateMachine.GetPreviousState() == ShopSellingState.i)
            gameController.StateMachine.Pop();
        else
            StartCoroutine(SelectPokemonAndUseItem());
    }

    void OnBack()
    {
        SelectedItem = null;
        gameController.StateMachine.Pop();
    }

    IEnumerator SelectPokemonAndUseItem()
    {
        var previousState = gameController.StateMachine.GetPreviousState();
        if (previousState == BattleState.i)
        {
            if (!SelectedItem.CanUseInBattle)
            {
                if (!SelectedItem.CanUseOutsideBattle)
                    yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
                else
                    yield return DialogManager.Instance.ShowDialogText($"This item can't be used in a battle!");

                yield break;
            }
        }
        else
        {
            if (!SelectedItem.CanUseOutsideBattle)
            {
                if (!SelectedItem.CanUseInBattle)
                    yield return DialogManager.Instance.ShowDialogText($"It won't have any effect!");
                else
                    yield return DialogManager.Instance.ShowDialogText($"This item can only be used in a battle!");
                yield break;
            }
        }

        if (SelectedItem is PokeBallItem)
        {
            inventory.UseItem(SelectedItem, null);
            gameController.StateMachine.Pop();
            yield break;
        }
        else if (SelectedItem is BattleItem)
        {
            var pokemon = BattleSystem.Instance.GetComponent<BattleUnit>().Pokemon;
            inventory.UseItem(SelectedItem, pokemon);
            gameController.StateMachine.Pop();
            yield break;
        }
        else if (SelectedItem is OtherItems && (SelectedItem.ItemName == "Repel" || SelectedItem.ItemName == "Super Repel" || SelectedItem.ItemName == "Max Repel"))
        {
            var otherItem = SelectedItem as OtherItems;
            bool isRepelItem = otherItem.RepelAmount > 0;
            if (isRepelItem)
            {
                var player = inventory.GetComponent<PlayerController>();
                yield return DialogManager.Instance.ShowDialogText($"playerName used {otherItem.ItemName}!");
                player.repelStepsRemaining += otherItem.RepelAmount;
                inventory.UseItem(SelectedItem, null);
                gameController.StateMachine.Pop();
                yield break;
            }
        }

        yield return gameController.StateMachine.PushAndWait(PartyState.i);

        if (previousState == BattleState.i)
        {
            if (UseItemState.i.ItemUsed == true)
                gameController.StateMachine.Pop();
        }
    }
}