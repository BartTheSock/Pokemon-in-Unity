using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Add new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("Health Points")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("Power Points")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;
    [SerializeField] bool restoreHalfPP;

    [Header("Status Effects")]
    [SerializeField] StatusConditionID status1;
    [SerializeField] StatusConditionID status2;
    [SerializeField] bool recoverAllStatus;

    [Header("Revives")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    Pokemon _pokemon;
    string recoveryItemMessage;
    public override string ItemUseMessage => recoveryItemMessage;

    public override bool Use(Pokemon pokemon)
    {
        _pokemon = pokemon;

        // Revive and Max Revive (only for fainted Pokémon)
        if (revive || maxRevive)
        {
            recoveryItemMessage = base.ItemUseMessage;
            if (pokemon.HP > 0)
                return false;

            if (revive)
                pokemon.IncreaseHP(Mathf.CeilToInt((float)pokemon.MaxHP / 2f));
            else if (maxRevive)
                pokemon.IncreaseHP(pokemon.MaxHP);

            pokemon.CureStatus();
            pokemon.CureVolatileStatus();

            return true;
        }

        // No items besides Revives and Max Revives may be used on a fainted Pokémon
        if (pokemon.HP <= 0)
            return false;

        // Full Restores
        if (restoreMaxHP && recoverAllStatus)
        {
            if (pokemon.HP == pokemon.MaxHP && pokemon.Status == null && pokemon.VolatileStatus == null)
                return false;

            pokemon.IncreaseHP(pokemon.MaxHP);
            pokemon.CureStatus();
            pokemon.CureVolatileStatus();
        }

        // Potions, Super Potions, Hyper Potions, and Max Potions
        if ((hpAmount > 0 || restoreMaxHP) && !recoverAllStatus)
        {
            if (pokemon.HP == pokemon.MaxHP)
                return false;

            if (restoreMaxHP)
            {
                recoveryItemMessage = base.ItemUseMessage;
                pokemon.IncreaseHP(pokemon.MaxHP);
            }
            else
            {
                if ((pokemon.MaxHP - pokemon.HP) < hpAmount)
                {
                    int revisedHpAmount = (pokemon.MaxHP - pokemon.HP);
                    if (revisedHpAmount == 1 && !restoreMaxHP)
                        recoveryItemMessage = $"'s HP was restored by 1 point!";
                    else if (revisedHpAmount > 1 && !restoreMaxHP)
                        recoveryItemMessage = $"'s HP was restored by {revisedHpAmount} points!";
                    else
                        recoveryItemMessage = base.ItemUseMessage;
                    pokemon.IncreaseHP(revisedHpAmount);
                }
                else
                {
                    recoveryItemMessage = base.ItemUseMessage;
                    pokemon.IncreaseHP(hpAmount);
                }
            }
        }

        // Status healers
        if ((recoverAllStatus || (status1 != StatusConditionID.None || status2 != StatusConditionID.None)) && !restoreMaxHP)
        {
            recoveryItemMessage = base.ItemUseMessage;
            if (pokemon.Status == null && pokemon.VolatileStatus == null)
                return false;

            if (recoverAllStatus)
            {
                pokemon.CureStatus();
                pokemon.CureVolatileStatus();
            }
            else
            {
                if (pokemon.Status.Id == status1 || pokemon.Status.Id == status2)
                    pokemon.CureStatus();
                else if (pokemon.VolatileStatus.Id == status1 || pokemon.VolatileStatus.Id == status2)
                    pokemon.CureVolatileStatus();
                else
                    return false;
            }
        }

        // Restore PP
        if (restoreMaxPP)
        {
            recoveryItemMessage = base.ItemUseMessage;
            pokemon.Moves.ForEach(m => m.IncreasePP(m.Base.PP));
        }
        else if (restoreHalfPP)
        {
            recoveryItemMessage = base.ItemUseMessage;
            pokemon.Moves.ForEach(m => m.IncreasePP(Mathf.CeilToInt((float)m.Base.PP / 2f)));
        }
        else if (ppAmount > 0)
        {
            recoveryItemMessage = base.ItemUseMessage;
            pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }

        return true;
    }
}