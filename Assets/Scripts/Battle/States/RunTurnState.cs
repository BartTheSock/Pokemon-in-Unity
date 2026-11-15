using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BTSUtilities.StateMachine;

public class RunTurnState : State<BattleSystem>
{
    public List<Move> Moves { get; set; }
    public static RunTurnState i { get; private set; }

    BattleSystem battleSystem;
    private void Awake()
    {
        i = this;
    }
    // Input
    public List<BattleAction> Actions { get; set; }

    BattleDialogBox dialogBox;
    PartyScreen partyScreen;
    PokemonParty playerParty;
    PokemonParty trainerParty;
    PokemonParty trainerPartyOne;
    PokemonParty trainerPartyTwo;
    PokemonParty gymLeaderParty;
    Pokemon wildPokemon;
    List<Pokemon> pokemonThatJoinedBattle;
    bool isTrainerBattle;
    bool isDoubleTrainerBattle;
    bool isGymLeaderBattle;
    AudioClip physicalAttackSound;
    AudioClip specialAttackSound;
    AudioClip statusAttackSound;
    Image playerImage;
    Image trainerImage;
    Image gymLeaderImage;
    GameObject pokeBallSprite;
    List<Sprite> pokeBallImagesA;
    List<Sprite> pokeBallImagesB;
    List<Sprite> pokeBallImagesC;
    BattleTrigger battleTrigger;
    Vector3 PlayerImagePosition;
    int turnCount = 0;

    public override void Enter(BattleSystem owner)
    {
        battleSystem = owner;

        dialogBox = battleSystem.DialogBox;
        partyScreen = battleSystem.PartyScreen;
        isTrainerBattle = battleSystem.IsTrainerBattle;
        isDoubleTrainerBattle = battleSystem.IsDoubleTrainerBattle;
        isGymLeaderBattle = battleSystem.IsGymLeaderBattle;
        playerParty = battleSystem.PlayerParty;
        trainerParty = battleSystem.TrainerParty;
        trainerPartyOne = battleSystem.TrainerPartyOne;
        trainerPartyTwo = battleSystem.TrainerPartyTwo;
        gymLeaderParty = battleSystem.GymLeaderParty;
        wildPokemon = battleSystem.WildPokemon;
        pokemonThatJoinedBattle = battleSystem.PokemonThatBattled;
        physicalAttackSound = battleSystem.PhysicalAttackSound;
        specialAttackSound = battleSystem.SpecialAttackSound;
        statusAttackSound = battleSystem.StatusAttackSound;
        playerImage = battleSystem.PlayerImage;
        trainerImage = battleSystem.TrainerImage;
        gymLeaderImage = battleSystem.GymLeaderImage;
        pokeBallSprite = battleSystem.PokeBallSprite;
        pokeBallImagesA = battleSystem.PokeBallImagesA;
        pokeBallImagesB = battleSystem.PokeBallImagesB;
        pokeBallImagesC = battleSystem.PokeBallImagesC;
        battleTrigger = battleSystem.BattleTrigger;
        PlayerImagePosition = battleSystem.PlayerImagePosition;

        StartCoroutine(RunTurns());
    }

    public override void Execute()
    {
        
    }

    public override void Exit()
    {
        
    }

    void OnMoveSelected(int selection)
    {

    }

    void OnBack()
    {
        
    }

