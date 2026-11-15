using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Add new Gym Badge")]
public class GymBadge : ItemBase
{
    public override bool IsReusable => true;

    public override bool IsSellable => false;

    public override int Price => 0;

    public override bool Use(Pokemon pokemon)
    {
        return false;
    }

    public override bool CanUseInBattle => false;

    public override bool CanUseOutsideBattle => false;
}