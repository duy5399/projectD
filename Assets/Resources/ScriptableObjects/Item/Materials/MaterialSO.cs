using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Materials", menuName = "ScriptableObjects/Item/Materials")]
public class MaterialSO : ItemSO
{
    [SerializeField] private ItemUses itemUses;
    [SerializeField] private bool canCombine;

    public MaterialSO(string _itemID, ItemType _itemType, Sprite _itemIcon, string _itemName, RarityTier _itemTier, string _itemDescription, int _maxStackSize) : base(_itemID, _itemType, _itemIcon, _itemName, _itemTier, _itemDescription, _maxStackSize)
    {
    }

    public ItemUses itemUses_ => itemUses;
    public bool canCombine_ => canCombine;

    void Awake()
    {
        itemType = ItemType.Material;
    }

    public static MaterialSO Init(string _itemID, ItemType _itemType, Sprite _itemIcon, string _itemName, RarityTier _itemTier, string _itemDescription, int _maxStackSize, ItemUses _itemUses, bool _canCombine)
    {
        MaterialSO newMaterial = ScriptableObject.CreateInstance<MaterialSO>();
        newMaterial.itemID = _itemID;
        newMaterial.itemType = _itemType;
        newMaterial.itemName = _itemName;
        newMaterial.itemIcon = _itemIcon;
        newMaterial.itemName = _itemName;
        newMaterial.itemTier = _itemTier;
        newMaterial.itemDescription = _itemDescription;
        newMaterial.maxStackSize = _maxStackSize;
        newMaterial.itemUses = _itemUses;
        newMaterial.canCombine = _canCombine;

        return newMaterial;
    }
}

public enum ItemUses
{
    None,
    Upgrade,
    Enchant
}

