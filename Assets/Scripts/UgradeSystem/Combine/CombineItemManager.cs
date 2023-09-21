using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ItemSO;

public class CombineItemManager : MonoBehaviour
{
    #region Singleton
    public static CombineItemManager instance { get; private set; }
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    #endregion
    private void OnEnable()
    {
        previewProductSlot.GetChild(0).GetComponent<Image>().enabled = false;
        previewProductSlot.GetChild(1).GetComponent<Image>().enabled = false;
    }

    private void OnDisable()
    {
        ResetCombineSystem();
    }

    [SerializeField] private Transform openUpgradeBtn;
    [SerializeField] private Transform openCombineBtn;
    [SerializeField] private Transform combineBtn;
    [SerializeField] private Transform alertResult;
    [SerializeField] private Transform closeBtn;

    [SerializeField] private bool canCombine = false;

    //-----------------------------------------------------------------------------------------------------------------------
    [Header("Combine Item")]
    [SerializeField] private List<ItemInstance> itemCombineList;

    [Header("Combine Slot")]
    [SerializeField] private List<Transform> itemCombineSlot;

    [Header("Finished Item")]
    [SerializeField] private Transform previewProductSlot;
    [SerializeField] private Transform finishedProductSlot;


    public bool AddItem(ItemInstance itemInstance)
    {
        if (itemCombineList.Count >= 4)
            return false;
        else
        {
            if (finishedProductSlot.GetComponent<UpgradeSlotController>().slotInUse_)
            {
                finishedProductSlot.GetComponent<UpgradeSlotController>().RemoveItem();
            }
            for (int i = 0; i < itemCombineSlot.Count; i++)
            {
                if (itemCombineSlot[i].GetComponent<UpgradeSlotController>().slotInUse_ == false)
                {
                    itemCombineList.Add(itemInstance);
                    itemCombineSlot[i].GetComponent<UpgradeSlotController>().LoadInforItemSlot(itemInstance);
                    //DisplaySuccessRateaAndGoldRequired();
                    DisplayPreviewFinishedProduct();
                    return true;
                }
            }
        }
        return false;
    }

    public void RemoveItem(ItemInstance itemInstance)
    {
        itemCombineList.Remove(itemInstance);
    }

    public void ResetPreviewSlot()
    {
        previewProductSlot.GetChild(0).GetComponent<Image>().enabled = false;
        previewProductSlot.GetChild(1).GetComponent<Image>().enabled = false;
        previewProductSlot.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
        previewProductSlot.GetChild(3).GetComponent<TextMeshProUGUI>().text = "0%";
        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 0";
    }

    //trả các vật phẩm còn trong ô Dung luyện về kho đồ nếu có
    public void ResetCombineSystem()
    {
        itemCombineList.Clear();
        ResetPreviewSlot();
        //--------
        for (int i = 0; i < itemCombineSlot.Count; i++)
        {
            if (itemCombineSlot[i].GetComponent<UpgradeSlotController>().slotInUse_ == true)
            {
                itemCombineSlot[i].GetComponent<UpgradeSlotController>().ResetUpgradeSlot();
            }
        }
        if (finishedProductSlot.GetComponent<UpgradeSlotController>().slotInUse_ == true)
        {
            finishedProductSlot.GetComponent<UpgradeSlotController>().ResetUpgradeSlot();
        }
    }

    //lấy thông tin vật phẩm dung luyện thành công
    public void DisplayPreviewFinishedProduct()
    {
        if (itemCombineList.Count <= 0)
        {
            ResetPreviewSlot();
            canCombine = false;
        }
        else
        {
            var inforBasic02_0 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemCombineList[0].CustomData["inforBasic02"]);
            for (int i = 0; i < itemCombineList.Count; i++)
            {
                var inforBasic02_i = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemCombineList[i].CustomData["inforBasic02"]);
                if (itemCombineList[0].ItemId != itemCombineList[i].ItemId || inforBasic02_0["tier"] != inforBasic02_i["tier"])
                {
                    ResetPreviewSlot();
                    canCombine = false;
                    return;
                }
            }

