using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Add new battle item")]
public class BattleItem : ItemBase
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts => boosts;

    Pokemon _pokemon;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public override bool CanUseInBattle => true;
    public override bool CanUseOutsideBattle => false;
}