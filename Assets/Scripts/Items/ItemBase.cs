using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] string itemNamePlural;

    [TextArea]
    [SerializeField] string itemDescription;

    [TextArea]
    [SerializeField] string itemUseMessage;

    [SerializeField] Sprite itemIcon;

    [SerializeField] bool isSellable;
    [SerializeField] int price;

    public virtual string ItemName => itemName;
    public virtual string ItemNamePlural => itemNamePlural;

    public virtual string ItemDescription => itemDescription;

    public virtual string ItemUseMessage => itemUseMessage;

    public Sprite ItemIcon => itemIcon;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

    public virtual bool IsReusable => false;

    public virtual bool CanUseInBattle => true;

    public virtual bool CanUseOutsideBattle => true;

    public virtual bool IsSellable => isSellable;

    public virtual int Price => price;
}