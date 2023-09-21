using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DailyLoginRewardSystemManager : MonoBehaviour
{
    [SerializeField] private Dictionary<string , GameObject> dailyLoginRewardTable = new Dictionary<string , GameObject>();
    [SerializeField] private Transform tf_dailyRewardContent;
    [SerializeField] private GameObject go_dailyRewardSlot;
    [SerializeField] private bool bl_getGetDailyLoginRewardTable = false;
    [SerializeField] private int z = 0;


    public void Awake()
    {
        //GetDailyLoginRewardTable();
    }

    public void GetDailyLoginRewardTable()
    {
        if(bl_getGetDailyLoginRewardTable == false)
        {
            z++;
            GetTitleDataRequest request = new GetTitleDataRequest()
            {
                Keys = new List<string>() { "DailyLoginRewardTable" }
            };
            PlayFabClientAPI.GetTitleData(request,
                result =>
                {
                    if (result != null && result.Data.ContainsKey("DailyLoginRewardTable"))
                    {
                        var value = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.Data["DailyLoginRewardTable"]);
                        foreach (var i in value)
                        {
                            var reward = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(i.Value);

                            GameObject obj = Instantiate(go_dailyRewardSlot, tf_dailyRewardContent);
                            obj.GetComponent<DailyLoginRewardSlotController>().LoadInforItemSlot(PlayfabDataManager.instance.catalogItemsDictionary_[reward["name"]], reward["quantity"]);
                            dailyLoginRewardTable.Add(i.Key, obj);
                        }
                        GetDailyLogin();
                        bl_getGetDailyLoginRewardTable = true;
                    }
                    else
                        Debug.Log("TitleData không chứa DailyLoginRewardTable");
                },
                error =>
                {
                    Debug.Log("GetTitleData thất bại");
                });
        }
    }

    public void GetDailyLogin()
    {
        GetUserDataRequest request = new GetUserDataRequest()
        {
            Keys = new List<string>() { "dailyLogin" }
        };
        PlayFabClientAPI.GetUserReadOnlyData(request,
            result => {
                if (result.Data == null || !result.Data.ContainsKey("dailyLogin"))
                    Debug.Log("No Ancestor");
                else
                {
                    
                    var dailyLogin = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, bool>>(result.Data["dailyLogin"].Value);
                    foreach (var item in dailyLogin)
                    {
                        Debug.Log("dailyLogin: " + item.Key + " - " + item.Value);
                    }
                    int currentDay = DateTime.Now.Day;
                    Debug.Log("currentDay: " + currentDay);
                    for(int i = 1; i <= currentDay; i++)
                    {
                        if (dailyLogin["day" + i] == false)
                        {
                            if (i == currentDay)
                                dailyLoginRewardTable["day" + i].GetComponent<DailyLoginRewardSlotController>().OnBorder(true);
                            else
                                dailyLoginRewardTable["day" + i].GetComponent<DailyLoginRewardSlotController>().OnMark(false);
                        }
                        else
                            dailyLoginRewardTable["day" + i].GetComponent<DailyLoginRewardSlotController>().OnMark(true);
                    }
                }
            }, (error) => {
                Debug.Log(error.GenerateErrorReport());
            });
    }
}
