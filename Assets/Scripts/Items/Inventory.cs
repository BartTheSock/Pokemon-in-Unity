using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { RecoveryItems, PokeBalls, BattleItems, EvolutionItems, TMs, Berries, Treasures, OtherItems, KeyItems, GymBadges }

public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> recoveryItemSlots;
    [SerializeField] List<ItemSlot> pokeBallSlots;
    [SerializeField] List<ItemSlot> battleItemSlots;
    [SerializeField] List<ItemSlot> evolutionItemSlots;
    [SerializeField] List<ItemSlot> tmSlots;
    [SerializeField] List<ItemSlot> berrySlots;
    [SerializeField] List<ItemSlot> treasureSlots;
    [SerializeField] List<ItemSlot> otherItemSlots;
    [SerializeField] List<ItemSlot> keyItemSlots;
    [SerializeField] List<ItemSlot> gymBadgeSlots;

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    public static Inventory i { get; private set; }

    private void Awake()
    {
        i = this;
        allSlots = new List<List<ItemSlot>>() { recoveryItemSlots, pokeBallSlots, battleItemSlots, evolutionItemSlots, tmSlots, berrySlots, treasureSlots, otherItemSlots, keyItemSlots, gymBadgeSlots };
    }

    public void ClearInventory()
    {
        recoveryItemSlots.Clear();
        pokeBallSlots.Clear();
        battleItemSlots.Clear();
        evolutionItemSlots.Clear();
        tmSlots.Clear();
        berrySlots.Clear();
        treasureSlots.Clear();
        otherItemSlots.Clear();
        keyItemSlots.Clear();
        gymBadgeSlots.Clear();
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "Medicine",
        "Poké Balls",
        "Battle Items",
        "Evolution",
        "TMs & HMs",
        "Berries",
        "Treasures",
        "Other Items",
        "Key Items",
        "Gym Badges"
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[itemIndex].Item;
    }

    public ItemBase UseItem(int itemIndex, Pokemon selectedPokemon, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        return UseItem(item, selectedPokemon);
    }

    public ItemBase UseItem(ItemBase item, Pokemon selectedPokemon)
    {
        bool itemUsed = item.Use(selectedPokemon);

        if (itemUsed)
        {
            if (!item.IsReusable)
                RemoveItem(item);

            return item;
        }

        return null;
    }

    public void AddItem(ItemBase item, int count = 1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if (itemSlot != null)
            itemSlot.Count += count;
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        OnUpdated?.Invoke();
    }

    public int GetItemCount(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);

        if (itemSlot != null)
            return itemSlot.Count;
        else
            return 0;
    }

    public void RemoveItem(ItemBase item, int countToRemove = 1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count -= countToRemove;
        if (itemSlot.Count == 0)
            currentSlots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

    public bool HasItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        return currentSlots.Exists(slot => slot.Item == item);
    }

    public ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.RecoveryItems;
        else if (item is PokeBallItem)
            return ItemCategory.PokeBalls;
        else if (item is BattleItem)
            return ItemCategory.BattleItems;
        else if (item is EvolutionItem)
            return ItemCategory.EvolutionItems;
        else if (item is TMItem)
            return ItemCategory.TMs;
        else if (item is BerryItem)
            return ItemCategory.Berries;
        else if (item is TreasureItem)
            return ItemCategory.Treasures;
        else if (item is KeyItem)
            return ItemCategory.KeyItems;
        else if (item is GymBadge)
            return ItemCategory.GymBadges;
        else
            return ItemCategory.OtherItems;
    }

    public static Inventory GetInventory()
    {
        return FindFirstObjectByType<PlayerController>(FindObjectsInactive.Exclude).GetComponent<Inventory>();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            recoveryItems = recoveryItemSlots.Select(i => i.GetSaveData()).ToList(),
            pokeBalls = pokeBallSlots.Select(i => i.GetSaveData()).ToList(),
            battleItems = battleItemSlots.Select(i => i.GetSaveData()).ToList(),
            evolutionItems = evolutionItemSlots.Select(i => i.GetSaveData()).ToList(),
            tms = tmSlots.Select(i => i.GetSaveData()).ToList(),
            berries = berrySlots.Select(i => i.GetSaveData()).ToList(),
            treasureItems = treasureSlots.Select(i => i.GetSaveData()).ToList(),
            otherItems = otherItemSlots.Select(i => i.GetSaveData()).ToList(),
            keyItems = keyItemSlots.Select(i => i.GetSaveData()).ToList(),
            gymBadges = gymBadgeSlots.Select(i => i.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        recoveryItemSlots = saveData.recoveryItems.Select(i => new ItemSlot(i)).ToList();
        pokeBallSlots = saveData.pokeBalls.Select(i => new ItemSlot(i)).ToList();
        battleItemSlots = saveData.battleItems.Select(i => new ItemSlot(i)).ToList();
        evolutionItemSlots = saveData.evolutionItems.Select(i => new ItemSlot(i)).ToList();
        tmSlots = saveData.tms.Select(i => new ItemSlot(i)).ToList();
        berrySlots = saveData.berries.Select(i => new ItemSlot(i)).ToList();
        treasureSlots = saveData.treasureItems.Select(i => new ItemSlot(i)).ToList();
        otherItemSlots = saveData.otherItems.Select(i => new ItemSlot(i)).ToList();
        keyItemSlots = saveData.keyItems.Select(i => new ItemSlot(i)).ToList();
        gymBadgeSlots = saveData.gymBadges.Select(i => new ItemSlot(i)).ToList();

        allSlots = new List<List<ItemSlot>>() { recoveryItemSlots, pokeBallSlots, battleItemSlots, evolutionItemSlots, tmSlots, berrySlots, treasureSlots, otherItemSlots, keyItemSlots, gymBadgeSlots };

        OnUpdated?.Invoke();
    }
}

[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSlot()
    {
        
    }

    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.itemName);
        count = saveData.count;
    }

    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            itemName = item.name,
            count = count
        };

        return saveData;
    }

    public ItemBase Item { get => item; set => item = value; }
    public int Count { get => count; set => count = value; }
}

[Serializable]
public class ItemSaveData
{
    public string itemName;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> recoveryItems;
    public List<ItemSaveData> pokeBalls;
    public List<ItemSaveData> battleItems;
    public List<ItemSaveData> evolutionItems;
    public List<ItemSaveData> tms;
    public List<ItemSaveData> berries;
    public List<ItemSaveData> treasureItems;
    public List<ItemSaveData> otherItems;
    public List<ItemSaveData> keyItems;
    public List<ItemSaveData> gymBadges;
}