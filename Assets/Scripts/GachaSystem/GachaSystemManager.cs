using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaSystemManager : MonoBehaviour
{
    [SerializeField] private List<Transform> lst_rewards;
    [SerializeField] private Transform tf_keyRequirements;
    [SerializeField] private Transform tf_rewardResult;
    [SerializeField] private Button btn_startSpin;

    private Dictionary<string, StoreItem> dict_rewards = new Dictionary<string, StoreItem>();

    public void GetRandomRewardsList(string catalogVersion, string storeID)
    {
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
        {
            StoreId = storeID
        },
        result =>
        {
            foreach (StoreItem item in result.Store)
            {
                dict_rewards.Add(item.ItemId, item);
            }
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }


}
