using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [SerializeField] protected string itemID;
    [SerializeField] protected ItemType itemType;
    [SerializeField] protected Sprite itemIcon;
    [SerializeField] protected string itemName;
    [SerializeField] protected RarityTier itemTier;
    [SerializeField] protected string itemDescription;
    [SerializeField] protected int maxStackSize;

    public string itemID_ => itemID;
    public ItemType itemType_ => itemType;
    public Sprite itemIcon_ => itemIcon;
    public string itemName_ => itemName;
    public RarityTier itemTier_ => itemTier;
    public string itemDescription_ => itemDescription;
    public int maxStackSize_ => maxStackSize;

    public ItemSO(string _itemID, ItemType _itemType, Sprite _itemIcon, string _itemName, RarityTier _itemTier, string _itemDescription, int _maxStackSize)
    {
        this.itemID = _itemID;
        this.itemType = _itemType;
        this.itemIcon = _itemIcon;
        this.itemName = _itemName;
        this.itemTier = _itemTier;
        this.itemDescription = _itemDescription;
        this.maxStackSize = _maxStackSize;
    }

    public enum ItemType
    {
        Equipment,
        Material
    }

    public enum RarityTier
    {
        common,
        uncommmon,
        rare,
        epic,
        legendary,
        mythic
    }
}

