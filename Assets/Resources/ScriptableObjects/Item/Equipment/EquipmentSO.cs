using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "ScriptableObjects/Item/Equipment")]
public class EquipmentSO : ItemSO
{
    [SerializeField] private ItemSlots itemSlot;
    [SerializeField] private Sprite itemShow;
    [SerializeField] private Stats[] itemStats;
    [SerializeField] private bool canUpgrade;
    [SerializeField] private int itemStrength;
    [SerializeField] private Sprite itemStrengthImg;

    public EquipmentSO(string _itemID, ItemType _itemType, Sprite _itemIcon, string _itemName, RarityTier _itemTier, string _itemDescription, int _maxStackSize, ItemSlots itemSlot, Sprite itemShow, Stats[] itemStats, bool canUpgrade, int itemStrength, Sprite itemStrengthImg) : base(_itemID, _itemType, _itemIcon, _itemName, _itemTier, _itemDescription, _maxStackSize)
    {
        this.itemSlot = itemSlot;
        this.itemShow = itemShow;
        this.itemStats = itemStats;
        this.canUpgrade = canUpgrade;
        this.itemStrength = itemStrength;
        this.itemStrengthImg = itemStrengthImg;
    }

    public ItemSlots itemSlots_ => itemSlot;
    public Sprite itemShow_ => itemShow;
    public Stats[] itemStats_ => itemStats;
    public bool canUpgrade_ => canUpgrade;
    public int itemStrength_ => itemStrength;
    public Sprite itemStrengthImg_ => itemStrengthImg;
    void Awake()
    {
        itemType = ItemType.Equipment;
        canUpgrade = true;
    }

    public static EquipmentSO Init(string _itemID, ItemType _itemType, Sprite _itemIcon, string _itemName, RarityTier _itemTier, string _itemDescription, int _maxStackSize, ItemSlots _itemSlot, Sprite _itemShow, Stats[] _itemStats, bool _canUpgrade, int _itemStrength, Sprite _itemStrengthImg)
    {
        EquipmentSO newEquipment = ScriptableObject.CreateInstance<EquipmentSO>();
        newEquipment.itemID = _itemID;
        newEquipment.itemType = _itemType;
        newEquipment.itemName = _itemName;
        newEquipment.itemIcon = _itemIcon;
        newEquipment.itemStats = _itemStats;
        newEquipment.itemTier = _itemTier;
        newEquipment.itemDescription = _itemDescription;
        newEquipment.maxStackSize = _maxStackSize;
        newEquipment.itemSlot = _itemSlot;
        newEquipment.itemShow = _itemShow;
        newEquipment.canUpgrade = _canUpgrade;
        newEquipment.itemStrength = _itemStrength;
        newEquipment.itemStrengthImg = _itemStrengthImg;

        return newEquipment;
    }

    public EquipmentSO GetEquipmentSO()
    {
        return this;
    }

    public string GetRandomItemID()
    {
        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string randomID = "";
        for(int i = 0; i < 6; i++)
        {
            randomID += characters[UnityEngine.Random.Range(0, characters.Length)];
        }
        return randomID;
    }

    public void SetItemID(string _idItem)
    {
        itemID = _idItem;
    }

    public void SetItemIcon(Sprite _itemIcon)
    {
        itemIcon = _itemIcon;
    }

    public void SetTier(RarityTier _tier)
    {
        itemTier = _tier;
    }

    public void SetItemStrength(int _itemStrength)
    {
        itemStrength = _itemStrength;
    }

    public void SetItemStrengthImg(Sprite _itemStrengthImg)
    {
        itemStrengthImg = _itemStrengthImg;
    }

