using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BerryType { None, Cheri, Chesto, Pecha, Rawst, Aspear, Leppa, Oran, Persim, Lum, Sitrus, Pomeg, Kelpsy, Qualot }

public class BerryTree : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] List<Sprite> berrySprites;
    [SerializeField] List<ItemBase> berryItemList;
    [SerializeField] BerryType startingBerry;
    
    BerryType plantedBerry;
    ItemBase berryItem;
    int growthTimeMinutes;
    int plantTime;
    int age;
    int minCount;
    int maxCount;

    public bool Picked { get; set; } = false;

    PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        plantedBerry = startingBerry;
        berryItem = DetermineBerryItem(plantedBerry);
        growthTimeMinutes = CalculateGrowthTime(plantedBerry);
        minCount = CalculateMinimumCount(plantedBerry);
        maxCount = CalculateMaximumCount(plantedBerry);
        age = 0;
        plantTime = -growthTimeMinutes * 60;

        StartCoroutine(HandleAnimation());
    }

    public ItemBase DetermineBerryItem(BerryType berry)
    {
        var berryItemID = berryItemList[0];
        if (berry == BerryType.Cheri)
            berryItemID = berryItemList[0];
        else if (berry == BerryType.Chesto)
            berryItemID = berryItemList[1];
        else if (berry == BerryType.Pecha)
            berryItemID = berryItemList[2];
        else if (berry == BerryType.Rawst)
            berryItemID = berryItemList[3];
        else if (berry == BerryType.Aspear)
            berryItemID = berryItemList[4];
        else if (berry == BerryType.Leppa)
            berryItemID = berryItemList[5];
        else if (berry == BerryType.Oran)
            berryItemID = berryItemList[6];
        else if (berry == BerryType.Persim)
            berryItemID = berryItemList[7];
        else if (berry == BerryType.Lum)
            berryItemID = berryItemList[8];
        else if (berry == BerryType.Sitrus)
            berryItemID = berryItemList[9];
        else if (berry == BerryType.Pomeg)
            berryItemID = berryItemList[10];
        else if (berry == BerryType.Kelpsy)
            berryItemID = berryItemList[11];
        else if (berry == BerryType.Qualot)
            berryItemID = berryItemList[12];

        return berryItemID;
    }

    public int CalculateGrowthTime(BerryType berry)
    {
        var growthTime = 0;
        if (berry == BerryType.Cheri)
            growthTime = 30;
        else if (berry == BerryType.Chesto)
            growthTime = 30;
        else if (berry == BerryType.Pecha)
            growthTime = 30;
        else if (berry == BerryType.Rawst)
            growthTime = 30;
        else if (berry == BerryType.Aspear)
            growthTime = 30;
        else if (berry == BerryType.Leppa)
            growthTime = 50;
        else if (berry == BerryType.Oran)
            growthTime = 30;
        else if (berry == BerryType.Persim)
            growthTime = 40;
        else if (berry == BerryType.Lum)
            growthTime = 60;
        else if (berry == BerryType.Sitrus)
            growthTime = 70;
        else if (berry == BerryType.Pomeg)
            growthTime = 30;
        else if (berry == BerryType.Kelpsy)
            growthTime = 45;
        else if (berry == BerryType.Qualot)
            growthTime = 60;

        return growthTime;
    }

    public int CalculateMinimumCount(BerryType berry)
    {
        var minimumCount = 3;
        if (berry == BerryType.Cheri)
            minimumCount = 3;
        else if (berry == BerryType.Chesto)
            minimumCount = 3;
        else if (berry == BerryType.Pecha)
            minimumCount = 3;
        else if (berry == BerryType.Rawst)
            minimumCount = 3;
        else if (berry == BerryType.Aspear)
            minimumCount = 3;
        else if (berry == BerryType.Leppa)
            minimumCount = 3;
        else if (berry == BerryType.Oran)
            minimumCount = 3;
        else if (berry == BerryType.Persim)
            minimumCount = 3;
        else if (berry == BerryType.Lum)
            minimumCount = 3;
        else if (berry == BerryType.Sitrus)
            minimumCount = 3;
        else if (berry == BerryType.Pomeg)
            minimumCount = 6;
        else if (berry == BerryType.Kelpsy)
            minimumCount = 6;
        else if (berry == BerryType.Qualot)
            minimumCount = 6;

        return minimumCount;
    }

    public int CalculateMaximumCount(BerryType berry)
    {
        var maximumCount = 4;
        if (berry == BerryType.Cheri)
            maximumCount = 8;
        else if (berry == BerryType.Chesto)
            maximumCount = 8;
        else if (berry == BerryType.Pecha)
            maximumCount = 8;
        else if (berry == BerryType.Rawst)
            maximumCount = 8;
        else if (berry == BerryType.Aspear)
            maximumCount = 8;
        else if (berry == BerryType.Leppa)
            maximumCount = 6;
        else if (berry == BerryType.Oran)
            maximumCount = 8;
        else if (berry == BerryType.Persim)
            maximumCount = 8;
        else if (berry == BerryType.Lum)
            maximumCount = 6;
        else if (berry == BerryType.Sitrus)
            maximumCount = 6;
        else if (berry == BerryType.Pomeg)
            maximumCount = 12;
        else if (berry == BerryType.Kelpsy)
            maximumCount = 12;
        else if (berry == BerryType.Qualot)
            maximumCount = 12;
        return maximumCount;
    }

    public IEnumerator HandleAnimation()
    {
        var berryTreeAnimation = GetComponent<SpriteRenderer>();
        while (!Picked)
        {
            if (plantedBerry == BerryType.None)
                GetComponent<SpriteRenderer>().enabled = false;
            else
            {
                GetComponent<SpriteRenderer>().enabled = true;
                int berryID = (int)plantedBerry;
                if (age >= 0 && age < growthTimeMinutes * 15)
                {
                    berryTreeAnimation.sprite = berrySprites[0];
                    yield return new WaitForSeconds(1f);
                }
                else if (age >= growthTimeMinutes * 15 && age < growthTimeMinutes * 30)
                {
                    berryTreeAnimation.sprite = berrySprites[1];
                    yield return new WaitForSeconds(0.5f);
                    berryTreeAnimation.sprite = berrySprites[2];
                    yield return new WaitForSeconds(0.5f);
                }
                else if (age >= growthTimeMinutes * 30 && age < growthTimeMinutes * 45)
                {
                    berryTreeAnimation.sprite = berrySprites[(berryID * 6) - 3];
                    yield return new WaitForSeconds(0.5f);
                    berryTreeAnimation.sprite = berrySprites[(berryID * 6) - 2];
                    yield return new WaitForSeconds(0.5f);
                }
                else if (age >= growthTimeMinutes * 45 && age < growthTimeMinutes * 60)
                {
                    berryTreeAnimation.sprite = berrySprites[(berryID * 6) - 1];
                    yield return new WaitForSeconds(0.5f);
                    berryTreeAnimation.sprite = berrySprites[(berryID * 6)];
                    yield return new WaitForSeconds(0.5f);
                }
                else if (age >= growthTimeMinutes)
                {
                    berryTreeAnimation.sprite = berrySprites[(berryID * 6) + 1];
                    yield return new WaitForSeconds(0.5f);
                    berryTreeAnimation.sprite = berrySprites[(berryID * 6) + 2];
                    yield return new WaitForSeconds(0.5f);
                }
            }
            CalculateAge();
        }
    }

    public void CalculateAge()
    {
        age = PlayTimeTimer.i.PlayTime - plantTime;
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (age >= 0 && age < growthTimeMinutes * 15)
            yield return DialogManager.Instance.ShowDialogText($"One {berryItem.ItemName} berry was planted here.");
        else if (age >= growthTimeMinutes * 15 && age < growthTimeMinutes * 30)
            yield return DialogManager.Instance.ShowDialogText($"{berryItem.ItemName} has sprouted.");
        else if (age >= growthTimeMinutes * 30 && age < growthTimeMinutes * 45)
            yield return DialogManager.Instance.ShowDialogText($"This {berryItem.ItemName} plant is growing taller.");
        else if (age >= growthTimeMinutes * 45 && age < growthTimeMinutes * 60)
            yield return DialogManager.Instance.ShowDialogText($"These {berryItem.ItemName} flowers are blooming prettily.");
        else if (age >= growthTimeMinutes * 60)
        {
            var count = UnityEngine.Random.Range(minCount, maxCount + 1);
            initiator.GetComponent<Inventory>().AddItem(berryItem, count);

            Picked = true;
            AudioManager.i.PlaySfx(AudioID.ItemObtained, pauseMusic: true);

            if (count == 1)
                yield return DialogManager.Instance.ShowDialogText($"playerName picked 2 {berryItem.ItemName} berries!");
            else
                yield return DialogManager.Instance.ShowDialogText($"playerName picked {count + 1} {berryItem.ItemName} berries!");

            yield return DialogManager.Instance.ShowDialogText($"The soil returned to its soft, earthy state.");
            yield return DialogManager.Instance.ShowDialogText($"playerName planted a new berry in its place.");

            plantTime = PlayTimeTimer.i.PlayTime;
            CalculateAge();
            Picked = false;
            StartCoroutine(HandleAnimation());
        }
    }

    public object CaptureState()
    {
        var saveData = new BerryTreeSaveData()
        {
            pickedBerry = Picked,
            berryPlanted = plantedBerry,
            growthInMinutes = growthTimeMinutes,
            ageInSeconds = age,
            plantedSeconds = plantTime,
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as BerryTreeSaveData;

        Picked = saveData.pickedBerry;
        plantedBerry = saveData.berryPlanted;
        growthTimeMinutes = saveData.growthInMinutes;
        age = saveData.ageInSeconds;
        plantTime = saveData.plantedSeconds;
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => false;
}

[System.Serializable]
public class BerryTreeSaveData
{
    public bool pickedBerry;
    public BerryType berryPlanted;
    public int growthInMinutes;
    public int ageInSeconds;
    public int plantedSeconds;
}