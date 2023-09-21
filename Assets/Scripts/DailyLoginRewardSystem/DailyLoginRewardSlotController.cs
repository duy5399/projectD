using DG.Tweening;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DailyLoginRewardSlotController : InventorySlotController
{
    [SerializeField] private CatalogItem catalogItem;
    [SerializeField] private Image imgMark;
    [SerializeField] private Sprite sprRightMark;
    [SerializeField] private Sprite sprWrongMark;
    [SerializeField] private string quantity;

    public Image imgBorder_ => imgBorder;
    public Image imgMark_ => imgMark;
    protected override void Awake()
    {
        base.Awake();
        imgMark = transform.GetChild(3).GetComponent<Image>();
    }

    public virtual void LoadInforItemSlot(CatalogItem catalogItem, string quantity)
    {
        this.catalogItem = catalogItem;
        this.quantity = quantity;
        var customData = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
        var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customData["inforBasic01"]);
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
        imgBorder.enabled = false;
        imgMark.enabled = false;
        txtQuantity.text = catalogItem.DisplayName + " x" + string.Format("{0:0,0}", int.Parse(quantity));
    }

    public void OnBorder(bool boolean)
    {
        if (boolean)
        {
            imgBorder.enabled = true;
            imgBorder.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, -360), 1f,RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
        else
            imgBorder.enabled = false;
    }

    public void OnMark(bool boolean)
    {
        if (boolean)
            imgMark.sprite = sprRightMark;
        else
            imgMark.sprite = sprWrongMark;
        imgMark.enabled = true;
    }

    public void GetReward()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "CheckIn"
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("CheckIn: " + result.FunctionResult);
                if(result.FunctionResult.ToString() == "Đã điểm danh hôm nay")
                {
                    UIManager.instance.questionDialogUI_.DisplayPurchaseFailed("Bạn đã nhận quà điểm danh hôm nay! Hãy quay lại vào ngày mai!", () => { }, () => { });
                }
                else if (result.FunctionResult.ToString() == "Không tìm thấy bảng thưởng")
                {
                    UIManager.instance.questionDialogUI_.DisplayPurchaseFailed("Không tìm thấy phần thưởng quà tháng này, hãy đăng nhập lại!", () => { }, () => { });
                }
                else
                {
                    OnBorder(false);
                    OnMark(true);
                    UIManager.instance.questionDialogUI_.DisplayConfirmWithContentItem("Chúc mừng bạn nhận thưởng thành công, vật phẩm sẽ tự động thêm vào kho đồ!", imgIcon.sprite, catalogItem.DisplayName, quantity, "", "", null, () => { }, () => { });
                }
            },
            error =>
            {
                Debug.Log("Get User Data Error: " + error.Error);
            });
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick"); 
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GetReward();
        }
    }
}