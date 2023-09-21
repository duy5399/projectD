using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeSlotController : InventorySlotController
{
    [SerializeField] private bool slotInUse;

    public bool slotInUse_ => slotInUse;

    protected override void Awake()
    {
        base.Awake();
        imgBorder.enabled = false;
        imgIcon.enabled = false;
    }

    #region Display img item and quantity

    public override void LoadInforItemSlot(ItemInstance itemInstance)
    {
        if (slotInUse)
        {
            RemoveItem();
        }
        base.LoadInforItemSlot(itemInstance);
        txtQuantity.enabled = false;
        slotInUse = true;
    }

    public void ResetUpgradeSlot()
    {
        if(slotInUse)
        {
            imgBorder.enabled = false;
            imgIcon.sprite = null;
            imgIcon.enabled = false;
            slotInUse = false;
            itemInstance = null;
        }
    }

    public void RemoveItem()
    {
        UIManager.instance.inventoryDesciptionController_.HiddenDescription();
        InventoryManager.instance.AddItem(itemInstance);

        GameObject upgradeSystemInterface = GameObject.Find("UpgradeSystemInterface");

        if (upgradeSystemInterface != null && upgradeSystemInterface.activeInHierarchy)
        {
            GameObject upgradeInterface = GameObject.Find("UpgradeInterface");
            GameObject combineInterface = GameObject.Find("CombineInterface");
            if (upgradeInterface != null && upgradeInterface.activeInHierarchy)
            {
                if (itemInstance.ItemClass == "Equipment")
                    UpgradeEquipmentManager.instance.RemoveEquipment();
                else
                {
                    if (itemInstance.ItemId == "godCharm_01")
                        UpgradeEquipmentManager.instance.RemoveGodCharm();
                    else
                        UpgradeEquipmentManager.instance.RemoveStrengthStone(itemInstance);
                }
                ResetUpgradeSlot();
                UpgradeEquipmentManager.instance.DisplaySuccessRateaAndGoldRequired();
            }
            else if (combineInterface != null && combineInterface.activeInHierarchy)
            {
                CombineItemManager.instance.RemoveItem(itemInstance);
                CombineItemManager.instance.DisplayPreviewFinishedProduct();
                ResetUpgradeSlot();
            }
        }
    }
    #endregion

    public override void DisplayItemDescription()
    {
        //if(equipment != null)
        //{
        //    UIManager.instance.inventoryDesciptionController_.SetDescription(equipment);
        //}
        //else
        //{
        //    UIManager.instance.inventoryDesciptionController_.SetDescription(material);
        //}
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, null, null, null, () => { RemoveItem(); }, null);
        }
    }

    public override void OnLeftClick()
    {
        if (slotInUse)
        {
            DisplayItemDescription();
        }
    }

    public override void OnRightClick()
    {
        if (slotInUse)
        {
            //if (equipment != null)
            //{
            //    RemoveGearFromUpgradeSlot();
            //}
            //else
            //{
            //    RemoveMaterialFromUpgradeSlot();
            //}
        }
    }
}
