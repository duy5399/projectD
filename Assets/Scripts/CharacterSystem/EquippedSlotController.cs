using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemSO;

public class EquippedSlotController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Animator anim;
    [SerializeField] private EquipmentSO itemEquipped;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBorder;
    [SerializeField] private Text quantity;
    [SerializeField] private Transform playerDisplayImg;
    [SerializeField] private Sprite defaultDisplayImg;
    [SerializeField] private bool slotInUse;

    public EquipmentSO itemEquipped_ => itemEquipped;

    void Awake()
    {
        imgBorder = transform.GetChild(1).GetComponent<Image>();
        imgBorder.enabled = false;
        imgIcon = transform.GetChild(2).GetComponent<Image>();
        imgIcon.enabled = false;
        slotInUse = false;
        itemInstance = null;
        if (playerDisplayImg != null)
        {
            anim = playerDisplayImg.gameObject.GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        DisplayAnimation();
    }

    //display description of item
    public void DisplayItemDescription()
    {
        UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, () => { UnequipItem(); }, null, null, null, null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (slotInUse)
        {
            //Debug.Log("EquippedSlot clickkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk");
            DisplayItemDescription();
            AudioManager.instance.ClickSuccessSFX();
        }
    }

    public void OnRightClick()
    {
        if(slotInUse)
        {
            AudioManager.instance.UnequipGearSFX();
        }
        //Debug.Log("EquippedSlot clickzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
    }

    //-------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private ItemInstance itemInstance;

    [SerializeField] private Image imgIcon;
    [SerializeField] private Image imgBorder;
    [SerializeField] private Transform tfDisplayItem;

    public async void LoadInforEquipSlot(ItemInstance itemInstance)
    {
        //if this slot is equipped, unequip and sent it to inventory before equip new gear
        if (slotInUse)
        {
            UnequipItem();
        }
        await Task.Delay(1000);
        this.itemInstance = itemInstance;
        //đặt slot thành trạng thái "đã sử dụng"
        slotInUse = true;

        var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic01"]);
        var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic02"]);
        //set icon cho equip slot
        try
        {
            imgIcon.sprite = ImageLoader.instance.imgIconDictionary_[inforBasic01["iconImg"]];
            imgIcon.enabled = true;
        }
        catch
        {
            imgIcon.sprite = null;
            imgIcon.enabled = false;
        }

        //set border cho equip slot
        switch (inforBasic02["tier"])
        {
            case "common":
                imgBorder.color = new Color32(209, 213, 216, 255);
                break;
            case "uncommon":
                imgBorder.color = new Color32(65, 168, 95, 255);
                break;
            case "rare":
                imgBorder.color = new Color32(44, 130, 201, 255);
                break;
            case "epic":
                imgBorder.color = new Color32(147, 101, 184, 255);
                break;
            case "legendary":
                imgBorder.color = new Color32(250, 197, 28, 255);
                break;
            case "mythic":
                imgBorder.color = new Color32(226, 80, 65, 255);
                break;
            default:
                Debug.Log("Not found rarity tier of item: " + itemInstance.ItemId);
                break;
        }
        imgBorder.enabled = true;

        //set ảnh (show) cho nhân vật
        if (itemInstance != null && slotInUse)
        {
            if (!PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains("armlet") && !PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains("ring") && !PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains("offhand"))
            {
                Debug.Log("PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags: " + PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.ToArray());
                if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains("weapon") || PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags.Contains("wing"))
                {
                    DisplayAnimation();
                }
                else
                {
                    tfDisplayItem.gameObject.GetComponent<Image>().sprite = ImageLoader.instance.imgShowDictionary_[inforBasic01["showImg"]];
                }
            }
        }

        //UpdatePlayerStats();
        //DisplayPlayerStats();
        //Debug.Log("EquipGear successful => UpdateUIInventory ");
        //InventoryManager.instance.UpdateUIInventory();
    }

    public void UnequipItem()
    {
        AsyncLoadingScene.instance.LoadingScreen(true);
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "ChangeEquipStatusOfItem",
            FunctionParameter = new { itemInstanceId = itemInstance.ItemInstanceId }
        };
        Debug.Log("unequip  itemInstance.ItemInstanceId: " + itemInstance.ItemInstanceId);
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("Gỡ trang bị thành công: " + result.FunctionResult);
                GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
                player.GetComponent<PlayerStats>().GetPlayerStat();
                var playerStats = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, int>>(result.FunctionResult.ToString());
                CharacterEquipmentManager.instance.DisplayPlayerStats(playerStats["atk"], playerStats["def"], playerStats["hp"], playerStats["luk"]);
                slotInUse = false;

                //set icon cho equip slot
                imgIcon.sprite = null;
                imgIcon.enabled = false;

                //set border cho equip slot
                imgBorder.enabled = false;
                
                //set ảnh (show) cho nhân vật
                if (transform.name != "ArmletSlot" && transform.name != "RingSlot" && transform.name != "OffhandSlot")
                {
                    if (transform.name == "WeaponSlot" || transform.name == "WingSlot")
                    {
                        DisplayAnimation();
                    }
                    playerDisplayImg.gameObject.GetComponent<Image>().sprite = defaultDisplayImg;
                }
                itemInstance = null;
                //PlayerStats.instance.GetPlayerStat();
                UIManager.instance.inventoryDesciptionController_.HiddenDescription();
                InventoryManager.instance.GetInventoryUser();
            },
            error =>
            {
                Debug.Log("Bán vật phẩm thất bại!");
            });
        AsyncLoadingScene.instance.LoadingScreen(false);
    }

    //display aniamtion of item (Wing, Face, ...)
    public void DisplayAnimation()
    {
        if (slotInUse)
        {
            if (transform.name == "WeaponSlot")
            {
                var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforUpgrade"]);
                int strenghLv = int.Parse(inforUpgrade["strenghLv"].ToString());
                //Debug.Log("item.itemStrength: " + itemEquipped.itemStrength_);
                anim.SetInteger("WeaponStrength", strenghLv);
                anim.enabled = true;
            }
            else if (transform.name == "WingSlot")
            {
                //Debug.Log("item.itemID wing: " + itemEquipped.itemID_);
                string[] parameter = itemInstance.ItemId.Split(new char[] { '_' });
                anim.SetInteger("Wing", int.Parse(parameter[1]));
                anim.enabled = true;
            }
        }
        else
        {
            if (transform.name == "WeaponSlot")
            {
                anim.SetInteger("WeaponStrength", 0);
                anim.enabled = false;
            }
            else if (transform.name == "WingSlot")  
            {
                anim.SetInteger("Wing", 0);
                anim.enabled = false;
            }
        }
    }
    //void FixedUpdate()
    //{
    //    if (itemEquipped != null && slotInUse)
    //    {
    //        if (itemEquipped.itemSlots_ != ItemSlots.Armlet && itemEquipped.itemSlots_ != ItemSlots.Ring)
    //        {
    //            if (itemEquipped.itemSlots_ == ItemSlots.Weapon || itemEquipped.itemSlots_ == ItemSlots.Wing)
    //            {
    //                DisplayAnimation();
    //            }
    //            else
    //            {
    //                playerDisplayImg.gameObject.GetComponent<Image>().sprite = itemEquipped.itemShow_;
    //            }
    //        }
    //    }
    //}
}