    public T RandomEnum<T>()
    {
        var enumValues = Enum.GetValues(typeof(T));
        return (T)enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));
    }

    public T RandomEnum<T>(int _min, int _max)
    {
        var enumValues = Enum.GetValues(typeof(T));
        return (T)enumValues.GetValue(UnityEngine.Random.Range(_min, _max));
    }

    public void RandomTier()
    {
        itemTier = RandomEnum<RarityTier>();
    }

    public void RandomTier(int _min, int _max)
    {
        itemTier = RandomEnum<RarityTier>(_min, _max);
    }

    public void RandomAttributes()
    {
        //for (int i = 0; i < itemStats.Length; i++)
        //{
        //    itemStats[i].attributes = RandomEnum<Attributes>();
        //}
    }

    public Stats[] RandomStats(RarityTier _itemTier)
    {
        switch (_itemTier)
        {
            case RarityTier.common:
                itemStats = new Stats[1];
                break;
            case RarityTier.uncommmon:
                itemStats = new Stats[2];
                break;
            case RarityTier.rare:
                itemStats = new Stats[2];
                break;
            case RarityTier.epic:
                itemStats = new Stats[3];
                break;
            case RarityTier.legendary:
                itemStats = new Stats[4];
                break;
            case RarityTier.mythic:
                itemStats = new Stats[4];
                break;
        }
        //Debug.Log(" itemStats.Length: " + itemStats.Length);
        for (int i = 0; i < itemStats.Length; i++)
        {
            itemStats[i] = new Stats(itemTier);
            itemStats[i].SetAttributes(RandomEnum<Attributes>());
        }
        return itemStats;
    }

    public string GetStats()
    {
        string stats = "";
        for (int i = 0; i < itemStats.Length; i++)
        {
            switch (itemStats[i].attributes_)
            {
                case Attributes.attack:
                    stats += "Tấn công: " + itemStats[i].valueBasic_ + (itemStats[i].valueBonusStrength_ > 0 ? " (+" + itemStats[i].valueBonusStrength_.ToString() +")": "") + " \n";
                    break;
                case Attributes.defense:
                    stats += "Phòng thủ: " + itemStats[i].valueBasic_ + (itemStats[i].valueBonusStrength_ > 0 ? " (+" + itemStats[i].valueBonusStrength_.ToString() + ")" : "") + " \n";
                    break;
                case Attributes.hp:
                    stats += "HP: " + itemStats[i].valueBasic_ + (itemStats[i].valueBonusStrength_ > 0 ? " (+" + itemStats[i].valueBonusStrength_.ToString() + ")" : "") + " \n";
                    break;
                case Attributes.luck:
                    stats += "May mắn: " + itemStats[i].valueBasic_ + (itemStats[i].valueBonusStrength_ > 0 ? " (+" + itemStats[i].valueBonusStrength_.ToString() + ")" : "") + " \n";
                    break;
            }
        }
        return stats;
    }

    public string GetStatsInfo()
    {
        string stats = "";
        for (int i = 0; i < itemStats.Length; i++)
        {
            stats += itemStats[i].attributes_ + ":" + itemStats[i].valueBasic_;
        }
        return stats;
    }
}

[System.Serializable]
public class Stats{
    #region
    [SerializeField] private Attributes attributes;
    [SerializeField] private int valueBasic;
    [SerializeField] private int valueBonusStrength;
    [SerializeField] private int valueBasicMin;
    [SerializeField] private int valueBasicMax;
    #endregion
    public Attributes attributes_ => attributes;
    public int valueBasic_ => valueBasic;
    public int valueBonusStrength_ => valueBonusStrength;
    public int valueBasicMin_ => valueBasicMin;
    public int valueBasicMax_ => valueBasicMax;
    public Stats(ItemSO.RarityTier _itemTier)
    {
        switch (_itemTier)
        {
            case ItemSO.RarityTier.common:
                valueBasicMin = 0; valueBasicMax = 100;
                break;
            case ItemSO.RarityTier.uncommmon:
                valueBasicMin = 101; valueBasicMax = 200;
                break;
            case ItemSO.RarityTier.rare:
                valueBasicMin = 201; valueBasicMax = 300;
                break;
            case ItemSO.RarityTier.epic:
                valueBasicMin = 301; valueBasicMax = 500;
                break;
            case ItemSO.RarityTier.legendary:
                valueBasicMin = 501; valueBasicMax = 700;
                break;
            case ItemSO.RarityTier.mythic:
                valueBasicMin = 701; valueBasicMax = 1000;
                break;
        }
        valueBasic = UnityEngine.Random.Range(valueBasicMin, valueBasicMax);
    }

    public void SetAttributes(Attributes _attributes)
    {
        attributes = _attributes;
    }

    public void SetValueBonusStrength(int _valueBonusStrength)
    {
        valueBonusStrength = _valueBonusStrength;
    }
}

public enum ItemSlots
{ 
    weapon, 
    offhand, 
    head, 
    hair, 
    glass, 
    face, 
    armlet, 
    ring, 
    cloth, 
    wing 
}

public enum Attributes
{
    attack,
    defense,
    hp,
    luck,
}
