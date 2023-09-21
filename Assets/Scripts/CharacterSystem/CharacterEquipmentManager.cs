using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;

public class CharacterEquipmentManager : MonoBehaviour
{
    #region Singleton
    public static CharacterEquipmentManager instance { get; private set; }
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    #endregion

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

    [SerializeField] private TextMeshProUGUI attackStatTxt; //Bag - PlayerStats - ATK - Stat
    [SerializeField] private TextMeshProUGUI defenseStatTxt; //Bag - PlayerStats - DEF - Stat
    [SerializeField] private TextMeshProUGUI hitPointStatTxt; //Bag - PlayerStats - HP - Stat
    [SerializeField] private TextMeshProUGUI luckStatTxt; //Bag - PlayerStats - LUCK - Stat

    private void OnEnable()
    {
        //DisplayPlayerStats(intAttack, intDefense, intHitPoint, intLuck);
    }

    public void CloseButton()
    {
        //InventoryManager.instance.ResetParentInventory();
        
    }

    public void OnApplicationQuit()
    {
        //inventorySO.listEquipmentSO_.equipments.Clear();
    }



    //------------------------------------------------------------------------------------------------------
    public void EquipGear(ItemInstance itemInstance)
    {
        if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].ItemClass == ItemSO.ItemType.Equipment.ToString())
        {
            Debug.Log("PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].ItemClass: " + PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].ItemClass);
            if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.head.ToString()))
            {
                headSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.hair.ToString()))
            {
                hairSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.glass.ToString()))
            {
                glassSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.face.ToString()))
            {
                faceSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.armlet.ToString()))
            {
                armletSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.ring.ToString()))
            {
                ringSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.cloth.ToString()))
            {
                clothSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.wing.ToString()))
            {
                wingSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.weapon.ToString()))
            {
                weaponSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains(ItemSlots.offhand.ToString()))
            {
                offhandSlot.LoadInforEquipSlot(itemInstance);
                return;
            }
            else
            {
                Debug.Log("Lỗi khi trang bị item");
                return;
            }
        }
    }

    public void DisplayPlayerStats(int atk, int def, int hp, int luk)
    {
        Debug.Log("DisplayPlayerStats: " + atk + def + hp + luk);
        attackStatTxt.text = atk.ToString();
        defenseStatTxt.text = def.ToString();
        hitPointStatTxt.text = hp.ToString();
        luckStatTxt.text = luk.ToString();
    }
}
