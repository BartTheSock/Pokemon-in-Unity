using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Ledge : MonoBehaviour
{
    [SerializeField] float xDirection;
    [SerializeField] float yDirection;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool TryToJump(Character character, Vector2 moveDirection)
    {
        if (moveDirection.x == xDirection && moveDirection.y == yDirection)
        {
            StartCoroutine(Jump(character));
            return true;
        }
        else
            return false;
    }

    IEnumerator Jump(Character character)
    {
        GameController.Instance.PauseGame(true);

        character.Animator.IsJumping = true;

        AudioManager.i.PlaySfx(AudioID.LedgeJump, pauseMusic: false);
        var jumpDestination = character.transform.position + new Vector3(xDirection, yDirection) * 2f;
        var shadowJumpDestination = character.transform.position + new Vector3(xDirection * 2f, (yDirection * 2f) - 0.125f);
        yield return character.Shadow.transform.DOJump(shadowJumpDestination, 0f, 1, 0.6f);
        yield return character.transform.DOJump(jumpDestination, 1f, 1, 0.6f).WaitForCompletion();

        GameController.Instance.PauseGame(false);

        character.Animator.IsJumping = false;
    }
}