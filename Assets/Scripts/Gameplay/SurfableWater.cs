using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using E7.Introloop;

public class SurfableWater : MonoBehaviour, Interactable, IPlayerTriggerable
{
    [SerializeField] IntroloopAudio surfingMusic;
    [SerializeField] int chanceOfEncounter;
    [SerializeField] BattleTrigger battleTrigger;

    [SerializeField] ItemBase requiredBadge;

    public int ChanceOfEncounter {
        get { return chanceOfEncounter; }
    }

    bool IsJumpingToWater = false;
    public IntroloopAudio SurfingMusic => surfingMusic;
    public IEnumerator Interact(Transform initiator)
    {
        var animator = initiator.GetComponent<CharacterAnimator>();
        if (animator.IsSurfing || IsJumpingToWater)
            yield break;

        var pokemonWithSurf = initiator.GetComponent<PokemonParty>().PokemonList.FirstOrDefault(p => p.Moves.Any(m => m.Base.MoveName == "Surf"));

        var hasRequiredBadge = initiator.GetComponent<Inventory>().HasItem(requiredBadge);

        if (pokemonWithSurf != null && hasRequiredBadge)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"The water is a deep blue... Do you want to surf on it?",
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithSurf.Base.PokemonName} used Surf!");

                var direction = new Vector3(animator.MoveX, animator.MoveY);
                var targetPosition = initiator.transform.position + direction;
                var shadowTargetPosition = initiator.transform.position + direction + new Vector3(0, -0.125f);

                animator.IsJumpingToWater = true;
                yield return initiator.GetComponent<PlayerController>().Shadow.transform.DOJump(targetPosition, 0f, 1, 0.6f);
                yield return initiator.transform.DOJump(targetPosition, 0.6f, 1, 0.6f).WaitForCompletion();
                animator.IsJumpingToWater = false;

                animator.IsSurfing = true;
                AudioManager.i.PlayMusic(surfingMusic, true);
            }
        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (player.repelStepsRemaining == -1)
        {
            if (UnityEngine.Random.Range(0, 2880) <= ChanceOfEncounter)
                GameController.Instance.StartBattle(battleTrigger);
        }
    }

    public bool TriggerRepeatedly => true;

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => false;
}