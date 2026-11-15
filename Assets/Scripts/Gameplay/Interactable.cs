using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    IEnumerator Interact(Transform initiator);
    IEnumerator Move(Vector2 moveVector, bool checkCollisions);

    bool IsMoving { get; }
}