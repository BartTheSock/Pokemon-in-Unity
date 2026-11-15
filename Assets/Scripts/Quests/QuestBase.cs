using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Create new quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] string questName;
    [SerializeField] string questDescription;

    [SerializeField] Dialog startDialog;
    [SerializeField] Dialog inProgressDialog;
    [SerializeField] Dialog completedDialog;

    [SerializeField] ItemBase requiredItem;
    [SerializeField] ItemBase rewardItem;
    [SerializeField] int rewardCount;
    [SerializeField] bool increasesLevelCap;
    [SerializeField] int newLevelCap;

    public string QuestName => questName;
    public string QuestDescription => questDescription;

    public Dialog StartDialog => startDialog;
    public Dialog InProgressDialog => inProgressDialog?.Lines?.Count > 0 ? inProgressDialog : startDialog;
    public Dialog CompletedDialog => completedDialog;

    public ItemBase RequiredItem => requiredItem;
    public ItemBase RewardItem => rewardItem;
    public int RewardCount => rewardCount;
    public bool IncreasesLevelCap => increasesLevelCap;
    public int NewLevelCap => newLevelCap;
}