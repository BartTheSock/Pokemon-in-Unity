using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTSUtilities.StateMachine;

public class ShopBuyingState : State<GameController>
{
    [SerializeField] Vector2 shopCameraOffset;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] ShopUI shopUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelectorUI;

    public static ShopBuyingState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    // Input
    public List<ItemBase> AvailableItems { get; set; }

    bool browseItems = false;

    GameController gameController;

    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }

    public override void Enter(GameController owner)
    {
        gameController = owner;
        browseItems = false;
        StartCoroutine(StartBuyingState());
    }

    public override void Execute()
    {
        if (browseItems)
            shopUI.HandleUpdate();
    }

    IEnumerator StartBuyingState()
    {
        yield return GameController.Instance.MoveCamera(shopCameraOffset);
        walletUI.Show();
        shopUI.Show(AvailableItems, (item) => StartCoroutine(BuyItem(item)),
            () => StartCoroutine(OnBackFromBuying()));

        browseItems = true;
    }

    IEnumerator BuyItem(ItemBase item)
    {
        browseItems = false;

        yield return DialogManager.Instance.ShowDialogText($"{item.ItemName}? Certainly. How many would you like?",
            waitForInput: false, autoClose: false);

        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.Price,
            (selectedCount) => countToBuy = selectedCount);

        DialogManager.Instance.CloseDialog();

        int totalPrice = item.Price * countToBuy;

        if (Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"{item.ItemName}? And you wanted {countToBuy}? That will be ${totalPrice}.",
                waitForInput: false,
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

            if (selectedChoice == 0)
            {   // Buy
                inventory.AddItem(item, countToBuy);
                Wallet.i.SubtractMoney(totalPrice);
                yield return DialogManager.Instance.ShowDialogText("Here you go! Thank you very much!");
            }
        }
        else
            yield return DialogManager.Instance.ShowDialogText("You don’t have enough money.");

        browseItems = true;
    }

    IEnumerator OnBackFromBuying()
    {
        yield return GameController.Instance.MoveCamera(-shopCameraOffset);
        shopUI.Close();
        walletUI.Close();
        gameController.StateMachine.Pop();
    }
}