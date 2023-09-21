using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardRewardSlotController : InventorySlotController
{
    [SerializeField] private CatalogItem catalogItem;
    protected override void Awake()
    {
        LoadComponents();
    }

    public virtual void LoadInforItemSlot(CatalogItem catalogItem, string quantity)
    {
        this.catalogItem = catalogItem;
        var customData = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
        var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customData["inforBasic01"]);
        var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customData["inforBasic02"]);
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

        switch (inforBasic02["tier"])
        {
            case "common":
                imgBorder.color = new Color32(209, 213, 216, 255);
                imgBorder.enabled = true;
                break;
            case "uncommon":
                imgBorder.color = new Color32(65, 168, 95, 255);
                imgBorder.enabled = true;
                break;
            case "rare":
                imgBorder.color = new Color32(44, 130, 201, 255);
                imgBorder.enabled = true;
                break;
            case "epic":
                imgBorder.color = new Color32(147, 101, 184, 255);
                imgBorder.enabled = true;
                break;
            case "legendary":
                imgBorder.color = new Color32(250, 197, 28, 255);
                imgBorder.enabled = true;
                break;
            case "mythic":
                imgBorder.color = new Color32(226, 80, 65, 255);
                imgBorder.enabled = true;
                break;
            default:
                Debug.Log("Not found rarity tier of item: " + catalogItem.ItemId);
                break;
        }
        txtQuantity.text = "x" + string.Format("{0:0,0}", int.Parse(quantity));
    }
}
