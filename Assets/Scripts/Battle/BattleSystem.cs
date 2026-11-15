using BTSUtilities.StateMachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using E7.Introloop;

public enum BattleTrigger
{ 
    Plains, PlainsGrass, PlainsRock, Forest, ForestGrass, ForestRock, WaterEdge, Water, CityTown, Mountain, MountainGrass, MountainRock, Cave, CaveWater,
    CaveRock, BuildingA, BuildingB, BuildingC, Snow, Sand, SandRock, DeepSand, DeepSandRock, Marsh, MarshRock, CaveIce, PlainsWet, EliteFour, Champion,
    Omanyte, Kabuto, Aerodactyl, Lileep, Anorith, Cranidos, Shieldon, Tirtouga, Archen, Tyrunt, Amaura, Dracozolt, Dracovish, Arctozolt, Arctovish,
    Articuno, Zapdos, Moltres, Mewtwo, Raikou, Entei, Suicune, Lugia, HoOh, Regirock, Regice, Registeel, Latias, Latios, Kyogre, Groudon, Rayquaza, Uxie,
    Mesprit, Azelf, Dialga, Palkia, Giratina, Heatran, Regigigas, Cresselia, Cobalion, Terrakion, Virizion, Tornadus, Thundurus, Landorus, Reshiram,
    Zekrom, Kyurem, Xerneas, Yveltal, Zygarde, TypeNull, TapuKoko, TapuLele, TapuBulu, TapuFini, Cosmog1, Cosmog2, Necrozma, Zacian, Zamazenta, Eternatus,
    Kubfu, Regieleki, Regidrago, Glastrier, Spectrier, Calyrex, Mew, Celebi, Jirachi, Deoxys, Manaphy, Phione, Darkrai, Shaymin, Victini, Keldeo, Meloetta,
    Genesect, Diancie, Hoopa, Volcanion, Magearna, Marshadow, Zeraora, Meltan, Zarude, Arceus, Nihilego, Buzzwole, Pheromosa, Xurkitree, Celesteela,
    Kartana, Guzzlord, Poipole, Stakataka, Blacephalon, MissingNo
}

