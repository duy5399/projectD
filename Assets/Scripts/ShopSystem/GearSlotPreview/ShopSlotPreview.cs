using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlotPreview : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private StoreItem storeItem;
    [SerializeField] private Image imgIcon;
    [SerializeField] private Transform showPreviewItem;
    [SerializeField] private Animator anim;
    [SerializeField] private Sprite defaultDisplayImg;

    void Awake()
    {
        imgIcon = transform.GetChild(1).GetComponent<Image>();
    }

    public void PreviewIconAndShow(StoreItem storeItem)
    {
        this.storeItem = storeItem;
        var customdata_catalog = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].CustomData);
        var inforBasic01_catalog = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata_catalog["inforBasic01"]);
        imgIcon.sprite = ImageLoader.instance.imgIconDictionary_[inforBasic01_catalog["iconImg"].ToString()];
        imgIcon.enabled = true;
        showPreviewItem.GetComponent<Image>().sprite = ImageLoader.instance.imgShowDictionary_[inforBasic01_catalog["showImg"]];

        if (PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].Tags[0].ToString() == "wing")
        {
            string[] parameter = storeItem.ItemId.Split(new char[] { '_' });
            anim = showPreviewItem.GetComponent<Animator>();
            anim.SetInteger("Wing", int.Parse(parameter[1]));
            anim.enabled = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            DisplayItemDescription();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //ResetGearSlotPreview();
        }
    }

    public void DisplayItemDescription()
    {
        UIManager.instance.inventoryDesciptionController_.DescriptionItem(storeItem);
    }

    //reset lại hình ảnh ở slot item và hiển thị hình ảnh xem trước
    public void ResetGearSlotPreview()
    {
        if(storeItem != null)
        {
            imgIcon.sprite = null;
            imgIcon.enabled = false;
            showPreviewItem.GetComponent<Image>().sprite = defaultDisplayImg;

            if (transform.name == "wingSlot" && anim != null)
            {
                anim.SetInteger("Wing", 0);
                anim.enabled = false;
            }
            storeItem = null;
        }
        else
        {
            Debug.Log("storeItem == null");
        }
    }
}