    IEnumerator RunTurns()
    {
        turnCount++;
        foreach (var action in Actions)
        {
            if (action.IsInvalid)
                continue;

            if (action.Type == BattleActionType.Move)
            {
                action.User.Pokemon.CurrentMove = action.SelectedMove;

                yield return RunMove(action.User, action.Target, action.SelectedMove);
                yield return RunAfterTurn(action.User, action.Target);
            }
            else if (action.Type == BattleActionType.SwitchPokemon)
                yield return battleSystem.SwitchPokemon(action.SelectedPokemon, action.User);
            else if (action.Type == BattleActionType.UseItem)
            {
                if (action.SelectedItem is PokeBallItem)
                    yield return battleSystem.ThrowPokeBall(action.SelectedItem as PokeBallItem);
                else if (action.SelectedItem is BattleItem)
                    yield return battleSystem.ApplyItemBoosts(action.SelectedItem as BattleItem, action.User);
            }
            else if (action.Type == BattleActionType.Run)
                yield return TryToEscape();

            if (battleSystem.IsBattleOver) break;
        }

        battleSystem.ClearTurnData();

        if (!battleSystem.IsBattleOver)
            battleSystem.StateMachine.ChangeState(ActionSelectionState.i);
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool cantMoveFromTruant = (sourceUnit.Pokemon.Ability?.Id == AbilityID.Truant && turnCount % 2 == 0);
        if (cantMoveFromTruant)
        {
            if (sourceUnit.IsPlayerUnit)
                yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.PokemonName} is loafing around!");
            else
            {
                if (isTrainerBattle || isGymLeaderBattle)
                    yield return dialogBox.TypeDialog($"The opponent's {sourceUnit.Pokemon.Base.PokemonName} is loafing around!");
                else
                    yield return dialogBox.TypeDialog($"The wild {sourceUnit.Pokemon.Base.PokemonName} is loafing around!");
            }

            yield return ShowStatusChanges(sourceUnit);
            yield break;
        }

        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit);
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit);

        bool targetHasPressure = targetUnit.Pokemon.Ability?.Id == AbilityID.Pressure;

        if (targetHasPressure && move.PP > 1)
            move.PP -= 2;
        else
            move.PP--;

        if (move.Base == GlobalSettings.i.BackupMove)
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.PokemonName} has no moves left!");
        
        if (sourceUnit.IsPlayerUnit)
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.PokemonName} used {move.Base.MoveName}!");
        else
        {
            if (isTrainerBattle)
                yield return dialogBox.TypeDialog($"The opponent's {sourceUnit.Pokemon.Base.PokemonName} used {move.Base.MoveName}!");
            else
                yield return dialogBox.TypeDialog($"The wild {sourceUnit.Pokemon.Base.PokemonName} used {move.Base.MoveName}!");
        }

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            float typeEffectiveness = 1f;
            int strikeCount = 1;
            for (int i = 1; i <= move.Base.GetStrikeTimes(); i++)
            {
                if (move.Base.Category == MoveCategory.Physical)
                {
                    sourceUnit.PlayPhysicalAttackAnimation();
                    AudioManager.i.PlaySfx(physicalAttackSound);
                    yield return new WaitForSeconds(1f);
                    targetUnit.PlayPhysicalHitAnimation();
                    AudioManager.i.PlaySfx(AudioID.NonStatusHit);
                }
                else if (move.Base.Category == MoveCategory.Special)
                {
                    sourceUnit.PlaySpecialAttackAnimation();
                    AudioManager.i.PlaySfx(specialAttackSound);
                    yield return new WaitForSeconds(1f);
                    targetUnit.PlaySpecialHitAnimation();
                    AudioManager.i.PlaySfx(AudioID.NonStatusHit);
                }
                else if (move.Base.Category == MoveCategory.Status)
                {
                    sourceUnit.PlayStatusAttackAnimation();
                    AudioManager.i.PlaySfx(statusAttackSound);
                    yield return new WaitForSeconds(1f);
                    if (move.Base.Target == MoveTarget.Self)
                        sourceUnit.PlayStatusHitAnimation();
                    else
                        targetUnit.PlayStatusHitAnimation();
                    AudioManager.i.PlaySfx(AudioID.StatusHit);
                }
                
                if (move.Base.Category == MoveCategory.Status)
                    yield return RunMoveEffects(move.Base.Effects, move, sourceUnit, targetUnit, move.Base.Target);
                else
                {
                    float weatherModifier = battleSystem.Field.Weather?.OnDamageModify?.Invoke(move) ?? 1f;
                    var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon, targetUnit.Pokemon, weatherModifier);
                    yield return targetUnit.HUD.UpdateHPAsync();
                    yield return ShowDamageDetails(damageDetails);
                    typeEffectiveness = damageDetails.TypeEffectiveness;
                }
                
                if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.Ability?.Id != AbilityID.ShieldDust)
                {
                    foreach (var secondary in move.Base.SecondaryEffects)
                    {
                        var random = UnityEngine.Random.Range(1, 101);
                        
                        if (random <= secondary.Chance)
                            yield return RunMoveEffects(secondary, move, sourceUnit, targetUnit, secondary.Target);
                    }
                }

                strikeCount = i;
                if (targetUnit.Pokemon.HP <= 0)
                    break;
            }

            yield return ShowTypeEffectiveness(typeEffectiveness, sourceUnit, targetUnit);

            if (move.Base.IsMultistrikeMove)
            {
                if (strikeCount == 1)
                    yield return dialogBox.TypeDialog($"Hit 1 time!");
                else if (strikeCount > 1)
                    yield return dialogBox.TypeDialog($"Hit {strikeCount} times!");
            }

            if (targetUnit.Pokemon.HP <= 0)
                yield return HandlePokemonFainted(targetUnit);
        }
        else
        {
            if (move.Base.Target == MoveTarget.Self)
                yield return dialogBox.TypeDialog($"But it failed!");
            else
                yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.PokemonName}'s attack missed!");
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Move move, BattleUnit source, BattleUnit target, MoveTarget moveTarget)
    {
        // Stat Boosts
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.Pokemon.ApplyBoosts(effects.Boosts, source.Pokemon);
            else
            {
                if (!(target.Pokemon.Ability?.Id == AbilityID.Soundproof && move.Base.HasFlag(MoveFlag.Sound)))
                    target.Pokemon.ApplyBoosts(effects.Boosts, source.Pokemon);
            }

            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }

        // Status Effects
        if (effects.Status != StatusConditionID.None)
        {
            if (moveTarget == MoveTarget.Self)
                source.Pokemon.SetStatus(effects.Status);
            else
            {
                if (!(target.Pokemon.Ability?.Id == AbilityID.Soundproof && move.Base.HasFlag(MoveFlag.Sound)))
                    target.Pokemon.SetStatus(effects.Status);
            }
            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }

        // Volatile Status Effects
        if (effects.VolatileStatus != StatusConditionID.None)
        {
            if (moveTarget == MoveTarget.Self)
            {
                if (target.Pokemon.CurrentMove == null)
                    source.Pokemon.SetVolatileStatus(effects.VolatileStatus, target.Pokemon, source.Pokemon.CurrentMove.Base.Priority, 0);
                else
                    source.Pokemon.SetVolatileStatus(effects.VolatileStatus, target.Pokemon, source.Pokemon.CurrentMove.Base.Priority, target.Pokemon.CurrentMove.Base.Priority);
            }
            else
            {
                if (!(target.Pokemon.Ability?.Id == AbilityID.Soundproof && move.Base.HasFlag(MoveFlag.Sound)))
                {
                    if (target.Pokemon.CurrentMove == null)
                        target.Pokemon.SetVolatileStatus(effects.VolatileStatus, source.Pokemon, 0, source.Pokemon.CurrentMove.Base.Priority);
                    else
                        target.Pokemon.SetVolatileStatus(effects.VolatileStatus, source.Pokemon, target.Pokemon.CurrentMove.Base.Priority, source.Pokemon.CurrentMove.Base.Priority);
                }
            }

            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }

        // Weather Effects
        if (effects.Weather != WeatherConditionID.None)
        {
            battleSystem.Field.SetWeather(effects.Weather, 5);
            yield return dialogBox.TypeDialog(battleSystem.Field.Weather.StartByMoveMessage);

            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }

        if (battleSystem.Field?.Weather != null)
            yield return RunWeatherEffects(battleSystem.Field.Weather);

        // Absorbing Effects
        if (effects.HealUser)
        {
            target.Pokemon.TakeDamage(move, source.Pokemon, target.Pokemon, 1f, true, false);
            yield return ShowStatusChanges(target, 1);
            yield return source.HUD.UpdateHPAsync();
        }

        // Recoil Effects
        if (effects.Recoil)
        {
            source.Pokemon.TakeDamage(move, source.Pokemon, target.Pokemon, 1f, false, true);
            yield return ShowStatusChanges(source, 2);
            yield return source.HUD.UpdateHPAsync();
        }
    }

    IEnumerator RunWeatherEffects(WeatherCondition weather)
    {
        if (battleSystem.Field.WeatherDuration != null)
        {
            if (battleSystem.Field.WeatherDuration > 0)
                battleSystem.Field.WeatherDuration--;
            else
            {
                battleSystem.Field.SetWeather(WeatherConditionID.None, null);
                if (weather.EndMessage != null)
                    yield return dialogBox.TypeDialog(weather.EndMessage);

                yield break;
            }
        }

        if (weather.EffectMessage != null)
            yield return dialogBox.TypeDialog(weather.EffectMessage);

        var units = battleSystem.PlayerUnits.Concat(battleSystem.EnemyUnits);

        foreach (var unit in units)
        {
            if (unit.Pokemon == null || unit.Pokemon.HP <= 0)
                continue;

            weather.OnWeatherEffect?.Invoke(unit.Pokemon);
            yield return ShowStatusChanges(unit);

            if (unit.Pokemon.HP <= 0)
                yield return HandlePokemonFainted(unit);
        }
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        if (battleSystem.IsBattleOver) yield break;

        // Status effects like Burn or Poison will hurt the Pokémon after their turn ends.
        if (sourceUnit.Pokemon.VolatileStatus?.Id == StatusConditionID.Seeded)
        {
            sourceUnit.Pokemon.OnAfterTurnSeeded(targetUnit.Pokemon);
            yield return ShowStatusChanges(sourceUnit);
            sourceUnit.Pokemon.OnAfterTurnSeeder(targetUnit.Pokemon);
            yield return ShowStatusChanges(targetUnit);
        }
        else
        {
            sourceUnit.Pokemon.OnAfterTurn();
            yield return sourceUnit.HUD.UpdateHPAsync();
            yield return ShowStatusChanges(sourceUnit);
        }

        if (sourceUnit.Pokemon.HP <= 0)
            yield return HandlePokemonFainted(sourceUnit);
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.accuracy];
        int evasion = target.StatBoosts[Stat.evasion];

        var boostValues = new float[] { 1f, (4f / 3f), (5f / 3f), 2f, (7f / 3f), (8f / 3f), 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (source.Ability?.Id != AbilityID.Illuminate && source.Ability?.Id != AbilityID.KeenEye)
        {
            if (evasion > 0)
                moveAccuracy /= boostValues[evasion];
            else
                moveAccuracy *= boostValues[-evasion];
        }

        moveAccuracy = source.ModifyAcc(moveAccuracy, target, move);

        if (target.VolatileStatus?.Id == StatusConditionID.Confuse && target.Ability?.Id == AbilityID.TangledFeet)
            moveAccuracy = moveAccuracy / 2f;

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(BattleUnit pokemonUnit, int typeOfEffect = 0)
    {
        if (typeOfEffect == 1)
        {
            pokemonUnit.Pokemon.AddStatusEvent($"{pokemonUnit.Pokemon.Base.PokemonName} had its energy drained!");
            yield return ShowGeneralStatusChanges(pokemonUnit);
        }
        else if (typeOfEffect == 2)
        {
            pokemonUnit.Pokemon.AddStatusEvent($"{pokemonUnit.Pokemon.Base.PokemonName} was hit with recoil!");
            yield return ShowGeneralStatusChanges(pokemonUnit);
        }
        else if (typeOfEffect == 0)
            yield return ShowGeneralStatusChanges(pokemonUnit);
    }

    IEnumerator ShowGeneralStatusChanges(BattleUnit pokemonUnit)
    {
        var pokemon = pokemonUnit.Pokemon;

        while (pokemon.StatusChanges.Count > 0)
        {
            var statusEvent = pokemon.StatusChanges.Dequeue();
            if (pokemonUnit.IsPlayerUnit || statusEvent.Message == "It hurt itself in its confusion!" || statusEvent.Message == "But it failed!" || statusEvent.Message == "But nothing happened!")
                yield return dialogBox.TypeDialog($"{statusEvent.Message}");
            else
            {
                if (isTrainerBattle || isGymLeaderBattle)
                    yield return dialogBox.TypeDialog($"The opponent's {statusEvent.Message}");
                else
                    yield return dialogBox.TypeDialog($"The wild {statusEvent.Message}");

                if (statusEvent.Type == StatusEventType.Damage)
                {
                    pokemonUnit.PlayPhysicalHitAnimation();
                    AudioManager.i.PlaySfx(AudioID.StatusHit);
                    yield return pokemonUnit.HUD.UpdateHPAsync();
                }
            }
        }
    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        if (!faintedUnit.IsPlayerUnit)
        {
            if (isTrainerBattle || isDoubleTrainerBattle || isGymLeaderBattle)
                yield return dialogBox.TypeDialog($"The opponent's {faintedUnit.Pokemon.Base.PokemonName} fainted!");
            else
                yield return dialogBox.TypeDialog($"The wild {faintedUnit.Pokemon.Base.PokemonName} fainted!");
        }
        else
        {
            yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.PokemonName} fainted!");
            faintedUnit.Pokemon.ChangeFriendship(-5, -8, -12);
        }

        faintedUnit.PlayFaintedAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            bool battleWon = true;
            if (isTrainerBattle)
                battleWon = trainerParty.GetHealthyPokemon() == null;
            else if (isGymLeaderBattle)
                battleWon = gymLeaderParty.GetHealthyPokemon() == null;
            else if (isDoubleTrainerBattle)
                battleWon = trainerPartyOne.GetHealthyPokemon() == null && trainerPartyTwo.GetHealthyPokemon() == null;

            if (battleWon)
            {
                if (isTrainerBattle || isDoubleTrainerBattle)
                    AudioManager.i.PlayMusic(battleSystem.TrainerBattleVictory, true);
                else if (isGymLeaderBattle)
                    AudioManager.i.PlayMusic(battleSystem.GymLeaderBattleVictory, true);
                else
                    AudioManager.i.PlayMusic(battleSystem.WildBattleVictory, true);
            }

            for (int i = 0; i < battleSystem.PlayerParty.PokemonList.Count; i++)
            {
                var playerPokemon = battleSystem.PlayerParty.PokemonList[i];
                
                playerPokemon.GainEVs(faintedUnit.Pokemon.Base.EVYields);

                // Gain EXP
                int expYield = faintedUnit.Pokemon.Base.ExpYield;
                int enemyLevel = faintedUnit.Pokemon.Level;
                float trainerBonus = 1f;
                if (isTrainerBattle || isDoubleTrainerBattle || isGymLeaderBattle)
                    trainerBonus = 1.5f;

                int expGain;

                if ((playerPokemon.Level >= PlayerController.i.pokemonLevelCap || playerPokemon.HP <= 0) || (battleTrigger == BattleTrigger.Aerodactyl || battleTrigger == BattleTrigger.Archen || battleTrigger == BattleTrigger.Zapdos || battleTrigger == BattleTrigger.Moltres || battleTrigger == BattleTrigger.Lugia || battleTrigger == BattleTrigger.HoOh || battleTrigger == BattleTrigger.Latios || battleTrigger == BattleTrigger.Latias || battleTrigger == BattleTrigger.Cresselia || battleTrigger == BattleTrigger.Tornadus || battleTrigger == BattleTrigger.Thundurus || battleTrigger == BattleTrigger.Landorus || battleTrigger == BattleTrigger.TapuKoko || battleTrigger == BattleTrigger.TapuLele || battleTrigger == BattleTrigger.TapuBulu || battleTrigger == BattleTrigger.TapuFini || battleTrigger == BattleTrigger.Regieleki || battleTrigger == BattleTrigger.Regidrago || battleTrigger == BattleTrigger.Omanyte || battleTrigger == BattleTrigger.Kabuto || battleTrigger == BattleTrigger.Lileep || battleTrigger == BattleTrigger.Anorith || battleTrigger == BattleTrigger.Cranidos || battleTrigger == BattleTrigger.Shieldon || battleTrigger == BattleTrigger.Tyrunt || battleTrigger == BattleTrigger.Amaura || battleTrigger == BattleTrigger.Dracozolt || battleTrigger == BattleTrigger.Arctozolt || battleTrigger == BattleTrigger.Raikou || battleTrigger == BattleTrigger.Entei || battleTrigger == BattleTrigger.Suicune || battleTrigger == BattleTrigger.Regirock || battleTrigger == BattleTrigger.Registeel || battleTrigger == BattleTrigger.Groudon || battleTrigger == BattleTrigger.Uxie || battleTrigger == BattleTrigger.Mesprit || battleTrigger == BattleTrigger.Azelf || battleTrigger == BattleTrigger.Dialga || battleTrigger == BattleTrigger.Palkia || battleTrigger == BattleTrigger.Heatran || battleTrigger == BattleTrigger.Regigigas || battleTrigger == BattleTrigger.Shaymin || battleTrigger == BattleTrigger.Victini || battleTrigger == BattleTrigger.Cobalion || battleTrigger == BattleTrigger.Terrakion || battleTrigger == BattleTrigger.Virizion || battleTrigger == BattleTrigger.Reshiram || battleTrigger == BattleTrigger.Zekrom || battleTrigger == BattleTrigger.Keldeo || battleTrigger == BattleTrigger.Meloetta || battleTrigger == BattleTrigger.Genesect || battleTrigger == BattleTrigger.Xerneas || battleTrigger == BattleTrigger.Yveltal || battleTrigger == BattleTrigger.Zygarde || battleTrigger == BattleTrigger.Diancie || battleTrigger == BattleTrigger.Hoopa || battleTrigger == BattleTrigger.Volcanion || battleTrigger == BattleTrigger.TypeNull || battleTrigger == BattleTrigger.Magearna || battleTrigger == BattleTrigger.Marshadow || battleTrigger == BattleTrigger.Zeraora || battleTrigger == BattleTrigger.Meltan || battleTrigger == BattleTrigger.Zacian || battleTrigger == BattleTrigger.Zamazenta || battleTrigger == BattleTrigger.Eternatus || battleTrigger == BattleTrigger.Kubfu || battleTrigger == BattleTrigger.Zarude || battleTrigger == BattleTrigger.Spectrier || battleTrigger == BattleTrigger.Calyrex || battleTrigger == BattleTrigger.Tirtouga || battleTrigger == BattleTrigger.Kyogre || battleTrigger == BattleTrigger.Phione || battleTrigger == BattleTrigger.Manaphy || battleTrigger == BattleTrigger.Articuno || battleTrigger == BattleTrigger.Regice || battleTrigger == BattleTrigger.Kyurem || battleTrigger == BattleTrigger.Glastrier || battleTrigger == BattleTrigger.Mewtwo || battleTrigger == BattleTrigger.Mew || battleTrigger == BattleTrigger.Celebi || battleTrigger == BattleTrigger.Rayquaza || battleTrigger == BattleTrigger.Jirachi || battleTrigger == BattleTrigger.Deoxys || battleTrigger == BattleTrigger.Giratina || battleTrigger == BattleTrigger.Arceus || battleTrigger == BattleTrigger.MissingNo || battleTrigger == BattleTrigger.Cosmog1 || battleTrigger == BattleTrigger.Cosmog2 || battleTrigger == BattleTrigger.Necrozma || battleTrigger == BattleTrigger.Eternatus || battleTrigger == BattleTrigger.Nihilego || battleTrigger == BattleTrigger.Buzzwole || battleTrigger == BattleTrigger.Pheromosa || battleTrigger == BattleTrigger.Xurkitree || battleTrigger == BattleTrigger.Celesteela || battleTrigger == BattleTrigger.Kartana || battleTrigger == BattleTrigger.Guzzlord || battleTrigger == BattleTrigger.Poipole || battleTrigger == BattleTrigger.Stakataka || battleTrigger == BattleTrigger.Blacephalon))
                    expGain = 0;
                else
                {
                    if (pokemonThatJoinedBattle.Contains(playerPokemon))
                        expGain = Mathf.Max(Mathf.Min(Mathf.FloorToInt(((expYield * enemyLevel) / 5f) * trainerBonus), playerPokemon.Base.GetEXPForLevel(PlayerController.i.pokemonLevelCap) - playerPokemon.EXP), 1);
                    else
                        expGain = Mathf.Max(Mathf.Min(Mathf.FloorToInt(((expYield * enemyLevel) / 12f) * trainerBonus), playerPokemon.Base.GetEXPForLevel(PlayerController.i.pokemonLevelCap) - playerPokemon.EXP), 1);
                }

                playerPokemon.EXP += expGain;

                if (pokemonThatJoinedBattle.Contains(playerPokemon))
                {
                    if (playerPokemon.Level < PlayerController.i.pokemonLevelCap)
                    {
                        yield return dialogBox.TypeDialog($"{playerPokemon.Base.PokemonName} gained {expGain} EXP points!");
                        if (battleSystem.PlayerUnits[0].Pokemon == playerPokemon)
                            yield return battleSystem.PlayerUnits[0].HUD.SetEXPSmooth();
                    }
                }

                // Check for level up
                while (playerPokemon.CheckForLevelUp())
                {
                    if (battleSystem.PlayerUnits[0].Pokemon == playerPokemon)
                    {
                        battleSystem.PlayerUnits[0].HUD.SetLevel();
                        battleSystem.PlayerUnits[0].HUD.SetHPBar();
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

                    if (battleSystem.PlayerUnits[0].Pokemon == playerPokemon)
                        yield return battleSystem.PlayerUnits[0].HUD.SetEXPSmooth(true);
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        yield return NextStepsAfterFainting(faintedUnit);
    }

    IEnumerator NextStepsAfterFainting(BattleUnit faintedUnit)
    {
        // Remove the action of the fainted Pokemon.
        var actionToRemove = Actions.FirstOrDefault(a => a.User == faintedUnit);
        if (actionToRemove != null)
            actionToRemove.IsInvalid = true;

        if (faintedUnit.IsPlayerUnit)
        {
            var activePokemonList = battleSystem.PlayerUnits.Select(u => u.Pokemon).Where(p => p.HP > 0).ToList();

            var nextPokemon = playerParty.GetHealthyPokemon(dontInclude: activePokemonList);
            if (nextPokemon == null && activePokemonList.Count == 0)
            {
                // End the battle in a loss.
                turnCount = 0;
                battleSystem.BattleOver(false, false);
            }
            else if (nextPokemon == null && activePokemonList.Count > 0)
            {
                // No new Pokemon to send out, but battle can technically continue.
                battleSystem.PlayerUnits.Remove(faintedUnit);
                faintedUnit.HUD.gameObject.SetActive(false);

                // Attacks targeted at the fainted unit should be changed.
                var actionsToChange = Actions.Where(a => a.Target == faintedUnit).ToList();
                actionsToChange.ForEach(a => a.Target = battleSystem.PlayerUnits.First());
            }
            else if (nextPokemon != null)
            {
                // Send out the next Pokemon.
                yield return GameController.Instance.StateMachine.PushAndWait(PartyState.i);
                yield return battleSystem.SwitchPokemon(PartyState.i.SelectedPokemon, faintedUnit);
            }
        }
        else
        {
            if (!isTrainerBattle && !isDoubleTrainerBattle && !isGymLeaderBattle)
            {
                turnCount = 0;
                battleSystem.BattleOver(true, false);
            }
            else if (isTrainerBattle && !isDoubleTrainerBattle && !isGymLeaderBattle)
            {
                var activePokemonList = battleSystem.EnemyUnits.Select(u => u.Pokemon).Where(p => p.HP > 0).ToList();

                var nextPokemon = trainerParty.GetHealthyPokemon(dontInclude: activePokemonList);

                if (nextPokemon == null && activePokemonList.Count == 0)
                {
                    // End the battle in a victory.
                    trainerImage.gameObject.SetActive(true);
                    var originalTrainerPosition = new Vector3(trainerImage.transform.localPosition.x - 480f, trainerImage.transform.localPosition.y);
                    yield return dialogBox.TypeDialog($"{battleSystem.Player.PlayerName} defeated {battleSystem.Trainer.TrainerClass} {battleSystem.Trainer.TrainerName}!");
                    trainerImage.transform.DOLocalMoveX(originalTrainerPosition.x + 120f, 0.75f);
                    yield return dialogBox.TypeDialog($"{battleSystem.Trainer.DialogEndBattle}");
                    yield return new WaitForSeconds(1f);

                    yield return dialogBox.TypeDialog($"{battleSystem.Player.PlayerName} won ${battleSystem.Trainer.MoneyToGive * 3}!");
                    Wallet.i.AddMoney(battleSystem.Trainer.MoneyToGive * 3);

                    yield return new WaitForSeconds(1f);
                    trainerImage.transform.localPosition = originalTrainerPosition;
                    trainerImage.gameObject.SetActive(false);
                    turnCount = 0;
                    playerImage.gameObject.SetActive(true);
                    playerImage.transform.localPosition = PlayerImagePosition + new Vector3(480f, 0f);
                    playerImage.gameObject.SetActive(false);
                    battleSystem.BattleOver(true, false);
                }
                else if (nextPokemon == null && activePokemonList.Count > 0)
                {
                    // No new Pokemon for opponent to send out, but battle can technically continue.
                    battleSystem.EnemyUnits.Remove(faintedUnit);
                    faintedUnit.HUD.gameObject.SetActive(false);

                    // Attacks targeted at the fainted unit should be changed.
                    var actionsToChange = Actions.Where(a => a.Target == faintedUnit).ToList();
                    actionsToChange.ForEach(a => a.Target = battleSystem.EnemyUnits.First());
                }
                else if (nextPokemon != null)
                {
                    if (battleSystem.UnitCount == 1)
                    {   // Give the player a chance to switch their Pokemon before the trainer sends in their next Pokemon.
                        AboutToUseState.i.NewPokemon = nextPokemon;
                        yield return battleSystem.StateMachine.PushAndWait(AboutToUseState.i);
                    }
                    else // Send out the next Pokemon.
                        yield return battleSystem.SendNextTrainerPokemon(faintedUnit);
                }
            }
            else if (!isTrainerBattle && isDoubleTrainerBattle && !isGymLeaderBattle)
            {
                var activePokemonList = battleSystem.EnemyUnits.Select(u => u.Pokemon).Where(p => p.HP > 0).ToList();

                var nextPokemonOne = trainerPartyOne.GetHealthyPokemon(dontInclude: activePokemonList);
                var nextPokemonTwo = trainerPartyTwo.GetHealthyPokemon(dontInclude: activePokemonList); ;
                var nextPokemon = nextPokemonOne;

                if (nextPokemonOne == null)
                    nextPokemon = nextPokemonTwo;

                if (nextPokemon == null && activePokemonList.Count == 0)
                {
                    // End the battle in a victory.
                    trainerImage.gameObject.SetActive(true);
                    var originalTrainerPosition = new Vector3(trainerImage.transform.localPosition.x - 480f, trainerImage.transform.localPosition.y);
                    yield return dialogBox.TypeDialog($"{battleSystem.Player.PlayerName} defeated {battleSystem.TrainerOne.TrainerClass} {battleSystem.TrainerOne.TrainerName} and {battleSystem.TrainerTwo.TrainerName}!");
                    trainerImage.transform.DOLocalMoveX(originalTrainerPosition.x + 120f, 0.75f);
                    yield return dialogBox.TypeDialog($"{battleSystem.TrainerOne.DialogEndBattle}");
                    yield return new WaitForSeconds(1f);
                    
                    yield return dialogBox.TypeDialog($"{battleSystem.Player.PlayerName} won ${(battleSystem.TrainerOne.MoneyToGive + battleSystem.TrainerTwo.MoneyToGive) * 3}!");
                    Wallet.i.AddMoney((battleSystem.TrainerOne.MoneyToGive + battleSystem.TrainerTwo.MoneyToGive) * 3);

                    yield return new WaitForSeconds(1f);
                    trainerImage.transform.localPosition = originalTrainerPosition;
                    trainerImage.gameObject.SetActive(false);
                    turnCount = 0;
                    playerImage.gameObject.SetActive(true);
                    playerImage.transform.localPosition = PlayerImagePosition + new Vector3(480f, 0f);
                    playerImage.gameObject.SetActive(false);
                    battleSystem.BattleOver(true, false);
                }
                else if (nextPokemon == null && activePokemonList.Count > 0)
                {
                    // No new Pokemon for opponent to send out, but battle can technically continue.
                    battleSystem.EnemyUnits.Remove(faintedUnit);
                    faintedUnit.HUD.gameObject.SetActive(false);

                    // Attacks targeted at the fainted unit should be changed.
                    var actionsToChange = Actions.Where(a => a.Target == faintedUnit).ToList();
                    actionsToChange.ForEach(a => a.Target = battleSystem.EnemyUnits.First());
                }
                else if (nextPokemon != null) // Send out the next Pokemon.
                    yield return battleSystem.SendNextDoubleTrainerPokemon(faintedUnit);
            }
            else if (!isTrainerBattle && !isDoubleTrainerBattle && isGymLeaderBattle)
            {
                var activePokemonList = battleSystem.EnemyUnits.Select(u => u.Pokemon).Where(p => p.HP > 0).ToList();

                var nextPokemon = gymLeaderParty.GetHealthyPokemon(dontInclude: activePokemonList);
                if (nextPokemon == null && activePokemonList.Count == 0)
                {
                    // End the battle in a victory.
                    gymLeaderImage.gameObject.SetActive(true);
                    var originalGymLeaderPosition = new Vector3(gymLeaderImage.transform.localPosition.x - 480f, gymLeaderImage.transform.localPosition.y);
                    yield return dialogBox.TypeDialog($"{battleSystem.Player.PlayerName} defeated Gym Leader {battleSystem.GymLeader.TrainerName}!");
                    gymLeaderImage.transform.DOLocalMoveX(originalGymLeaderPosition.x + 120f, 0.75f);
                    yield return dialogBox.TypeDialog($"{battleSystem.GymLeader.DialogEndBattle}");
                    yield return new WaitForSeconds(1f);
                    
                    yield return dialogBox.TypeDialog($"{battleSystem.Player.PlayerName} won ${battleSystem.GymLeader.MoneyToGive * 3}!");
                    Wallet.i.AddMoney(battleSystem.GymLeader.MoneyToGive * 3);

                    yield return new WaitForSeconds(1f);
                    gymLeaderImage.transform.localPosition = originalGymLeaderPosition;
                    gymLeaderImage.gameObject.SetActive(false);
                    turnCount = 0;
                    playerImage.gameObject.SetActive(true);
                    playerImage.transform.localPosition = PlayerImagePosition + new Vector3(480f, 0f);
                    playerImage.gameObject.SetActive(false);
                    battleSystem.BattleOver(true, false);
                }
                else if (nextPokemon == null && activePokemonList.Count > 0)
                {
                    // No new Pokemon for opponent to send out, but battle can technically continue.
                    battleSystem.EnemyUnits.Remove(faintedUnit);
                    faintedUnit.HUD.gameObject.SetActive(false);

                    // Attacks targeted at the fainted unit should be changed.
                    var actionsToChange = Actions.Where(a => a.Target == faintedUnit).ToList();
                    actionsToChange.ForEach(a => a.Target = battleSystem.EnemyUnits.First());
                }
                else if (nextPokemon != null)
                {
                    if (battleSystem.UnitCount == 1)
                    {   // Give the player a chance to switch their Pokemon before the trainer sends in their next Pokemon.
                        AboutToUseState.i.NewPokemon = nextPokemon;
                        yield return battleSystem.StateMachine.PushAndWait(AboutToUseState.i);
                    }
                    else // Send out the next Pokemon.
                        yield return battleSystem.SendNextGymLeaderPokemon(faintedUnit);
                }
            }
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog($"A critical hit!");
    }

    IEnumerator ShowTypeEffectiveness(float typeEffectiveness, BattleUnit attacker, BattleUnit defender)
    {
        if (typeEffectiveness > 1f)
            yield return dialogBox.TypeDialog($"It's super effective!");
        else if (typeEffectiveness < 1f)
        {
            if (typeEffectiveness == 0f)
            {
                if (defender.IsPlayerUnit)
                    yield return dialogBox.TypeDialog($"It doesn't affect {defender.Pokemon.Base.PokemonName}...");
                else
                {
                    if (isTrainerBattle || isDoubleTrainerBattle || isGymLeaderBattle)
                        yield return dialogBox.TypeDialog($"It doesn't affect the opponent's {defender.Pokemon.Base.PokemonName}...");
                    else
                        yield return dialogBox.TypeDialog($"It doesn't affect the wild {defender.Pokemon.Base.PokemonName}...");
                }
            }
            else
                yield return dialogBox.TypeDialog($"It's not very effective...");
        }
    }

    IEnumerator TryToEscape()
    {
        if (isTrainerBattle || isDoubleTrainerBattle || isGymLeaderBattle)
        {
            yield return dialogBox.TypeDialog($"Hey! There's no running from trainer battles!");
            yield break;
        }

        battleSystem.EscapeAttempts++;

        int playerSpeed = battleSystem.PlayerUnits[0].Pokemon.Speed;
        int enemySpeed = battleSystem.EnemyUnits[0].Pokemon.Speed;

        if (enemySpeed <= playerSpeed || battleSystem.PlayerUnits[0].Pokemon.IsOfType(PokemonType.Ghost) || battleSystem.PlayerUnits[0].Pokemon.Ability?.Id == AbilityID.RunAway)
        {
            if (battleSystem.PlayerUnits[0].Pokemon.Ability?.Id == AbilityID.RunAway)
                yield return dialogBox.TypeDialog($"{battleSystem.PlayerUnits[0].Pokemon.Base.PokemonName} fled using Run Away!");
            else
                yield return dialogBox.TypeDialog($"You got away safely!");
            turnCount = 0;
            battleSystem.BattleOver(true, false);
        }
        else
        {
            float f = ((playerSpeed * 128) / enemySpeed) + 30 * battleSystem.EscapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"You got away safely!");
                turnCount = 0;
                battleSystem.BattleOver(true, false);
            }
            else
                yield return dialogBox.TypeDialog($"You couldn't get away!");
        }
    }
}