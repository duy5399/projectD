using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected Image imgBorder;
    [SerializeField] protected Image imgIcon;
    [SerializeField] protected TextMeshProUGUI txtQuantity;
    [SerializeField] protected ItemInstance itemInstance;
    public ItemInstance ItemInstance_ => itemInstance;

    protected virtual void Awake()
    {
        LoadComponents();
    }

    public virtual void LoadComponents()
    {
        imgBorder = transform.GetChild(0).GetComponent<Image>();
        imgIcon = transform.GetChild(1).GetComponent<Image>();
        txtQuantity = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public virtual void LoadInforItemSlot(ItemInstance itemInstance )
    {
        this.itemInstance = itemInstance;
        var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic01"]);
        var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic02"]);
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
                Debug.Log("Not found rarity tier of item: " + itemInstance.ItemId);
                break;
        }
        txtQuantity.text = PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].ItemClass == "Equipment" ? "" : itemInstance.RemainingUses.ToString();
    }

    public async void EquipItem()
    {
        AsyncLoadingScene.instance.LoadingScreen(true);
        UIManager.instance.inventoryDesciptionController_.HiddenDescription();
        try
        {
            CharacterEquipmentManager.instance.EquipGear(itemInstance);
        }
        catch
        {
            Debug.Log("CharacterEquipmentManager.instance.EquipGear(itemInstance) lỗi");
        }
        await Task.Delay(1000);
        var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic02"]);
        if (inforBasic02["equipped"] == "0")
        {
            Debug.Log("ChangeEquipStatusOfItem: " + inforBasic02["equipped"]);
            ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
            {
                FunctionName = "ChangeEquipStatusOfItem",
                FunctionParameter = new { itemInstanceId = itemInstance.ItemInstanceId }
            };
            PlayFabClientAPI.ExecuteCloudScript(request,
                result =>
                {
                    GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
                    player.GetComponent<PlayerStats>().GetPlayerStat();
                    Debug.Log("Trang bị thành công: " + result.FunctionResult.ToString());
                    var playerStats = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, int>>(result.FunctionResult.ToString());
                    CharacterEquipmentManager.instance.DisplayPlayerStats(playerStats["atk"], playerStats["def"], playerStats["hp"], playerStats["luk"]);
                },
                error =>
                {
                    Debug.Log("Trang bị thất bại!" + error.Error);
                });
        }
        AsyncLoadingScene.instance.LoadingScreen(false);
        Destroy(gameObject);
    }

    public void SellItem()
    {
        AsyncLoadingScene.instance.LoadingScreen(true);
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "SellItemFromInventoryUser",
            FunctionParameter = new { itemInstanceId = itemInstance.ItemInstanceId, currency = "GD", amount = (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].VirtualCurrencyPrices["GD"]/10) }
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                AsyncLoadingScene.instance.LoadingScreen(false);
                Debug.Log("Bán vật phẩm thành công: " + result.FunctionResult);
                UIManager.instance.inventoryDesciptionController_.HiddenDescription();                
                UIManager.instance.questionDialogUI_.DisplayPurchaseSuccesful("Bán vật phẩm thành công, " + (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].VirtualCurrencyPrices["GD"] / 10).ToString() + "GD đã được chuyển vào túi đồ!", () => { }, () => { });
                InventoryManager.instance.GetVirtualCurrencyUser();
                Destroy(gameObject);
            },
            error =>
            {
                AsyncLoadingScene.instance.LoadingScreen(false);
                Debug.Log("Bán vật phẩm thất bại!");
            });       
    }

    public void UpgradeItem()
    {
        UIManager.instance.inventoryDesciptionController_.HiddenDescription();
        if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].ItemClass == "Equipment")
        {
            var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic02"]);
            if (inforBasic02["equipped"] == "0")
            {
                UpgradeEquipmentManager.instance.AddEquipmentUpgrade(itemInstance);
                Destroy(gameObject);
            }
        }
        else
        {
            if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags[0] == "upgrade")
            {
                if (itemInstance.ItemId == "godCharm_01")
                {
                    UpgradeEquipmentManager.instance.AddGodCharm(itemInstance);
                    if (int.Parse(txtQuantity.text) > 1)
                        txtQuantity.text = (int.Parse(txtQuantity.text) - 1).ToString();
                    else
                        Destroy(gameObject);
                }
                else
                {
                    if (UpgradeEquipmentManager.instance.AddStrengthStone(itemInstance))
                    {
                        if (int.Parse(txtQuantity.text) > 1)
                            txtQuantity.text = (int.Parse(txtQuantity.text) - 1).ToString();
                        else
                            Destroy(gameObject);
                    }
                }
            }
        }
    }

    public void CombineItem()
    {
        UIManager.instance.inventoryDesciptionController_.HiddenDescription();
        if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].ItemClass == "Equipment")
        {
            CombineItemManager.instance.AddItem(itemInstance);
            Destroy(gameObject);
        }
        else
        {
            if (CombineItemManager.instance.AddItem(itemInstance))
            {
                if (int.Parse(txtQuantity.text) > 1)
                    txtQuantity.text = (int.Parse(txtQuantity.text) - 1).ToString();
                else
                    Destroy(gameObject);
            }
        }
    }

    protected virtual void SetDataForItem(string itemInstance, string itemID, string tier, string mainStat, string iconImg, string showImg)
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

    protected virtual void SetDataForItem(string itemInstance, string data)
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

    //display description of item
    public virtual void DisplayItemDescription()
    {
        //UIManager.instance.inventoryDesciptionController_.SetDescription(item);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick"); 
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AudioManager.instance.EquipGearSFX();
            GameObject characterSystemInterface = GameObject.Find("CharacterSystemInterface");
            GameObject upgradeSystemInterface = GameObject.Find("UpgradeSystemInterface");

            if (characterSystemInterface != null && characterSystemInterface.activeInHierarchy)
            {
                if (itemInstance.ItemClass == "Equipment")
                    UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, () => { EquipItem(); }, null, () => { SellItem(); }, null, null, null);
                else
                    UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, null, () => { SellItem(); }, null, null, null);
            }
            else if (upgradeSystemInterface != null && upgradeSystemInterface.activeInHierarchy)
            {
                GameObject upgradeInterface = GameObject.Find("UpgradeInterface");
                GameObject combineInterface = GameObject.Find("CombineInterface");
                if (upgradeInterface != null && upgradeInterface.activeInHierarchy)
                {
                    var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforUpgrade"]);
                    var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic02"]);
                    if(PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].ItemClass == "Equipment")
                    {
                        if (inforUpgrade["canUpgrade"] == "1" && int.Parse(inforUpgrade["strenghLv"]) < 16)
                            UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, null, null, () => { UpgradeItem(); }, null, null);
                        else
                            UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, null, null, null, null, null);
                    }
                    else
                    {
                        if (PlayfabDataManager.instance.catalogItemsDictionary_[itemInstance.ItemId].Tags[0] == "upgrade")
                            UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, null, null, () => { UpgradeItem(); }, null, null);
                        else
                            UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, null, null, null, null, null);
                    }                    
                }
                else if (combineInterface != null && combineInterface.activeInHierarchy)
                {
                    var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforUpgrade"]);
                    var inforBasic02 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(itemInstance.CustomData["inforBasic02"]);
                    if (inforUpgrade["canCombine"] == "1" && inforBasic02["tier"] != "mythic")
                        UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, null, null, null, null, () => { CombineItem(); });
                    else
                        UIManager.instance.inventoryDesciptionController_.DescriptionItem(itemInstance, null, null, null, null, null, null);
                }
            }

        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
            //{
            //    FunctionName = "ChangeEquippedPlayerData",
            //    //FunctionParameter = new { itemInstanceId = itemInstance.ItemInstanceId }
            //    //GeneratePlayStreamEvent = true
            //};
            //PlayFabClientAPI.ExecuteCloudScript(request,
            //    result =>
            //    {
            //        Debug.Log("ChangeEquippedPlayerData thành công: ");
            //    },
            //    error =>
            //    {
            //        Debug.Log("ChangeEquippedPlayerData thất bại!" + error.Error);
            //    });
        }
    }

    public virtual void OnLeftClick()
    {
        DisplayItemDescription();
    }

    public virtual void OnRightClick()
    {
        //Debug.Log("OnRightClick");
    }
}
