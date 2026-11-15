using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovableBoulder : MonoBehaviour, Interactable
{
    [SerializeField] ItemBase requiredBadge;
    [SerializeField] float moveSpeed;

    public bool IsMoving { get; private set; }

    public IEnumerator Interact(Transform initiator)
    {
        var animator = initiator.GetComponent<CharacterAnimator>();
        if (animator.IsUsingStrength)
            yield break;

        var pokemonWithStrength = initiator.GetComponent<PokemonParty>().PokemonList.FirstOrDefault(p => p.Moves.Any(m => m.Base.MoveName == "Strength"));
        
        var hasRequiredBadge = initiator.GetComponent<Inventory>().HasItem(requiredBadge);

        if (pokemonWithStrength != null && hasRequiredBadge)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"It's a big boulder, but a Pokémon may be able to push it aside. Would you like to use Strength?",
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithStrength.Base.PokemonName} used Strength!");
                yield return DialogManager.Instance.ShowDialogText($"{pokemonWithStrength.Base.PokemonName}'s Strength made it possible to push boulders around!");

                animator.IsUsingStrength = true;
            }
        }
        else
            yield return DialogManager.Instance.ShowDialogText("It's a big boulder, but a Pokémon may be able to move it.");
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions = true)
    {
        float moveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        float moveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPosition = transform.position;
        targetPosition.x += moveVector.x;
        targetPosition.y += moveVector.y;

        if (checkCollisions && !IsPathClear(targetPosition))
            yield break;

        IsMoving = true;
        
        while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            yield return null;
        }
        transform.position = targetPosition;

        IsMoving = false;
    }

    private bool IsPathClear(Vector3 targetPosition)
    {
        var difference = targetPosition - transform.position;
        var direction = difference.normalized;
        var collisionLayers = GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer | GameLayers.i.WaterLayer | GameLayers.i.StrengthBoulderLayer | GameLayers.i.PortalLayer;

        if (Physics2D.BoxCast(transform.position + direction, new Vector2(0.2f, 0.2f), 0f, direction, difference.magnitude - 1, collisionLayers))
            return false;

        return true;
    }
}