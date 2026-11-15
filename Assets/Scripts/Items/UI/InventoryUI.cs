using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using BTS.GenericSelectionUI;

public class InventoryUI : SelectionUI<TextSlot>
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Text itemCategoryText;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    int selectedCategory = 0;

    float selectionTimer = 0;

    const float selectionSpeed = 5;

    const int itemsInViewport = 8;

    List<ItemSlotUI> slotUIList;
    Inventory inventory;
    RectTransform itemListRect;

    private MainGameControls inventoryControls;
    private InputAction itemCategorySelect;
    protected override void Awake()
    {
        inventoryControls = new MainGameControls();
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
        base.Awake();
    }
    protected override void OnEnable()
    {
        itemCategorySelect = inventoryControls.ItemCategory.ItemCategory;
        itemCategorySelect.Enable();
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        itemCategorySelect.Disable();
        base.OnDisable();
    }

    private void Start()
    {
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObject = Instantiate(itemSlotUI, itemList.transform);
            slotUIObject.SetData(itemSlot);

            slotUIList.Add(slotUIObject);
        }

        ResetSelection();
        SetItems(slotUIList.Select(s => s.GetComponent<TextSlot>()).ToList());

        UpdateSelectionInUI();
    }

    public override void HandleUpdate()
    {
        UpdateSelectionTimer();
        int previousCategory = selectedCategory;

        float categorySelect = itemCategorySelect.ReadValue<float>();
        if (selectionTimer == 0 && Mathf.Abs(categorySelect) == 1f)
        {
            if (categorySelect == 1f)
                selectedCategory++;
            else if (categorySelect == -1f)
                selectedCategory--;
            selectionTimer = 1 / selectionSpeed;
        }

        if (selectedCategory > Inventory.ItemCategories.Count - 1)
            selectedCategory = 0;
        else if (selectedCategory < 0)
            selectedCategory = Inventory.ItemCategories.Count - 1;

        if (previousCategory != selectedCategory)
        {
            ResetSelection();
            itemCategoryText.text = Inventory.ItemCategories[selectedCategory];
            UpdateItemList();
        }

        base.HandleUpdate();
    }

    void UpdateSelectionTimer()
    {
        if (selectionTimer > 0)
            selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime, 0, selectionTimer);
    }

    public override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();

        var slots = inventory.GetSlotsByCategory(selectedCategory);

        if (slots.Count > 0)
        {
            itemIcon.gameObject.SetActive(true);

            var item = slots[selectedItem].Item;

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

        float scrollPosition = selectedItem * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPosition);

        bool showUpArrow = selectedItem > (itemsInViewport / 2);
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + (itemsInViewport / 2) < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    public ItemBase SelectedItem => inventory.GetItem(selectedItem, selectedCategory);

    public int SelectedCategory => selectedCategory;
}