            if (PlayfabDataManager.instance.catalogItemsDictionary_[itemCombineList[0].ItemId].ItemClass == "Equipment")
            {
                var inforBasic01_0 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemCombineList[0].CustomData["inforBasic01"]);
                switch (inforBasic02_0["tier"])
                {
                    case "common":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(65, 168, 95, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 100";
                        break;
                    case "uncommon":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(44, 130, 201, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 200";
                        break;
                    case "rare":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(147, 101, 184, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 300";
                        break;
                    case "epic":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(250, 197, 28, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 400";
                        break;
                    case "legendary":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(226, 80, 65, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 500";
                        break;
                    case "mythic":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(226, 80, 65, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 1,000";
                        break;
                    default:
                        //Debug.Log("Not found rarity tier of item: " + equipment.itemName_);
                        break;
                }
                previewProductSlot.GetChild(0).GetComponent<Image>().enabled = true;
                previewProductSlot.GetChild(1).GetComponent<Image>().sprite = ImageLoader.instance.imgIconDictionary_[inforBasic01_0["iconImg"]];
                previewProductSlot.GetChild(1).GetComponent<Image>().enabled = true;
                previewProductSlot.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemCombineList[0].DisplayName;
                previewProductSlot.GetChild(3).GetComponent<TextMeshProUGUI>().text = "100%";
                canCombine = true;
            }
            else
            {
                CatalogItem finishedProductPreview = PlayfabDataManager.instance.catalogItemsDictionary_[inforBasic02_0["nextCombine"]];
                var customdata = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(finishedProductPreview.CustomData);
                var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["inforBasic01"]);
                var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["inforBasic02"]);
                switch (inforBasic02["tier"])
                {
                    case "common":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(209, 213, 216, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 100";
                        break;
                    case "uncommmon":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(65, 168, 95, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 200";
                        break;
                    case "rare":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(44, 130, 201, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 300";
                        break;
                    case "epic":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(147, 101, 184, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 400";
                        break;
                    case "legendary":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(250, 197, 28, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 500";
                        break;
                    case "mythic":
                        previewProductSlot.GetChild(0).GetComponent<Image>().color = new Color32(226, 80, 65, 255);
                        previewProductSlot.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Xu: 1,000";
                        break;
                    default:
                        //Debug.Log("Not found rarity tier of item: " + equipment.itemName_);
                        break;
                }
                previewProductSlot.GetChild(0).GetComponent<Image>().enabled = true;
                previewProductSlot.GetChild(1).GetComponent<Image>().sprite = ImageLoader.instance.imgIconDictionary_[inforBasic01["iconImg"]];
                previewProductSlot.GetChild(1).GetComponent<Image>().enabled = true;
                previewProductSlot.GetChild(2).GetComponent<TextMeshProUGUI>().text = finishedProductPreview.DisplayName;
                previewProductSlot.GetChild(3).GetComponent<TextMeshProUGUI>().text = "100%";
                canCombine = true;
            }
        }
    }

    //khi nhấn nút Dung luyện
    public void OnClickCombine()
    {
        if (itemCombineList.Count == 4 && canCombine)
        {
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
            {
                FunctionName = "GetResultCombine",
                FunctionParameter = new { itemCombineList }
                //GeneratePlayStreamEvent = true
            };
            PlayFabClientAPI.ExecuteCloudScript(request,
                result =>
                {
                    //Debug.Log("OnClickCombine: " + result.FunctionResult);
                    if (result.FunctionResult.ToString() == "notEnoughVirtualCurrency")
                    {
                        Debug.Log("Not enough VirtualCurrency: " + result.FunctionResult);
                        UIManager.instance.questionDialogUI_.DisplayPurchaseFailed("Không đủ Xu để dung luyện!", () => { }, () => { });
                    }
                    else
                    {
                        InventoryManager.instance.GetVirtualCurrencyUser();
                        Debug.Log("VirtualCurrency: " + result.FunctionResult);
                        ResetPreviewSlot();
                        itemCombineList.Clear();
                        for (int i = 0; i < itemCombineSlot.Count; i++)
                        {
                            itemCombineSlot[i].GetComponent<UpgradeSlotController>().ResetUpgradeSlot();
                        }
                        var resultCombine = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
                        if (resultCombine["result"] == "true")                                             //upgrade successful
                        {
                            ItemInstance resultItemInstance = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<ItemInstance>(resultCombine["newItemCombine"]);
                            finishedProductSlot.GetComponent<UpgradeSlotController>().LoadInforItemSlot(resultItemInstance);
                            AudioManager.instance.UpgradeSuccessSFX();
                            StartCoroutine(DisplayedResultUpgrade(true));
                        }
                        else                                                            //upgrade failed
                        {
                            AudioManager.instance.UpgradeFailureSFX();
                            StartCoroutine(DisplayedResultUpgrade(false));
                        }
                    }
                },
                error =>
                {
                    Debug.Log("test Error: " + error.Error);
                });
        }
    }

    public IEnumerator DisplayedResultUpgrade(bool _resultUpgrade)
    {
        closeBtn.GetComponent<Button>().interactable = false;
        openUpgradeBtn.GetComponent<Button>().interactable = false;
        openCombineBtn.GetComponent<Button>().interactable = false;
        combineBtn.GetComponent<Button>().interactable = false;
        alertResult.gameObject.SetActive(true);
        alertResult.GetComponent<Animator>().SetTrigger(_resultUpgrade.ToString());
        yield return new WaitForSeconds(1.5f);
        alertResult.GetComponent<Animator>().ResetTrigger(_resultUpgrade.ToString());
        alertResult.GetComponent<Animator>().SetTrigger("Default");
        yield return new WaitForSeconds(0.5f);
        alertResult.gameObject.SetActive(false);
        openUpgradeBtn.GetComponent<Button>().interactable = true;
        openCombineBtn.GetComponent<Button>().interactable = true;
        combineBtn.GetComponent<Button>().interactable = true;
        closeBtn.GetComponent<Button>().interactable = true;
    }
}
