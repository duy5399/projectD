using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

using static ItemSO;

public class ShopSystemManager : MonoBehaviour
{
    public static ShopSystemManager instance { get; private set; }

    [SerializeField] private ShopDatabaseSO shopDatabase;
    [SerializeField] private GameObject shopItemSlot;
    [SerializeField] private List<Transform> shopItemSlotParent;
    [SerializeField] private ScrollRect shopItem;
    [SerializeField] private Sprite subBtnUp;
    [SerializeField] private Sprite subBtnDown;
    [SerializeField] private List<Transform> slotItem;
    [SerializeField] private Transform currency;

    void Awake()
    {
        if(instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    void Start()
    {
        LoadStoreByPrices("store_01", "GD");
        LoadStoreByPrices("store_01", "MD");
        LoadDiscountStoreWithRandomItems();
    }

    private void OnEnable()
    {
        GetChangeDiscountShop();
        LoadCurrency();
    }

    public void GetChangeDiscountShop()
    {
        AsyncLoadingScene.instance.LoadingScreen(true);
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetChangeDiscountShop"
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            async result =>
            {
                if (result.FunctionResult.ToString() == "1")
                {
                    Destroy(discountStore);
                    var itemToRemove = listShopItemsByPrice.Find(x => x.name == "ShopDiscountWithRandomItems");
                    listShopItemsByPrice.Remove(itemToRemove);
                    PlayfabDataManager.instance.GetDiscountStoreData("MainCatalog");                    
                    await Task.Delay(2000);
                    LoadDiscountStoreWithRandomItems();
                    DisplayShopByPrice(subBtnByPrices[0].GetComponent<Button>());
                    DisplayShopByTag(subBtnByTags[0].GetComponent<Button>());
                    //AsyncLoadingScene.instance.LoadingScreen();
                    Debug.Log("GetChangeDiscountShop: 1");
                }
                else
                {
                    DisplayShopByPrice(subBtnByPrices[0].GetComponent<Button>());
                    DisplayShopByTag(subBtnByTags[0].GetComponent<Button>());
                    //AsyncLoadingScene.instance.LoadingScreen();
                    Debug.Log("GetChangeDiscountShop: 0");
                }
                AsyncLoadingScene.instance.LoadingScreen(false);
            },
            error =>
            {
                Debug.Log("GetChangeDiscountShop Error: " + error.Error);
            });
    }

    //xem trước vật phẩm - phòng thử đồ
    public void TryOnEquiment(StoreItem storeItem)
    {
        int indexSlot_ = slotItem.FindIndex(x => x.name == PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].Tags[0].ToString() + "Slot");
        if (indexSlot_ > -1)
        {
            slotItem[indexSlot_].GetComponent<ShopSlotPreview>().PreviewIconAndShow(storeItem);
        }
    }

    public void RemoveEquipmentPreview()
    {
        foreach(Transform slot in slotItem)
        {
            slot.GetComponent<ShopSlotPreview>().ResetGearSlotPreview();
        }
    }

