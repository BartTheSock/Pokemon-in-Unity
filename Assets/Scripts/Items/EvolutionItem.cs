using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Add new evolution item")]
public class EvolutionItem : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public bool CanEvolveUsingItem(Pokemon pokemon)
    {
        var evolution = pokemon.CheckForEvolution(this);
        if (evolution != null)
            return true;
        else
            return false;
    }

    public override bool CanUseInBattle => false;
}