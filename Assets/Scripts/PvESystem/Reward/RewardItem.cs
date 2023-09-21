using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ItemSO;

public class RewardItem : InventorySlotController
{
    protected override void Awake()
    {
        base.Awake();
    }

    //thêm thông tin vật phẩm cho object RewardItem này
    public void LoadInforItemSlot(string iconName, string tier, string quantity)
    {
        imgIcon.sprite = ImageLoader.instance.imgIconDictionary_[iconName];
        txtQuantity.text = quantity;

        switch (tier)
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
                //Debug.Log("Not found rarity tier of item: " + _item.itemName_);
                break;
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnLeftClick();
        }
    }

}