public class BattleSystem : MonoBehaviour
{
    [Header("General Variables")]
    [SerializeField] BattleUnit playerUnitSingle;
    [SerializeField] BattleUnit enemyUnitSingle;
    [SerializeField] List<BattleUnit> playerUnitsMultiple;
    [SerializeField] List<BattleUnit> enemyUnitsMultiple;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] Image gymLeaderImage;
    [SerializeField] GameObject pokeBallSprite;
    [SerializeField] List<Sprite> pokeBallImagesA;
    [SerializeField] List<Sprite> pokeBallImagesB;
    [SerializeField] List<Sprite> pokeBallImagesC;
    [SerializeField] MoveToForgetSelectionUI moveToForgetSelectionUI;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] GameObject singleBattleElements;
    [SerializeField] GameObject multiBattleElements;

    [Header("Music")]
    [SerializeField] IntroloopAudio wildBattleMusic;
    [SerializeField] IntroloopAudio trainerBattleMusic;
    [SerializeField] IntroloopAudio gymLeaderBattleMusic;
    [SerializeField] IntroloopAudio badGuyBattleMusic;
    [SerializeField] IntroloopAudio legendaryBattleMusic;
    [SerializeField] IntroloopAudio mythicalBattleMusic;
    [SerializeField] IntroloopAudio wildBattleVictory;
    [SerializeField] IntroloopAudio trainerBattleVictory;
    [SerializeField] IntroloopAudio gymLeaderBattleVictory;

    [Header("Sound Effects")]
    [SerializeField] AudioClip physicalAttackSound;
    [SerializeField] AudioClip specialAttackSound;
    [SerializeField] AudioClip statusAttackSound;

    [Header("Background")]
    [SerializeField] Image backgroundImage;
    [SerializeField] List<Sprite> battleBackgrounds;

    List<BattleUnit> playerUnits;
    List<BattleUnit> enemyUnits;

    int unitCount = 1;
    int unitInSelectionIndex = 0;
    public StateMachine<BattleSystem> StateMachine { get; private set; }
    public event Action<bool, bool> OnBattleOver;
    List<BattleAction> battleActions;
    public bool IsBattleOver { get; private set; }
    public PokemonParty PlayerParty { get; private set; }
    public PokemonParty TrainerParty { get; private set; }
    public PokemonParty TrainerPartyOne { get; private set; }
    public PokemonParty TrainerPartyTwo { get; private set; }
    public PokemonParty GymLeaderParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }
    public List<Pokemon> PokemonThatBattled { get; set; }
    public BattleField Field { get; private set; }
    public bool IsTrainerBattle { get; private set; } = false;
    public bool IsGymLeaderBattle { get; private set; } = false;
    public bool IsDoubleTrainerBattle { get; private set; } = false;
    public bool IsStaticEncounterBattle { get; private set; } = false;
    public PlayerController Player { get; private set; }
    public TrainerController Trainer { get; private set; }
    public DoubleTrainerController TrainerOne { get; private set; }
    public DoubleTrainerController TrainerTwo { get; private set; }
    public GymLeaderController GymLeader { get; private set; }
    public PokemonStaticEncounter StaticEncounter { get; private set; }
    public int EscapeAttempts { get; set; }
    public bool CaughtWildPokemon { get; private set; }

    BattleTrigger battleTrigger;

    public static BattleSystem Instance;
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon, BattleTrigger trigger = BattleTrigger.PlainsGrass, WeatherConditionID weather = WeatherConditionID.None)
    {
        this.PlayerParty = playerParty;
        this.WildPokemon = wildPokemon;
        this.unitCount = 1;
        PokemonThatBattled = new List<Pokemon>();
        
        IsTrainerBattle = false;
        IsGymLeaderBattle = false;
        IsDoubleTrainerBattle = false;
        IsStaticEncounterBattle = false;
        Player = playerParty.GetComponent<PlayerController>();

        battleTrigger = trigger;

        AudioManager.i.PlayMusic(wildBattleMusic, true);

        StartCoroutine(SetupBattle(weather));
    }

    public void StartStaticEncounterBattle(PokemonParty playerParty, Pokemon wildPokemon, BattleTrigger trigger = BattleTrigger.PlainsGrass, WeatherConditionID weather = WeatherConditionID.None)
    {
        this.PlayerParty = playerParty;
        this.WildPokemon = wildPokemon;
        this.unitCount = 1;
        PokemonThatBattled = new List<Pokemon>();

        IsTrainerBattle = false;
        IsGymLeaderBattle = false;
        IsDoubleTrainerBattle = false;
        IsStaticEncounterBattle = true;
        Player = playerParty.GetComponent<PlayerController>();

        battleTrigger = trigger;

        AudioManager.i.PlayMusic(wildBattleMusic, true);

        StartCoroutine(SetupBattle(weather));
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty, BattleTrigger trigger = BattleTrigger.Plains, WeatherConditionID weather = WeatherConditionID.None, int unitCount = 1)
    {
        this.PlayerParty = playerParty;
        this.TrainerParty = trainerParty;
        this.unitCount = unitCount;
        PokemonThatBattled = new List<Pokemon>();

        IsTrainerBattle = true;
        IsGymLeaderBattle = false;
        IsDoubleTrainerBattle = false;
        IsStaticEncounterBattle = false;
        Player = playerParty.GetComponent<PlayerController>();
        Trainer = trainerParty.GetComponent<TrainerController>();

        battleTrigger = trigger;

        if (Trainer.UsesBadGuyMusic)
            AudioManager.i.PlayMusic(badGuyBattleMusic, true);
        else
            AudioManager.i.PlayMusic(trainerBattleMusic, true);

        StartCoroutine(SetupBattle(weather));
    }

    public void StartDoubleTrainerBattle(PokemonParty playerParty, PokemonParty trainerPartyOne, PokemonParty trainerPartyTwo, BattleTrigger trigger = BattleTrigger.Plains, WeatherConditionID weather = WeatherConditionID.None, int unitCount = 2)
    {
        this.PlayerParty = playerParty;
        this.TrainerPartyOne = trainerPartyOne;
        this.TrainerPartyTwo = trainerPartyTwo;
        this.unitCount = unitCount;
        PokemonThatBattled = new List<Pokemon>();

        IsTrainerBattle = false;
        IsGymLeaderBattle = false;
        IsDoubleTrainerBattle = true;
        IsStaticEncounterBattle = false;
        Player = playerParty.GetComponent<PlayerController>();
        TrainerOne = trainerPartyOne.GetComponent<DoubleTrainerController>();
        TrainerTwo = trainerPartyTwo.GetComponent<DoubleTrainerController>();

        battleTrigger = trigger;

        AudioManager.i.PlayMusic(trainerBattleMusic, true);

        StartCoroutine(SetupBattle(weather));
    }

    public void StartGymLeaderBattle(PokemonParty playerParty, PokemonParty gymLeaderParty, BattleTrigger trigger = BattleTrigger.BuildingA, WeatherConditionID weather = WeatherConditionID.None, int unitCount = 1)
    {
        this.PlayerParty = playerParty;
        this.GymLeaderParty = gymLeaderParty;
        this.unitCount = unitCount;
        PokemonThatBattled = new List<Pokemon>();

        IsTrainerBattle = false;
        IsGymLeaderBattle = true;
        IsDoubleTrainerBattle = false;
        IsStaticEncounterBattle = false;
        Player = playerParty.GetComponent<PlayerController>();
        GymLeader = gymLeaderParty.GetComponent<GymLeaderController>();

        battleTrigger = trigger;

        AudioManager.i.PlayMusic(gymLeaderBattleMusic, true);
        playerParty.PokemonList.ForEach(p => p.ChangeFriendship(8, 5, 3));

        StartCoroutine(SetupBattle(weather));
    }

    public IEnumerator SetupBattle(WeatherConditionID weather)
    {
        singleBattleElements.SetActive(unitCount == 1);
        multiBattleElements.SetActive(unitCount > 1);

        if (unitCount == 1)
        {
            playerUnits = new List<BattleUnit>() { playerUnitSingle };
            enemyUnits = new List<BattleUnit>() { enemyUnitSingle };
        }
        else if (unitCount > 1)
        {
            playerUnits = playerUnitsMultiple.GetRange(0, playerUnitsMultiple.Count);
            enemyUnits = enemyUnitsMultiple.GetRange(0, enemyUnitsMultiple.Count);
        }

        StateMachine = new StateMachine<BattleSystem>(this);
        battleActions = new List<BattleAction>();

        for (int i = 0; i < unitCount; i++)
        {
            playerUnits[i].Clear();
            enemyUnits[i].Clear();
        }

        if (battleTrigger == BattleTrigger.Plains)
            backgroundImage.sprite = battleBackgrounds[0];
        else if (battleTrigger == BattleTrigger.PlainsGrass)
            backgroundImage.sprite = battleBackgrounds[1];
        else if (battleTrigger == BattleTrigger.PlainsRock || battleTrigger == BattleTrigger.SandRock || battleTrigger == BattleTrigger.DeepSandRock)
            backgroundImage.sprite = battleBackgrounds[2];
        else if (battleTrigger == BattleTrigger.Forest)
            backgroundImage.sprite = battleBackgrounds[3];
        else if (battleTrigger == BattleTrigger.ForestGrass)
            backgroundImage.sprite = battleBackgrounds[4];
        else if (battleTrigger == BattleTrigger.ForestRock || battleTrigger == BattleTrigger.MarshRock)
            backgroundImage.sprite = battleBackgrounds[5];
        else if (battleTrigger == BattleTrigger.WaterEdge)
            backgroundImage.sprite = battleBackgrounds[6];
        else if (battleTrigger == BattleTrigger.Water)
            backgroundImage.sprite = battleBackgrounds[7];
        else if (battleTrigger == BattleTrigger.CityTown)
            backgroundImage.sprite = battleBackgrounds[8];
        else if (battleTrigger == BattleTrigger.Mountain)
            backgroundImage.sprite = battleBackgrounds[9];
        else if (battleTrigger == BattleTrigger.MountainGrass)
            backgroundImage.sprite = battleBackgrounds[10];
        else if (battleTrigger == BattleTrigger.MountainRock || battleTrigger == BattleTrigger.Aerodactyl || battleTrigger == BattleTrigger.Archen || battleTrigger == BattleTrigger.Zapdos || battleTrigger == BattleTrigger.Moltres || battleTrigger == BattleTrigger.Lugia || battleTrigger == BattleTrigger.HoOh || battleTrigger == BattleTrigger.Latios || battleTrigger == BattleTrigger.Latias || battleTrigger == BattleTrigger.Cresselia || battleTrigger == BattleTrigger.Tornadus || battleTrigger == BattleTrigger.Thundurus || battleTrigger == BattleTrigger.Landorus || battleTrigger == BattleTrigger.TapuKoko || battleTrigger == BattleTrigger.TapuLele || battleTrigger == BattleTrigger.TapuBulu || battleTrigger == BattleTrigger.TapuFini || battleTrigger == BattleTrigger.Regieleki || battleTrigger == BattleTrigger.Regidrago)
            backgroundImage.sprite = battleBackgrounds[11];
        else if (battleTrigger == BattleTrigger.Cave || battleTrigger == BattleTrigger.CaveRock || battleTrigger == BattleTrigger.Omanyte || battleTrigger == BattleTrigger.Kabuto || battleTrigger == BattleTrigger.Lileep || battleTrigger == BattleTrigger.Anorith || battleTrigger == BattleTrigger.Cranidos || battleTrigger == BattleTrigger.Shieldon || battleTrigger == BattleTrigger.Tyrunt || battleTrigger == BattleTrigger.Amaura || battleTrigger == BattleTrigger.Dracozolt || battleTrigger == BattleTrigger.Arctozolt || battleTrigger == BattleTrigger.Raikou || battleTrigger == BattleTrigger.Entei || battleTrigger == BattleTrigger.Suicune || battleTrigger == BattleTrigger.Regirock || battleTrigger == BattleTrigger.Registeel || battleTrigger == BattleTrigger.Groudon || battleTrigger == BattleTrigger.Uxie || battleTrigger == BattleTrigger.Mesprit || battleTrigger == BattleTrigger.Azelf || battleTrigger == BattleTrigger.Dialga || battleTrigger == BattleTrigger.Palkia || battleTrigger == BattleTrigger.Heatran || battleTrigger == BattleTrigger.Regigigas || battleTrigger == BattleTrigger.Shaymin || battleTrigger == BattleTrigger.Victini || battleTrigger == BattleTrigger.Cobalion || battleTrigger == BattleTrigger.Terrakion || battleTrigger == BattleTrigger.Virizion || battleTrigger == BattleTrigger.Reshiram || battleTrigger == BattleTrigger.Zekrom || battleTrigger == BattleTrigger.Keldeo || battleTrigger == BattleTrigger.Meloetta || battleTrigger == BattleTrigger.Genesect || battleTrigger == BattleTrigger.Xerneas || battleTrigger == BattleTrigger.Yveltal || battleTrigger == BattleTrigger.Zygarde || battleTrigger == BattleTrigger.Diancie || battleTrigger == BattleTrigger.Hoopa || battleTrigger == BattleTrigger.Volcanion || battleTrigger == BattleTrigger.TypeNull || battleTrigger == BattleTrigger.Magearna || battleTrigger == BattleTrigger.Marshadow || battleTrigger == BattleTrigger.Zeraora || battleTrigger == BattleTrigger.Meltan || battleTrigger == BattleTrigger.Zacian || battleTrigger == BattleTrigger.Zamazenta || battleTrigger == BattleTrigger.Kubfu || battleTrigger == BattleTrigger.Zarude || battleTrigger == BattleTrigger.Spectrier || battleTrigger == BattleTrigger.Calyrex)
            backgroundImage.sprite = battleBackgrounds[12];
        else if (battleTrigger == BattleTrigger.CaveWater || battleTrigger == BattleTrigger.Tirtouga || battleTrigger == BattleTrigger.Kyogre || battleTrigger == BattleTrigger.Phione || battleTrigger == BattleTrigger.Manaphy)
            backgroundImage.sprite = battleBackgrounds[13];
        else if (battleTrigger == BattleTrigger.BuildingA)
            backgroundImage.sprite = battleBackgrounds[14];
        else if (battleTrigger == BattleTrigger.BuildingB)
            backgroundImage.sprite = battleBackgrounds[15];
        else if (battleTrigger == BattleTrigger.BuildingC)
            backgroundImage.sprite = battleBackgrounds[16];
        else if (battleTrigger == BattleTrigger.Snow)
            backgroundImage.sprite = battleBackgrounds[17];
        else if (battleTrigger == BattleTrigger.Sand || battleTrigger == BattleTrigger.DeepSand)
            backgroundImage.sprite = battleBackgrounds[18];
        else if (battleTrigger == BattleTrigger.Marsh)
            backgroundImage.sprite = battleBackgrounds[19];
        else if (battleTrigger == BattleTrigger.CaveIce || battleTrigger == BattleTrigger.Articuno || battleTrigger == BattleTrigger.Regice || battleTrigger == BattleTrigger.Kyurem || battleTrigger == BattleTrigger.Glastrier)
            backgroundImage.sprite = battleBackgrounds[20];
        else if (battleTrigger == BattleTrigger.PlainsWet)
            backgroundImage.sprite = battleBackgrounds[21];
        else if (battleTrigger == BattleTrigger.EliteFour)
            backgroundImage.sprite = battleBackgrounds[22];
        else if (battleTrigger == BattleTrigger.Champion || battleTrigger == BattleTrigger.Mewtwo || battleTrigger == BattleTrigger.Mew || battleTrigger == BattleTrigger.Celebi || battleTrigger == BattleTrigger.Rayquaza || battleTrigger == BattleTrigger.Jirachi || battleTrigger == BattleTrigger.Deoxys || battleTrigger == BattleTrigger.Giratina || battleTrigger == BattleTrigger.Arceus || battleTrigger == BattleTrigger.MissingNo || battleTrigger == BattleTrigger.Cosmog1 || battleTrigger == BattleTrigger.Cosmog2 || battleTrigger == BattleTrigger.Necrozma || battleTrigger == BattleTrigger.Eternatus || battleTrigger == BattleTrigger.Nihilego || battleTrigger == BattleTrigger.Buzzwole || battleTrigger == BattleTrigger.Pheromosa || battleTrigger == BattleTrigger.Xurkitree || battleTrigger == BattleTrigger.Celesteela || battleTrigger == BattleTrigger.Kartana || battleTrigger == BattleTrigger.Guzzlord || battleTrigger == BattleTrigger.Poipole || battleTrigger == BattleTrigger.Stakataka || battleTrigger == BattleTrigger.Blacephalon)
            backgroundImage.sprite = battleBackgrounds[23];

        if (!IsTrainerBattle && !IsDoubleTrainerBattle && !IsGymLeaderBattle)
        {   // Wild Pokémon Battle
            playerUnits[0].Setup(PlayerParty.GetHealthyPokemon());
            
            if (!PokemonThatBattled.Contains(playerUnits[0].Pokemon))
                PokemonThatBattled.Add(playerUnits[0].Pokemon);
            
            enemyUnits[0].Setup(WildPokemon);

            dialogBox.SetMoveNames(playerUnits[0].Pokemon.Moves);

            dialogBox.EnableActionSelector(false);

            yield return dialogBox.TypeDialog($"A wild {enemyUnits[0].Pokemon.Base.PokemonName} appeared!");
            yield return dialogBox.TypeDialog($"Go! {playerUnits[0].Pokemon.Base.PokemonName}!");
        }
        else
        {
            if (IsTrainerBattle)
            {   // Trainer Battle
                // Show trainer and player characters
                for (int i = 0; i < unitCount; i++)
                {
                    playerUnits[i].gameObject.SetActive(false);
                    enemyUnits[i].gameObject.SetActive(false);
                }

                dialogBox.EnableActionSelector(false);

                playerImage.gameObject.SetActive(true);
                trainerImage.gameObject.SetActive(true);

                playerImage.sprite = Player.PlayerSprite;
                trainerImage.sprite = Trainer.TrainerSprite;

                var originalPlayerPosition = playerImage.transform.localPosition;
                var originalTrainerPosition = trainerImage.transform.localPosition;

                playerImage.transform.localPosition = new Vector3(originalPlayerPosition.x - 480f, originalPlayerPosition.y);
                trainerImage.transform.localPosition = new Vector3(originalTrainerPosition.x + 480f, originalTrainerPosition.y);

                playerImage.transform.DOLocalMoveX(originalPlayerPosition.x, 0.9f);
                trainerImage.transform.DOLocalMoveX(originalTrainerPosition.x, 0.9f);

                yield return dialogBox.TypeDialog($"{Trainer.TrainerClass} {Trainer.TrainerName} wants to battle!");
                yield return new WaitForSeconds(0.5f);

                trainerImage.transform.DOLocalMoveX(originalTrainerPosition.x + 480f, 0.9f);

                yield return new WaitForSeconds(0.5f);
                trainerImage.gameObject.SetActive(false);

                var enemyPokemonList = TrainerParty.GetMultipleHealthyPokemon(unitCount);
                for (int i = 0; i < enemyPokemonList.Count; i++)
                {
                    enemyUnits[i].gameObject.SetActive(true);
                    enemyUnits[i].Setup(enemyPokemonList[i]);
                }

                var enemyPokemonNames = String.Join(" and ", enemyPokemonList.Select(p => p.Base.PokemonName));
                yield return dialogBox.TypeDialog($"{Trainer.TrainerClass} {Trainer.TrainerName} sent out {enemyPokemonNames}!");

                playerImage.transform.DOLocalMoveX(originalPlayerPosition.x - 480f, 0.9f);

                yield return new WaitForSeconds(0.5f);
                playerImage.gameObject.SetActive(false);

                var playerPokemonList = PlayerParty.GetMultipleHealthyPokemon(unitCount);
                for (int i = 0; i < playerPokemonList.Count; i++)
                {
                    playerUnits[i].gameObject.SetActive(true);
                    playerUnits[i].Setup(playerPokemonList[i]);
                    
                    if (!PokemonThatBattled.Contains(playerPokemonList[i]))
                        PokemonThatBattled.Add(playerPokemonList[i]);
                }

                var playerPokemonNames = String.Join(" and ", playerPokemonList.Select(p => p.Base.PokemonName));
                yield return dialogBox.TypeDialog($"Go, {playerPokemonNames}!");
            }
            else if (IsGymLeaderBattle)
            {   // Gym Leader Battle
                // Show gym leader and player characters
                for (int i = 0; i < unitCount; i++)
                {
                    playerUnits[i].gameObject.SetActive(false);
                    enemyUnits[i].gameObject.SetActive(false);
                }

                dialogBox.EnableActionSelector(false);

                playerImage.gameObject.SetActive(true);
                gymLeaderImage.gameObject.SetActive(true);

                playerImage.sprite = Player.PlayerSprite;
                gymLeaderImage.sprite = GymLeader.TrainerSprite;

                var originalPlayerPosition = playerImage.transform.localPosition;
                var originalGymLeaderPosition = gymLeaderImage.transform.localPosition;

                playerImage.transform.localPosition = new Vector3(originalPlayerPosition.x - 480f, originalPlayerPosition.y);
                gymLeaderImage.transform.localPosition = new Vector3(originalGymLeaderPosition.x + 480f, originalGymLeaderPosition.y);

                playerImage.transform.DOLocalMoveX(originalPlayerPosition.x, 0.9f);
                gymLeaderImage.transform.DOLocalMoveX(originalGymLeaderPosition.x, 0.9f);

                yield return dialogBox.TypeDialog($"Gym Leader {GymLeader.TrainerName} wants to battle!");
                yield return new WaitForSeconds(0.5f);

                gymLeaderImage.transform.DOLocalMoveX(originalGymLeaderPosition.x + 480f, 0.9f);

                yield return new WaitForSeconds(0.5f);
                gymLeaderImage.gameObject.SetActive(false);

                var enemyPokemonList = GymLeaderParty.GetMultipleHealthyPokemon(unitCount);
                for (int i = 0; i < enemyPokemonList.Count; i++)
                {
                    enemyUnits[i].gameObject.SetActive(true);
                    enemyUnits[i].Setup(enemyPokemonList[i]);
                }

                var enemyPokemonNames = String.Join(" and ", enemyPokemonList.Select(p => p.Base.PokemonName));
                yield return dialogBox.TypeDialog($"Gym Leader {GymLeader.TrainerName} sent out {enemyPokemonNames}!");

                playerImage.transform.DOLocalMoveX(originalPlayerPosition.x - 480f, 0.9f);

                yield return new WaitForSeconds(0.5f);
                playerImage.gameObject.SetActive(false);

                var playerPokemonList = PlayerParty.GetMultipleHealthyPokemon(unitCount);
                for (int i = 0; i < playerPokemonList.Count; i++)
                {
                    playerUnits[i].gameObject.SetActive(true);
                    playerUnits[i].Setup(playerPokemonList[i]);

                    if (!PokemonThatBattled.Contains(playerPokemonList[i]))
                        PokemonThatBattled.Add(playerPokemonList[i]);
                }

                var playerPokemonNames = String.Join(" and ", playerPokemonList.Select(p => p.Base.PokemonName));
                yield return dialogBox.TypeDialog($"Go, {playerPokemonNames}!");
            }
            else if (IsDoubleTrainerBattle)
            {   // Double Trainer Battle
                // Show trainer and player characters
                for (int i = 0; i < unitCount; i++)
                {
                    playerUnits[i].gameObject.SetActive(false);
                    enemyUnits[i].gameObject.SetActive(false);
                }
                
                dialogBox.EnableActionSelector(false);

                playerImage.gameObject.SetActive(true);
                trainerImage.gameObject.SetActive(true);

                playerImage.sprite = Player.PlayerSprite;
                trainerImage.sprite = TrainerOne.TrainerSprite;

                var originalPlayerPosition = playerImage.transform.localPosition;
                var originalTrainerPosition = trainerImage.transform.localPosition;

                playerImage.transform.localPosition = new Vector3(originalPlayerPosition.x - 480f, originalPlayerPosition.y);
                trainerImage.transform.localPosition = new Vector3(originalTrainerPosition.x + 480f, originalTrainerPosition.y);

                playerImage.transform.DOLocalMoveX(originalPlayerPosition.x, 0.9f);
                trainerImage.transform.DOLocalMoveX(originalTrainerPosition.x, 0.9f);

                yield return dialogBox.TypeDialog($"{TrainerOne.TrainerClass} {TrainerOne.TrainerName} and {TrainerTwo.TrainerName} want to battle!");
                yield return new WaitForSeconds(0.5f);

                trainerImage.transform.DOLocalMoveX(originalTrainerPosition.x + 480f, 0.9f);

                yield return new WaitForSeconds(0.5f);
                trainerImage.gameObject.SetActive(false);

                List<Pokemon> enemyPokemonListOne = TrainerPartyOne.GetMultipleHealthyPokemon(unitCount);
                List<Pokemon> enemyPokemonListTwo = TrainerPartyTwo.GetMultipleHealthyPokemon(unitCount);
                List<Pokemon> enemyPokemonList = new List<Pokemon>();
                enemyPokemonList = enemyPokemonListOne;
                for (int i = 0; i < enemyPokemonList.Count; i++)
                {
                    enemyUnits[i].gameObject.SetActive(true);
                    enemyUnits[i].Setup(enemyPokemonList[i]);
                }

                var enemyPokemonNames = String.Join(" and ", enemyPokemonList.Select(p => p.Base.PokemonName));
                yield return dialogBox.TypeDialog($"{TrainerOne.TrainerClass} {TrainerOne.TrainerName} and {TrainerTwo.TrainerName} sent out {enemyPokemonNames}!");

                playerImage.transform.DOLocalMoveX(originalPlayerPosition.x - 480f, 0.9f);
                
                yield return new WaitForSeconds(0.5f);
                playerImage.gameObject.SetActive(false);

                var playerPokemonList = PlayerParty.GetMultipleHealthyPokemon(unitCount);
                for (int i = 0; i < playerPokemonList.Count; i++)
                {
                    playerUnits[i].gameObject.SetActive(true);
                    playerUnits[i].Setup(playerPokemonList[i]);

                    if (!PokemonThatBattled.Contains(playerPokemonList[i]))
                        PokemonThatBattled.Add(playerPokemonList[i]);
                }
                
                var playerPokemonNames = String.Join(" and ", playerPokemonList.Select(p => p.Base.PokemonName));
                yield return dialogBox.TypeDialog($"Go, {playerPokemonNames}!");
            }
        }

        Field = new BattleField();
        if (weather != WeatherConditionID.None)
        {
            Field.SetWeather(weather);
            yield return dialogBox.TypeDialog(Field.Weather.StartMessage);
        }

        IsBattleOver = false;
        CaughtWildPokemon = false;
        EscapeAttempts = 0;
        partyScreen.Initialize();
        unitInSelectionIndex = 0;
        StateMachine.ChangeState(ActionSelectionState.i);
    }

    public void BattleOver(bool won, bool caught)
    {
        IsBattleOver = true;
        CaughtWildPokemon = caught;

        PlayerParty.PokemonList.ForEach(p => p.OnBattleOver());

        playerUnits.ForEach(u => u.HUD.ClearData());
        playerUnits.ForEach(v => v.Pokemon.CalculateStats());
        enemyUnits.ForEach(e => e.HUD.ClearData());
        enemyUnits.ForEach(f => f.Pokemon.CalculateStats());

        OnBattleOver(won, caught);
    }

    public void HandleUpdate()
    {
        StateMachine.Execute();
    }

    public void AddBattleAction(BattleAction battleAction)
    {
        battleAction.User = UnitInSelection;
        battleActions.Add(battleAction);

        if (battleActions.Count == ActivePlayerUnitsCount)
        {
            // Add enemy actions
            foreach (var enemyUnit in enemyUnits)
            {
                enemyUnit.Pokemon.CalculateStats();
                var targetUnit = playerUnits[UnityEngine.Random.Range(0, ActivePlayerUnitsCount)];
                var selectedMove = enemyUnit.Pokemon.ChooseMove(targetUnit.Pokemon, Trainer, GymLeader, TrainerOne);
                battleActions.Add(new BattleAction()
                {
                    Type = BattleActionType.Move,
                    User = enemyUnit,
                    Target = targetUnit,
                    SelectedMove = selectedMove,
                });
            }

            // Sort actions by priority and Speed
            battleActions = battleActions.OrderByDescending(a => a.Priority).ThenByDescending(a => a.User.Pokemon.ModifySpd(a.User.Pokemon.Speed, a.Target.Pokemon, a.SelectedMove)).ToList();

            // Run Turns
            RunTurnState.i.Actions = battleActions;
            StateMachine.ChangeState(RunTurnState.i);
        }
        else
        {
            // Select another action
            unitInSelectionIndex++;
            StateMachine.ChangeState(ActionSelectionState.i);
        }
    }

    public void ClearTurnData()
    {
        battleActions = new List<BattleAction>();
        unitInSelectionIndex = 0;
    }

    public IEnumerator SwitchPokemon(Pokemon newPokemon, BattleUnit unitToSwitch)
    {
        if (unitToSwitch.Pokemon.HP > 0)
        {
            unitToSwitch.Pokemon.CureVolatileStatus();
            yield return dialogBox.TypeDialog($"Come back, {unitToSwitch.Pokemon.Base.PokemonName}!");
            unitToSwitch.PlayExitAnimation();
            yield return new WaitForSeconds(2f);
        }
        else
        {
            if (PokemonThatBattled.Contains(unitToSwitch.Pokemon))
                PokemonThatBattled.Remove(unitToSwitch.Pokemon);
        }

        unitToSwitch.Setup(newPokemon);
        if (!PokemonThatBattled.Contains(newPokemon))
            PokemonThatBattled.Add(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go, {newPokemon.Base.PokemonName}!");
    }

    public IEnumerator SendNextTrainerPokemon(BattleUnit unitToSwitch)
    {
        var activePokemonList = EnemyUnits.Select(u => u.Pokemon).Where(p => p.HP > 0).ToList();

        var nextPokemon = TrainerParty.GetHealthyPokemon(dontInclude: activePokemonList);
        unitToSwitch.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{Trainer.TrainerClass} {Trainer.TrainerName} sent out {nextPokemon.Base.PokemonName}!");

        if (unitCount == 1)
            PokemonThatBattled = new List<Pokemon>() { playerUnits[0].Pokemon };
    }

    public IEnumerator SendNextDoubleTrainerPokemon(BattleUnit unitToSwitch)
    {
        var activePokemonList = EnemyUnits.Select(u => u.Pokemon).Where(p => p.HP > 0).ToList();

        var nextPokemon = TrainerPartyOne.GetHealthyPokemon(dontInclude: activePokemonList);
        if (nextPokemon == null)
            nextPokemon = TrainerPartyTwo.GetHealthyPokemon(dontInclude: activePokemonList);

        unitToSwitch.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{TrainerOne.TrainerClass} {TrainerOne.TrainerName} and {TrainerTwo.TrainerName} sent out {nextPokemon.Base.PokemonName}!");
    }

    public IEnumerator SendNextGymLeaderPokemon(BattleUnit unitToSwitch)
    {
        var activePokemonList = EnemyUnits.Select(u => u.Pokemon).Where(p => p.HP > 0).ToList();

        var nextPokemon = GymLeaderParty.GetHealthyPokemon(dontInclude: activePokemonList);
        unitToSwitch.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"Gym Leader {GymLeader.TrainerName} sent out {nextPokemon.Base.PokemonName}!");
    }

    public IEnumerator ApplyItemBoosts(BattleItem battleItem, BattleUnit usedOn)
    {
        if (battleItem.Boosts != null)
        {
            usedOn.Pokemon.ApplyBoosts(battleItem.Boosts, usedOn.Pokemon);
            yield break;
        }
    }
    
    public IEnumerator ThrowPokeBall(PokeBallItem pokeBallItem)
    {
        dialogBox.EnableActionSelector(false);
        
        if (IsTrainerBattle || IsGymLeaderBattle)
        {
            yield return dialogBox.TypeDialog($"Hey! You can't steal a trainer's Pokémon!");
            yield break;
        }
        
        if (pokeBallItem.ItemName == "Ultra Ball")
            yield return dialogBox.TypeDialog($"{Player.PlayerName} threw an Ultra Ball!");
        else
            yield return dialogBox.TypeDialog($"{Player.PlayerName} threw a {pokeBallItem.ItemName}!");
        
        var pokeBallObject = Instantiate(pokeBallSprite, playerUnits[0].transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeBall = pokeBallObject.GetComponent<SpriteRenderer>();
        
        int pokeBallID = pokeBallItem.BallID;
        
        pokeBall.sprite = pokeBallImagesA[pokeBallID];
        yield return pokeBall.transform.DOJump(new Vector3(enemyUnits[0].transform.position.x, 3.6f), 2f, 1, 1f).WaitForCompletion();

        pokeBall.sprite = pokeBallImagesB[pokeBallID];
        yield return new WaitForSeconds(0.1f);
        pokeBall.sprite = pokeBallImagesC[pokeBallID];
        
        yield return enemyUnits[0].PlayCaptureAnimation();
        
        pokeBall.sprite = pokeBallImagesB[pokeBallID];
        yield return new WaitForSeconds(0.1f);
        pokeBall.sprite = pokeBallImagesA[pokeBallID];
        
        yield return pokeBall.transform.DOJump(new Vector3(enemyUnits[0].transform.position.x, 0.6f), 2.5f, 1, 0.75f).WaitForCompletion();
        yield return pokeBall.transform.DOJump(new Vector3(enemyUnits[0].transform.position.x, 0.6f), 1.5f, 1, 0.6f).WaitForCompletion();
        yield return pokeBall.transform.DOJump(new Vector3(enemyUnits[0].transform.position.x, 0.6f), 1f, 1, 0.4f).WaitForCompletion();
        yield return pokeBall.transform.DOJump(new Vector3(enemyUnits[0].transform.position.x, 0.6f), 0.5f, 1, 0.2f).WaitForCompletion();
        yield return new WaitForSeconds(0.25f);
        
        float punchStrength = 12f;
        float punchPower = -0.11f;
        
        int shakeCount = TryToCatchPokemon(enemyUnits[0].Pokemon, pokeBallItem);
        
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.3f);
            yield return pokeBall.transform.DOPunchRotation(new Vector3(0f, 0f, punchStrength), 0.8f);
            yield return pokeBall.transform.DOPunchPosition(new Vector3(punchPower, 0f, 0f), 0.8f).WaitForCompletion();
            
            punchStrength -= 4f;
            punchPower += 0.025f;
        }
        
        if (shakeCount == 4)
        {
            yield return pokeBall.DOColor(Color.gray, 0.1f).WaitForCompletion();
            yield return pokeBall.DOColor(Color.white, 0.1f).WaitForCompletion();
            yield return new WaitForSeconds(0.1f);
            yield return dialogBox.TypeDialog($"Gotcha! {enemyUnits[0].Pokemon.Base.PokemonName} was caught!");
            yield return pokeBall.DOFade(0, 1.5f).WaitForCompletion();
            
            if (PlayerParty.PokemonList.Count == 6)
                yield return dialogBox.TypeDialog($"{enemyUnits[0].Pokemon.Base.PokemonName} has been put in one of your boxes!");
            else
                yield return dialogBox.TypeDialog($"{enemyUnits[0].Pokemon.Base.PokemonName} has been added to your party!");
            
            Destroy(pokeBall);

            for (int i = 0; i < PlayerParty.PokemonList.Count; i++)
            {
                var playerPokemon = PlayerParty.PokemonList[i];

                playerPokemon.GainEVs(enemyUnits[0].Pokemon.Base.EVYields);

                // Gain EXP
                int expYield = enemyUnits[0].Pokemon.Base.ExpYield;
                int enemyLevel = enemyUnits[0].Pokemon.Level;
                int expGain;

                if ((playerPokemon.Level >= Player.pokemonLevelCap || playerPokemon.HP <= 0) || (battleTrigger == BattleTrigger.Aerodactyl || battleTrigger == BattleTrigger.Archen || battleTrigger == BattleTrigger.Zapdos || battleTrigger == BattleTrigger.Moltres || battleTrigger == BattleTrigger.Lugia || battleTrigger == BattleTrigger.HoOh || battleTrigger == BattleTrigger.Latios || battleTrigger == BattleTrigger.Latias || battleTrigger == BattleTrigger.Cresselia || battleTrigger == BattleTrigger.Tornadus || battleTrigger == BattleTrigger.Thundurus || battleTrigger == BattleTrigger.Landorus || battleTrigger == BattleTrigger.TapuKoko || battleTrigger == BattleTrigger.TapuLele || battleTrigger == BattleTrigger.TapuBulu || battleTrigger == BattleTrigger.TapuFini || battleTrigger == BattleTrigger.Regieleki || battleTrigger == BattleTrigger.Regidrago || battleTrigger == BattleTrigger.Omanyte || battleTrigger == BattleTrigger.Kabuto || battleTrigger == BattleTrigger.Lileep || battleTrigger == BattleTrigger.Anorith || battleTrigger == BattleTrigger.Cranidos || battleTrigger == BattleTrigger.Shieldon || battleTrigger == BattleTrigger.Tyrunt || battleTrigger == BattleTrigger.Amaura || battleTrigger == BattleTrigger.Dracozolt || battleTrigger == BattleTrigger.Arctozolt || battleTrigger == BattleTrigger.Raikou || battleTrigger == BattleTrigger.Entei || battleTrigger == BattleTrigger.Suicune || battleTrigger == BattleTrigger.Regirock || battleTrigger == BattleTrigger.Registeel || battleTrigger == BattleTrigger.Groudon || battleTrigger == BattleTrigger.Uxie || battleTrigger == BattleTrigger.Mesprit || battleTrigger == BattleTrigger.Azelf || battleTrigger == BattleTrigger.Dialga || battleTrigger == BattleTrigger.Palkia || battleTrigger == BattleTrigger.Heatran || battleTrigger == BattleTrigger.Regigigas || battleTrigger == BattleTrigger.Shaymin || battleTrigger == BattleTrigger.Victini || battleTrigger == BattleTrigger.Cobalion || battleTrigger == BattleTrigger.Terrakion || battleTrigger == BattleTrigger.Virizion || battleTrigger == BattleTrigger.Reshiram || battleTrigger == BattleTrigger.Zekrom || battleTrigger == BattleTrigger.Keldeo || battleTrigger == BattleTrigger.Meloetta || battleTrigger == BattleTrigger.Genesect || battleTrigger == BattleTrigger.Xerneas || battleTrigger == BattleTrigger.Yveltal || battleTrigger == BattleTrigger.Zygarde || battleTrigger == BattleTrigger.Diancie || battleTrigger == BattleTrigger.Hoopa || battleTrigger == BattleTrigger.Volcanion || battleTrigger == BattleTrigger.TypeNull || battleTrigger == BattleTrigger.Magearna || battleTrigger == BattleTrigger.Marshadow || battleTrigger == BattleTrigger.Zeraora || battleTrigger == BattleTrigger.Meltan || battleTrigger == BattleTrigger.Zacian || battleTrigger == BattleTrigger.Zamazenta || battleTrigger == BattleTrigger.Eternatus || battleTrigger == BattleTrigger.Kubfu || battleTrigger == BattleTrigger.Zarude || battleTrigger == BattleTrigger.Spectrier || battleTrigger == BattleTrigger.Calyrex || battleTrigger == BattleTrigger.Tirtouga || battleTrigger == BattleTrigger.Kyogre || battleTrigger == BattleTrigger.Phione || battleTrigger == BattleTrigger.Manaphy || battleTrigger == BattleTrigger.Articuno || battleTrigger == BattleTrigger.Regice || battleTrigger == BattleTrigger.Kyurem || battleTrigger == BattleTrigger.Glastrier || battleTrigger == BattleTrigger.Mewtwo || battleTrigger == BattleTrigger.Mew || battleTrigger == BattleTrigger.Celebi || battleTrigger == BattleTrigger.Rayquaza || battleTrigger == BattleTrigger.Jirachi || battleTrigger == BattleTrigger.Deoxys || battleTrigger == BattleTrigger.Giratina || battleTrigger == BattleTrigger.Arceus || battleTrigger == BattleTrigger.MissingNo || battleTrigger == BattleTrigger.Cosmog1 || battleTrigger == BattleTrigger.Cosmog2 || battleTrigger == BattleTrigger.Necrozma || battleTrigger == BattleTrigger.Eternatus || battleTrigger == BattleTrigger.Nihilego || battleTrigger == BattleTrigger.Buzzwole || battleTrigger == BattleTrigger.Pheromosa || battleTrigger == BattleTrigger.Xurkitree || battleTrigger == BattleTrigger.Celesteela || battleTrigger == BattleTrigger.Kartana || battleTrigger == BattleTrigger.Guzzlord || battleTrigger == BattleTrigger.Poipole || battleTrigger == BattleTrigger.Stakataka || battleTrigger == BattleTrigger.Blacephalon))
                    expGain = 0;
                else
                {
                    if (PokemonThatBattled.Contains(playerPokemon))
                        expGain = Mathf.Max(Mathf.Min(Mathf.FloorToInt((expYield * enemyLevel) / 5f), playerPokemon.Base.GetEXPForLevel(Player.pokemonLevelCap) - playerPokemon.EXP), 1);
                    else
                        expGain = Mathf.Max(Mathf.Min(Mathf.FloorToInt((expYield * enemyLevel) / 12f), playerPokemon.Base.GetEXPForLevel(Player.pokemonLevelCap) - playerPokemon.EXP), 1);
                }

                playerPokemon.EXP += expGain;

                if (PokemonThatBattled.Contains(playerPokemon))
                {
                    if (playerPokemon.Level < Player.pokemonLevelCap)
                    {
                        yield return dialogBox.TypeDialog($"{playerPokemon.Base.PokemonName} gained {expGain} EXP points!");
                        if (PlayerUnits[0].Pokemon == playerPokemon)
                            yield return PlayerUnits[0].HUD.SetEXPSmooth();
                    }
                }

                // Check for level up
                while (playerPokemon.CheckForLevelUp())
                {
                    if (PlayerUnits[0].Pokemon == playerPokemon)
                    {
                        PlayerUnits[0].HUD.SetLevel();
                        PlayerUnits[0].HUD.SetHPBar();
                    }
                    yield return dialogBox.TypeDialog($"{playerPokemon.Base.PokemonName} grew to Level {playerPokemon.Level}!");

                    // Try to learn new move
                    var newMove = playerPokemon.GetLearnableMoveAtCurrentLevel();
                    if (newMove != null && !playerPokemon.HasMove(newMove.Base))
                    {
                        if (playerPokemon.Moves.Count < PokemonBase.MaxNumberOfMoves)
                        {
                            playerPokemon.LearnMove(newMove.Base);
                            yield return dialogBox.TypeDialog($"{playerPokemon.Base.PokemonName} learned {newMove.Base.MoveName}!");
                            dialogBox.SetMoveNames(playerPokemon.Moves);
                        }
                        else
                        {
                            yield return dialogBox.TypeDialog($"{playerPokemon.Base.PokemonName} is trying to learn {newMove.Base.MoveName}.");
                            yield return dialogBox.TypeDialog($"But it already knows {PokemonBase.MaxNumberOfMoves} moves!");
                            yield return dialogBox.TypeDialog($"Choose a move you want {playerPokemon.Base.PokemonName} to forget.");

                            MoveToForgetState.i.CurrentMoves = playerPokemon.Moves.Select(m => m.Base).ToList();
                            MoveToForgetState.i.NewMove = newMove.Base;
                            yield return GameController.Instance.StateMachine.PushAndWait(MoveToForgetState.i);

                            int moveIndex = MoveToForgetState.i.Selection;

                            if (moveIndex == PokemonBase.MaxNumberOfMoves || moveIndex == -1)
                                yield return dialogBox.TypeDialog($"{playerPokemon.Base.PokemonName} did not learn {newMove.Base.MoveName}.");
                            else
                            {
                                var selectedMove = playerPokemon.Moves[moveIndex].Base;
                                yield return dialogBox.TypeDialog($"{playerPokemon.Base.PokemonName} forgot {selectedMove.MoveName} and learned {newMove.Base.MoveName}!");

                                playerPokemon.Moves[moveIndex] = new Move(newMove.Base);
                            }
                        }
                    }

                    if (PlayerUnits[0].Pokemon == playerPokemon)
                        yield return PlayerUnits[0].HUD.SetEXPSmooth(true);
                }
            }

            PlayerParty.AddPokemon(enemyUnits[0].Pokemon);

            yield return new WaitForSeconds(0.5f);

            BattleOver(true, true);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            pokeBall.sprite = pokeBallImagesB[pokeBallID];
            yield return new WaitForSeconds(0.1f);
            pokeBall.sprite = pokeBallImagesC[pokeBallID];
            yield return pokeBall.DOFade(0, 0.2f);
            
            yield return enemyUnits[0].PlayBreakOutAnimation();
            
            if (shakeCount == 3)
                yield return dialogBox.TypeDialog($"Gah! It was so close, too!");
            else if (shakeCount == 2)
                yield return dialogBox.TypeDialog($"Aargh! Almost had it!");
            else if (shakeCount == 1)
                yield return dialogBox.TypeDialog($"Aww! It appeared to be caught!");
            else if (shakeCount == 0)
                yield return dialogBox.TypeDialog($"Oh no! The Pokémon broke free!");
            
            Destroy(pokeBall);
        }
    }

    int TryToCatchPokemon(Pokemon pokemon, PokeBallItem pokeBallItem)
    {
        float catchRateModifier;

        if (pokemon.Base.PokemonStatus == PokemonStatus.Rare)
            catchRateModifier = pokeBallItem.CatchRateRare;
        else if (pokemon.Base.PokemonStatus == PokemonStatus.FirstPartner)
            catchRateModifier = pokeBallItem.CatchRateFirstPartner;
        else if (pokemon.Base.PokemonStatus == PokemonStatus.PseudoLegendary)
            catchRateModifier = pokeBallItem.CatchRatePseudoLegendary;
        else if (pokemon.Base.PokemonStatus == PokemonStatus.Legendary)
            catchRateModifier = pokeBallItem.CatchRateLegendary;
        else if (pokemon.Base.PokemonStatus == PokemonStatus.Mythical)
            catchRateModifier = pokeBallItem.CatchRateMythical;
        else
            catchRateModifier = pokeBallItem.CatchRateStandard;

        float a;

        if (pokeBallItem.BallID == 5)
            a = 255f;
        else
            a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * catchRateModifier * StatusConditionsDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHP);
        
        if (a >= 255 || catchRateModifier >= 255)
            return 4;
        
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;
            shakeCount++;
        }
        
        return shakeCount;
    }

    public bool IsPokemonSelectedToSwitch(Pokemon pokemon)
    {
        return battleActions.Any(a => a.Type == BattleActionType.SwitchPokemon && a.SelectedPokemon == pokemon);
    }

    public BattleDialogBox DialogBox => dialogBox;
    public List<BattleUnit> PlayerUnits => playerUnits;
    public List<BattleUnit> EnemyUnits => enemyUnits;
    public BattleUnit UnitInSelection => playerUnits[unitInSelectionIndex];
    public PartyScreen PartyScreen => partyScreen;
    public IntroloopAudio WildBattleMusic => wildBattleMusic;
    public IntroloopAudio TrainerBattleMusic => trainerBattleMusic;
    public IntroloopAudio GymLeaderBattleMusic => gymLeaderBattleMusic;
    public IntroloopAudio BadGuyBattleMusic => badGuyBattleMusic;
    public IntroloopAudio LegendaryBattleMusic => legendaryBattleMusic;
    public IntroloopAudio MythicalBattleMusic => mythicalBattleMusic;
    public IntroloopAudio WildBattleVictory => wildBattleVictory;
    public IntroloopAudio TrainerBattleVictory => trainerBattleVictory;
    public IntroloopAudio GymLeaderBattleVictory => gymLeaderBattleVictory;
    public AudioClip PhysicalAttackSound => physicalAttackSound;
    public AudioClip SpecialAttackSound => specialAttackSound;
    public AudioClip StatusAttackSound => statusAttackSound;
    public Image PlayerImage => playerImage;
    public Vector3 PlayerImagePosition => playerImage.transform.localPosition;
    public Image TrainerImage => trainerImage;
    public Image GymLeaderImage => gymLeaderImage;
    public GameObject PokeBallSprite => pokeBallSprite;
    public List<Sprite> PokeBallImagesA => pokeBallImagesA;
    public List<Sprite> PokeBallImagesB => pokeBallImagesB;
    public List<Sprite> PokeBallImagesC => pokeBallImagesC;
    public int UnitCount => unitCount;
    public int ActivePlayerUnitsCount => playerUnits.Count(u => u.Pokemon != null && u.Pokemon.HP > 0);
    public int ActiveEnemyUnitsCount => enemyUnits.Count(u => u.Pokemon != null && u.Pokemon.HP > 0);
    public BattleTrigger BattleTrigger => battleTrigger;
}