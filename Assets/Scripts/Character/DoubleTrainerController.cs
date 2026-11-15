using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using E7.Introloop;

public class DoubleTrainerController : TrainerController
{
    [SerializeField] DoubleTrainerController otherTrainer;

    // State
    Character character;
    SpriteRenderer exclamationRendererOne;
    SpriteRenderer exclamationRendererTwo;

    private void Awake()
    {
        character = GetComponent<Character>();
        exclamationRendererOne = exclamation.GetComponent<SpriteRenderer>();
        exclamationRendererTwo = otherTrainer.exclamation.GetComponent<SpriteRenderer>();
    }
    public override void Start()
    {
        SetFOVRotation(character.Animator.DefaultDirection);
    }
    private void Update()
    {
        character.HandleUpdate();
    }
    public override IEnumerator Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);

        if (!battleLost)
        {
            var playerPartyCount = initiator.GetComponent<PokemonParty>().PokemonList.Count;
            if (BattleUnitCount > playerPartyCount)
            {
                yield return DialogManager.Instance.ShowDialog(dialogRefuseBattle);
                if (character.Animator.DefaultDirection == FacingDirection.Up)
                    character.LookTowards(originalTrainerPosition + new Vector3(0, 1));
                else if (character.Animator.DefaultDirection == FacingDirection.Down)
                    character.LookTowards(originalTrainerPosition + new Vector3(0, -1));
                else if (character.Animator.DefaultDirection == FacingDirection.Left)
                    character.LookTowards(originalTrainerPosition + new Vector3(-1, 0));
                else if (character.Animator.DefaultDirection == FacingDirection.Right)
                    character.LookTowards(originalTrainerPosition + new Vector3(1, 0));
            }
            else
            {
                AudioManager.i.PlaySfx(AudioID.TrainerExclaim, pauseMusic: true);
                
                exclamation.SetActive(true);
                otherTrainer.exclamation.SetActive(true);
                exclamationRendererOne.sprite = exclamationSprites[0];
                exclamationRendererTwo.sprite = exclamationSprites[0];
                yield return new WaitForSeconds(0.05f);
                exclamationRendererOne.sprite = exclamationSprites[1];
                exclamationRendererTwo.sprite = exclamationSprites[1];
                yield return new WaitForSeconds(0.05f);

                AudioManager.i.PlayMusic(trainerAppearsMusic, true);

                exclamationRendererOne.sprite = exclamationSprites[2];
                exclamationRendererTwo.sprite = exclamationSprites[2];
                yield return new WaitForSeconds(0.4f);
                exclamation.SetActive(false);
                otherTrainer.exclamation.SetActive(false);

                yield return DialogManager.Instance.ShowDialog(dialog);
                GameController.Instance.StartDoubleTrainerBattle(this, otherTrainer, trainerBattleTrigger);
            }
        }
        else
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
    }

    public IEnumerator TriggerDoubleTrainerBattle(PlayerController player)
    {
        var playerPartyCount = player.GetComponent<PokemonParty>().PokemonList.Count;
        if (battleUnitCount <= playerPartyCount)
        {
            GameController.Instance.StateMachine.Push(CutsceneState.i);

            AudioManager.i.PlaySfx(AudioID.TrainerExclaim, pauseMusic: true);

            // Show exclamation
            exclamation.SetActive(true);
            otherTrainer.exclamation.SetActive(true);
            exclamationRendererOne.sprite = exclamationSprites[0];
            exclamationRendererTwo.sprite = exclamationSprites[0];
            yield return new WaitForSeconds(0.05f);
            exclamationRendererOne.sprite = exclamationSprites[1];
            exclamationRendererTwo.sprite = exclamationSprites[1];
            yield return new WaitForSeconds(0.05f);

            AudioManager.i.PlayMusic(trainerAppearsMusic, true);

            exclamationRendererOne.sprite = exclamationSprites[2];
            exclamationRendererTwo.sprite = exclamationSprites[2];
            yield return new WaitForSeconds(0.4f);
            exclamation.SetActive(false);
            otherTrainer.exclamation.SetActive(false);

            // Walk towards player
            var difference = player.transform.position - transform.position;
            var moveVector = difference - difference.normalized;
            moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));

            StartCoroutine(otherTrainer.character.Move(moveVector));
            yield return character.Move(moveVector);

            player.Character.LookTowards(transform.position);

            // Show dialog
            yield return DialogManager.Instance.ShowDialog(dialog);

            GameController.Instance.StateMachine.Pop();

            GameController.Instance.StartDoubleTrainerBattle(this, otherTrainer, trainerBattleTrigger);
        }
    }
    public GameObject Exclamation => exclamation;
    public GameObject FOV => fov;
}