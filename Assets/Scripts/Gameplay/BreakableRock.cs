using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakableRock : MonoBehaviour, Interactable
{
    [SerializeField] List<Sprite> breakableRockSprites;
    [SerializeField] ItemBase requiredBadge;

    [SerializeField] BattleTrigger battleTrigger;

    [SerializeField] int chanceOfEncounter;

    public int ChanceOfEncounter
    {
        get { return chanceOfEncounter; }
    }

    public IEnumerator Interact(Transform initiator)
    {
        var pokemonWithRockSmash = initiator.GetComponent<PokemonParty>().PokemonList.FirstOrDefault(p => p.Moves.Any(m => m.Base.MoveName == "Rock Smash"));
        var breakRockAnimation = GetComponent<SpriteRenderer>();

        var hasRequiredBadge = initiator.GetComponent<Inventory>().HasItem(requiredBadge);

        if (pokemonWithRockSmash != null && hasRequiredBadge)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"This rock appears to be breakable. Would you like to use Rock Smash?",
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                AudioManager.i.PlaySfx(AudioID.CutTree, pauseMusic: false);
                breakRockAnimation.sprite = breakableRockSprites[1];
                yield return new WaitForSeconds(0.1f);
                breakRockAnimation.sprite = breakableRockSprites[2];
                yield return new WaitForSeconds(0.1f);
                breakRockAnimation.sprite = breakableRockSprites[3];
                yield return new WaitForSeconds(0.1f);
                gameObject.SetActive(false);
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithRockSmash.Base.PokemonName} used Rock Smash!");

                if (UnityEngine.Random.Range(0, 2880) <= ChanceOfEncounter)
                    GameController.Instance.StartBattle(battleTrigger);
            }
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText("It's a rugged rock, but a Pokémon might be able to smash it.");
        }
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => false;
}