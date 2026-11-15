using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Add new treasure")]
public class TreasureItem : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return false;
    }

    public override bool CanUseInBattle => false;

    public override bool CanUseOutsideBattle => false;
}