using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTSUtilities.StateMachine;

public class GameMenuState : State<GameController>
{
    [SerializeField] MenuController menuController;
    [SerializeField] PlayTimeTimerUI timerUI;

    public static GameMenuState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;

        menuController.gameObject.SetActive(true);
        menuController.OnSelected += OnMenuItemSelected;
        menuController.OnBack += OnBack;
    }

    public override void Execute()
    {
        menuController.HandleUpdate();

        var currentState = gameController.StateMachine.GetCurrentState();  
        
        if (currentState == GameMenuState.i && !timerUI.gameObject.activeSelf)
            timerUI.Show();
        else if (currentState != GameMenuState.i && timerUI.gameObject.activeSelf)
            timerUI.Close();
    }

    public override void Exit()
    {
        menuController.gameObject.SetActive(false);
        menuController.OnSelected -= OnMenuItemSelected;
        menuController.OnBack -= OnBack;
    }

    void OnMenuItemSelected(int selection)
    {
        if (selection == 0) // Pokémon
            gameController.StateMachine.Push(PartyState.i);
        else if (selection == 1) // Open bag
            gameController.StateMachine.Push(InventoryState.i);
        else if (selection == 2) // Save game
            StartCoroutine(SaveSelected());
        else if (selection == 3) // Quit game
            StartCoroutine(QuitSelected());
    }

    IEnumerator SaveSelected()
    {
        yield return Fader.i.FadeIn(0.5f);
        SavingSystem.i.Save("saveSlot");
        yield return Fader.i.FadeOut(0.5f);
    }

    IEnumerator QuitSelected()
    {
        yield return Fader.i.FadeIn(0.5f);
        SavingSystem.i.Save("saveSlot");
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    void OnBack()
    {
        gameController.StateMachine.Pop();
    }
}