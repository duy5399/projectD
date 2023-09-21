using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using static ItemSO;

public class InventoryDesciptionController : MonoBehaviour
{
    [SerializeField] private ItemInstance itemInstance;

    [Header("Descripton")]
    [SerializeField] private Image itemStrengthImg;
    [SerializeField] private Image itemIconImg;
    [SerializeField] private TextMeshProUGUI itemNameTxt;
    [SerializeField] private TextMeshProUGUI itemTierTxt;
    [SerializeField] private TextMeshProUGUI itemTypeTxt;
    [SerializeField] private TextMeshProUGUI itemStatsTxt;
    [SerializeField] private TextMeshProUGUI itemDescriptionTxt;

    [Header("Button")]
    [SerializeField] private Button equipBtn;
    [SerializeField] private Button unequipBtn;
    [SerializeField] private Button sellBtn;
    [SerializeField] private Button upgradeBtn;
    [SerializeField] private Button unUpgradeBtn;
    [SerializeField] private Button combineBtn;

    [SerializeField] private UnityAction equipAction;
    [SerializeField] private UnityAction unequipAction;
    [SerializeField] private UnityAction sellAction;
    [SerializeField] private UnityAction upgradeAction;
    [SerializeField] private UnityAction unUpgradeAction;
    [SerializeField] private UnityAction combineAction;

    public void HiddenDescription()
    {
        ResetDescription();
        transform.gameObject.SetActive(false);
    }

    public void ResetDescription()
    {
        //itemUpgrade.sprite = null;
        itemIconImg.sprite = null;
        itemNameTxt.text = null;
        itemTierTxt.text = null;
        itemTypeTxt.text = null;
        itemStatsTxt.text = null;
        itemDescriptionTxt.text = null;
        equipBtn.gameObject.SetActive(false);
        unequipBtn.gameObject.SetActive(false);
        sellBtn.gameObject.SetActive(false);
        upgradeBtn.gameObject.SetActive(false);
        unUpgradeBtn.gameObject.SetActive(false);
        combineBtn.gameObject.SetActive(false);
        equipAction = null;
        unequipAction = null;
        sellAction = null;
        upgradeAction = null;
        unUpgradeAction = null;
        combineAction = null;
    }

    private void OnEnable()
    {
        if (equipAction != null)
            equipBtn.gameObject.SetActive(true);
        else
            equipBtn.gameObject.SetActive(false);
        if (unequipAction != null)
            unequipBtn.gameObject.SetActive(true);
        else
            unequipBtn.gameObject.SetActive(false);
        if (sellAction != null)
            sellBtn.gameObject.SetActive(true);
        else
            sellBtn.gameObject.SetActive(false);
        if (upgradeAction != null)
            upgradeBtn.gameObject.SetActive(true);
        else
            upgradeBtn.gameObject.SetActive(false);
        if (unUpgradeAction != null)
            unUpgradeBtn.gameObject.SetActive(true);
        else
            unUpgradeBtn.gameObject.SetActive(false);
        if(combineAction != null)
            combineBtn.gameObject.SetActive(true);
        else
            combineBtn.gameObject.SetActive(false);
        equipBtn.onClick.AddListener(EquipBtnClicked);
        unequipBtn.onClick.AddListener(UnequipBtnClicked);
        sellBtn.onClick.AddListener(SellBtnClicked);
        upgradeBtn.onClick.AddListener(UpgradeBtnClicked);
        unUpgradeBtn.onClick.AddListener(UnUpgradeBtnClicked);
        combineBtn.onClick.AddListener(CombineBtnClicked);
    }

    private void OnDisable()
    {
        if (equipBtn != null)
        {
            equipBtn.onClick.RemoveListener(EquipBtnClicked);
        }
        if (unequipBtn != null)
        {
            unequipBtn.onClick.RemoveListener(UnequipBtnClicked);
        }
        if (sellBtn != null)
        {
            sellBtn.onClick.RemoveListener(SellBtnClicked);
        }
        if (upgradeBtn != null)
        {
            upgradeBtn.onClick.RemoveListener(UpgradeBtnClicked);
        }
        if (unUpgradeBtn != null)
        {
            upgradeBtn.onClick.RemoveListener(UpgradeBtnClicked);
        }
        if (combineBtn != null)
        {
            combineBtn.onClick.RemoveListener(CombineBtnClicked);
        }
    }

