using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour, ISavable
{
    [SerializeField] ItemBase item;
    [SerializeField] int count = 1;
    [SerializeField] Dialog dialog;

    bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);

        player.GetComponent<Inventory>().AddItem(item, count);

        used = true;

        int category = (int)player.GetComponent<Inventory>().GetCategoryFromItem(item);

        if (category == 4)
            AudioManager.i.PlaySfx(AudioID.TMObtained, pauseMusic: true);
        else
            AudioManager.i.PlaySfx(AudioID.ItemObtained, pauseMusic: true);

        if (count == 1)
            yield return DialogManager.Instance.ShowDialogText($"playerName received 1 {item.ItemName}!");
        else
            yield return DialogManager.Instance.ShowDialogText($"playerName received {count} {item.ItemNamePlural}!");
    }

    public bool CanBeGiven()
    {
        return (item != null && !used && count > 0);
    }

    public object CaptureState()
    {
        var saveData = new ItemGiverSaveData()
        {
            gaveItem = used,
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (ItemGiverSaveData)state;
        used = saveData.gaveItem;
    }
}

[Serializable]
public class ItemGiverSaveData
{
    public bool gaveItem;
}