using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using PlayFab.Internal;
using PlayFab.Json;

public class UpgradeEquipmentManager : MonoBehaviour
{
    #region Singleton
    public static UpgradeEquipmentManager instance { get; private set; }
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
        
    }

    private void OnDisable()
    {
        ResetUpgradeSystem();
    }

    [Header("Upgrade Item")]
    [SerializeField] private ItemInstance equipment = null;
    [SerializeField] private List<ItemInstance> strengthStoneList;
    [SerializeField] private ItemInstance godCharm = null;
    [Header("Upgrade Slot")]
    [SerializeField] private Transform equipmentUpgradeSlot; //UpgradeSystem - Interface - Upgrade - EquipmentUpgrade
    [SerializeField] private List<Transform> strengthStoneSlot; //UpgradeSystem - Interface - Upgrade - StrengthStone
    [SerializeField] private Transform godCharmSlot; //UpgradeSystem - Interface - Upgrade - StrengthStone
    [Header("Success Rate and Gold Requires")]
    [SerializeField] private TextMeshProUGUI txtSuccessRate;
    [SerializeField] private TextMeshProUGUI txtGoldRequire;

    [SerializeField] private Transform openUpgradeBtn;
    [SerializeField] private Transform openCombineBtn;
    [SerializeField] private Transform upgradeStrengthBtn;
    [SerializeField] private Transform alertResult;
    [SerializeField] private Transform closeBtn;    

    #region Trang bị
    public void AddEquipmentUpgrade(ItemInstance itemInstance)
    {
        equipmentUpgradeSlot.GetComponent<UpgradeSlotController>().LoadInforItemSlot(itemInstance);
        equipment = itemInstance;
        DisplaySuccessRateaAndGoldRequired();
    }

    public void RemoveEquipment()
    {
        if(equipment != null)
            equipment = null;
    }
    #endregion

    #region Đá cường hóa
    public bool AddStrengthStone(ItemInstance itemInstance)
    {
        if(strengthStoneList.Count >= 3)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < strengthStoneSlot.Count; i++)
            {
                if (strengthStoneSlot[i].GetComponent<UpgradeSlotController>().slotInUse_ == false)
                {
                    strengthStoneList.Add(itemInstance);
                    strengthStoneSlot[i].GetComponent<UpgradeSlotController>().LoadInforItemSlot(itemInstance);
                    DisplaySuccessRateaAndGoldRequired();
                    return true;
                }
            }
        }
        return false;
    }

    public void RemoveStrengthStone(ItemInstance itemInstance)
    {
        strengthStoneList.Remove(itemInstance);
    }
    #endregion

    #region Bùa ma thuật
    public bool AddGodCharm(ItemInstance itemInstance)
    {
        if (godCharmSlot.GetComponent<UpgradeSlotController>().slotInUse_ == false)
        {
            godCharm = itemInstance;
            godCharmSlot.GetComponent<UpgradeSlotController>().LoadInforItemSlot(itemInstance);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveGodCharm()
    {
        if(godCharm != null)
        {
            godCharm = null;
            Debug.Log("RemoveGodCharm");
        }
    }
    #endregion

    #region Tỉ lệ nâng cấp thành công
    public float UpgradeSuccessRate(ItemInstance equipment, ItemInstance strengthStone)
    {
        float successRate = 0f;
        var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(equipment.CustomData["inforUpgrade"]);
        if(equipment.ItemClass == "Equipment" && strengthStone.ItemClass == "Material")
        {
            if (PlayfabDataManager.instance.catalogItemsDictionary_[strengthStone.ItemId].Tags[0] == "upgrade")
            {
                if (inforUpgrade["canUpgrade"] == "1" && int.Parse(inforUpgrade["strenghLv"]) < 15)
                {
                    switch (int.Parse(inforUpgrade["strenghLv"]))
                    {
                        case 0:
                            successRate = 100f;
                            break;
                        case 1:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 80f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 100f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 100f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 100f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 100f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 2:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 50f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 80f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 100f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 100f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 100f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 3:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 20f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 40f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 60f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 80f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 100f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 4:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 20f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 30f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 40f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 60f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 100f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 5:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 5f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 10f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 20f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 30f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 50f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 6:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 0.11f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 0.41f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 1.66f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 6.66f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 26.66f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 7:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 0.09f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 0.36f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 1.45f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 5.83f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 23.33f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 8:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 0.07f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 0.32f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 1.25f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 5f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 20f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 9:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 0.06f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 0.26f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 1.04f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 4.16f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 16.66f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 10:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 0.05f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 0.2f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 0.83f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 3.33f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 13.33f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 11:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 0.03f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 0.16f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 0.63f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 2.5f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 10f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 12:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 0.02f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 0.11f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 0.44f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 1.75f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 7f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 13:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 0.02f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 0.08f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 0.32f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 1.25f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 5f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                        case 14:
                            switch (strengthStone.ItemId)
                            {
                                case "strengthStone_01":
                                    successRate = 0.01f;
                                    break;
                                case "strengthStone_02":
                                    successRate = 0.05f;
                                    break;
                                case "strengthStone_03":
                                    successRate = 0.19f;
                                    break;
                                case "strengthStone_04":
                                    successRate = 0.75f;
                                    break;
                                case "strengthStone_05":
                                    successRate = 3f;
                                    break;
                                default:
                                    successRate = 0f;
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    successRate = 0f;
                }
            }
        }       
        return successRate;
    }
    //tỉ lệ thành công khi nâng cấp
    public float TotalUpdateSuccessRate()
    {
        if (CanUpgrade())
        {
            float successRate = 0f;
            for (int i = 0; i < strengthStoneList.Count; i++)
            {
                successRate += UpgradeSuccessRate(equipment, strengthStoneList[i]);
            }
            return successRate;
        }
        else
            return 0f;
    }
    #endregion
    #region Vàng yêu cầu
    public int UpgradeGoldRequired()
    {
        if (CanUpgrade())
        {
            int goldRequired = 0;
            var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(equipment.CustomData["inforUpgrade"]);
            switch (int.Parse(inforUpgrade["strenghLv"]))
            {
                case 0:
                    goldRequired = 0;
                    break;
                case 1:
                    goldRequired = 500;
                    break;
                case 2:
                    goldRequired = 500;
                    break;
                case 3:
                    goldRequired = 500;
                    break;
                case 4:
                    goldRequired = 1000;
                    break;
                case 5:
                    goldRequired = 1000;
                    break;
                case 6:
                    goldRequired = 1000;
                    break;
                case 7:
                    goldRequired = 1500;
                    break;
                case 8:
                    goldRequired = 1500;
                    break;
                case 9:
                    goldRequired = 1500;
                    break;
                case 10:
                    goldRequired = 2000;
                    break;
                case 11:
                    goldRequired = 2000;
                    break;
                case 12:
                    goldRequired = 2000;
                    break;
                case 13:
                    goldRequired = 5000;
                    break;
                case 14:
                    goldRequired = 5000;
                    break;
            }
            return goldRequired;
        }
        else
        {
            return 0;
        }
    }
    #endregion

    public void DisplaySuccessRateaAndGoldRequired()
    {
        txtSuccessRate.text = TotalUpdateSuccessRate() > 100 ? "100%" : TotalUpdateSuccessRate().ToString() + "%";
        txtGoldRequire.text = UpgradeGoldRequired().ToString();
        Debug.Log("successRateTxt.text: " + txtSuccessRate.text);
    }

    //kiểm tra các vật phẩm đầu vào,yêu cầu: 1 trang bị có cấp độ cường hóa < 15 và ít nhất 1 đá cường hóa
    public bool CanUpgrade()
    {
        if (equipmentUpgradeSlot.GetComponent<UpgradeSlotController>().slotInUse_ && strengthStoneList.Count > 0)
        {
            var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(equipment.CustomData["inforUpgrade"]);
            if (inforUpgrade["canUpgrade"] == "1" && int.Parse(inforUpgrade["strenghLv"]) >= 0 && int.Parse(inforUpgrade["strenghLv"]) < 15)
                return true;
        }
        return false;
    }

    public void ResetUpgradeSystem()
    {
        txtSuccessRate.text = "0%";
        txtGoldRequire.text = "0";
        RemoveGodCharm();
        RemoveEquipment();
        strengthStoneList.Clear();
        equipmentUpgradeSlot.GetComponent<UpgradeSlotController>().ResetUpgradeSlot();
        godCharmSlot.GetComponent<UpgradeSlotController>().ResetUpgradeSlot();
        for (int i = 0; i < strengthStoneSlot.Count; i++)
        {
            if (strengthStoneSlot[i].GetComponent<UpgradeSlotController>().slotInUse_ == true)
                strengthStoneSlot[i].GetComponent<UpgradeSlotController>().ResetUpgradeSlot();
        }
    }

    public void OnClickUpgrade()
    {
        if (CanUpgrade() && TotalUpdateSuccessRate() > 0)  //if success rate > 0 => can upgrade
        {
            DisplaySuccessRateaAndGoldRequired();
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
            {
                FunctionName = "GetResultUpgrade",
                FunctionParameter = new { strenghStoneList = strengthStoneList, successRate = TotalUpdateSuccessRate(), itemInstance = equipment , godCharmItem = godCharm}
                //GeneratePlayStreamEvent = true
            };
            PlayFabClientAPI.ExecuteCloudScript(request,
                result =>
                {
                    if (result.FunctionResult.ToString() == "notEnoughVirtualCurrency")
                    {
                        Debug.Log("Not enough VirtualCurrency: " + result.FunctionResult);
                        UIManager.instance.questionDialogUI_.DisplayPurchaseFailed("Không đủ Xu để cường hóa!", () => { }, () => { });
                    }
                    else
                    {
                        InventoryManager.instance.GetVirtualCurrencyUser();
                        Debug.Log("VirtualCurrency: " + result.FunctionResult);
                        txtSuccessRate.text = "0%";
                        txtGoldRequire.text = "0";
                        strengthStoneList.Clear();
                        for (int i = 0; i < strengthStoneSlot.Count; i++)
                        {
                            strengthStoneSlot[i].GetComponent<UpgradeSlotController>().ResetUpgradeSlot();
                        }
                        godCharmSlot.GetComponent<UpgradeSlotController>().ResetUpgradeSlot();
                        var resultUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
                        ItemInstance resultItemInstance = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<ItemInstance>(resultUpgrade["equipment"]);
                        if (resultUpgrade["result"] == "true")                                             //upgrade successful
                        {
                            AudioManager.instance.UpgradeSuccessSFX();
                            StartCoroutine(DisplayedResultUpgrade(true));
                        }
                        else                                                            //upgrade failed
                        {
                            AudioManager.instance.UpgradeFailureSFX();
                            StartCoroutine(DisplayedResultUpgrade(false));
                        }
                        equipment = resultItemInstance;
                        equipmentUpgradeSlot.GetComponent<UpgradeSlotController>().ResetUpgradeSlot();
                        equipmentUpgradeSlot.GetComponent<UpgradeSlotController>().LoadInforItemSlot(resultItemInstance);
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
        upgradeStrengthBtn.GetComponent<Button>().interactable = false;
        openUpgradeBtn.GetComponent<Button>().interactable = false;
        openCombineBtn.GetComponent<Button>().interactable = false;
        alertResult.gameObject.SetActive(true);
        alertResult.GetComponent<Animator>().SetTrigger(_resultUpgrade.ToString());
        yield return new WaitForSeconds(1.5f);
        alertResult.GetComponent<Animator>().ResetTrigger(_resultUpgrade.ToString());
        alertResult.GetComponent<Animator>().SetTrigger("Default");
        yield return new WaitForSeconds(0.5f);
        alertResult.gameObject.SetActive(false);
        upgradeStrengthBtn.GetComponent<Button>().interactable = true;
        openUpgradeBtn.GetComponent<Button>().interactable = true;
        openCombineBtn.GetComponent<Button>().interactable = true;
        closeBtn.GetComponent<Button>().interactable = true;
    }
}