using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;

public class PlayfabDataManager : MonoBehaviour
{
    public static PlayfabDataManager instance { get; private set; }

    private Dictionary<string, CatalogItem> catalogItemsDictionary = new Dictionary<string, CatalogItem>();
    private Dictionary<string, Dictionary<string, StoreItem>> listStoresItemsDictionary = new Dictionary<string, Dictionary<string, StoreItem>>();
    private Dictionary<string, StoreItem> listDiscountItemsDictionary = new Dictionary<string, StoreItem>();
    public Dictionary<string, CatalogItem> catalogItemsDictionary_ => catalogItemsDictionary;
    public Dictionary<string, Dictionary<string, StoreItem>> listStoresItemsDictionary_ => listStoresItemsDictionary;
    public Dictionary<string, StoreItem> listDiscountItemsDictionary_ => listDiscountItemsDictionary;

    [SerializeField] private string displayname;
    [SerializeField] private string playFabID;

    [SerializeField] private UserAccountInfo userAccountInfo;
    public UserAccountInfo userAccountInfo_ => userAccountInfo;
    public string displayname_ => displayname;
    public string playFabID_ => playFabID;

    void Awake()
    {
        if(instance == null && instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    //lấy dữ liệu-------------------------------------------------------------------------------------------------------------------------------------
    public void GetUserData(string key)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getUserData",
            FunctionParameter = new { keys = key }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            GetUserDataResult =>
            {
                if(GetUserDataResult == null)
                {
                    Debug.Log("Get User Data Result: null");

                }                    
                else
                {
                    Debug.Log("Get User Data Result: " + GetUserDataResult.FunctionResult);
                }
                //Debug.Log(JsonWrapper.SerializeObject(GetUserDataResult.FunctionResult));
            },
            GetUserDataError =>
            {
                Debug.Log("Get User Data Error: " + GetUserDataError.Error);
            });
    }

