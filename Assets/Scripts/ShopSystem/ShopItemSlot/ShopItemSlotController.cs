using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using PlayFab.Internal;

public class ShopItemSlotController : InventorySlotController
{
    [SerializeField] private string storeID;
    [SerializeField] private StoreItem storeItem;
    [SerializeField] private string currencyPrice;
    [SerializeField] private string itemID;
    [SerializeField] private string orderID;
    [SerializeField] private PaymentOption paymentOption;
    [SerializeField] private Sprite goldPriceIcon;
    [SerializeField] private Sprite medalPriceIcon;

    [SerializeField] private Button btnBuy;
    [SerializeField] private Button btnTryOn;

    [SerializeField] private TextMeshProUGUI txtItemName;
    [SerializeField] private TextMeshProUGUI txtItemPrice;
    [SerializeField] private Image imgItemCurrency;
    [SerializeField] private ItemSO currency;
    [SerializeField] private int price;

    protected override void Awake()
    {
        LoadComponents();
    }

    public override void LoadComponents()
    {
        base.LoadComponents();
        txtItemName = transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        txtItemPrice = transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        imgItemCurrency = transform.GetChild(5).GetComponent<Image>();
        btnBuy = transform.GetChild(6).GetComponent<Button>();
        btnTryOn = transform.GetChild(7).GetComponent<Button>();
    }

