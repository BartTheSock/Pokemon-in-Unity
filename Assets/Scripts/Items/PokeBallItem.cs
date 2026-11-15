using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Add new Poké Ball")]
public class PokeBallItem : ItemBase
{
    [SerializeField] float catchRateStandard;
    [SerializeField] float catchRateMythical;
    [SerializeField] float catchRateLegendary;
    [SerializeField] float catchRateFirstPartner;
    [SerializeField] float catchRatePseudoLegendary;
    [SerializeField] float catchRateRare;

    [SerializeField] int ballID;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public override bool CanUseOutsideBattle => false;

    public float CatchRateStandard => catchRateStandard;
    public float CatchRateMythical => catchRateMythical;
    public float CatchRateLegendary => catchRateLegendary;
    public float CatchRateFirstPartner => catchRateFirstPartner;
    public float CatchRatePseudoLegendary => catchRatePseudoLegendary;
    public float CatchRateRare => catchRateRare;

    public int BallID => ballID;
}