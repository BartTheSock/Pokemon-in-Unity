using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPickup : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] ItemBase item;
    [SerializeField] int count = 1;
    [SerializeField] List<Sprite> hiddenItemSprites;

    public bool Used { get; set; } = false;

    PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        StartCoroutine(HandleUpdate());
    }

    public IEnumerator HandleUpdate()
    {
        var twinkleAnimation = GetComponent<SpriteRenderer>();
        while (!Used)
        {
            twinkleAnimation.sprite = hiddenItemSprites[0];
            yield return new WaitForSeconds(0.2f);
            twinkleAnimation.sprite = hiddenItemSprites[1];
            yield return new WaitForSeconds(0.2f);
            twinkleAnimation.sprite = hiddenItemSprites[2];
            yield return new WaitForSeconds(0.2f);
            twinkleAnimation.sprite = hiddenItemSprites[1];
            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used && count > 0)
        {
            initiator.GetComponent<Inventory>().AddItem(item, count);
            Used = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            int category = (int)initiator.GetComponent<Inventory>().GetCategoryFromItem(item);

            if (category == 4)
                AudioManager.i.PlaySfx(AudioID.TMObtained, pauseMusic: true);
            else
                AudioManager.i.PlaySfx(AudioID.ItemObtained, pauseMusic: true);

            if (count == 1)
                yield return DialogManager.Instance.ShowDialogText($"playerName found 1 {item.ItemName}!");
            else
                yield return DialogManager.Instance.ShowDialogText($"playerName found {count} {item.ItemNamePlural}!");
        }
    }

    public object CaptureState()
    {
        var saveData = new HiddenPickupSaveData()
        {
            pickedUpHiddenPickup = Used,
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (HiddenPickupSaveData)state;
        Used = saveData.pickedUpHiddenPickup;

        if (Used)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public IEnumerator Move(Vector2 moveVector, bool checkCollisions)
    {
        yield break;
    }

    public bool IsMoving => false;
}

[Serializable]
public class HiddenPickupSaveData
{
    public bool pickedUpHiddenPickup;
}