    //set dữ liệu-------------------------------------------------------------------------------------------------------------------------------------
    public void SetUserData(string key, string value)
    {
        Dictionary<string, string> data = new Dictionary<string, string> { { key, value } };
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "setUserData",
            FunctionParameter = new { data }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            SetUserDataResult =>
            {
                if (SetUserDataResult.FunctionResult.ToString() == "true")
                {
                    Debug.Log("Set User Data Result: " + SetUserDataResult.FunctionResult);
                }
                else
                {
                    Debug.Log("Set User Data Fail: " + SetUserDataResult.FunctionResult);
                }
            },
            SetUserDataError =>
            {
                Debug.Log("Set User Data Error: " + SetUserDataError.Error);
            });
    }

    //lấy dữ liệu catalog items (client)-------------------------------------------------------------------------------------------------------------------------------------
    public void GetCatalogItems(string catalogVersion)
    {
        GetCatalogItemsRequest request = new GetCatalogItemsRequest { CatalogVersion = catalogVersion };
        PlayFabClientAPI.GetCatalogItems(request,
            GetCatalogItemsResult =>
            {
                foreach (CatalogItem item in GetCatalogItemsResult.Catalog)
                {
                    //Debug.Log("catalogItemsDictionary: " + item.ItemId);
                    catalogItemsDictionary.Add(item.ItemId, item);
                    var customdata = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(item.CustomData);
                    var inforBasic01 = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["inforBasic01"]);
                    var inforUpgrade = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(customdata["inforUpgrade"]);
                }
            },
            GetCatalogItemsError =>
            {
                Debug.Log(GetCatalogItemsError.GenerateErrorReport());
            });
    }

    //lấy dữ liệu cửa hàng vật phẩm-------------------------------------------------------------------------------------------------------------------------------------
    public void GetStoreItems(string catalogVersion, string storeID)
    {
        GetCatalogItemsRequest request = new GetCatalogItemsRequest { CatalogVersion = catalogVersion };
        PlayFabClientAPI.GetCatalogItems(request,
            GetCatalogItemsResult =>
            {
                PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
                {
                    StoreId = storeID
                },
                GetStoreItemsResult =>
                {
                    Dictionary<string, StoreItem> storeItemsDictionary = new Dictionary<string, StoreItem>();
                    foreach (StoreItem item in GetStoreItemsResult.Store)
                    {
                        storeItemsDictionary.Add(item.ItemId, item);
                        //Debug.Log("ItemId:" + item.ItemId + " - VirtualCurrencyPrices:" + item.VirtualCurrencyPrices + " - DisplayName:" + catalogItemsDictionary[item.ItemId].DisplayName);
                    }
                    listStoresItemsDictionary.Add(storeID, storeItemsDictionary);
                    //if (listStoresItemsDictionary.ContainsKey(storeID))
                    //{
                    //    Dictionary<string, StoreItem> storeItemsDictionary = listStoresItemsDictionary[storeID];
                    //    Debug.Log("---------- " + storeItemsDictionary["weapon_01"].VirtualCurrencyPrices["GD"]);
                    //    Debug.Log("++++++++++ " + listStoresItemsDictionary[storeID]["weapon_01"].VirtualCurrencyPrices["GD"]); ;
                    //}
                },
                GetStoreItemsError =>
                {
                    Debug.Log(GetStoreItemsError.GenerateErrorReport());
                });
            },
            GetCatalogItemsError =>
            {
                Debug.Log(GetCatalogItemsError.GenerateErrorReport());
            }) ;
    }

    //lấy dữ liệu cửa hàng vật phẩm giảm giá ngẫu nhiên-------------------------------------------------------------------------------------------------------------------------------------
    public void GetDiscountStoreData(string catalogVersion)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetDiscountStoreData"
            //FunctionParameter = new { storeID = "store_01" }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                if(result != null)
                {
                    ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
                    {
                        FunctionName = "GetItemDiscountStore"
                        //FunctionParameter = new { storeID = "store_01" }
                        //GeneratePlayStreamEvent = true
                    };
                    PlayFabClientAPI.ExecuteCloudScript(request,
                        result =>
                        {
                            //Debug.Log("discountStoreData: " + result.FunctionResult.ToString());
                            try
                            {
                                listDiscountItemsDictionary = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, StoreItem>>(result.FunctionResult.ToString());
                            }
                            catch
                            {
                                Debug.Log("Không lấy được lấy dữ liệu cửa hàng vật phẩm giảm giá ngẫu nhiên");
                            }
                            //foreach (var item in listDiscountItemsDictionary)
                            //{
                            //    Debug.Log("listDiscountItemsDictionary: " + item.Key + " : " + item.Value.ItemId);
                            //}
                        },
                        error =>
                        {
                            Debug.Log("GetCurrentTime Error: " + error.Error);
                        });
                }
                else
                {
                    Debug.Log("Get Discount Store Data Result: null");
                }
            },
            error =>
            {
                Debug.Log("Get Discount Store Data Error: " + error.Error);
            });
    }
    public void DeleteItemDiscountStore(string itemID)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "DeleteItemDiscountStore",
            FunctionParameter = new { id = itemID }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                if( result.FunctionResult.ToString() == "1" )
                    Debug.Log("DeleteItemDiscountStore: 1");
                else
                    Debug.Log("DeleteItemDiscountStore: 0");
            },
            error =>
            {
                Debug.Log("DeleteItemDiscountStore Error: " + error.Error);
            });
    }

    public void Test(string id)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetItemData01",
            FunctionParameter = new {  itemID = id }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("Test: " + result.FunctionResult);
                if (result.FunctionResult != null)
                    Debug.Log("Test: 1" + result.FunctionResult);
                else
                    Debug.Log("test: 0");
            },
            error =>
            {
                Debug.Log("test Error: " + error.Error);
            });
    }

    public void GetDisplayname(string displayname)
    {
        this.displayname = displayname;
    }
    public void GetPlayFabID(string playFabID)
    {
        this.playFabID = playFabID;
    }

    public void GetUserAccountInfo(UserAccountInfo userAccountInfo)
    {
        this.userAccountInfo = userAccountInfo;
    }

    
}
