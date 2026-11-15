using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] Color highlightedColor;

    int selectedItem;

    List<ItemBase> availableItems;

    Action<ItemBase> onItemSelected;
    Action onBack;

    List<ItemSlotUI> slotUIList;

    const int itemsInViewport = 8;
    RectTransform itemListRect;
    
    float selectTimer = 0;
    const float selectSpeed = 5;

    private MainGameControls gameControls;
    private InputAction shopSelector;
    private InputAction confirmAction;
    private InputAction cancelAction;
    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
        gameControls = new MainGameControls();
    }
    private void OnEnable()
    {
        shopSelector = gameControls.ShopUI.Select;
        confirmAction = gameControls.ShopUI.Confirm;
        cancelAction = gameControls.ShopUI.Cancel;
        shopSelector.Enable();
        confirmAction.Enable();
        cancelAction.Enable();
    }

    private void OnDisable()
    {
        shopSelector.Disable();
        confirmAction.Disable();
        cancelAction.Disable();
    }

    public void Show(List<ItemBase> availableItems, Action<ItemBase> onItemSelected,
        Action onBack)
    {
        this.availableItems = availableItems;
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;

        gameObject.SetActive(true);

        UpdateItemList();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void HandleUpdate()
    {
        UpdateSelectTimer();
        var previousSelection = selectedItem;

        float select = shopSelector.ReadValue<float>();

        if (selectTimer == 0 && Mathf.Abs(select) == 1f)
        {
            if (select == 1f)
                selectedItem--;
            else if (select == -1f)
                selectedItem++;
            selectTimer = 1 / selectSpeed;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        if (previousSelection != selectedItem)
            UpdateItemSelection();

        if (confirmAction.WasPressedThisFrame())
            onItemSelected?.Invoke(availableItems[selectedItem]);
        else if (cancelAction.WasPressedThisFrame())
            onBack?.Invoke();
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var item in availableItems)
        {
            var slotUIObject = Instantiate(itemSlotUI, itemList.transform);
            slotUIObject.SetNameAndPrice(item);

            slotUIList.Add(slotUIObject);
        }

        UpdateItemSelection();
    }

    void UpdateItemSelection()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].ItemNameText.color = highlightedColor;
                slotUIList[i].ItemCountText.color = highlightedColor;
            }
            else
            {
                slotUIList[i].ItemNameText.color = Color.black;
                slotUIList[i].ItemCountText.color = Color.black;
            }
        }

        if (availableItems.Count > 0)
        {
            itemIcon.gameObject.SetActive(true);

            var item = availableItems[selectedItem];

            itemIcon.sprite = item.ItemIcon;
            itemDescription.text = item.ItemDescription;
        }
        else
        {
            itemIcon.gameObject.SetActive(false);
            itemDescription.text = "";
        }

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport) return;

        float scrollPosition = Mathf.Clamp(selectedItem - (itemsInViewport / 2), 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPosition);

        bool showUpArrow = selectedItem > (itemsInViewport / 2);
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + (itemsInViewport / 2) < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    void UpdateSelectTimer()
    {
        if (selectTimer > 0)
            selectTimer = Mathf.Clamp(selectTimer - Time.deltaTime, 0, selectTimer);
    }
}