    //load tiền 
    public void LoadCurrency()
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest { };
        PlayFabClientAPI.GetUserInventory(request,
            result =>
            {
                currency.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0:0,0}", result.VirtualCurrency["GD"]);
                currency.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0:0,0}", result.VirtualCurrency["MD"]);
            },
            error => {
                Debug.LogError(error);
            });
    }

    //-------------------------------------------/-------------------------------------------/-------------------------------------------/-------------------------------------------
    [SerializeField] private List<GameObject> listShopItemsByPrice = new List<GameObject>(); //danh sách các shop theo từng loại tiền
    [SerializeField] private GameObject displayShopItems; //prefab - object chứa danh sách vật phẩm để hiển thị
    [SerializeField] private GameObject discountStore;
    [SerializeField] private List<Transform> subBtnByPrices;
    [SerializeField] private List<Transform> subBtnByTags;
    [SerializeField] private Transform shopDisplaying;
    public void LoadStoreByPrices(string storeID, string currencyPrice)
    {
        GameObject newDisplayShopItems = Instantiate(displayShopItems, transform.GetChild(0));
        newDisplayShopItems.name = "Shop" + currencyPrice + "Price";
        listShopItemsByPrice.Add(newDisplayShopItems);
        foreach (var item in PlayfabDataManager.instance.listStoresItemsDictionary_[storeID])
        {
            try
            {
                if (item.Value.VirtualCurrencyPrices[currencyPrice].ToString() != "0")
                {
                    //Debug.Log("Loadstore currencyPrice: " + item.Value);
                    if (PlayfabDataManager.instance.catalogItemsDictionary_[item.Value.ItemId].Tags.Contains("weapon"))
                    {
                        Debug.Log("Loadstore catalogItemsDictionary_: " + item.Key);
                        Transform parent = newDisplayShopItems.transform.GetChild(0).Find("ListContentWeapon");
                        GameObject obj = Instantiate(shopItemSlot, parent);
                        obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(storeID, item.Value, currencyPrice);
                        obj.GetComponent<ShopItemSlotController>().LoadComponents();
                    }
                    else if (PlayfabDataManager.instance.catalogItemsDictionary_[item.Value.ItemId].Tags.Contains("cloth"))
                    {
                        Transform parent = newDisplayShopItems.transform.GetChild(0).Find("ListContentCloth");
                        GameObject obj = Instantiate(shopItemSlot, parent);
                        obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(storeID, item.Value, currencyPrice);
                        obj.GetComponent<ShopItemSlotController>().LoadComponents();
                    }
                    else if (PlayfabDataManager.instance.catalogItemsDictionary_[item.Value.ItemId].Tags.Contains("head"))
                    {
                        Transform parent = newDisplayShopItems.transform.GetChild(0).Find("ListContentHead");
                        GameObject obj = Instantiate(shopItemSlot, parent);
                        obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(storeID, item.Value, currencyPrice);
                        obj.GetComponent<ShopItemSlotController>().LoadComponents();
                    }
                    else if (PlayfabDataManager.instance.catalogItemsDictionary_[item.Value.ItemId].Tags.Contains("hair"))
                    {
                        Transform parent = newDisplayShopItems.transform.GetChild(0).Find("ListContentHair");
                        GameObject obj = Instantiate(shopItemSlot, parent);
                        obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(storeID, item.Value, currencyPrice);
                        obj.GetComponent<ShopItemSlotController>().LoadComponents();
                    }
                    else if (PlayfabDataManager.instance.catalogItemsDictionary_[item.Value.ItemId].Tags.Contains("glass"))
                    {
                        Transform parent = newDisplayShopItems.transform.GetChild(0).Find("ListContentGlass");
                        GameObject obj = Instantiate(shopItemSlot, parent);
                        obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(storeID, item.Value, currencyPrice);
                        obj.GetComponent<ShopItemSlotController>().LoadComponents();
                    }
                    else if (PlayfabDataManager.instance.catalogItemsDictionary_[item.Value.ItemId].Tags.Contains("face"))
                    {
                        Transform parent = newDisplayShopItems.transform.GetChild(0).Find("ListContentFace");
                        GameObject obj = Instantiate(shopItemSlot, parent);
                        obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(storeID, item.Value, currencyPrice);
                        obj.GetComponent<ShopItemSlotController>().LoadComponents();
                    }
                    else if (PlayfabDataManager.instance.catalogItemsDictionary_[item.Value.ItemId].Tags.Contains("wing"))
                    {
                        Transform parent = newDisplayShopItems.transform.GetChild(0).Find("ListContentWing");
                        GameObject obj = Instantiate(shopItemSlot, parent);
                        obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(storeID, item.Value, currencyPrice);
                        obj.GetComponent<ShopItemSlotController>().LoadComponents();
                    }
                    else if (PlayfabDataManager.instance.catalogItemsDictionary_[item.Value.ItemId].Tags.Contains("ring") || PlayfabDataManager.instance.catalogItemsDictionary_[item.Value.ItemId].Tags.Contains("armlet"))
                    {
                        Transform parent = newDisplayShopItems.transform.GetChild(0).Find("ListContentRingAndArmlet");
                        GameObject obj = Instantiate(shopItemSlot, parent);
                        obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(storeID, item.Value, currencyPrice);
                        obj.GetComponent<ShopItemSlotController>().LoadComponents();
                    }
                    else
                    {
                        Transform parent = newDisplayShopItems.transform.GetChild(0).Find("ListContentMaterial");
                        GameObject obj = Instantiate(shopItemSlot, parent);
                        obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(storeID, item.Value, currencyPrice);
                        obj.GetComponent<ShopItemSlotController>().LoadComponents();
                    }
                    
                    
                    //Debug.Log(item.Value.ItemId + " : " + item.Value.VirtualCurrencyPrices["GD"] + " + " + item.Value.VirtualCurrencyPrices["MD"]);
                }
            }
            catch
            {
                continue;
            }
        }
    }

    //cửa hàng giảm giá ngẫu nhiên
    public void LoadDiscountStoreWithRandomItems()
    {
        discountStore = Instantiate(displayShopItems, transform.GetChild(0));
        discountStore.name = "ShopDiscountWithRandomItems";
        listShopItemsByPrice.Add(discountStore);
        foreach(var i in PlayfabDataManager.instance.listDiscountItemsDictionary_)
        {
            try
            {
                Transform parent = discountStore.transform.GetChild(0).GetChild(discountStore.transform.GetChild(0).childCount - 1);
                //Debug.Log("newDisplayShopItems.transform.GetChild(0).childCount - 1: " + newDisplayShopItems.transform.GetChild(0).GetChild(newDisplayShopItems.transform.GetChild(0).childCount - 1).name);
                GameObject obj = Instantiate(shopItemSlot, parent);
                string[] temp = i.Key.Split(new char[] { '_' });
                obj.GetComponent<ShopItemSlotController>().LoadInforItemSlot(temp[0], i.Value, GetVirtualCurrencyPrices(i.Value), i.Key);
                obj.GetComponent<ShopItemSlotController>().LoadComponents();
            }
            catch
            {
                Debug.Log("LoadDiscountStoreWithRandomItems error:" + i.Value.ItemId);
                continue;
            }
        }
    }

    private string GetVirtualCurrencyPrices(StoreItem storeItem)
    {
        if (storeItem.VirtualCurrencyPrices["GD"] != 0 && storeItem.VirtualCurrencyPrices["MD"] != 0)
        {
            return UnityEngine.Random.Range(0, 2) == 0 ? "GD" : "MD"; 
        }
        else if (storeItem.VirtualCurrencyPrices["GD"] == 0 && storeItem.VirtualCurrencyPrices["MD"] != 0)
        {
            return "GD";
        }
        else if (storeItem.VirtualCurrencyPrices["GD"] != 0 && storeItem.VirtualCurrencyPrices["MD"] == 0)
        {
            return "MD";
        }
        return null;
    }

    //hiển thị danh sách vật phẩm theo từng loại (tùy button truyền vào khi nhấn)
    public void DisplayShopByTag(Button _subBtnByTag)
    {
        AudioManager.instance.ClickSuccessSFX();
        //Debug.Log(_subBtn.name + " " + _subBtn.gameObject);
        switch (_subBtnByTag.name)
        {
            case "SubBtnWeapon":
                foreach(Transform i in listShopItemsByPrice.Find(x => x.activeSelf == true).transform.GetChild(0))
                {
                    if (i.name == "ListContentWeapon")
                    {
                        i.gameObject.SetActive(true);
                        shopDisplaying.GetComponent<ScrollRect>().content = i.GetComponent<RectTransform>();
                    }
                    else
                    {
                        i.gameObject.SetActive(false);
                    }
                }
                break;
            case "SubBtnCloth":
                foreach (Transform i in listShopItemsByPrice.Find(x => x.activeSelf == true).transform.GetChild(0))
                {
                    if (i.name == "ListContentCloth")
                    {
                        i.gameObject.SetActive(true);
                        shopDisplaying.GetComponent<ScrollRect>().content = i.GetComponent<RectTransform>();
                    }
                    else
                    {
                        i.gameObject.SetActive(false);
                    }
                }
                break;
            case "SubBtnHead":
                foreach (Transform i in listShopItemsByPrice.Find(x => x.activeSelf == true).transform.GetChild(0))
                {
                    if (i.name == "ListContentHead")
                    {
                        i.gameObject.SetActive(true);
                        shopDisplaying.GetComponent<ScrollRect>().content = i.GetComponent<RectTransform>();
                    }
                    else
                    {
                        i.gameObject.SetActive(false);
                    }
                }
                break;
            case "SubBtnHair":
                foreach (Transform i in listShopItemsByPrice.Find(x => x.activeSelf == true).transform.GetChild(0))
                {
                    if (i.name == "ListContentHair")
                    {
                        i.gameObject.SetActive(true);
                        shopDisplaying.GetComponent<ScrollRect>().content = i.GetComponent<RectTransform>();
                    }
                    else
                    {
                        i.gameObject.SetActive(false);
                    }
                }
                break;
            case "SubBtnGlass":
                foreach (Transform i in listShopItemsByPrice.Find(x => x.activeSelf == true).transform.GetChild(0))
                {
                    if (i.name == "ListContentGlass")
                    {
                        i.gameObject.SetActive(true);
                        shopDisplaying.GetComponent<ScrollRect>().content = i.GetComponent<RectTransform>();
                    }
                    else
                    {
                        i.gameObject.SetActive(false);
                    }
                }
                break;
            case "SubBtnFace":
                foreach (Transform i in listShopItemsByPrice.Find(x => x.activeSelf == true).transform.GetChild(0))
                {
                    if (i.name == "ListContentFace")
                    {
                        i.gameObject.SetActive(true);
                        shopDisplaying.GetComponent<ScrollRect>().content = i.GetComponent<RectTransform>();
                    }
                    else
                    {
                        i.gameObject.SetActive(false);
                    }
                }
                break;
            case "SubBtnWing":
                foreach (Transform i in listShopItemsByPrice.Find(x => x.activeSelf == true).transform.GetChild(0))
                {
                    if (i.name == "ListContentWing")
                    {
                        i.gameObject.SetActive(true);
                        shopDisplaying.GetComponent<ScrollRect>().content = i.GetComponent<RectTransform>();
                    }
                    else
                    {
                        i.gameObject.SetActive(false);
                    }
                }
                break;
            case "SubBtnRingAndArmlet":
                foreach (Transform i in listShopItemsByPrice.Find(x => x.activeSelf == true).transform.GetChild(0))
                {
                    if (i.name == "ListContentRingAndArmlet")
                    {
                        i.gameObject.SetActive(true);
                        shopDisplaying.GetComponent<ScrollRect>().content = i.GetComponent<RectTransform>();
                    }
                    else
                    {
                        i.gameObject.SetActive(false);
                    }
                }
                break;
            case "SubBtnMaterial":
                foreach (Transform i in listShopItemsByPrice.Find(x => x.activeSelf == true).transform.GetChild(0))
                {
                    if (i.name == "ListContentMaterial")
                    {
                        i.gameObject.SetActive(true);
                        shopDisplaying.GetComponent<ScrollRect>().content = i.GetComponent<RectTransform>();
                    }
                    else
                    {
                        i.gameObject.SetActive(false);
                    }
                }
                break;
        }
        for (int i = 0; i < subBtnByTags.Count; i++)
        {
            //Debug.Log(subBtnByPrices[i].name + " - " + _subBtnByPrice.name);
            if (subBtnByTags[i].name == _subBtnByTag.name)
            {
                Debug.Log("true");
                subBtnByTags[i].GetComponent<Image>().sprite = subBtnUp;
            }
            else
            {
                subBtnByTags[i].GetComponent<Image>().sprite = subBtnDown;
            }
        }
    }

    //hiển thị 1 danh sách vật phẩm theo giá tiền - ẩn các shop khác
    public void DisplayShopByPrice(Button _subBtnByPrice)
    {
        AudioManager.instance.ClickSuccessSFX();
        //Debug.Log(_subBtnByPrice.name + " " + _subBtnByPrice.gameObject);
        switch (_subBtnByPrice.name)
        {
            case "BtnGoldPrice":
                for(int i = 0; i < listShopItemsByPrice.Count; i++)
                {
                    if (listShopItemsByPrice[i].name == "ShopGDPrice")
                    {
                        listShopItemsByPrice[i].SetActive(true);
                        shopDisplaying = listShopItemsByPrice[i].transform;
                    }
                    else
                    {
                        listShopItemsByPrice[i].SetActive(false);
                    }                    
                }
                break;
            case "BtnMedalPrice":
                for (int i = 0; i < listShopItemsByPrice.Count; i++)
                {
                    if (listShopItemsByPrice[i].name == "ShopMDPrice")
                    {
                        listShopItemsByPrice[i].SetActive(true);
                    }
                    else
                    {
                        listShopItemsByPrice[i].SetActive(false);
                    }
                }
                break;
            case "BtnDiscountPrice":
                for (int i = 0; i < listShopItemsByPrice.Count; i++)
                {
                    if (listShopItemsByPrice[i].name == "ShopDiscountWithRandomItems")
                    {
                        listShopItemsByPrice[i].SetActive(true);
                    }
                    else
                    {
                        listShopItemsByPrice[i].SetActive(false);
                    }
                }
                break;
        }
        for (int i = 0; i < subBtnByPrices.Count; i++)
        {
            Debug.Log(subBtnByPrices[i].name + " - " + _subBtnByPrice.name);
            if (subBtnByPrices[i].name == _subBtnByPrice.name)
            {
                Debug.Log("true");
                subBtnByPrices[i].GetComponent<Image>().sprite = subBtnUp;
            }
            else
            {
                subBtnByPrices[i].GetComponent<Image>().sprite = subBtnDown;
            }
        }
    }

}
