using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CuttableTree : MonoBehaviour, Interactable
{
    [SerializeField] List<Sprite> cuttableTreeSprites;
    [SerializeField] ItemBase requiredBadge;

    public IEnumerator Interact(Transform initiator)
    {
        var pokemonWithCut = initiator.GetComponent<PokemonParty>().PokemonList.FirstOrDefault(p => p.Moves.Any(m => m.Base.MoveName == "Cut"));
        var cutTreeAnimation = GetComponent<SpriteRenderer>();

        var hasRequiredBadge = initiator.GetComponent<Inventory>().HasItem(requiredBadge);

        if (pokemonWithCut != null && hasRequiredBadge)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"This tree looks like it can be cut down. Would you like to cut it?",
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                AudioManager.i.PlaySfx(AudioID.CutTree, pauseMusic: false);
                cutTreeAnimation.sprite = cuttableTreeSprites[1];
                yield return new WaitForSeconds(0.1f);
                cutTreeAnimation.sprite = cuttableTreeSprites[2];
                yield return new WaitForSeconds(0.1f);
                cutTreeAnimation.sprite = cuttableTreeSprites[3];
                yield return new WaitForSeconds(0.1f);
                gameObject.SetActive(false);
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithCut.Base.PokemonName} used Cut!");
            }
        }
        else
            yield return DialogManager.Instance.ShowDialogText("This tree looks like it can be cut down.");
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => false;
}