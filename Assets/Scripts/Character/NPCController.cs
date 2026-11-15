using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable, ISavable
{
    [Header("Dialog")]
    [SerializeField] Dialog dialog;

    [Header("Quests")]
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;

    [Header("Movement")]
    [SerializeField] int minX;
    [SerializeField] int minY;
    [SerializeField] int maxX;
    [SerializeField] int maxY;
    [SerializeField] float startingIdleTimer;

    NPCState state;
    float idleTimer;
    Vector2 startPosition;
    Quest activeQuest;

    Character character;
    ItemGiver itemGiver;
    PokemonGiver pokemonGiver;
    FirstPokemonGiver firstPokemonGiver;
    Healer healer;
    Clerk clerk;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        pokemonGiver = GetComponent<PokemonGiver>();
        firstPokemonGiver = GetComponent<FirstPokemonGiver>();
        healer = GetComponent<Healer>();
        clerk = GetComponent<Clerk>();
        idleTimer = startingIdleTimer;
        startPosition = transform.position;
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            if (questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                yield return quest.CompleteQuest(initiator.GetComponent<PlayerController>());
                questToComplete = null;
            }

            if (firstPokemonGiver != null && firstPokemonGiver.CanBeGiven())
            {
                yield return firstPokemonGiver.GivePokemon(initiator.GetComponent<PlayerController>());
                if (itemGiver != null && itemGiver.CanBeGiven())
                    yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (pokemonGiver != null && pokemonGiver.CanBeGiven())
            {
                yield return pokemonGiver.GivePokemon(initiator.GetComponent<PlayerController>());
                if (itemGiver != null && itemGiver.CanBeGiven())
                    yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;

                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(initiator.GetComponent<PlayerController>());
                    activeQuest = null;
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest(initiator.GetComponent<PlayerController>());
                    activeQuest = null;
                }
                else
                    yield return DialogManager.Instance.ShowDialog(activeQuest.Base.InProgressDialog);
            }
            else if (healer != null)
                yield return healer.HealPokemon(initiator);
            else if (clerk != null)
                yield return clerk.Trade();
            else
                yield return DialogManager.Instance.ShowDialog(dialog);
            
            idleTimer = 0f;
            state = NPCState.Idle;
        }
    }

    private void Update()
    {
        if (state == NPCState.Idle)
        {
            if (idleTimer > 0f)
                idleTimer -= Time.deltaTime;
            else
            {
                if (minX != 0 || minY != 0 || maxX != 0 || maxY != 0)
                {
                    StartCoroutine(Walk());
                    idleTimer = startingIdleTimer;
                }
            }
        }

        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPosition = transform.position;

        int randomDirection = UnityEngine.Random.Range(0, 4);

        while (((randomDirection == 0 && oldPosition.x <= startPosition.x - minX) || (randomDirection == 1 && oldPosition.y <= startPosition.y - minY) || (randomDirection == 2 && oldPosition.x >= startPosition.x + maxX) || (randomDirection == 3 && oldPosition.y >= startPosition.y + maxY)))
        {
            randomDirection = UnityEngine.Random.Range(0, 4);
        }

        if (randomDirection == 0) // Left
            yield return character.Move(new Vector2(-1, 0));
        else if (randomDirection == 1) // Down
            yield return character.Move(new Vector2(0, -1));
        else if (randomDirection == 2) // Right
            yield return character.Move(new Vector2(1, 0));
        else if (randomDirection == 3) // Up
            yield return character.Move(new Vector2(0, 1));

        state = NPCState.Idle;
    }

    public object CaptureState()
    {
        var saveData = new NPCQuestSaveData();
        saveData.activeQuest = activeQuest?.GetSaveData();
        
        if (questToStart != null)
            saveData.questToStart = (new Quest(questToStart)).GetSaveData();

        if (questToComplete != null)
            saveData.questToComplete = (new Quest(questToComplete)).GetSaveData();

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as NPCQuestSaveData;
        if (saveData != null)
        {
            activeQuest = (saveData.activeQuest != null)? new Quest(saveData.activeQuest) : null;
            questToStart = (saveData.questToStart != null) ? new Quest(saveData.questToStart).Base : null;
            questToComplete = (saveData.questToComplete != null) ? new Quest(saveData.questToComplete).Base : null;
        }
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => true;
}

[System.Serializable]
public class NPCQuestSaveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}

public enum NPCState { Idle, Walking, Dialog }