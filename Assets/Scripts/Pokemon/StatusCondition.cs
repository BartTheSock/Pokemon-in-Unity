using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCondition
{
    public StatusConditionID Id { get; set; }
    public string StatusConditionName { get; set; }
    public string StatusConditionDescription { get; set; }
    public string StatusConditionStartMessage { get; set; }
    public string StatusConditionEffectMessage { get; set; }

    public Action<Pokemon> OnStart { get; set; }
    public Func<Pokemon, bool> OnBeforeMove { get; set; }
    public Action<Pokemon> OnAfterTurn { get; set; }

    public Action<Pokemon, Pokemon> OnAfterTurnSeeded { get; set; }
    public Action<Pokemon, Pokemon> OnAfterTurnSeeder { get; set; }

    public Func<Pokemon, Pokemon, Move, float> OnDamageModify { get; set; }
}