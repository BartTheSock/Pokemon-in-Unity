using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] Sprite signSprite;

    bool reading = false;

    public IEnumerator Interact(Transform initiator)
    {
        if (!reading)
        {
            reading = true;
            yield return DialogManager.Instance.ShowDialog(dialog);
            reading = false;
        }
    }
    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }
    public bool IsMoving => false;
}