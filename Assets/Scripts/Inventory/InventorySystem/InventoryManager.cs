using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager instance { get; private set; }
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        //LoadDataInventory();
       // GetInventoryUser();
    }
    #endregion

    [SerializeField] private Transform equipmentParent; //Canvas - InventorySystem - Equipments - Viewport - InventoryEquipment
    [SerializeField] private Transform materialParent; //Canvas - InventorySystem - Marterials - Viewport - InventoryMaterial

    private void OnEnable()
    {
        GetInventoryUser();
        GetPlayerStats();
        GetVirtualCurrencyUser();
    }

    //set parent cho inventory (mở trang bị nhân vật thì củn hiển thị kho đồ và set parent là CharacterSystem)
    public void SetParentInventory(Transform inventoryParent)
    {
        transform.SetParent(inventoryParent);
        transform.gameObject.SetActive(!transform.gameObject.activeSelf);
    }

    //--------------------------------------------------------------------------------------
    private Dictionary<string, ItemInstance> userInventory = new Dictionary<string, ItemInstance>();
    private Dictionary<string, int> userVirtualCurrency = new Dictionary<string, int>();
    public Dictionary<string, int> userVirtualCurrency_ => userVirtualCurrency;
    private Dictionary<string, VirtualCurrencyRechargeTime> userVirtualCurrencyRechargeTime = new Dictionary<string, VirtualCurrencyRechargeTime>();
    public Dictionary<string, VirtualCurrencyRechargeTime> userVirtualCurrencyRechargeTime_ => userVirtualCurrencyRechargeTime;

    private Dictionary<string, GameObject> inventorySlotDisplayed = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject inventorySlotPrefab;

    [SerializeField] private Transform tf_currency;


    public void GetInventoryUser()
    {
        AsyncLoadingScene.instance.LoadingScreen(true);
        GetUserInventoryRequest request = new GetUserInventoryRequest{};
        PlayFabClientAPI.GetUserInventory(request,
            result =>
            {
                userInventory.Clear();
                foreach (var item in result.Inventory)
                {
                    
                    if (!userInventory.ContainsKey(item.ItemInstanceId))
                    {
                        userInventory.Add(item.ItemInstanceId, item);
                        Debug.Log("item.ItemInstanceId: " + item.ItemInstanceId + " - " + "item.DisplayName" + item.DisplayName);
                    }
                    else
                    {
                        userInventory[item.ItemInstanceId] = item;
                    }
                }
                UpdateUserInventory();
                AsyncLoadingScene.instance.LoadingScreen(false);
            },
            error => {
                Debug.LogError(error);
                AsyncLoadingScene.instance.LoadingScreen(false);
            });
    }

    public void GetPlayerStats()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getUserData",
            FunctionParameter = new { keys = "playerStats" }
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("GetPlayerStats thành công: " + result.FunctionResult);
                var playerStats = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, int>>(result.FunctionResult.ToString());
                try
                {
                    CharacterEquipmentManager.instance.DisplayPlayerStats(playerStats["atk"], playerStats["def"], playerStats["hp"], playerStats["luk"]);
                }
                catch
                {
                    Debug.Log("KDisplayPlayerStats lỗi");
                }
            },
            error =>
            {
                Debug.Log("GetPlayerStats thất bại!" + error.Error);
            });
    }

    public void GetVirtualCurrencyUser()
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest { };
        PlayFabClientAPI.GetUserInventory(request,
            result =>
            {
                userVirtualCurrency = result.VirtualCurrency;
                userVirtualCurrencyRechargeTime = result.VirtualCurrencyRechargeTimes;
                tf_currency.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0:0,0}", result.VirtualCurrency["GD"]);
                tf_currency.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Format("{0:0,0}", result.VirtualCurrency["MD"]);
            },
            error => {
                Debug.LogError(error);
            });
    }


    public void UpdateUserInventory()
    {
        foreach (var item in userInventory)
        {
            //dùng Dictionary để kiểm tra các vật phẩm đã hiển thị, nếu có key tồn tại (hiển thị rồi) thì chỉ cần cập nhật lại số lượng
            if (inventorySlotDisplayed.ContainsKey(item.Key))
            {
                if(item.Value.ItemClass == "Equipment")
                {
                    var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(item.Value.CustomData["inforBasic02"]);
                    if (inventorySlotDisplayed[item.Key] == null && inforBasic02["equipped"] == "0")
                    {
                        GameObject obj = Instantiate(inventorySlotPrefab, equipmentParent);
                        obj.GetComponent<InventorySlotController>().LoadInforItemSlot(item.Value);
                        inventorySlotDisplayed[item.Key] = obj;
                    }
                }
                else
                {
                    if (inventorySlotDisplayed[item.Key] == null)
                    {
                        GameObject obj = Instantiate(inventorySlotPrefab, materialParent);
                        obj.GetComponent<InventorySlotController>().LoadInforItemSlot(item.Value);
                        inventorySlotDisplayed[item.Key] = obj;
                    }
                    else
                    {
                        inventorySlotDisplayed[item.Key].GetComponentInChildren<TextMeshProUGUI>().text = item.Value.RemainingUses.ToString();
                    }
                    
                }
            }
            else
            {
                if (item.Value.ItemClass == "Equipment")
                {
                    GameObject obj = Instantiate(inventorySlotPrefab, equipmentParent);
                    obj.GetComponent<InventorySlotController>().LoadInforItemSlot(item.Value);
                    var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(item.Value.CustomData["inforBasic02"]);
                    inventorySlotDisplayed.Add(item.Key, obj);
                    if (inforBasic02["equipped"] == "1")
                    {
                        GameObject characterSystemInterface = GameObject.Find("CharacterSystemInterface");
                        if(characterSystemInterface != null && characterSystemInterface.activeInHierarchy)
                        {
                            obj.GetComponent<InventorySlotController>().EquipItem();
                            inventorySlotDisplayed[item.Key] = null;
                        }
                        else
                        {
                            Destroy(obj);
                            inventorySlotDisplayed.Remove(item.Key);
                        }
                    }
                }
                else
                {
                    GameObject obj = Instantiate(inventorySlotPrefab, materialParent);
                    obj.GetComponent<InventorySlotController>().LoadInforItemSlot(item.Value);
                    inventorySlotDisplayed.Add(item.Key, obj);
                }         
            }
        }
    }

    public void AddItem(ItemInstance itemInstance)
    {
        if (!inventorySlotDisplayed.ContainsKey(itemInstance.ItemInstanceId) || inventorySlotDisplayed[itemInstance.ItemInstanceId] == null)
        {
            if (itemInstance.ItemClass == "Equipment")
            {
                GameObject obj = Instantiate(inventorySlotPrefab, equipmentParent);
                obj.GetComponent<InventorySlotController>().LoadInforItemSlot(itemInstance);
                var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic02"]);
                inventorySlotDisplayed[itemInstance.ItemInstanceId] = obj;
            }
            else
            {
                GameObject obj = Instantiate(inventorySlotPrefab, materialParent);
                obj.GetComponent<InventorySlotController>().LoadInforItemSlot(itemInstance);
                //obj.GetComponent<InventorySlotController>().LoadComponents();
                inventorySlotDisplayed[itemInstance.ItemInstanceId] = obj;
            }
        }
        else
        {
            inventorySlotDisplayed[itemInstance.ItemInstanceId].GetComponentInChildren<TextMeshProUGUI>().text = (int.Parse(inventorySlotDisplayed[itemInstance.ItemInstanceId].GetComponentInChildren<TextMeshProUGUI>().text) + 1).ToString();
        }
    }

    public void RemoveInventorySlotDisplayed(ItemInstance itemInstance)
    {
        //inventorySlotDisplayed.Remove(itemInstance);
    }
}

