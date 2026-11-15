using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using E7.Introloop;
using BTSUtilities.StateMachine;

public class BattleState : State<GameController>
{
    [SerializeField] BattleSystem battleSystem;

    public BattleTrigger trigger { get; set; }
    public TrainerController trainer { get; set; }
    public DoubleTrainerController trainerOne { get; set; }
    public DoubleTrainerController trainerTwo { get; set; }
    public GymLeaderController gymLeader { get; set; }
    public PokemonStaticEncounter staticEncounter { get; set; }

    public static BattleState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gameController;

    public override void Enter(GameController owner)
    {
        gameController = owner;
        battleSystem.gameObject.SetActive(true);
        gameController.WorldCamera.gameObject.SetActive(false);

        var playerParty = gameController.PlayerController.GetComponent<PokemonParty>();
        var mapArea = gameController.CurrentScene.GetComponent<MapArea>();

        if (trainer == null && trainerOne == null && trainerTwo == null && gymLeader == null && staticEncounter == null)
        {
            var wildPokemon = mapArea.GetRandomWildPokemon(trigger);
            var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level, false);
            battleSystem.StartBattle(playerParty, wildPokemonCopy, trigger, mapArea.Weather);
        }
        else if (trainer != null && trainerOne == null && trainerTwo == null && gymLeader == null && staticEncounter == null)
        {
            var trainerParty = trainer.GetComponent<PokemonParty>();
            battleSystem.StartTrainerBattle(playerParty, trainerParty, trigger, mapArea.Weather, unitCount: trainer.BattleUnitCount);
        }
        else if (trainer == null && trainerOne != null && trainerTwo != null && gymLeader == null && staticEncounter == null)
        {
            var trainerPartyOne = trainerOne.GetComponent<PokemonParty>();
            var trainerPartyTwo = trainerTwo.GetComponent<PokemonParty>();
            battleSystem.StartDoubleTrainerBattle(playerParty, trainerPartyOne, trainerPartyTwo, trigger, mapArea.Weather, unitCount: trainerOne.BattleUnitCount);
        }
        else if (trainer == null && trainerOne == null && trainerTwo == null && gymLeader != null && staticEncounter == null)
        {
            var gymLeaderParty = gymLeader.GetComponent<PokemonParty>();
            battleSystem.StartGymLeaderBattle(playerParty, gymLeaderParty, trigger, mapArea.Weather, unitCount: gymLeader.BattleUnitCount);
        }
        else if (trainer == null && trainerOne == null && trainerTwo == null && gymLeader == null && staticEncounter != null)
        {
            var staticEncounter = mapArea.GetRandomWildPokemon(trigger);
            var staticEncounterCopy = new Pokemon(staticEncounter.Base, staticEncounter.Level, false);
            battleSystem.StartStaticEncounterBattle(playerParty, staticEncounterCopy, trigger, mapArea.Weather);
        }
        
        battleSystem.OnBattleOver += EndBattle;
    }

    public override void Execute()
    {
        battleSystem.HandleUpdate();
    }

    public override void Exit()
    {
        battleSystem.gameObject.SetActive(false);
        gameController.WorldCamera.gameObject.SetActive(true);
        battleSystem.OnBattleOver -= EndBattle;
    }

    void EndBattle(bool won, bool caught)
    {
        if (trainer != null && trainerOne == null && trainerTwo == null && gymLeader == null && staticEncounter == null && won == true)
        {
            StartCoroutine(trainer.LostBattle(GameController.Instance.PlayerController.transform));
            trainer = null;
        }
        else if (trainer == null && trainerOne != null && trainerTwo != null && gymLeader == null && staticEncounter == null && won == true)
        {
            StartCoroutine(trainerOne.LostBattle(GameController.Instance.PlayerController.transform));
            StartCoroutine(trainerTwo.LostBattle(GameController.Instance.PlayerController.transform));
            trainerOne = null;
            trainerTwo = null;
        }
        else if (trainer == null && trainerOne == null && trainerTwo == null && gymLeader != null && staticEncounter == null && won == true)
        {
            StartCoroutine(gymLeader.LostBattle(GameController.Instance.PlayerController.transform));
            gymLeader = null;
        }
        else if (trainer == null && trainerOne == null && trainerTwo == null && gymLeader == null && staticEncounter != null && caught == true)
        {
            StartCoroutine(staticEncounter.CaughtPokemon());
            staticEncounter = null;
        }

        GameController.Instance.PartyScreen.SetPartyData();

        battleSystem.gameObject.SetActive(false);
        GameController.Instance.WorldCamera.gameObject.SetActive(true);

        var playerParty = GameController.Instance.PlayerController.GetComponent<PokemonParty>();
        bool hasEvolutions = playerParty.CheckForEvolutions();

        if (hasEvolutions)
            StartCoroutine(playerParty.RunEvolutions());
        else
        {
            var playerAnimator = GameController.Instance.PlayerController.GetComponent<CharacterAnimator>();
            var isSurfing = playerAnimator.IsSurfing;
            if (isSurfing)
            {
                var surfMusic = FindFirstObjectByType<SurfableWater>(FindObjectsInactive.Exclude).SurfingMusic;
                AudioManager.i.PlayMusic(surfMusic, true);
            }
            else
                AudioManager.i.PlayMusic(GameController.Instance.CurrentScene.SceneMusic, true);
        }

        gameController.StateMachine.Pop();
    }

    public BattleSystem BattleSystem => battleSystem;
}