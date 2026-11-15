using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] Text itemNameText;
    [SerializeField] Text itemCountText;

    RectTransform rectTransform;

    public Text ItemNameText => itemNameText;

    public Text ItemCountText => itemCountText;

    public float Height => rectTransform.rect.height;

    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        itemNameText.text = itemSlot.Item.ItemName;
        itemCountText.text = $"x {itemSlot.Count}";
    }

    public void SetNameAndPrice(ItemBase item)
    {
        rectTransform = GetComponent<RectTransform>();
        itemNameText.text = item.ItemName;
        itemCountText.text = $"$ {item.Price}";
    }
}