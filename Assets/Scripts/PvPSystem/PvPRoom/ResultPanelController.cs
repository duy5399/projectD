using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class ResultPanelController : MonoBehaviour
{
    public static ResultPanelController instance { get; private set; }

    private void Awake()
    {
        if (instance != null & instance != this)
            Destroy(this);
        else
            instance = this;
    }

    [SerializeField] private Image imgResultBatlle;
    [SerializeField] private Sprite spriteVictory;
    [SerializeField] private Sprite spriteLose;

    [SerializeField] private GameObject goDetailBattlePvPSlot;
    [SerializeField] private Transform tfResultTeamA;
    [SerializeField] private Transform tfResultTeamB;

    [SerializeField] private Button btnOK;

    private void OnEnable()
    {
        LoadInfo();
        btnOK.onClick.AddListener(OnClickBackToWaitingRoom);
    }

    private void OnDisable()
    {
        btnOK.onClick.RemoveListener(OnClickBackToWaitingRoom);
        gameObject.SetActive(false);
    }

    public void LoadImgResult(string teamVictory)
    {
        if (PvPRoomManager.instance.dictTeamList_[teamVictory].Contains(PhotonNetwork.LocalPlayer.NickName))
        {
            imgResultBatlle.sprite = spriteVictory;
            UpdateTopPvPLeaderboard(10);
        }
        else
        {
            imgResultBatlle.sprite = spriteLose;
            UpdateTopPvPLeaderboard(5);
        }
    }

    private void LoadInfo()
    {
        foreach (var team in PvPRoomManager.instance.dictTeamList_)
        {
            foreach(var player in team.Value)
            {
                if (player != null)
                {
                    switch(team.Key)
                    {
                        case "team0":
                            InstantiateDetailSlot(player, tfResultTeamA);
                            break;
                        case "team1":
                            InstantiateDetailSlot(player, tfResultTeamB);
                            break;
                    }
                }
            }
        }
    }

    void InstantiateDetailSlot(string player, Transform parent)
    {
        GameObject obj = Instantiate(goDetailBattlePvPSlot, parent);
        Dictionary<string, int> dictKills = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["kills"];
        Dictionary<string, int> dictDamageDealt = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["damageDealt"];
        Dictionary<string, int> dictHealing = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["healing"];
        string playerName = player;
        string kills = "0";
        string damageDealt = "0";
        string healing = "0";
        if(dictKills != null)
            kills = dictKills.ContainsKey(player) ? dictKills[player].ToString() : "0";
        if (dictDamageDealt != null)
            damageDealt = dictDamageDealt.ContainsKey(player) ? dictDamageDealt[player].ToString() : "0";
        if(dictHealing != null)
            healing = dictHealing.ContainsKey(player) ? dictHealing[player].ToString() : "0";
        obj.GetComponent<DetailBattlePvPSlotController>().LoadInfo(playerName, kills, damageDealt, healing);
    }

    void OnClickBackToWaitingRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Homepage");
    }

    private void UpdateTopPvPLeaderboard(int scorePvP)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdateStatistics",
            FunctionParameter = new { statisticName = "TopPvP", value = scorePvP }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                //PlayerStats.instance.GetPlayerStat();
                Debug.Log("UpdateStatistics thành công: " + result.FunctionResult);
            },
            error =>
            {
                Debug.Log("UpdateStatistics thất bại!" + error.Error);
            });
    }
}
