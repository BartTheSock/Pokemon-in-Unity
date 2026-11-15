using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToFreeRoamState : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;

        PlayTimeTimer.i.StartTimer();
        GameController.Instance.StateMachine.ChangeState(FreeRoamState.i);
    }
}