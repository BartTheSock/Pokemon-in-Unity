using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestBase Base { get; private set; }
    public QuestStatus Status { get; private set; }
    
    public Quest(QuestBase _base)
    {
        Base = _base;
    }

    public Quest(QuestSaveData saveData)
    {
        Base = QuestDB.GetObjectByName(saveData.questName);
        Status = saveData.questStatus;
    }

    public QuestSaveData GetSaveData()
    {
        var saveData = new QuestSaveData()
        {
            questName = Base.name,
            questStatus = Status
        };
        return saveData;
    }

    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;
        yield return DialogManager.Instance.ShowDialog(Base.StartDialog);

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    }

    public IEnumerator CompleteQuest(PlayerController player)
    {
        Status = QuestStatus.Completed;
        yield return DialogManager.Instance.ShowDialog(Base.CompletedDialog);

        var inventory = Inventory.GetInventory();
        
        if (Base.RequiredItem != null)
            inventory.RemoveItem(Base.RequiredItem);

        if (Base.RewardItem != null)
        {
            int category = (int)player.GetComponent<Inventory>().GetCategoryFromItem(Base.RewardItem);

            if (category == 4)
                AudioManager.i.PlaySfx(AudioID.TMObtained, pauseMusic: true);
            else
                AudioManager.i.PlaySfx(AudioID.ItemObtained, pauseMusic: true);

            inventory.AddItem(Base.RewardItem, Base.RewardCount);
            if (Base.RewardCount == 1)
                yield return DialogManager.Instance.ShowDialogText($"playerName recieved 1 {Base.RewardItem.ItemName}!");
            else
                yield return DialogManager.Instance.ShowDialogText($"playerName recieved {Base.RewardCount} {Base.RewardItem.ItemNamePlural}!");
        }

        if (Base.IncreasesLevelCap)
        {
            if (Base.NewLevelCap > player.pokemonLevelCap)
            {
                player.pokemonLevelCap = Base.NewLevelCap;
                yield return DialogManager.Instance.ShowDialogText($"The maximum level a Pokemon can be is now {Base.NewLevelCap}!");
            }
        }

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    }

    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            if (!inventory.HasItem(Base.RequiredItem))
                return false;
        }

        return true;
    }
}

[System.Serializable]
public class QuestSaveData
{
    public string questName;
    public QuestStatus questStatus;
}

public enum QuestStatus { None, Started, Completed }