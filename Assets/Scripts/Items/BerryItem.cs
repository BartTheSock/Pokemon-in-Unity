using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Add new berry")]
public class BerryItem : ItemBase
{
    [Header("HP, PP, and Friendship")]
    [SerializeField] int hpAmount;
    [SerializeField] int ppAmount;
    [SerializeField] int lowFriendshipAmount;
    [SerializeField] int medFriendshipAmount;
    [SerializeField] int highFriendshipAmount;

    [Header("Status Effects")]
    [SerializeField] StatusConditionID status1;
    [SerializeField] StatusConditionID status2;
    [SerializeField] bool recoverAllStatus;

    [Header("Berry ID")]
    [SerializeField] BerryType berryType;

    Pokemon _pokemon;
    string berryItemMessage;
    public override string ItemUseMessage => berryItemMessage;

    public override bool Use(Pokemon pokemon)
    {
        _pokemon = pokemon;

        // No items besides Revives and Max Revives may be used on a fainted Pokémon
        if (pokemon.HP <= 0)
            return false;

        // Potions, Super Potions, Hyper Potions, and Max Potions
        if (hpAmount > 0 && !recoverAllStatus)
        {
            if (pokemon.HP == pokemon.MaxHP)
                return false;

            if ((pokemon.MaxHP - pokemon.HP) < hpAmount)
            {
                int revisedHpAmount = (pokemon.MaxHP - pokemon.HP);
                if (revisedHpAmount == 1)
                    berryItemMessage = $"'s HP was restored by 1 point!";
                else if (revisedHpAmount > 1)
                    berryItemMessage = $"'s HP was restored by {revisedHpAmount} points!";
                else
                    berryItemMessage = base.ItemUseMessage;
                pokemon.IncreaseHP(revisedHpAmount);
            }
            else
            {
                berryItemMessage = base.ItemUseMessage;
                pokemon.IncreaseHP(hpAmount);
            }
        }

        // Status healers
        if (recoverAllStatus || status1 != StatusConditionID.None || status2 != StatusConditionID.None)
        {
            berryItemMessage = base.ItemUseMessage;
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
        if (ppAmount > 0)
        {
            berryItemMessage = base.ItemUseMessage;
            pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }

        // Increase Friendship
        if ((lowFriendshipAmount > 0 && medFriendshipAmount > 0 && highFriendshipAmount > 0) && pokemon.Friendship < 255)
        {
            berryItemMessage = base.ItemUseMessage;
            pokemon.ChangeFriendship(lowFriendshipAmount, medFriendshipAmount, highFriendshipAmount);
        }
        else if (pokemon.Friendship == 255)
        {
            berryItemMessage = $" adores you!";
            return false;
        }
        return true;
    }
}