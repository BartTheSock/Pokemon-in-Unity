using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitiesDB
{
    public static void Initialize()
    {
        foreach (var keyValuePair in Abilities)
        {
            var abilityId = keyValuePair.Key;
            var ability = keyValuePair.Value;

            ability.Id = abilityId;
            ability.Source = EffectSource.Ability;
            ability.SourceId = (int)abilityId;
        }
    }

    public static Dictionary<AbilityID, Ability> Abilities = new Dictionary<AbilityID, Ability>()
    {
        {
            AbilityID.None,
            new Ability()
            {
                AbilityName = "None",
                AbilityDescription = "This Pokémon does not have an ability.",
            }
        },
        {
            AbilityID.Adaptability,
            new Ability()
            {
                AbilityName = "Adaptability",
                AbilityDescription = "Boosts the power of moves of the same type as the Pokémon by 100% instead of 50%.",
            }
        },
        {
            AbilityID.Aerilate,
            new Ability()
            {
                AbilityName = "Aerilate",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Aftermath,
            new Ability()
            {
                AbilityName = "Aftermath",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.AirLock,
            new Ability()
            {
                AbilityName = "Air Lock",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Analytic,
            new Ability()
            {
                AbilityName = "Analytic",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.AngerPoint,
            new Ability()
            {
                AbilityName = "Anger Point",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.AngerShell,
            new Ability()
            {
                AbilityName = "Anger Shell",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Anticipation,
            new Ability()
            {
                AbilityName = "Anticipation",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ArenaTrap,
            new Ability()
            {
                AbilityName = "Arena Trap",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ArmorTail,
            new Ability()
            {
                AbilityName = "Armor Tail",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.AromaVeil,
            new Ability()
            {
                AbilityName = "Aroma Veil",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.AsOneG,
            new Ability()
            {
                AbilityName = "As One",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.AsOneS,
            new Ability()
            {
                AbilityName = "As One",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.AuraBreak,
            new Ability()
            {
                AbilityName = "Aura Break",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.BadDreams,
            new Ability()
            {
                AbilityName = "Bad Dreams",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.BallFetch,
            new Ability()
            {
                AbilityName = "Ball Fetch",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Battery,
            new Ability()
            {
                AbilityName = "Battery",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.BattleArmor,
            new Ability()
            {
                AbilityName = "Battle Armor",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.BattleBond,
            new Ability()
            {
                AbilityName = "Battle Bond",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.BeadsOfRuin,
            new Ability()
            {
                AbilityName = "Beads of Ruin",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.BeastBoost,
            new Ability()
            {
                AbilityName = "Beast Boost",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Berserk,
            new Ability()
            {
                AbilityName = "Berserk",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.BigPecks,
            new Ability()
            {
                AbilityName = "Big Pecks",
                AbilityDescription = "The Pokémon's Defense stat cannot be lowered by other Pokémon's moves or Abilities.",
                OnBoost = (Dictionary<Stat, int> boosts, Pokemon target, Pokemon source) =>
                {
                    if (source != null && target == source) return;

                    if (boosts.ContainsKey(Stat.Defense) && boosts[Stat.Defense] < 0)
                    {
                        boosts.Remove(Stat.accuracy);

                        target.AddStatusEvent($"{target.Base.PokemonName}'s Defense can't be decreased due to Big Pecks!");
                    }
                }
            }
        },
        {
            AbilityID.Blaze,
            new Ability()
            {
                AbilityName = "Blaze",
                AbilityDescription = "Boosts the power of the Pokémon's Fire-type moves by 50% when its HP drops to 1/3 or less of its max.",
                OnModifyAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Fire && attacker.HP <= attacker.MaxHP / 3)
                        atk = atk * 1.5f;

                    return atk;
                },
                OnModifySpAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Fire && attacker.HP <= attacker.MaxHP / 3)
                        atk = atk * 1.5f;

                    return atk;
                }
            }
        },
        {
            AbilityID.Bulletproof,
            new Ability()
            {
                AbilityName = "Bulletproof",
                AbilityDescription = "The Pokémon is immune to ball and bomb moves.",
            }
        },
        {
            AbilityID.CheekPouch,
            new Ability()
            {
                AbilityName = "Cheek Pouch",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ChillingNeigh,
            new Ability()
            {
                AbilityName = "Chilling Neigh",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Chlorophyll,
            new Ability()
            {
                AbilityName = "Chlorophyll",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ClearBody,
            new Ability()
            {
                AbilityName = "Clear Body",
                AbilityDescription = "The Pokémon's stats cannot be lowered by other Pokémon's moves or Abilities.",
                OnBoost = (Dictionary<Stat, int> boosts, Pokemon target, Pokemon source) =>
                {
                    if (source != null && target == source) return;

                    bool boostRemoved = false;

                    foreach (var stat in boosts.Keys.ToList()) {
                        if (boosts[stat] < 0)
                        {
                            boostRemoved = true;
                            boosts.Remove(Stat.accuracy);
                        }
                    }

                    if (boostRemoved)
                        target.AddStatusEvent($"{target.Base.PokemonName}'s clear body prevents stat loss!");
                }
            }
        },
        {
            AbilityID.CloudNine,
            new Ability()
            {
                AbilityName = "Cloud Nine",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ColorChange,
            new Ability()
            {
                AbilityName = "Color Change",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Comatose,
            new Ability()
            {
                AbilityName = "Comatose",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Commander,
            new Ability()
            {
                AbilityName = "Commander",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Competitive,
            new Ability()
            {
                AbilityName = "Competitive",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.CompoundEyes,
            new Ability()
            {
                AbilityName = "Compound Eyes",
                AbilityDescription = "Boosts the accuracy of the Pokémon's moves by 30%.",
                OnModifyAcc = (float acc, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    acc = acc * 1.3f;

                    return acc;
                }
            }
        },
        {
            AbilityID.Contrary,
            new Ability()
            {
                AbilityName = "Contrary",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Corrosion,
            new Ability()
            {
                AbilityName = "Corrosion",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Costar,
            new Ability()
            {
                AbilityName = "Costar",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.CottonDown,
            new Ability()
            {
                AbilityName = "Cotton Down",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.CudChew,
            new Ability()
            {
                AbilityName = "Cud Chew",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.CuriousMedicine,
            new Ability()
            {
                AbilityName = "Curious Medicine",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.CursedBody,
            new Ability()
            {
                AbilityName = "Cursed Body",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.CuteCharm,
            new Ability()
            {
                AbilityName = "Cute Charm",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Damp,
            new Ability()
            {
                AbilityName = "Damp",
                AbilityDescription = "Opponent Pokémon become unable to use explosive moves. Explosive Abilities also fail to trigger.",
            }
        },
        {
            AbilityID.Dancer,
            new Ability()
            {
                AbilityName = "Dancer",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.DarkAura,
            new Ability()
            {
                AbilityName = "Dark Aura",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.DauntlessShield,
            new Ability()
            {
                AbilityName = "Dauntless Shield",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Dazzling,
            new Ability()
            {
                AbilityName = "Dazzling",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Defeatist,
            new Ability()
            {
                AbilityName = "Defeatist",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Defiant,
            new Ability()
            {
                AbilityName = "Defiant",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.DeltaStream,
            new Ability()
            {
                AbilityName = "Delta Stream",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.DesolateLand,
            new Ability()
            {
                AbilityName = "Desolate Land",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Disguise,
            new Ability()
            {
                AbilityName = "Disguise",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Download,
            new Ability()
            {
                AbilityName = "Download",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.DragonsMaw,
            new Ability()
            {
                AbilityName = "Dragon's Maw",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Dragonize,
            new Ability()
            {
                AbilityName = "Dragonize",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Drizzle,
            new Ability()
            {
                AbilityName = "Drizzle",
                AbilityDescription = "Summons rain for 5 turns when the Pokémon enters a battle.",
            }
        },
        {
            AbilityID.Drought,
            new Ability()
            {
                AbilityName = "Drought",
                AbilityDescription = "Summons harsh sunlight for 5 turns when the Pokémon enters a battle.",
            }
        },
        {
            AbilityID.DrySkin,
            new Ability()
            {
                AbilityName = "Dry Skin",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.EarlyBird,
            new Ability()
            {
                AbilityName = "Early Bird",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.EarthEater,
            new Ability()
            {
                AbilityName = "Earth Eater",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Eelevate,
            new Ability()
            {
                AbilityName = "Eelevate",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.EffectSpore,
            new Ability()
            {
                AbilityName = "Effect Spore",
                AbilityDescription = "When the Pokémon is hit by a contact move, the attacker has a 30% chance of being poisoned, paralyzed, or put to sleep.",
                OnDamagingHit = (float damage, Pokemon target, Pokemon attacker, Move move) =>
                {
                    var flags = move.Base.Flags;
                    if (move.Base.HasFlag(MoveFlag.Contact) && Random.Range(1, 11) <= 3)
                    {
                        int random = Random.Range(1, 31);
                        if (random < 10)
                            attacker.SetStatus(StatusConditionID.Poison, EffectSource.Ability);
                        else if (random >= 10 && random < 20)
                            attacker.SetStatus(StatusConditionID.Paralyze, EffectSource.Ability);
                        else if (random >= 20)
                            attacker.SetStatus(StatusConditionID.Sleep, EffectSource.Ability);
                    }
                }
            }
        },
        {
            AbilityID.ElectricSurge,
            new Ability()
            {
                AbilityName = "Electric Surge",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Electromorphosis,
            new Ability()
            {
                AbilityName = "Electromorphosis",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.EmbodyAspect,
            new Ability()
            {
                AbilityName = "Embody Aspect",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.EmergencyExit,
            new Ability()
            {
                AbilityName = "Emergency Exit",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.FairyAura,
            new Ability()
            {
                AbilityName = "Fairy Aura",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Filter,
            new Ability()
            {
                AbilityName = "Filter",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.FireMane,
            new Ability()
            {
                AbilityName = "Fire Mane",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.FlameBody,
            new Ability()
            {
                AbilityName = "Flame Body",
                AbilityDescription = "When the Pokémon is hit by a contact move, the attacker has a 30% chance of being burned.",
                OnDamagingHit = (float damage, Pokemon target, Pokemon attacker, Move move) =>
                {
                    var flags = move.Base.Flags;
                    if (move.Base.HasFlag(MoveFlag.Contact) && Random.Range(1, 11) <= 3)
                        attacker.SetStatus(StatusConditionID.Burn, EffectSource.Ability);
                }
            }
        },
        {
            AbilityID.FlareBoost,
            new Ability()
            {
                AbilityName = "Flare Boost",
                AbilityDescription = "Powers up special moves when the Pokémon is burned.",
                OnModifySpAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (attacker.Status.Id == StatusConditionID.Burn)
                        atk = atk * 1.5f;

                    return atk;
                }
            }
        },
        {
            AbilityID.FlashFire,
            new Ability()
            {
                AbilityName = "Flash Fire",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.FlowerGift,
            new Ability()
            {
                AbilityName = "Flower Gift",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.FlowerVeil,
            new Ability()
            {
                AbilityName = "Flower Veil",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Fluffy,
            new Ability()
            {
                AbilityName = "Fluffy",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Forecast,
            new Ability()
            {
                AbilityName = "Forecast",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Forewarn,
            new Ability()
            {
                AbilityName = "Forewarn",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.FriendGuard,
            new Ability()
            {
                AbilityName = "Friend Guard",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Frisk,
            new Ability()
            {
                AbilityName = "Frisk",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.FullMetalBody,
            new Ability()
            {
                AbilityName = "Full Metal Body",
                AbilityDescription = "Prevents other Pokémon's moves or Abilities from lowering the Pokémon's stats.",
                OnBoost = (Dictionary<Stat, int> boosts, Pokemon target, Pokemon source) =>
                {
                    if (source != null && target == source) return;

                    bool boostRemoved = false;

                    foreach (var stat in boosts.Keys.ToList()) {
                        if (boosts[stat] < 0)
                        {
                            boostRemoved = true;
                            boosts.Remove(Stat.accuracy);
                        }
                    }

                    if (boostRemoved)
                        target.AddStatusEvent($"{target.Base.PokemonName}'s metal body prevents stat loss!");
                }
            }
        },
        {
            AbilityID.FurCoat,
            new Ability()
            {
                AbilityName = "Fur Coat",
                AbilityDescription = "Halves the damage the Pokémon takes from physical moves.",
                OnModifyDef = (float def, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    return def * 2f;
                },
            }
        },
        {
            AbilityID.GaleWings,
            new Ability()
            {
                AbilityName = "Gale Wings",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Galvanize,
            new Ability()
            {
                AbilityName = "Galvanize",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Gluttony,
            new Ability()
            {
                AbilityName = "Gluttony",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.GoodAsGold,
            new Ability()
            {
                AbilityName = "Good as Gold",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Gooey,
            new Ability()
            {
                AbilityName = "Gooey",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.GorillaTactics,
            new Ability()
            {
                AbilityName = "Gorilla Tactics",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.GrassPelt,
            new Ability()
            {
                AbilityName = "Grass Pelt",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.GrassySurge,
            new Ability()
            {
                AbilityName = "Grassy Surge",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.GrimNeigh,
            new Ability()
            {
                AbilityName = "Grim Neigh",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.GuardDog,
            new Ability()
            {
                AbilityName = "Guard Dog",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.GulpMissile,
            new Ability()
            {
                AbilityName = "Gulp Missile",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Guts,
            new Ability()
            {
                AbilityName = "Guts",
                AbilityDescription = "When the Pokémon has a status condition, its Attack stat is boosted by 50%. Being burned does not halve the damage dealt by the Pokémon's physical moves.",
                OnModifyAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (attacker.Status?.Id != StatusConditionID.None)
                        atk = atk * 1.5f;

                    return atk;
                },
            }
        },
        {
            AbilityID.HadronEngine,
            new Ability()
            {
                AbilityName = "Hadron Engine",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Harvest,
            new Ability()
            {
                AbilityName = "Harvest",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Healer,
            new Ability()
            {
                AbilityName = "Healer",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Heatproof,
            new Ability()
            {
                AbilityName = "Heatproof",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.HeavyMetal,
            new Ability()
            {
                AbilityName = "Heavy Metal",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.HoneyGather,
            new Ability()
            {
                AbilityName = "Honey Gather",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Hospitality,
            new Ability()
            {
                AbilityName = "Hospitality",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.HugePower,
            new Ability()
            {
                AbilityName = "Huge Power",
                AbilityDescription = "Doubles the power of the Pokémon's physical moves.",
                OnModifyAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    return atk * 2f;
                },
            }
        },
        {
            AbilityID.HungerSwitch,
            new Ability()
            {
                AbilityName = "Hunger Switch",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Hustle,
            new Ability()
            {
                AbilityName = "Hustle",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Hydration,
            new Ability()
            {
                AbilityName = "Hydration",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.HyperCutter,
            new Ability()
            {
                AbilityName = "Hyper Cutter",
                AbilityDescription = "The Pokémon's Attack stat cannot be lowered by other Pokémon's moves or Abilities.",
                OnBoost = (Dictionary<Stat, int> boosts, Pokemon target, Pokemon source) =>
                {
                    if (source != null && target == source) return;

                    if (boosts.ContainsKey(Stat.Attack) && boosts[Stat.Attack] < 0)
                    {
                        boosts.Remove(Stat.Attack);

                        target.AddStatusEvent($"{target.Base.PokemonName}'s attack can't be lowered due to Hyper Cutter!");
                    }
                }
            }
        },
        {
            AbilityID.IceBody,
            new Ability()
            {
                AbilityName = "Ice Body",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.IceFace,
            new Ability()
            {
                AbilityName = "Ice Face",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.IceScales,
            new Ability()
            {
                AbilityName = "Ice Scales",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Illuminate,
            new Ability()
            {
                AbilityName = "Illuminate",
                AbilityDescription = "The Pokémon ignores changes to targets' evasiveness and its accuracy cannot be lowered.",
                OnBoost = (Dictionary<Stat, int> boosts, Pokemon target, Pokemon source) =>
                {
                    if (source != null && target == source) return;

                    if (boosts.ContainsKey(Stat.accuracy) && boosts[Stat.accuracy] < 0)
                    {
                        boosts.Remove(Stat.accuracy);

                        target.AddStatusEvent($"{target.Base.PokemonName}'s accuracy can't be lowered due to Illuminate!");
                    }
                }
            }
        },
        {
            AbilityID.Illusion,
            new Ability()
            {
                AbilityName = "Illusion",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Immunity,
            new Ability()
            {
                AbilityName = "Immunity",
                AbilityDescription = "The Pokémon cannot be poisoned or badly poisoned.",
                OnTrySetStatus = (StatusConditionID statusId, Pokemon pokemon, EffectSource effect) =>
                {
                    if (statusId != StatusConditionID.Poison && statusId != StatusConditionID.BadPoison)
                        return true;

                    if (effect == EffectSource.Move)
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName}'s immune to poison!");

                    return false;
                }
            }
        },
        {
            AbilityID.Imposter,
            new Ability()
            {
                AbilityName = "Imposter",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Infiltrator,
            new Ability()
            {
                AbilityName = "Infiltrator",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.InnardsOut,
            new Ability()
            {
                AbilityName = "Innards Out",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.InnerFocus,
            new Ability()
            {
                AbilityName = "Inner Focus",
                AbilityDescription = "The Pokémon never flinches when attacked and is unaffected by Intimidate.",
                OnTrySetVolatile = (StatusConditionID statusId, Pokemon pokemon, EffectSource effect) =>
                {
                    if (statusId != StatusConditionID.Flinch)
                        return true;

                    if (effect == EffectSource.Move)
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName}'s immune to flinching!");
                    
                    return false;
                }
            }
        },
        {
            AbilityID.Insomnia,
            new Ability()
            {
                AbilityName = "Insomnia",
                AbilityDescription = "The Pokémon cannot become drowsy or be put to sleep.",
                OnTrySetStatus = (StatusConditionID statusId, Pokemon pokemon, EffectSource effect) =>
                {
                    if (statusId != StatusConditionID.Sleep)
                        return true;

                    if (effect == EffectSource.Move)
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName}'s immune to sleep!");

                    return false;
                },
                OnTrySetVolatile = (StatusConditionID statusId, Pokemon pokemon, EffectSource effect) =>
                {
                    if (statusId != StatusConditionID.Drowsy)
                        return true;

                    if (effect == EffectSource.Move)
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName}'s immune to drowsiness!");

                    return false;
                }
            }
        },
        {
            AbilityID.Intimidate,
            new Ability()
            {
                AbilityName = "Intimidate",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.IntrepidSword,
            new Ability()
            {
                AbilityName = "Intrepid Sword",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.IronBarbs,
            new Ability()
            {
                AbilityName = "Iron Barbs",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.IronFist,
            new Ability()
            {
                AbilityName = "Iron Fist",
                AbilityDescription = "Boosts the power of the Pokémon's punching moves by 20%.",
                OnBasePower = (float basePower, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.HasFlag(MoveFlag.Punch))
                        basePower = basePower * 1.2f;

                    return basePower;
                }
            }
        },
        {
            AbilityID.Justified,
            new Ability()
            {
                AbilityName = "Justified",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.KeenEye,
            new Ability()
            {
                AbilityName = "Keen Eye",
                AbilityDescription = "The Pokémon ignores changes to targets' evasiveness and its accuracy cannot be lowered.",
                OnBoost = (Dictionary<Stat, int> boosts, Pokemon target, Pokemon source) =>
                {
                    if (source != null && target == source) return;

                    if (boosts.ContainsKey(Stat.accuracy) && boosts[Stat.accuracy] < 0)
                    {
                        boosts.Remove(Stat.accuracy);

                        target.AddStatusEvent($"{target.Base.PokemonName}'s accuracy can't be lowered due to Keen Eye!");
                    }
                }
            }
        },
        {
            AbilityID.Klutz,
            new Ability()
            {
                AbilityName = "Klutz",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.LeafGuard,
            new Ability()
            {
                AbilityName = "Leaf Guard",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Levitate,
            new Ability()
            {
                AbilityName = "Levitate",
                AbilityDescription = "The Pokémon floats off the ground, making it immune to Ground-type moves.",
            }
        },
        {
            AbilityID.Libero,
            new Ability()
            {
                AbilityName = "Libero",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.LightMetal,
            new Ability()
            {
                AbilityName = "Light Metal",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.LightningRod,
            new Ability()
            {
                AbilityName = "Lightning Rod",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Limber,
            new Ability()
            {
                AbilityName = "Limber",
                AbilityDescription = "The Pokémon cannot be paralyzed.",
                OnTrySetStatus = (StatusConditionID statusId, Pokemon pokemon, EffectSource effect) =>
                {
                    if (statusId != StatusConditionID.Paralyze)
                        return true;

                    if (effect == EffectSource.Move)
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName}'s immune to paralysis!");

                    return false;
                }
            }
        },
        {
            AbilityID.LingeringAroma,
            new Ability()
            {
                AbilityName = "Lingering Aroma",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.LiquidOoze,
            new Ability()
            {
                AbilityName = "Liquid Ooze",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.LiquidVoice,
            new Ability()
            {
                AbilityName = "Liquid Voice",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.LongReach,
            new Ability()
            {
                AbilityName = "Long Reach",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MagicBounce,
            new Ability()
            {
                AbilityName = "Magic Bounce",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MagicGuard,
            new Ability()
            {
                AbilityName = "Magic Guard",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Magician,
            new Ability()
            {
                AbilityName = "Magician",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MagmaArmor,
            new Ability()
            {
                AbilityName = "Magma Armor",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MagnetPull,
            new Ability()
            {
                AbilityName = "Magnet Pull",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MarvelScale,
            new Ability()
            {
                AbilityName = "Marvel Scale",
                AbilityDescription = "When the Pokémon has a status condition, its Defense stat is boosted by 50%.",
                OnModifyDef = (float def, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (defender.Status?.Id != StatusConditionID.None)
                        def = def * 1.5f;

                    return def;
                },
            }
        },
        {
            AbilityID.MegaLauncher,
            new Ability()
            {
                AbilityName = "Mega Launcher",
                AbilityDescription = "Boosts the power of the Pokémon's pulse moves by 50%.",
                OnBasePower = (float basePower, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.HasFlag(MoveFlag.Pulse))
                        basePower = basePower * 1.5f;

                    return basePower;
                }
            }
        },
        {
            AbilityID.MegaSol,
            new Ability()
            {
                AbilityName = "Mega Sol",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Merciless,
            new Ability()
            {
                AbilityName = "Merciless",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Mimicry,
            new Ability()
            {
                AbilityName = "Mimicry",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MindsEye,
            new Ability()
            {
                AbilityName = "Mind's Eye",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Minus,
            new Ability()
            {
                AbilityName = "Minus",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MirrorArmor,
            new Ability()
            {
                AbilityName = "Mirror Armor",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MistySurge,
            new Ability()
            {
                AbilityName = "Misty Surge",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MoldBreaker,
            new Ability()
            {
                AbilityName = "Mold Breaker",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Moody,
            new Ability()
            {
                AbilityName = "Moody",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MotorDrive,
            new Ability()
            {
                AbilityName = "Motor Drive",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Moxie,
            new Ability()
            {
                AbilityName = "Moxie",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Multiscale,
            new Ability()
            {
                AbilityName = "Multiscale",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Multitype,
            new Ability()
            {
                AbilityName = "Multitype",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Mummy,
            new Ability()
            {
                AbilityName = "Mummy",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.MyceliumMight,
            new Ability()
            {
                AbilityName = "Mycelium Might",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.NaturalCure,
            new Ability()
            {
                AbilityName = "Natural Cure",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Neuroforce,
            new Ability()
            {
                AbilityName = "Neuroforce",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.NeutralizingGas,
            new Ability()
            {
                AbilityName = "Neutralizing Gas",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.NoGuard,
            new Ability()
            {
                AbilityName = "No Guard",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Normalize,
            new Ability()
            {
                AbilityName = "Normalize",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Oblivious,
            new Ability()
            {
                AbilityName = "Oblivious",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Opportunist,
            new Ability()
            {
                AbilityName = "Opportunist",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.OrichalcumPulse,
            new Ability()
            {
                AbilityName = "Orichalcum Pulse",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Overcoat,
            new Ability()
            {
                AbilityName = "Overcoat",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Overgrow,
            new Ability()
            {
                AbilityName = "Overgrow",
                AbilityDescription = "Boosts the power of the Pokémon's Grass-type moves by 50% when its HP drops to 1/3 or less of its max.",
                OnModifyAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Grass && attacker.HP <= attacker.MaxHP / 3)
                        atk = atk * 1.5f;

                    return atk;
                },
                OnModifySpAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Grass && attacker.HP <= attacker.MaxHP / 3)
                        atk = atk * 1.5f;

                    return atk;
                }
            }
        },
        {
            AbilityID.OwnTempo,
            new Ability()
            {
                AbilityName = "Own Tempo",
                AbilityDescription = "The Pokémon cannot become confused and is unaffected by Intimidate.",
                OnTrySetVolatile = (StatusConditionID statusId, Pokemon pokemon, EffectSource effect) =>
                {
                    if (statusId != StatusConditionID.Confuse)
                        return true;

                    if (effect == EffectSource.Move)
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName}'s immune to confusion!");

                    return false;
                }
            }
        },
        {
            AbilityID.ParentalBond,
            new Ability()
            {
                AbilityName = "Parental Bond",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PastelVeil,
            new Ability()
            {
                AbilityName = "Pastel Veil",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PerishBody,
            new Ability()
            {
                AbilityName = "Perish Body",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Pickpocket,
            new Ability()
            {
                AbilityName = "Pickpocket",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Pickup,
            new Ability()
            {
                AbilityName = "Pickup",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PiercingDrill,
            new Ability()
            {
                AbilityName = "Piercing Drill",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Pixilate,
            new Ability()
            {
                AbilityName = "Pixilate",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Plus,
            new Ability()
            {
                AbilityName = "Plus",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PoisonHeal,
            new Ability()
            {
                AbilityName = "Poison Heal",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PoisonPoint,
            new Ability()
            {
                AbilityName = "Poison Point",
                AbilityDescription = "When the Pokémon is hit by a contact move, the attacker has a 30% chance of being poisoned.",
                OnDamagingHit = (float damage, Pokemon target, Pokemon attacker, Move move) =>
                {
                    var flags = move.Base.Flags;
                    if (move.Base.HasFlag(MoveFlag.Contact) && Random.Range(1, 11) <= 3)
                        attacker.SetStatus(StatusConditionID.Poison, EffectSource.Ability);
                }
            }
        },
        {
            AbilityID.PoisonPuppeteer,
            new Ability()
            {
                AbilityName = "Poison Puppeteer",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PoisonTouch,
            new Ability()
            {
                AbilityName = "Poison Touch",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PowerConstruct,
            new Ability()
            {
                AbilityName = "Power Construct",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PowerOfAlchemy,
            new Ability()
            {
                AbilityName = "Power of Alchemy",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PowerSpot,
            new Ability()
            {
                AbilityName = "Power Spot",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Prankster,
            new Ability()
            {
                AbilityName = "Prankster",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Pressure,
            new Ability()
            {
                AbilityName = "Pressure",
                AbilityDescription = "Causes opponents to expend 1 more PP when using moves against the Pokémon.",
            }
        },
        {
            AbilityID.PrimordialSea,
            new Ability()
            {
                AbilityName = "Primordial Sea",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PrismArmor,
            new Ability()
            {
                AbilityName = "Prism Armor",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PropellerTail,
            new Ability()
            {
                AbilityName = "Propeller Tail",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Protean,
            new Ability()
            {
                AbilityName = "Protean",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Protosynthesis,
            new Ability()
            {
                AbilityName = "Protosynthesis",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PsychicSurge,
            new Ability()
            {
                AbilityName = "Psychic Surge",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PunkRock,
            new Ability()
            {
                AbilityName = "Punk Rock",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.PurePower,
            new Ability()
            {
                AbilityName = "Pure Power",
                AbilityDescription = "Doubles the power of the Pokémon's physical moves.",
                OnModifyAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    return atk * 2f;
                },
            }
        },
        {
            AbilityID.PurifyingSalt,
            new Ability()
            {
                AbilityName = "Purifying Salt",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.QuarkDrive,
            new Ability()
            {
                AbilityName = "Quark Drive",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.QueenlyMajesty,
            new Ability()
            {
                AbilityName = "Queenly Majesty",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.QuickDraw,
            new Ability()
            {
                AbilityName = "Quick Draw",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.QuickFeet,
            new Ability()
            {
                AbilityName = "Quick Feet",
                AbilityDescription = "When the Pokémon has a status condition, its Speed stat is boosted by 50%. Being paralyzed does not lower the Pokémon's Speed stat.",
                OnModifySpd = (float spd, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (defender.Status?.Id != StatusConditionID.None)
                        spd = spd * 1.5f;

                    return spd;
                },
            }
        },
        {
            AbilityID.RainDish,
            new Ability()
            {
                AbilityName = "Rain Dish",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Rattled,
            new Ability()
            {
                AbilityName = "Rattled",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Receiver,
            new Ability()
            {
                AbilityName = "Receiver",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Reckless,
            new Ability()
            {
                AbilityName = "Reckless",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Refrigerate,
            new Ability()
            {
                AbilityName = "Refrigerate",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Regenerator,
            new Ability()
            {
                AbilityName = "Regenerator",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Ripen,
            new Ability()
            {
                AbilityName = "Ripen",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Rivalry,
            new Ability()
            {
                AbilityName = "Rivalry",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.RKSSystem,
            new Ability()
            {
                AbilityName = "RKS System",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.RockHead,
            new Ability()
            {
                AbilityName = "Rock Head",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.RockyPayload,
            new Ability()
            {
                AbilityName = "Rocky Payload",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.RoughSkin,
            new Ability()
            {
                AbilityName = "Rough Skin",
                AbilityDescription = "When the Pokémon is hit by a contact move, the attacker takes damage equal to 1/8 of its max HP.",
                OnDamagingHit = (float damage, Pokemon target, Pokemon attacker, Move move) =>
                {
                    var flags = move.Base.Flags;
                    if (move.Base.HasFlag(MoveFlag.Contact))
                        attacker.DecreaseHP(Mathf.FloorToInt((float)attacker.MaxHP / 8f));
                }
            }
        },
        {
            AbilityID.RunAway,
            new Ability()
            {
                AbilityName = "Run Away",
                AbilityDescription = "Enables a sure getaway from wild Pokémon.",
            }
        },
        {
            AbilityID.SandForce,
            new Ability()
            {
                AbilityName = "Sand Force",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SandRush,
            new Ability()
            {
                AbilityName = "Sand Rush",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SandSpit,
            new Ability()
            {
                AbilityName = "Sand Spit",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SandStream,
            new Ability()
            {
                AbilityName = "Sand Stream",
                AbilityDescription = "Summons a sandstorm for 5 turns when the Pokémon enters a battle.",
            }
        },
        {
            AbilityID.SandVeil,
            new Ability()
            {
                AbilityName = "Sand Veil",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SapSipper,
            new Ability()
            {
                AbilityName = "Sap Sipper",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Schooling,
            new Ability()
            {
                AbilityName = "Schooling",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Scrappy,
            new Ability()
            {
                AbilityName = "Scrappy",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ScreenCleaner,
            new Ability()
            {
                AbilityName = "Screen Cleaner",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SeedSower,
            new Ability()
            {
                AbilityName = "Seed Sower",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SereneGrace,
            new Ability()
            {
                AbilityName = "Serene Grace",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ShadowShield,
            new Ability()
            {
                AbilityName = "Shadow Shield",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ShadowTag,
            new Ability()
            {
                AbilityName = "Shadow Tag",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Sharpness,
            new Ability()
            {
                AbilityName = "Sharpness",
                AbilityDescription = "Boosts the power of the Pokémon's slicing moves by 50%.",
                OnBasePower = (float basePower, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.HasFlag(MoveFlag.Slice))
                        basePower = basePower * 1.5f;

                    return basePower;
                }
            }
        },
        {
            AbilityID.ShedSkin,
            new Ability()
            {
                AbilityName = "Shed Skin",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SheerForce,
            new Ability()
            {
                AbilityName = "Sheer Force",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ShellArmor,
            new Ability()
            {
                AbilityName = "Shell Armor",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ShieldDust,
            new Ability()
            {
                AbilityName = "Shield Dust",
                AbilityDescription = "The Pokémon is immune to additional effects from attacks.",
            }
        },
        {
            AbilityID.ShieldsDown,
            new Ability()
            {
                AbilityName = "Shields Down",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Simple,
            new Ability()
            {
                AbilityName = "Simple",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SkillLink,
            new Ability()
            {
                AbilityName = "Skill Link",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SlowStart,
            new Ability()
            {
                AbilityName = "Slow Start",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SlushRush,
            new Ability()
            {
                AbilityName = "Slush Rush",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Sniper,
            new Ability()
            {
                AbilityName = "Sniper",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SnowCloak,
            new Ability()
            {
                AbilityName = "Snow Cloak",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SnowWarning,
            new Ability()
            {
                AbilityName = "Snow Warning",
                AbilityDescription = "Summons snow for 5 turns when the Pokémon enters a battle.",
            }
        },
        {
            AbilityID. SolarPower,
            new Ability()
            {
                AbilityName = "Solar Power",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SolidRock,
            new Ability()
            {
                AbilityName = "Solid Rock",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SoulHeart,
            new Ability()
            {
                AbilityName = "Soul Heart",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Soundproof,
            new Ability()
            {
                AbilityName = "Soundproof",
                AbilityDescription = "The Pokémon is immune to sound-based moves.",
            }
        },
        {
            AbilityID.SpeedBoost,
            new Ability()
            {
                AbilityName = "Speed Boost",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SpicySpray,
            new Ability()
            {
                AbilityName = "Spicy Spray",
                AbilityDescription = "When the Pokémon takes damage from a move, it burns the attacker.",
                OnDamagingHit = (float damage, Pokemon target, Pokemon attacker, Move move) =>
                {
                    attacker.SetStatus(StatusConditionID.Burn, EffectSource.Ability);
                }
            }
        },
        {
            AbilityID.Stakeout,
            new Ability()
            {
                AbilityName = "Stakeout",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Stall,
            new Ability()
            {
                AbilityName = "Stall",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Stalwart,
            new Ability()
            {
                AbilityName = "Stalwart",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Stamina,
            new Ability()
            {
                AbilityName = "Stamina",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.StanceChange,
            new Ability()
            {
                AbilityName = "Stance Change",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Static,
            new Ability()
            {
                AbilityName = "Static",
                AbilityDescription = "When the Pokémon is hit by a contact move, the attacker has a 30% chance of being paralyzed.",
                OnDamagingHit = (float damage, Pokemon target, Pokemon attacker, Move move) =>
                {
                    var flags = move.Base.Flags;
                    if (move.Base.HasFlag(MoveFlag.Contact) && Random.Range(1, 11) <= 3)
                        attacker.SetStatus(StatusConditionID.Paralyze, EffectSource.Ability);
                }
            }
        },
        {
            AbilityID.Steadfast,
            new Ability()
            {
                AbilityName = "Steadfast",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SteamEngine,
            new Ability()
            {
                AbilityName = "Steam Engine",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Steelworker,
            new Ability()
            {
                AbilityName = "Steelworker",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SteelySpirit,
            new Ability()
            {
                AbilityName = "Steely Spirit",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Stench,
            new Ability()
            {
                AbilityName = "Stench",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.StickyHold,
            new Ability()
            {
                AbilityName = "Sticky Hold",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.StormDrain,
            new Ability()
            {
                AbilityName = "Storm Drain",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.StrongJaw,
            new Ability()
            {
                AbilityName = "Strong Jaw",
                AbilityDescription = "Boosts the power of the Pokémon's biting moves by 50%.",
                OnBasePower = (float basePower, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.HasFlag(MoveFlag.Bite))
                        basePower = basePower * 1.5f;

                    return basePower;
                }
            }
        },
        {
            AbilityID.Sturdy,
            new Ability()
            {
                AbilityName = "Sturdy",
                AbilityDescription = "\tIf the Pokémon has full HP and takes damage from a move that would knock it out in one hit, it will endure the hit with 1 HP.",
            }
        },
        {
            AbilityID.SuctionCups,
            new Ability()
            {
                AbilityName = "Suction Cups",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SuperLuck,
            new Ability()
            {
                AbilityName = "Super Luck",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SupersweetSyrup,
            new Ability()
            {
                AbilityName = "Supersweet Syrup",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SupremeOverlord,
            new Ability()
            {
                AbilityName = "Supreme Overlord",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SurgeSurfer,
            new Ability()
            {
                AbilityName = "Surge Surfer",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Swarm,
            new Ability()
            {
                AbilityName = "Swarm",
                AbilityDescription = "Boosts the power of the Pokémon's Bug-type moves by 50% when its HP drops to 1/3 or less of its max.",
                OnModifyAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Bug && attacker.HP <= attacker.MaxHP / 3)
                        atk = atk * 1.5f;

                    return atk;
                },
                OnModifySpAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Bug && attacker.HP <= attacker.MaxHP / 3)
                        atk = atk * 1.5f;

                    return atk;
                }
            }
        },
        {
            AbilityID.SweetVeil,
            new Ability()
            {
                AbilityName = "Sweet Veil",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SwiftSwim,
            new Ability()
            {
                AbilityName = "Swift Swim",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.SwordOfRuin,
            new Ability()
            {
                AbilityName = "Sword of Ruin",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Symbiosis,
            new Ability()
            {
                AbilityName = "Symbiosis",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Synchronize,
            new Ability()
            {
                AbilityName = "Synchronize",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.TabletsOfRuin,
            new Ability()
            {
                AbilityName = "Tablets of Ruin",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.TangledFeet,
            new Ability()
            {
                AbilityName = "Tangled Feet",
                AbilityDescription = "Doubles the Pokémon's evasiveness if it is confused.",
            }
        },
        {
            AbilityID.TanglingHair,
            new Ability()
            {
                AbilityName = "Tangling Hair",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Technician,
            new Ability()
            {
                AbilityName = "Technician",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Telepathy,
            new Ability()
            {
                AbilityName = "Telepathy",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.TeraShell,
            new Ability()
            {
                AbilityName = "Tera Shell",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.TeraShift,
            new Ability()
            {
                AbilityName = "Tera Shift",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.TeraformZero,
            new Ability()
            {
                AbilityName = "Teraform Zero",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Teravolt,
            new Ability()
            {
                AbilityName = "Teravolt",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ThermalExchange,
            new Ability()
            {
                AbilityName = "Thermal Exchange",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ThickFat,
            new Ability()
            {
                AbilityName = "Thick Fat",
                AbilityDescription = "Halves the damage the Pokémon takes from Fire- and Ice-type moves.",
                OnModifyDef = (float def, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (attacker.Base.PokemonType1 == PokemonType.Fire || attacker.Base.PokemonType2 == PokemonType.Fire || attacker.Base.PokemonType1 == PokemonType.Ice || attacker.Base.PokemonType2 == PokemonType.Ice)
                        def = def * 2f;

                    return def;
                },
                OnModifySpDef = (float def, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (attacker.Base.PokemonType1 == PokemonType.Fire || attacker.Base.PokemonType2 == PokemonType.Fire || attacker.Base.PokemonType1 == PokemonType.Ice || attacker.Base.PokemonType2 == PokemonType.Ice)
                        def = def * 2f;

                    return def;
                },
            }
        },
        {
            AbilityID.TintedLens,
            new Ability()
            {
                AbilityName = "Tinted Lens",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Torrent,
            new Ability()
            {
                AbilityName = "Torrent",
                AbilityDescription = "Boosts the power of the Pokémon's Water-type moves by 50% when its HP drops to 1/3 or less of its max.",
                OnModifyAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Water && attacker.HP <= attacker.MaxHP / 3)
                        atk = atk * 1.5f;

                    return atk;
                },
                OnModifySpAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.MoveType == PokemonType.Water && attacker.HP <= attacker.MaxHP / 3)
                        atk = atk * 1.5f;

                    return atk;
                }
            }
        },
        {
            AbilityID.ToughClaws,
            new Ability()
            {
                AbilityName = "Tough Claws",
                AbilityDescription = "Boosts the power of the Pokémon's contact moves by 30%.",
                OnBasePower = (float basePower, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (move.Base.HasFlag(MoveFlag.Contact))
                        basePower = basePower * 1.3f;

                    return basePower;
                }
            }
        },
        {
            AbilityID.ToxicBoost,
            new Ability()
            {
                AbilityName = "Toxic Boost",
                AbilityDescription = "Powers up physical moves when the Pokémon is poisoned.",
                OnModifyAtk = (float atk, Pokemon attacker, Pokemon defender, Move move) =>
                {
                    if (attacker.Status.Id == StatusConditionID.Poison || attacker.Status.Id == StatusConditionID.BadPoison)
                        atk = atk * 1.5f;

                    return atk;
                }
            }
        },
        {
            AbilityID.ToxicChain,
            new Ability()
            {
                AbilityName = "Toxic Chain",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ToxicDebris,
            new Ability()
            {
                AbilityName = "Toxic Debris",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Trace,
            new Ability()
            {
                AbilityName = "Trace",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Transistor,
            new Ability()
            {
                AbilityName = "Transistor",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Triage,
            new Ability()
            {
                AbilityName = "Triage",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Truant,
            new Ability()
            {
                AbilityName = "Truant",
                AbilityDescription = "The Pokémon spends every other turn loafing around.",
            }
        },
        {
            AbilityID.Turboblaze,
            new Ability()
            {
                AbilityName = "Turboblaze",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Unaware,
            new Ability()
            {
                AbilityName = "Unaware",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Unburden,
            new Ability()
            {
                AbilityName = "Unburden",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.Unnerve,
            new Ability()
            {
                AbilityName = "Unnerve",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.UnseenFist,
            new Ability()
            {
                AbilityName = "Unseen Fist",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.VesselOfRuin,
            new Ability()
            {
                AbilityName = "Vessel of Ruin",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.VictoryStar,
            new Ability()
            {
                AbilityName = "Victory Star",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.VitalSpirit,
            new Ability()
            {
                AbilityName = "Vital Spirit",
                AbilityDescription = "The Pokémon cannot become drowsy or be put to sleep.",
                OnTrySetStatus = (StatusConditionID statusId, Pokemon pokemon, EffectSource effect) =>
                {
                    if (statusId != StatusConditionID.Sleep)
                        return true;

                    if (effect == EffectSource.Move)
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName}'s immune to sleep!");

                    return false;
                },
                OnTrySetVolatile = (StatusConditionID statusId, Pokemon pokemon, EffectSource effect) =>
                {
                    if (statusId != StatusConditionID.Drowsy)
                        return true;

                    if (effect == EffectSource.Move)
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName}'s immune to drowsiness!");

                    return false;
                }
            }
        },
        {
            AbilityID.VoltAbsorb,
            new Ability()
            {
                AbilityName = "Volt Absorb",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WanderingSpirit,
            new Ability()
            {
                AbilityName = "Wandering Spirit",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WaterAbsorb,
            new Ability()
            {
                AbilityName = "Water Absorb",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WaterBubble,
            new Ability()
            {
                AbilityName = "Water Bubble",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WaterCompaction,
            new Ability()
            {
                AbilityName = "Water Compaction",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WaterVeil,
            new Ability()
            {
                AbilityName = "Water Veil",
                AbilityDescription = "The Pokémon's water veil prevents it from being burned.",
                OnTrySetStatus = (StatusConditionID statusId, Pokemon pokemon, EffectSource effect) =>
                {
                    if (statusId != StatusConditionID.Burn)
                        return true;

                    if (effect == EffectSource.Move)
                        pokemon.AddStatusEvent($"{pokemon.Base.PokemonName}'s immune to burning!");

                    return false;
                }
            }
        },
        {
            AbilityID.WeakArmor,
            new Ability()
            {
                AbilityName = "Weak Armor",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WellBakedBody,
            new Ability()
            {
                AbilityName = "Well-Baked Body",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WhiteSmoke,
            new Ability()
            {
                AbilityName = "White Smoke",
                AbilityDescription = "The Pokémon's stats cannot be lowered by other Pokémon's moves or Abilities.",
                OnBoost = (Dictionary<Stat, int> boosts, Pokemon target, Pokemon source) =>
                {
                    if (source != null && target == source) return;

                    bool boostRemoved = false;

                    foreach (var stat in boosts.Keys.ToList()) {
                        if (boosts[stat] < 0)
                        {
                            boostRemoved = true;
                            boosts.Remove(Stat.accuracy);
                        }
                    }

                    if (boostRemoved)
                        target.AddStatusEvent($"{target.Base.PokemonName}'s white smoke prevents stat loss!");
                }
            }
        },
        {
            AbilityID.WimpOut,
            new Ability()
            {
                AbilityName = "Wimp Out",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WindPower,
            new Ability()
            {
                AbilityName = "Wind Power",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WindRider,
            new Ability()
            {
                AbilityName = "Wind Rider",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.WonderGuard,
            new Ability()
            {
                AbilityName = "Wonder Guard",
                AbilityDescription = "Its mysterious power only lets supereffective moves hit the Pokémon.",
            }
        },
        {
            AbilityID.WonderSkin,
            new Ability()
            {
                AbilityName = "Wonder Skin",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ZenMode,
            new Ability()
            {
                AbilityName = "Zen Mode",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
        {
            AbilityID.ZeroToHero,
            new Ability()
            {
                AbilityName = "Zero to Hero",
                AbilityDescription = "This ability currently does nothing.",
            }
        },
    };
}

public enum AbilityID
{
    None, AirLock, ArenaTrap, BattleArmor, Blaze, Chlorophyll, ClearBody, CloudNine, ColorChange, CompoundEyes, CuteCharm, Damp, Drizzle, Drought, EarlyBird, EffectSpore, FlameBody,
    FlashFire, Forecast, Guts, HugePower, Hustle, HyperCutter, Illuminate, Immunity, InnerFocus, Insomnia, Intimidate, KeenEye, Levitate, LightningRod, Limber, LiquidOoze, MagmaArmor,
    MagnetPull, MarvelScale, Minus, NaturalCure, Oblivious, Overgrow, OwnTempo, Pickup, Plus, PoisonPoint, Pressure, PurePower, RainDish, RockHead, RoughSkin, RunAway, SandStream,
    SandVeil, SereneGrace, ShadowTag, ShedSkin, ShellArmor, ShieldDust, Soundproof, SpeedBoost, Static, Stench, StickyHold, Sturdy, SuctionCups, Swarm, SwiftSwim, Synchronize, ThickFat,
    Torrent, Trace, Truant, VitalSpirit, VoltAbsorb, WaterAbsorb, WaterVeil, WhiteSmoke, WonderGuard, Adaptability, Aftermath, AngerPoint, Anticipation, BadDreams, Download, DrySkin,
    Filter, FlowerGift, Forewarn, Frisk, Gluttony, Heatproof, HoneyGather, Hydration, IceBody, IronFist, Klutz, LeafGuard, MagicGuard, MoldBreaker, MotorDrive, Multitype, NoGuard,
    Normalize, PoisonHeal, QuickFeet, Reckless, Rivalry, Scrappy, Simple, SkillLink, SlowStart, Sniper, SnowCloak, SnowWarning, SolarPower, SolidRock, Stall, Steadfast, StormDrain,
    SuperLuck, TangledFeet, Technician, TintedLens, Unaware, Unburden, Analytic, BigPecks, Contrary, CursedBody, Defeatist, Defiant, FlareBoost, FriendGuard, Harvest, Healer, HeavyMetal,
    Illusion, Imposter, Infiltrator, IronBarbs, Justified, LightMetal, MagicBounce, Moody, Moxie, Multiscale, Mummy, Overcoat, Pickpocket, PoisonTouch, Prankster, Rattled, Regenerator,
    SandForce, SandRush, SapSipper, SheerForce, Telepathy, Teravolt, ToxicBoost, Turboblaze, Unnerve, VictoryStar, WeakArmor, WonderSkin, ZenMode, Aerilate, AromaVeil, AuraBreak,
    Bulletproof, CheekPouch, Competitive, DarkAura, DeltaStream, DesolateLand, FairyAura, FlowerVeil, FurCoat, GaleWings, Gooey, GrassPelt, Magician, MegaLauncher, ParentalBond, Pixilate,
    PrimordialSea, Protean, Refrigerate, StanceChange, StrongJaw, SweetVeil, Symbiosis, ToughClaws, Battery, BattleBond, BeastBoost, Berserk, Comatose, Corrosion, Dancer, Dazzling,
    Disguise, ElectricSurge, EmergencyExit, Fluffy, FullMetalBody, Galvanize, GrassySurge, InnardsOut, LiquidVoice, LongReach, Merciless, MistySurge, Neuroforce, PowerConstruct,
    PowerOfAlchemy, PrismArmor, PsychicSurge, QueenlyMajesty, Receiver, RKSSystem, Schooling, ShadowShield, ShieldsDown, SlushRush, SoulHeart, Stakeout, Stamina, Steelworker, SurgeSurfer,
    TanglingHair, Triage, WaterBubble, WaterCompaction, WimpOut, AsOneG, AsOneS, BallFetch, ChillingNeigh, CottonDown, CuriousMedicine, DauntlessShield, DragonsMaw, GorillaTactics,
    GrimNeigh, GulpMissile, HungerSwitch, IceFace, IceScales, IntrepidSword, Libero, Mimicry, MirrorArmor, NeutralizingGas, PastelVeil, PerishBody, PowerSpot, PropellerTail, PunkRock,
    QuickDraw, Ripen, SandSpit, ScreenCleaner, Stalwart, SteamEngine, SteelySpirit, Transistor, UnseenFist, WanderingSpirit, AngerShell, ArmorTail, BeadsOfRuin, Commander, Costar,
    CudChew, Dragonize, EarthEater, Eelevate, Electromorphosis, EmbodyAspect, FireMane, GoodAsGold, GuardDog, HadronEngine, Hospitality, LingeringAroma, MegaSol, MindsEye, MyceliumMight,
    Opportunist, OrichalcumPulse, PiercingDrill, PoisonPuppeteer, Protosynthesis, PurifyingSalt, QuarkDrive, RockyPayload, SeedSower, Sharpness, SpicySpray, SupersweetSyrup,
    SupremeOverlord, SwordOfRuin, TabletsOfRuin, TeraShell, TeraShift, TeraformZero, ThermalExchange, ToxicChain, ToxicDebris, VesselOfRuin, WellBakedBody, WindPower, WindRider, ZeroToHero
}