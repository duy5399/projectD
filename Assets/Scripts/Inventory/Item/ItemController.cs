using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemController : MonoBehaviour
{
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private EquipmentSO equipment;
    [SerializeField] private MaterialSO material;

    void Awake()
    {
        //load Scriptable Objects cho gameobject
        string[] itemFolder = transform.name.ToString().Split(new char[] { '_' });
        string resourcePath = "ScriptableObjects/Item/" + transform.tag + "/" + itemFolder[0] + "/" + transform.name;
        itemSO = Resources.Load<ItemSO>(resourcePath);

        sr = GetComponent<SpriteRenderer>();
        sr.sprite = itemSO.itemIcon_;
    }

    public void PickUp()
    {
        if (itemSO == null)
        {
            return;
        }
        else
        {
            // tạo bản sao ScriptableObjects để truyền đi - tránh sửa đổi file ScriptableObjects gốc
            if (itemSO.itemType_ == ItemSO.ItemType.Equipment)
            {
                equipment = ScriptableObject.Instantiate((EquipmentSO)itemSO);
                equipment.RandomTier();
                equipment.RandomStats(equipment.itemTier_);
                equipment.SetItemID(equipment.itemID_ + "_" + equipment.itemTier_ + "_" + equipment.GetStatsInfo() + "_" + equipment.GetRandomItemID());
                material = null;
            }
            else if (itemSO.itemType_ == ItemSO.ItemType.Material)
            {
                material = ScriptableObject.Instantiate((MaterialSO)itemSO);
                equipment = null;
            }
            //if (itemSO.itemType_ == ItemSO.ItemType.Equipment)
            //{
            //    EquipmentSO equipment_ = (EquipmentSO)itemSO;
            //    equipment = EquipmentSO.Init(equipment_.itemID_, equipment_.itemType_, equipment_.itemIcon_, equipment_.itemName_, equipment_.itemTier_, equipment_.itemDescription_, equipment_.maxStackSize_, equipment_.itemSlots_, equipment_.itemShow_, equipment_.itemStats_, equipment_.canUpgrade_, equipment_.itemStrength_, equipment_.itemStrengthImg_);
            //    equipment.RandomStats();
            //    material = null;
            //}
            //else if (itemSO.itemType_ == ItemSO.ItemType.Material)
            //{
            //    MaterialSO material_ = (MaterialSO)itemSO;
            //    material = MaterialSO.Init(material_.itemID_, material_.itemType_, material_.itemIcon_, material_.itemName_, material_.itemTier_, material_.itemDescription_, material_.maxStackSize_, material_.itemUses_, material_.canCombine_);
            //    equipment = null;
            //}
        }
    }

    private void OnMouseDown()
    {
        PickUp();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PickUp();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            PickUp();
        }
    }
}
