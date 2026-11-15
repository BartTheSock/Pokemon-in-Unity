using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallGrass : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int chanceOfEncounter;

    [SerializeField] BattleTrigger battleTrigger;

    public int ChanceOfEncounter {
        get { return chanceOfEncounter; }
    }


    public void OnPlayerTriggered(PlayerController player)
    {
        bool playerRunning = player.GetComponent<Character>().IsRunning;
        if (player.repelStepsRemaining == -1)
        {
            if ((UnityEngine.Random.Range(0, 2880) <= ChanceOfEncounter && !playerRunning) || (UnityEngine.Random.Range(0, 2880) <= (ChanceOfEncounter * 0.75f) && playerRunning))
            {
                player.Character.Animator.IsMoving = false;
                GameController.Instance.StartBattle(battleTrigger);
            }
        }
    }

    public bool TriggerRepeatedly => true;
}