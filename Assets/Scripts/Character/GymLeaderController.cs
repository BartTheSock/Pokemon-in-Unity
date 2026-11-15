using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GymLeaderController : TrainerController
{
    // State
    Character character;
    ItemGiver itemGiver;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
    }

    public override void Start()
    {

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
            if (questToCompleteBeforeBattle != null)
            {
                var quest = new Quest(questToCompleteBeforeBattle);
                yield return quest.CompleteQuest(initiator.GetComponent<PlayerController>());
                questToCompleteBeforeBattle = null;
            }

            if (questToCompleteAfterBattle != null)
            {
                activeQuest = new Quest(questToCompleteAfterBattle);
                yield return activeQuest.StartQuest();
            }

            yield return DialogManager.Instance.ShowDialog(dialog);
            GameController.Instance.StartGymLeaderBattle(this, trainerBattleTrigger);
        }
        else
            yield return DialogManager.Instance.ShowDialog(dialogAfterBattle);
    }

    public override IEnumerator LostBattle(Transform initiator)
    {
        battleLost = true;
        if (questToCompleteAfterBattle != null)
        {
            yield return activeQuest.CompleteQuest(initiator.GetComponent<PlayerController>());
            questToCompleteAfterBattle = null;

            if (itemGiver != null && itemGiver.CanBeGiven())
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
        }
        yield break;
    }

    new public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }
}