    public void LoadInforItemSlot(string storeID, StoreItem storeItem, string currencyPrice)
    {
        this.storeID = storeID;
        this.storeItem = storeItem;
        this.currencyPrice = currencyPrice;
        var customdata_catalog = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].CustomData);
        var inforBasic01_catalog = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata_catalog["inforBasic01"]);
        if (PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].Tags[0] == "weapon" || PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].Tags[0] == "offhand"
        || PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].Tags[0] == "ring" || PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].Tags[0] == "armlet"
        || PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].ItemClass == "Material")
        {
            btnTryOn.gameObject.SetActive(false);
        }
        else
        {
            btnTryOn.gameObject.SetActive(true);
        }
        try
        {
            imgIcon.sprite = ImageLoader.instance.imgIconDictionary_[inforBasic01_catalog["iconImg"]];
            imgIcon.enabled = true;
        }
        catch
        {
            imgIcon.sprite = null;
            imgIcon.enabled = false;
        }
        try
        {
            txtItemPrice.text = storeItem.VirtualCurrencyPrices[currencyPrice].ToString();
            imgItemCurrency.sprite = currencyPrice == "GD" ? goldPriceIcon : medalPriceIcon;
        }
        catch
        {
            txtItemPrice.text = "-";
        }
        txtItemName.text = PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].DisplayName;
        var customdata = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(storeItem.CustomData.ToString());
        var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["inforBasic02"]);
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
                Debug.Log("Not found rarity tier of item: " + storeItem.ItemId);
                break;
        }
    }

    public void LoadInforItemSlot(string storeID, StoreItem storeItem, string currencyPrice, string itemID)
    {
        LoadInforItemSlot(storeID, storeItem, currencyPrice);
        this.itemID = itemID;
    }

    //mặc thử và xem trước vật phẩm
    public void TryOnEquipment()
    {
        ShopSystemManager.instance.TryOnEquiment(storeItem);
    }

    //hiển thị popup thông báo xác nhận giao dịch
    public void DisplayCheckout()
    {
        AudioManager.instance.ClickSuccessSFX();
        var customdata = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(storeItem.CustomData.ToString());
        var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["inforBasic02"]);
        UIManager.instance.questionDialogUI_.DisplayConfirmWithContentItem("Bạn có chắc muốn mua vật phẩm này?", imgIcon.sprite, txtItemName.text, "1" , txtItemPrice.text, inforBasic02["tier"], currencyPrice == "GD" ? goldPriceIcon : medalPriceIcon, () => { PurchaseItem(); }, () => { });
    }

    void StartPurchase()
    {
        StartPurchaseRequest request = new StartPurchaseRequest
        {
            CatalogVersion = "MainCatalog",
            StoreId = storeID,
            Items = new List<ItemPurchaseRequest> {
             // The presence of these lines are based on the results from GetStoreItems, and user selection - Yours will be more generic.
            new ItemPurchaseRequest { ItemId = storeItem.ItemId, Quantity = 1,}
        }
        };
        PlayFabClientAPI.StartPurchase(request,
            result => 
            { 
                Debug.Log("DefinePurchase: " + result.OrderId);
                orderID = result.OrderId;
                paymentOption = result.PaymentOptions[0];
            }, 
            error =>
            {
                Debug.Log(error.Error);
            });
    }

    void PayForPurchase()
    {
        var request = new PayForPurchaseRequest
        {
            OrderId = orderID, // orderId comes from StartPurchase above.
            Currency = paymentOption.Currency, // User defines which currency they wish to use to pay for this purchase (all items must have a defined/non-zero cost in this currency).
            ProviderName = paymentOption.ProviderName // providerName comes from the PaymentOptions in the result from StartPurchase above.
        };
        PlayFabClientAPI.PayForPurchase(request, 
            result =>
            {
                Debug.Log("DefinePaymentCurrency success" + result.OrderId);
            }, 
            error =>
            {
                Debug.Log("DefinePaymentCurrency error" + error.Error);
            });
    }


    //mua vật phẩm - tạo bản sao (trang bị thì set tier = common và random stats) => truyền vật phẩm vào inventory
    private void PurchaseItem()
    {
        AsyncLoadingScene.instance.LoadingScreen(true);
        PurchaseItemRequest request = new PurchaseItemRequest {
            StoreId = storeID,
            ItemId = storeItem.ItemId,
            VirtualCurrency = currencyPrice,
            Price = (int)storeItem.VirtualCurrencyPrices[currencyPrice]
            };
        PlayFabClientAPI.PurchaseItem(request,
            result =>
            {
                if (PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].ItemClass == "Equipment")
                {
                    var customdata = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(storeItem.CustomData.ToString());
                    var stat01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["stat01"]);
                    var statSlot0 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(stat01["statSlot0"]);
                    var customdata_catalog = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].CustomData);
                    var inforBasic01_catalog = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata_catalog["inforBasic01"]);
                    var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["inforBasic02"]);
                    SetDataForItem(result.Items[0].ItemInstanceId, storeItem.ItemId, inforBasic02["tier"], statSlot0.Keys.ElementAt(0), inforBasic01_catalog["iconImg"].ToString(), inforBasic01_catalog["showImg"].ToString());//customdata["tier"]
                }
                else
                {
                    SetDataForItem(result.Items[0].ItemInstanceId, PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].CustomData);//customdata["tier"]
                }
                AudioManager.instance.BuyitemSuccessSFX();
                UIManager.instance.questionDialogUI_.DisplayPurchaseSuccesful("Giao dịch thành công, vật phẩm sẽ được chuyển vào túi đồ!", () => { }, () => { });
                ShopSystemManager.instance.LoadCurrency();
                AsyncLoadingScene.instance.LoadingScreen(false);
                if (storeID.Contains("DiscountStore")){
                    PlayfabDataManager.instance.DeleteItemDiscountStore(itemID);
                    Destroy(gameObject);
                }
            },
            error =>
            {
                AsyncLoadingScene.instance.LoadingScreen(false);
                UIManager.instance.questionDialogUI_.DisplayPurchaseFailed("Giao dịch thất bại, không đủ vật phẩm yêu cầu để mua hàng!", () => { }, () => { });
            });
    }

    protected override void SetDataForItem(string itemInstance, string itemID, string tier, string mainStat, string iconImg, string showImg)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdateItemInstanceCustomData",
            FunctionParameter = new { itemInstance, itemID, tier, mainStat, iconImg, showImg }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("UpdateItemInstanceCustomData success");
            },
            SetUserDataError =>
            {
                Debug.Log("Get User Data Error: " + SetUserDataError.Error);
            });
    }

    protected override void SetDataForItem(string itemInstance, string data)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdateItemInstanceData",
            FunctionParameter = new { itemInstance, data }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("UpdateItemInstanceData success" + result.FunctionResult);
            },
            SetUserDataError =>
            {
                Debug.Log("Get User Data Error: " + SetUserDataError.Error);
            });
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        UIManager.instance.inventoryDesciptionController_.DescriptionItem(storeItem);
    }
}
