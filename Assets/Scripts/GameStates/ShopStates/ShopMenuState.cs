using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTSUtilities.StateMachine;

public class ShopMenuState : State<GameController>
{
    public static ShopMenuState i { get; private set; }

    private void Awake()
    {
        i = this;
    }
    
    // Input
    public List<ItemBase> AvailableItems { get; set; }
    
    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;
        StartCoroutine(StartMenuState());
    }

    IEnumerator StartMenuState()
    {
        int selectedChoice = 0;

        yield return DialogManager.Instance.ShowDialogText("How may I serve you?",
        waitForInput: false,
        choices: new List<string>() { "Buy", "Sell", "Leave" },
        onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {   // Buy
            ShopBuyingState.i.AvailableItems = AvailableItems;
            yield return gameController.StateMachine.PushAndWait(ShopBuyingState.i);
        }
        else if (selectedChoice == 1)
        {   // Sell
            yield return gameController.StateMachine.PushAndWait(ShopSellingState.i);
        }
        else if (selectedChoice == 2)
        {
            // Leave
        }

        gameController.StateMachine.Pop();
    }
}