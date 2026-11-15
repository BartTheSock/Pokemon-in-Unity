using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Add new TM or HM")]
public class TMItem : ItemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHM;

    public override string ItemName => $"{base.name} {move.MoveName}";
    public override string ItemNamePlural => $"{base.name} {move.MoveName}";

    public override string ItemDescription => $"Teaches a Pokémon {move.MoveName}. {move.MoveDescription}";

    public override bool Use(Pokemon pokemon)
    {
        // Learning a move is handled from InventoryUI. If it was learned, then return true.
        return pokemon.HasMove(move);
    }

    public bool CanBeTaught(Pokemon pokemon)
    {
        return pokemon.Base.LearnableByItems.Contains(move);
    }

    public override bool CanUseInBattle => false;

    public override bool IsReusable => true;

    public override bool IsSellable => false;

    public MoveBase Move => move;

    public bool IsHM => isHM;
}