using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Purchasing.MiniJSON;

public class LeaderboardInfoCharacterController : MonoBehaviour
{
    public static LeaderboardInfoCharacterController instance { get; private set; }

    [SerializeField] private TextMeshProUGUI txt_playerName;

    [SerializeField] private TextMeshProUGUI txt_statATK;
    [SerializeField] private TextMeshProUGUI txt_statDEF;
    [SerializeField] private TextMeshProUGUI txt_statHP;
    [SerializeField] private TextMeshProUGUI txt_statLUK;

    [SerializeField] private Transform tf_headSlot;
    [SerializeField] private Transform tf_hairSlot;
    [SerializeField] private Transform tf_glassSlot; 
    [SerializeField] private Transform tf_faceSlot;
    [SerializeField] private Transform tf_armletSlot;
    [SerializeField] private Transform tf_ringSlot;
    [SerializeField] private Transform tf_clothSlot;
    [SerializeField] private Transform tf_wingSlot;
    [SerializeField] private Transform tf_weaponSlot;
    [SerializeField] private Transform tf_offhandSlot;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void OnDisable()
    {
        ResetLeaderboardInfoCharacter();
    }

    public void GetUserData(string playFabId, string name)
    {
        ResetLeaderboardInfoCharacter();
        txt_playerName.text = name;
        GetUserDataRequest request = new GetUserDataRequest()
        {
            PlayFabId = playFabId,
            Keys = new List<string>() { "playerStats", "equippedItems" }
        };
        PlayFabClientAPI.GetUserReadOnlyData(request,
            result => {
                if (result.Data == null || !result.Data.ContainsKey("playerStats") || !result.Data.ContainsKey("equippedItems"))
                    Debug.Log("No Ancestor");
                else
                {
                    var playerStats = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, int>>(result.Data["playerStats"].Value);
                    var equippedItems = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, ItemInstance>>(result.Data["equippedItems"].Value);
                    SetPlayerStat(playerStats["atk"], playerStats["def"], playerStats["hp"], playerStats["luk"]);
                    foreach(var item in equippedItems)
                    {
                        LoadInfoItem(item.Key, item.Value);
                    }
                }
            }, (error) => {
                Debug.Log(error.GenerateErrorReport());
            });
    }


    void LoadInfoItem(string slot, ItemInstance itemInstance)
    {
        switch(slot)
        {
            case "head":
                tf_headSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
            case "hair":
                tf_hairSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
            case "glass":
                tf_glassSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
            case "face":
                tf_faceSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
            case "armlet":
                tf_armletSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
            case "ring":
                tf_ringSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
            case "cloth":
                tf_clothSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
            case "wing":
                tf_wingSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
            case "weapon":
                tf_weaponSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
            case "offhand":
                tf_offhandSlot.GetComponent<LeaderboardInforCharacterSlotController>().LoadInforItemSlot(itemInstance);
                break;
        }
    }

    void ResetLeaderboardInfoCharacter()
    {
        tf_headSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
        tf_hairSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
        tf_glassSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
        tf_faceSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
        tf_armletSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
        tf_ringSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
        tf_clothSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
        tf_wingSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
        tf_weaponSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
        tf_offhandSlot.GetComponent<LeaderboardInforCharacterSlotController>().ResetInfoCharacterSlot();
    }


    void SetPlayerStat(int statATK, int statDEF, int statHP, int statLUK)
    {
        txt_statATK.text = statATK.ToString();
        txt_statDEF.text = statDEF.ToString();
        txt_statHP.text = statHP.ToString();
        txt_statLUK.text = statLUK.ToString();
    }
}
