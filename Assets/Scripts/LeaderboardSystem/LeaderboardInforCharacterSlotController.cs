using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LeaderboardInforCharacterSlotController : InventorySlotController
{
    [SerializeField] private Animator anim;
    [SerializeField] private Transform tfDisplayItem;
    [SerializeField] private Sprite defaultDisplayImg;

    protected override void Awake()
    {
        base.Awake();
        if (tfDisplayItem != null)
        {
            anim = tfDisplayItem.gameObject.GetComponent<Animator>();
        }
    }

    public override void LoadInforItemSlot(ItemInstance itemInstance)
    {
        base.LoadInforItemSlot(itemInstance);
        LoadImgShow();
    }

    public void ResetInfoCharacterSlot()
    {
        this.itemInstance = null;
        imgIcon.sprite = null;
        imgIcon.enabled = false;
        imgBorder.enabled = false;
        LoadImgShow();
    }

    public void LoadImgShow()
    {
        if (itemInstance != null)
        {
            if (transform.name != "ArmletSlot" && transform.name != "RingSlot" && transform.name != "OffhandSlot")
            {
                if (transform.name == "WeaponSlot" || transform.name == "WingSlot")
                {
                    DisplayAnimation();
                }
                else
                {
                    var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic01"]);
                    tfDisplayItem.gameObject.GetComponent<Image>().sprite = ImageLoader.instance.imgShowDictionary_[inforBasic01["showImg"]];
                }
            }
        }
        else
        {
            if (transform.name != "ArmletSlot" && transform.name != "RingSlot" && transform.name != "OffhandSlot")
            {
                if (transform.name == "WeaponSlot" || transform.name == "WingSlot")
                {
                    DisplayAnimation();
                }
                tfDisplayItem.gameObject.GetComponent<Image>().sprite = defaultDisplayImg;
            }
        }
    }

    public void DisplayAnimation()
    {
        if (itemInstance != null)
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

    public override void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick"); 
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, null, null, null, null, null);
        }
    }
}