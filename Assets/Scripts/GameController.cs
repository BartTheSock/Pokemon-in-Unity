using BTSUtilities.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;

    public StateMachine<GameController> StateMachine { get; private set; }

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PreviousScene { get; private set; }

    public static GameController Instance;

    private MainGameControls gameControls;
    private InputAction exit1Action;
    private InputAction exit2Action;
    private InputAction fullscreenAction;

    private void Awake()
    {
        Instance = this;
        gameControls = new MainGameControls();
        PokemonDB.Initialize();
        MoveDB.Initialize();
        StatusConditionsDB.Initialize();
        WeatherConditionsDB.Initialize();
        AbilitiesDB.Initialize();
        ItemDB.Initialize();
        QuestDB.Initialize();
        MoveBase.InitializeMoves();
    }
    private void OnEnable()
    {
        exit1Action = gameControls.GameController.ExitGame1;
        exit2Action = gameControls.GameController.ExitGame2;
        fullscreenAction = gameControls.GameController.ToggleFullscreen;
        exit1Action.Enable();
        exit2Action.Enable();
        fullscreenAction.Enable();
    }

    private void OnDisable()
    {
        exit1Action.Disable();
        exit2Action.Disable();
        fullscreenAction.Disable();
    }

    private void Start()
    {
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(PauseState.i);

        partyScreen.Initialize();

        DialogManager.Instance.OnShowDialog += () =>
        {
            PlayerController.Character.Animator.IsMoving = false;
            PlayerController.Character.Animator.IsRunning = false;
            StateMachine.Push(DialogState.i);
        };

        DialogManager.Instance.OnDialogFinished += () =>
        {
            StateMachine.Pop();
        };
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            PlayerController.Character.Animator.IsMoving = false;
            PlayerController.Character.Animator.IsRunning = false;
            StateMachine.Push(PauseState.i);
        }
        else
            StateMachine.Pop();
    }

    public void StartBattle(BattleTrigger trigger)
    {
        BattleState.i.trigger = trigger;
        PlayerController.Character.Animator.IsMoving = false;
        PlayerController.Character.Animator.IsRunning = false;
        StateMachine.Push(BattleState.i);
    }

    PokemonStaticEncounter staticEncounter;

    public void StartStaticEncounterBattle(PokemonStaticEncounter staticEncounter, BattleTrigger trigger)
    {
        BattleState.i.staticEncounter = staticEncounter;
        BattleState.i.trigger = trigger;
        StateMachine.Push(BattleState.i);
    }

    TrainerController trainer;

    public void StartTrainerBattle(TrainerController trainer, BattleTrigger trigger)
    {
        BattleState.i.trainer = trainer;
        BattleState.i.trigger = trigger;
        StateMachine.Push(BattleState.i);
    }

    public void StartDoubleTrainerBattle(DoubleTrainerController trainerOne, DoubleTrainerController trainerTwo, BattleTrigger trigger)
    {
        BattleState.i.trainerOne = trainerOne;
        BattleState.i.trainerTwo = trainerTwo;
        BattleState.i.trigger = trigger;
        StateMachine.Push(BattleState.i);
    }

    public void StartGymLeaderBattle(GymLeaderController gymLeader, BattleTrigger trigger)
    {
        BattleState.i.gymLeader = gymLeader;
        BattleState.i.trigger = trigger;
        StateMachine.Push(BattleState.i);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    public void OnEnterDoubleTrainersView(DoubleTrainerController trainerOne)
    {
        StartCoroutine(trainerOne.TriggerDoubleTrainerBattle(playerController));
    }

    private void Update()
    {
        StateMachine.Execute();

        if (exit1Action.IsPressed() && exit2Action.IsPressed())
            Application.Quit();

        if (fullscreenAction.WasPressedThisFrame())
            Screen.fullScreen = !Screen.fullScreen;
    }

    public void SetCurrentScene(SceneDetails currentScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = currentScene;
        PlayerController.Character.Animator.IsUsingStrength = false;
    }

    public IEnumerator MoveCamera(Vector2 moveOffset, bool waitForFadeOut = false)
    {
        yield return Fader.i.FadeIn(0.5f);

        worldCamera.transform.position += new Vector3(moveOffset.x, moveOffset.y);

        if (waitForFadeOut)
            yield return Fader.i.FadeOut(0.5f);
        else
            StartCoroutine(Fader.i.FadeOut(0.5f));
    }

    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 30;
        style.normal.textColor = Color.white;
        /*
        GUILayout.Label("STATE STACK", style);
        foreach (var state in StateMachine.StateStack)
        {
            GUILayout.Label(state.GetType().ToString(), style);
        }
        */
    }

    public PlayerController PlayerController => playerController;

    public Camera WorldCamera => worldCamera;

    public PartyScreen PartyScreen => partyScreen;
}