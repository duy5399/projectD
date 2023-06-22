﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

public class CharacterEquipmentManager : BagManager
{
    #region Singleton
    public static CharacterEquipmentManager instance { get; private set; }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public Transform character;

    [SerializeField] private string savePathCharacterEquipment = "/characterEquipment.lsd";

    [SerializeField] private EquippedSlotController headSlot; //Bag - Player Equipment - Left - headSlot
    [SerializeField] private EquippedSlotController hairSlot; //Bag - Player Equipment - Left - hairSlot
    [SerializeField] private EquippedSlotController glassSlot; //Bag - Player Equipment - Left - glassSlot
    [SerializeField] private EquippedSlotController faceSlot; //Bag - Player Equipment - Left - faceSlot
    [SerializeField] private EquippedSlotController armletSlot; //Bag - Player Equipment - Right - armletSlot
    [SerializeField] private EquippedSlotController ringSlot; //Bag - Player Equipment - Right - ringSlot
    [SerializeField] private EquippedSlotController clothSlot; //Bag - Player Equipment - Right - clothSlot
    [SerializeField] private EquippedSlotController wingSlot; //Bag - Player Equipment - Center - wingSlot
    [SerializeField] private EquippedSlotController weaponSlot; //Bag - Player Equipment - Center - weaponSlot
    [SerializeField] private EquippedSlotController offhandSlot; //Bag - Player Equipment - Center - offhandSlot


    //equipment item for character
    public void EquipGear(EquipmentSO _item)
    {
        if (_item.itemType_ == ItemSO.ItemType.Equipment)
        {
            switch (_item.itemSlots_)
            {
                case ItemSlots.Head:
                    headSlot.EquipGear(_item);
                    break;
                case ItemSlots.Hair:
                    hairSlot.EquipGear(_item);
                    break;
                case ItemSlots.Glass:
                    glassSlot.EquipGear(_item);
                    break;
                case ItemSlots.Face:
                    faceSlot.EquipGear(_item);
                    break;
                case ItemSlots.Armlet:
                    armletSlot.EquipGear(_item);
                    break;
                case ItemSlots.Ring:
                    ringSlot.EquipGear(_item);
                    break;
                case ItemSlots.Cloth:
                    clothSlot.EquipGear(_item);
                    break;
                case ItemSlots.Wing:
                    wingSlot.EquipGear(_item);
                    break;
                case ItemSlots.Weapon:
                    weaponSlot.EquipGear(_item);
                    break;
                case ItemSlots.Offhand:
                    offhandSlot.EquipGear(_item);
                    break;
                default:
                    Debug.Log("cant equip gear");
                    break;
            }
        }
    }

    public void UpdateUICharacterEquipment()
    {
        //update ui list item in inventory
        for (int i = 0; i < inventorySO.listEquipmentSO_.equipments.Count; i++)
        {
            EquipmentSO equipment = EquipmentSO.Init(inventorySO.listEquipmentSO_.equipments[i].idItem_,
                inventorySO.listEquipmentSO_.equipments[i].type_,
                inventorySO.listEquipmentSO_.equipments[i].icon_,
                inventorySO.listEquipmentSO_.equipments[i].name_,
                inventorySO.listEquipmentSO_.equipments[i].tier_,
                inventorySO.listEquipmentSO_.equipments[i].description_,
                inventorySO.listEquipmentSO_.equipments[i].maxStack_,
                inventorySO.listEquipmentSO_.equipments[i].slot_, 
                inventorySO.listEquipmentSO_.equipments[i].show_, 
                inventorySO.listEquipmentSO_.equipments[i].stats_, 
                inventorySO.listEquipmentSO_.equipments[i].canUpgrade_,
                inventorySO.listEquipmentSO_.equipments[i].itemStrength_, 
                inventorySO.listEquipmentSO_.equipments[i].itemStrengthImg_);
            EquipGear(equipment);
        }
    }

    public void CloseButton()
    {
        //InventoryManager.instance.ResetParentInventory();
        
    }

    public void OnApplicationQuit()
    {
        inventorySO.listEquipmentSO_.equipments.Clear();
    }

    void Start()
    {
        //inventorySO.LoadEquipment(savePathCharacterEquipment);
        //UpdateUICharacterEquipment();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventorySO.SaveEquipment(savePathCharacterEquipment);
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventorySO.LoadEquipment(savePathCharacterEquipment);
            UpdateUICharacterEquipment();
        }
    }
}
