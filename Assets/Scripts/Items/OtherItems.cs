using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Add another item")]
public class OtherItems : ItemBase
{
    [SerializeField] bool increaseLevel;
    [SerializeField] int repelAmount;

    public override bool Use(Pokemon pokemon)
    {
        return (increaseLevel && pokemon.Level < PlayerController.i.pokemonLevelCap) || repelAmount > 0;
    }

    public override bool CanUseInBattle => false;
    public override bool CanUseOutsideBattle => increaseLevel || repelAmount > 0;

    public bool IncreaseLevel => increaseLevel;
    public int RepelAmount => repelAmount;
}