using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPC : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiator)
    {
        AudioManager.i.PlaySfx(AudioID.PCOn, pauseMusic: false);
        yield return DialogManager.Instance.ShowDialogText("playerName booted up the PC.");

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("Which PC should be accessed?",
            choices: new List<string>() { "Someone's PC", "Log Off" },
            onChoiceSelected: (selection) => selectedChoice = selection);

        if (selectedChoice == 0)
        {
            // Log into Pokémon Storage System
            AudioManager.i.PlaySfx(AudioID.PCAccess, pauseMusic: false);
            GameController.Instance.StateMachine.Push(StorageState.i);
        }
        else
            AudioManager.i.PlaySfx(AudioID.PCOff, pauseMusic: false);
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => false;
}