    private void EquipBtnClicked()
    {
        equipAction?.Invoke();
        //Like yesAction() but with a null check
        
    }

    private void UnequipBtnClicked()
    {
        
        unequipAction?.Invoke();
        //Like yesAction() but with a null check

    }

    private void SellBtnClicked()
    {
        sellAction?.Invoke();
        SellItem();
    }

    private void UpgradeBtnClicked()
    {
        upgradeAction?.Invoke();
    }

    private void UnUpgradeBtnClicked()
    {
        unUpgradeAction?.Invoke();
    }

    private void CombineBtnClicked()
    {
        combineAction?.Invoke();
    }

    //hiển thị popup thông báo xác nhận bán
    private void SellItem()
    {
        var strenghLv = "";
        if (itemInstance.ItemClass == "Equipment")
        {
            var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforUpgrade"]);
            strenghLv = inforUpgrade["strenghLv"].ToString() != "0" ? " +" + inforUpgrade["strenghLv"].ToString() : "";
        }
        itemNameTxt.text = PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].DisplayName + strenghLv;
        var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic01"]);
        var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic02"]);
        AudioManager.instance.ClickSuccessSFX();
        UIManager.instance.questionDialogUI_.DisplayConfirmWithContentItem("Bạn có chắc muốn bán vật phẩm này?",
            ImageLoader.instance.imgIconDictionary_[inforBasic01["iconImg"]], itemInstance.DisplayName,
            itemInstance.ItemClass == "Equipment" ? "1" : itemInstance.RemainingUses.ToString(), 
            (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].VirtualCurrencyPrices["GD"]/10).ToString() + " Xu",
            inforBasic02["tier"], 
            null,
            () => { sellAction(); }, () => { });
    }


    public void DescriptionItem(ItemInstance itemInstance, UnityAction equipAction, UnityAction unequipAction, UnityAction sellAction, UnityAction upgradeAction, UnityAction unUpgradeAction, UnityAction combineAction)
    {
        this.itemInstance = itemInstance;
        this.equipAction = equipAction;
        this.unequipAction = unequipAction;
        this.sellAction = sellAction;
        this.upgradeAction = upgradeAction;
        this.unUpgradeAction = unUpgradeAction;
        this.combineAction = combineAction;
        var stat01 = new Dictionary<string, string>();
        var stat02 = new Dictionary<string, string>();
        string strenghLv = "";
        if (itemInstance.ItemClass == "Equipment")
        {
            var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforUpgrade"]);
            stat01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["stat01"]);
            stat02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["stat02"]);
            strenghLv = inforUpgrade["strenghLv"].ToString() != "0" ? " +" + inforUpgrade["strenghLv"].ToString() : "";
            //equipBtn.gameObject.SetActive(true);
        }
        var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic01"]);
        var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic02"]);
        itemNameTxt.text = PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].DisplayName + strenghLv;
        try
        {
            itemIconImg.sprite = ImageLoader.instance.imgIconDictionary_[inforBasic01["iconImg"]];
            itemIconImg.enabled = true;
            //Debug.Log("itemIconImg: success");
        }
        catch
        {
            //Debug.Log("itemIconImg: error");
            itemIconImg.sprite = null;
            itemIconImg.enabled = false;
        }
        try
        {
            itemStrengthImg.sprite = ImageLoader.instance.upgradeCondition_.strengthLvAndImg_[int.Parse(strenghLv)].strengthImg_;
            itemStrengthImg.enabled = true;
            //Debug.Log("itemIconImg: success");
        }
        catch
        {
            //Debug.Log("itemIconImg: error");
            itemStrengthImg.sprite = null;
            itemStrengthImg.enabled = false;
        }
        switch (inforBasic02["tier"])
        {
            case "common":
                itemTierTxt.text = "Thường";
                itemNameTxt.color = new Color32(209, 213, 216, 255);
                itemTierTxt.color = new Color32(209, 213, 216, 255);
                itemStatsTxt.color = new Color32(209, 213, 216, 255);
                break;
            case "uncommon":
                itemTierTxt.text = "Cao cấp";
                itemNameTxt.color = new Color32(65, 168, 95, 255);
                itemTierTxt.color = new Color32(65, 168, 95, 255);
                itemStatsTxt.color = new Color32(65, 168, 95, 255);
                break;
            case "rare":
                itemTierTxt.text = "Hiếm";
                itemNameTxt.color = new Color32(44, 130, 201, 255);
                itemTierTxt.color = new Color32(44, 130, 201, 255);
                itemStatsTxt.color = new Color32(44, 130, 201, 255);
                break;
            case "epic":
                itemTierTxt.text = "Sử thi";
                itemNameTxt.color = new Color32(147, 101, 184, 255);
                itemTierTxt.color = new Color32(147, 101, 184, 255);
                itemStatsTxt.color = new Color32(147, 101, 184, 255);
                break;
            case "legendary":
                itemTierTxt.text = "Huyền thoại";
                itemNameTxt.color = new Color32(250, 197, 28, 255);
                itemTierTxt.color = new Color32(250, 197, 28, 255);
                itemStatsTxt.color = new Color32(250, 197, 28, 255);
                break;
            case "mythic":
                itemTierTxt.text = "Thần thoại";
                itemNameTxt.color = new Color32(226, 80, 65, 255);
                itemTierTxt.color = new Color32(226, 80, 65, 255);
                itemStatsTxt.color = new Color32(226, 80, 65, 255);
                break;
            default:
                Debug.Log("Not found rarity tier of item: " + itemInstance.ItemId);
                break;
        }
        switch (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].ItemClass)
        {
            case "Equipment":
                itemTypeTxt.text = "Trang bị";
                break;
            case "Material":
                itemTypeTxt.text = "Đạo cụ";
                break;
        }
        try
        {
            foreach (var j in stat01)
            {
                var statSlot = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(j.Value);
                foreach (var k in statSlot)
                {
                    switch (k.Key)
                    {

                        case "atk":
                            itemStatsTxt.text += "Tấn công: " + k.Value + "\n";
                            break;
                        case "def":
                            itemStatsTxt.text += "Phòng thủ: " + k.Value + "\n";
                            break;
                        case "hp":
                            itemStatsTxt.text += "HP: " + k.Value + "\n";
                            break;
                        case "luk":
                            itemStatsTxt.text += "May mắn: " + k.Value + "\n";
                            break;
                        default:
                            itemStatsTxt.text += "Chỉ số ngẫu nhiên: " + k.Value + "\n";
                            break;
                    }
                }
            }
            foreach (var j in stat02)
            {
                var statSlot = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(j.Value);
                foreach (var k in statSlot)
                {
                    switch (k.Key)
                    {

                        case "atk":
                            itemStatsTxt.text += "Tấn công: " + k.Value + "\n";
                            break;
                        case "def":
                            itemStatsTxt.text += "Phòng thủ: " + k.Value + "\n";
                            break;
                        case "hp":
                            itemStatsTxt.text += "HP: " + k.Value + "\n";
                            break;
                        case "luk":
                            itemStatsTxt.text += "May mắn: " + k.Value + "\n";
                            break;
                        default:
                            itemStatsTxt.text += "Chỉ số ngẫu nhiên: " + k.Value + "\n";
                            break;
                    }
                }
            }
        }
        catch
        {
            itemStatsTxt.text = "";
        }
        itemDescriptionTxt.text = PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Description + "\n";
        transform.gameObject.SetActive(true);
        Debug.Log("Description item sucess");
    }

    public void DescriptionItem(StoreItem storeItem)
    {
        var stat01 = new Dictionary<string, string>();
        var stat02 = new Dictionary<string, string>();
        var customdata_catalog = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].CustomData);
        var inforBasic01_catalog = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata_catalog["inforBasic01"]);
        try
        {
            itemIconImg.sprite = ImageLoader.instance.imgIconDictionary_[inforBasic01_catalog["iconImg"]];
            itemIconImg.enabled = true;
            Debug.Log("itemIconImg: success");
        }
        catch
        {
            Debug.Log("itemIconImg: error");
            itemIconImg.sprite = null;
            itemIconImg.enabled = false;
        }
        itemStrengthImg.enabled = false;
        var customdata = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(storeItem.CustomData.ToString());
        var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["inforBasic02"]);
        if (PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].ItemClass == "Equipment")
        {
            //var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["inforUpgrade"]);
            stat01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["stat01"]);
            stat02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["stat02"]);
        }
        switch (inforBasic02["tier"])
        {
            case "common":
                itemTierTxt.text = "Thường";
                itemNameTxt.color = new Color32(209, 213, 216, 255);
                itemTierTxt.color = new Color32(209, 213, 216, 255);
                itemStatsTxt.color = new Color32(209, 213, 216, 255);
                break;
            case "uncommon":
                itemTierTxt.text = "Cao cấp";
                itemNameTxt.color = new Color32(65, 168, 95, 255);
                itemTierTxt.color = new Color32(65, 168, 95, 255);
                itemStatsTxt.color = new Color32(65, 168, 95, 255);
                break;
            case "rare":
                itemTierTxt.text = "Hiếm";
                itemNameTxt.color = new Color32(44, 130, 201, 255);
                itemTierTxt.color = new Color32(44, 130, 201, 255);
                itemStatsTxt.color = new Color32(44, 130, 201, 255);
                break;
            case "epic":
                itemTierTxt.text = "Sử thi";
                itemNameTxt.color = new Color32(147, 101, 184, 255);
                itemTierTxt.color = new Color32(147, 101, 184, 255);
                itemStatsTxt.color = new Color32(147, 101, 184, 255);
                break;
            case "legendary":
                itemTierTxt.text = "Huyền thoại";
                itemNameTxt.color = new Color32(250, 197, 28, 255);
                itemTierTxt.color = new Color32(250, 197, 28, 255);
                itemStatsTxt.color = new Color32(250, 197, 28, 255);
                break;
            case "mythic":
                itemTierTxt.text = "Thần thoại";
                itemNameTxt.color = new Color32(226, 80, 65, 255);
                itemTierTxt.color = new Color32(226, 80, 65, 255);
                itemStatsTxt.color = new Color32(226, 80, 65, 255);
                break;
            default:
                Debug.Log("Not found rarity tier of item: " + storeItem.ItemId);
                break;
        }
        itemNameTxt.text = PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].DisplayName;
        switch (PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].ItemClass)
        {
            case "Equipment":
                itemTypeTxt.text = "Trang bị";
                break;
            case "Material":
                itemTypeTxt.text = "Đạo cụ";
                break;
        }
        try
        {
            foreach (var j in stat01)
            {
                var statSlot = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(j.Value);
                foreach (var k in statSlot)
                {
                    switch (k.Key)
                    {

                        case "atk":
                            itemStatsTxt.text += "Tấn công: " + k.Value + "\n";
                            break;
                        case "def":
                            itemStatsTxt.text += "Phòng thủ: " + k.Value + "\n";
                            break;
                        case "hp":
                            itemStatsTxt.text += "HP: " + k.Value + "\n";
                            break;
                        case "luk":
                            itemStatsTxt.text += "May mắn: " + k.Value + "\n";
                            break;
                        default:
                            itemStatsTxt.text += "Chỉ số ngẫu nhiên: " + k.Value + "\n";
                            break;
                    }
                }
            }
            foreach (var j in stat02)
            {
                var statSlot = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(j.Value);
                foreach (var k in statSlot)
                {
                    switch (k.Key)
                    {

                        case "atk":
                            itemStatsTxt.text += "Tấn công: " + k.Value + "\n";
                            break;
                        case "def":
                            itemStatsTxt.text += "Phòng thủ: " + k.Value + "\n";
                            break;
                        case "hp":
                            itemStatsTxt.text += "HP: " + k.Value + "\n";
                            break;
                        case "luk":
                            itemStatsTxt.text += "May mắn: " + k.Value + "\n";
                            break;
                        default:
                            itemStatsTxt.text += "Chỉ số ngẫu nhiên: " + k.Value + "\n";
                            break;
                    }
                }
            }
        }
        catch
        {
            itemStatsTxt.text = "";
        }
        itemDescriptionTxt.text = PlayfabDataManager.instance.catalogItemsDictionary_[storeItem.ItemId].Description + "\n";
        //equipBtn.gameObject.SetActive(false);
        //sellBtn.gameObject.SetActive(false);
        transform.gameObject.SetActive(true);
        Debug.Log("transform.gameObject.SetActive(true);");
    }
}
