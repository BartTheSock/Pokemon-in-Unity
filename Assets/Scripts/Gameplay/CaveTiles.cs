using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveTiles : MonoBehaviour, Interactable, IPlayerTriggerable
{
    [SerializeField] int chanceOfEncounter;
    [SerializeField] BattleTrigger battleTrigger;

    public int ChanceOfEncounter
    {
        get { return chanceOfEncounter; }
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!GameController.Instance.CurrentScene.IsDark)
            yield break;

        var animator = initiator.GetComponent<CharacterAnimator>();
        
        if (animator.IsUsingFlash)
            yield break;

        var pokemonWithFlash = initiator.GetComponent<PokemonParty>().PokemonList.FirstOrDefault(p => p.Moves.Any(m => m.Base.MoveName == "Flash"));

        if (pokemonWithFlash != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"It's pretty dark in here. Do you want to light up the area?",
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithFlash.Base.PokemonName} used Flash!");

                animator.IsUsingFlash = true;
            }
        }
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

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => false;
}