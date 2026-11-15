using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonStaticEncounter : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] BattleTrigger battleTrigger;
    [SerializeField] string dialogText;

    public bool Caught { get; set; } = false;

    public bool IsMoving { get; private set; } = false;

    PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!Caught)
        {
            int selectedChoice = 0;
            yield return DialogManager.Instance.ShowDialogText($"{dialogText}",
                choices: new List<string>() { "Yes", "No" },
                onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    GetComponent<SpriteRenderer>().enabled = true;
                    yield return new WaitForSeconds(0.05f);
                    GetComponent<SpriteRenderer>().enabled = false;
                    yield return new WaitForSeconds(0.05f);
                }

                GameController.Instance.StartStaticEncounterBattle(this, battleTrigger);

                GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    public IEnumerator CaughtPokemon()
    {
        Caught = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        yield break;
    }

    public object CaptureState()
    {
        var saveData = new StaticEncounterSaveData()
        {
            caughtStaticEncounter = Caught,
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (StaticEncounterSaveData)state;
        Caught = saveData.caughtStaticEncounter;

        if (Caught)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }
}

[Serializable]
public class StaticEncounterSaveData
{
    public bool caughtStaticEncounter;
}