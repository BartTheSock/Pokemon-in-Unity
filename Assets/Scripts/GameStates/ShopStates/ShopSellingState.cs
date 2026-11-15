using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTSUtilities.StateMachine;

public class ShopSellingState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;

    public static ShopSellingState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gameController;

    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public override void Enter(GameController owner)
    {
        gameController = owner;

        StartCoroutine(StartSellingState());
    }

    IEnumerator StartSellingState()
    {
        yield return gameController.StateMachine.PushAndWait(InventoryState.i);

        var selectedItem = InventoryState.i.SelectedItem;
        if (selectedItem != null)
        {
            yield return SellItem(selectedItem);
            StartCoroutine(StartSellingState());
        }
        else
            gameController.StateMachine.Pop();
    }

    IEnumerator SellItem(ItemBase item)
    {
        if (!item.IsSellable)
        {
            yield return DialogManager.Instance.ShowDialogText($"{item.ItemName}? Oh, no. I can’t buy that.");
            yield break;
        }

        walletUI.Show();

        int sellPrice = Mathf.CeilToInt((float)item.Price / 2f);
        int countToSell = 1;

        int itemCount = inventory.GetItemCount(item);
        if (itemCount > 1)
        {
            yield return DialogManager.Instance.ShowDialogText($"{item.ItemName}? How many would you like to sell?",
                waitForInput: false, autoClose: false);

            yield return countSelectorUI.ShowSelector(itemCount, sellPrice,
                (selectedCount) => countToSell = selectedCount);

            DialogManager.Instance.CloseDialog();
        }

        sellPrice = sellPrice * countToSell;

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText($"I can pay ${sellPrice}. Would that be okay?",
            waitForInput: false,
            choices: new List<string>() { "Yes", "No" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {   // Sell
            inventory.RemoveItem(item, countToSell);
            Wallet.i.AddMoney(sellPrice);
            yield return DialogManager.Instance.ShowDialogText($"Turned over the {item.ItemName} and received ${sellPrice}.");
        }

        walletUI.Close();
    }
}