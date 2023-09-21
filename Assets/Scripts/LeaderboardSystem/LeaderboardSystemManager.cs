using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using TMPro;

public class LeaderboardSystemManager : MonoBehaviour
{
    [SerializeField] private List<PlayerLeaderboardEntry> playerLeaderboardEntry;
    [SerializeField] private List<PlayerLeaderboardEntry> previousPlayerLeaderboardEntry;
    [SerializeField] private StatisticValue statisticValueOfMyself;
    [SerializeField] private int currentVersion;
    [SerializeField] private int previousVersion = -1;
    [SerializeField] private string str_typeOfLeaderboard;
    [SerializeField] private Transform tfLeaderboardScrollView;

    private Dictionary<int, GameObject> dict_leaderboardTopPvPDisplayed = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> dict_leaderboardTopAtkDisplayed = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> dict_leaderboardTopDefDisplayed = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> dict_leaderboardTopHpDisplayed = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> dict_leaderboardTopLukDisplayed = new Dictionary<int, GameObject>();

    private Dictionary<int, GameObject> dict_previousLeaderboardTopPvPDisplayed = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> dict_previousLeaderboardTopAtkDisplayed = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> dict_previousLeaderboardTopDefDisplayed = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> dict_previousLeaderboardTopHpDisplayed = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> dict_previousLeaderboardTopLukDisplayed = new Dictionary<int, GameObject>();

    private Dictionary<string, Dictionary<CatalogItem, string>> dict_rewardTopPvP = new Dictionary<string, Dictionary<CatalogItem, string>>();
    private Dictionary<string, Dictionary<CatalogItem, string>> dict_rewardTopATK = new Dictionary<string, Dictionary<CatalogItem, string>>();
    private Dictionary<string, Dictionary<CatalogItem, string>> dict_rewardTopDEF = new Dictionary<string, Dictionary<CatalogItem, string>>();
    private Dictionary<string, Dictionary<CatalogItem, string>> dict_rewardTopHP = new Dictionary<string, Dictionary<CatalogItem, string>>();
    private Dictionary<string, Dictionary<CatalogItem, string>> dict_rewardTopLUK = new Dictionary<string, Dictionary<CatalogItem, string>>();

    [SerializeField] private TextMeshProUGUI txt_nextResetTime;
    [SerializeField] private TextMeshProUGUI txt_previousNextResetTime;
    [SerializeField] private GameObject go_leaderboardSlot;
    [SerializeField] private Transform tf_leaderboardSlotOfMySelf;

    [SerializeField] private Transform tf_topPvPContent;
    [SerializeField] private Transform tf_topAtkContent;
    [SerializeField] private Transform tf_topDefContent;
    [SerializeField] private Transform tf_topHPContent;
    [SerializeField] private Transform tf_topLukContent;

    [SerializeField] private Transform tf_previousTopPvPContent;
    [SerializeField] private Transform tf_previousTopAtkContent;
    [SerializeField] private Transform tf_previousTopDefContent;
    [SerializeField] private Transform tf_previousTopHPContent;
    [SerializeField] private Transform tf_previousTopLukContent;

    [SerializeField] private Button btn_topPvP;
    [SerializeField] private Button btn_topATK;
    [SerializeField] private Button btn_topDEF;
    [SerializeField] private Button btn_topHP;
    [SerializeField] private Button btn_topLUK;


    private void Awake()
    {
        GetRewardsOfLeaderboard();
    }

    public void OpenLeaderboard()
    {
        btn_topPvP.onClick.Invoke();
    }

    public void GetLeaderboard(string statisticName, Dictionary<int, GameObject> dictionary, Transform transform)
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest{
            StatisticName = statisticName,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request,
            result =>{
                currentVersion = result.Version;
                playerLeaderboardEntry = result.Leaderboard;
                txt_nextResetTime.text = result.NextReset.ToString();
                UpdateLeaderboard(playerLeaderboardEntry, dictionary, transform, statisticName);
                Debug.Log("result.NextReset.ToString(): " + result.NextReset.ToString());
            },
            error => Debug.Log(error.GenerateErrorReport())
        );
    }

    private void UpdateLeaderboard(List<PlayerLeaderboardEntry> listPlayerLeaderboardEntry, Dictionary<int, GameObject> dictionary, Transform transform, string statisticName)
    {
        bool myRankInLeaderboard = false;
        foreach(var entry in listPlayerLeaderboardEntry)
        {
            if (dictionary.ContainsKey(entry.Position))
            {
                dictionary[entry.Position].GetComponent<LeaderboardSlotController>().LoadInfo(entry);
            }
            else
            {
                GameObject obj = Instantiate(go_leaderboardSlot, transform);
                obj.GetComponent<LeaderboardSlotController>().LoadInfo(entry);
                obj.GetComponent<LeaderboardSlotController>().LoadRewards(GetRewardWithRank(entry.Position, statisticName));
                dictionary.Add(entry.Position, obj);
            }
            if(entry.PlayFabId == PlayfabDataManager.instance.playFabID_)
            {
                tf_leaderboardSlotOfMySelf.GetComponent<LeaderboardSlotController>().LoadInfo(entry);
                tf_leaderboardSlotOfMySelf.GetComponent<LeaderboardSlotController>().LoadRewards(GetRewardWithRank(entry.Position, statisticName));
                myRankInLeaderboard = true;
            }
            Debug.Log(entry.DisplayName + " - " + entry.Position);
        }
        if(!myRankInLeaderboard)
        {
            tf_leaderboardSlotOfMySelf.GetComponent<LeaderboardSlotController>().LoadInfo(PlayfabDataManager.instance.displayname_);
        }
    }

    public void TypeOfLeaderboardOnChange(string value)
    {
        str_typeOfLeaderboard = value;
    }

    public void BtnTopPvPOnClick()
    {
        GetLeaderboard("TopPvP", dict_leaderboardTopPvPDisplayed, tf_topPvPContent);
        tf_topPvPContent.gameObject.SetActive(true);
        tf_topAtkContent.gameObject.SetActive(false);
        tf_topDefContent.gameObject.SetActive(false);
        tf_topHPContent.gameObject.SetActive(false);
        tf_topLukContent.gameObject.SetActive(false);
        tfLeaderboardScrollView.GetComponent<ScrollRect>().content = tf_topPvPContent.GetComponent<RectTransform>();
    }

    public void BtnTopAtkOnClick()
    {
        GetLeaderboard("TopATK", dict_leaderboardTopAtkDisplayed, tf_topAtkContent);
        tf_topPvPContent.gameObject.SetActive(false);
        tf_topAtkContent.gameObject.SetActive(true);
        tf_topDefContent.gameObject.SetActive(false);
        tf_topHPContent.gameObject.SetActive(false);
        tf_topLukContent.gameObject.SetActive(false);
        tfLeaderboardScrollView.GetComponent<ScrollRect>().content = tf_topAtkContent.GetComponent<RectTransform>();
    }
    public void BtnTopDefOnClick()
    {
        GetLeaderboard("TopDEF", dict_leaderboardTopDefDisplayed, tf_topDefContent);
        tf_topPvPContent.gameObject.SetActive(false);
        tf_topAtkContent.gameObject.SetActive(false);
        tf_topDefContent.gameObject.SetActive(true);
        tf_topHPContent.gameObject.SetActive(false);
        tf_topLukContent.gameObject.SetActive(false);
        tfLeaderboardScrollView.GetComponent<ScrollRect>().content = tf_topDefContent.GetComponent<RectTransform>();
    }
    public void BtnTopHpOnClick()
    {
        GetLeaderboard("TopHP", dict_leaderboardTopHpDisplayed, tf_topHPContent);
        tf_topPvPContent.gameObject.SetActive(false);
        tf_topAtkContent.gameObject.SetActive(false);
        tf_topDefContent.gameObject.SetActive(false);
        tf_topHPContent.gameObject.SetActive(true);
        tf_topLukContent.gameObject.SetActive(false);
        tfLeaderboardScrollView.GetComponent<ScrollRect>().content = tf_topHPContent.GetComponent<RectTransform>();
    }
    public void BtnTopLukOnClick()
    {
        GetLeaderboard("TopLUK", dict_leaderboardTopLukDisplayed, tf_topLukContent);
        tf_topPvPContent.gameObject.SetActive(false);
        tf_topAtkContent.gameObject.SetActive(false);
        tf_topDefContent.gameObject.SetActive(false);
        tf_topHPContent.gameObject.SetActive(false);
        tf_topLukContent.gameObject.SetActive(true);
        tfLeaderboardScrollView.GetComponent<ScrollRect>().content = tf_topLukContent.GetComponent<RectTransform>();
    }

    public void GetLeaderboard(string statisticName, Dictionary<int, GameObject> dictionary, Transform transform, int version)
    {
        Debug.Log("BtnPreviousLeaderboardOnClick");
        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            MaxResultsCount = 2,
            Version = version
        };
        PlayFabClientAPI.GetLeaderboard(request,
            result => {
                previousPlayerLeaderboardEntry = result.Leaderboard;
                var time = result.NextReset.Value.AddHours(-1);
                txt_previousNextResetTime.text = "TG kết thúc: " + time.ToString();
                UpdateLeaderboard(previousPlayerLeaderboardEntry, dictionary, transform, statisticName);
            },
            error => Debug.Log(error.GenerateErrorReport())
        );
    }

    public void BtnPreviousLeaderboardOnClick()
    {
        if(previousVersion != currentVersion -1)
        {
            previousVersion = currentVersion - 1;
            switch (str_typeOfLeaderboard)
            {
                case "TopPvP":
                    GetLeaderboard("TopPvP", dict_previousLeaderboardTopPvPDisplayed, tf_previousTopPvPContent, previousVersion);
                    tf_previousTopPvPContent.gameObject.SetActive(true);
                    tf_previousTopAtkContent.gameObject.SetActive(false);
                    tf_previousTopDefContent.gameObject.SetActive(false);
                    tf_previousTopHPContent.gameObject.SetActive(false);
                    tf_previousTopLukContent.gameObject.SetActive(false);
                    break;
                case "TopATK":
                    GetLeaderboard("TopATK", dict_previousLeaderboardTopAtkDisplayed, tf_previousTopAtkContent, previousVersion);
                    tf_previousTopPvPContent.gameObject.SetActive(false);
                    tf_previousTopAtkContent.gameObject.SetActive(true);
                    tf_previousTopDefContent.gameObject.SetActive(false);
                    tf_previousTopHPContent.gameObject.SetActive(false);
                    tf_previousTopLukContent.gameObject.SetActive(false);
                    break;
                case "TopDEF":
                    GetLeaderboard("TopDEF", dict_previousLeaderboardTopDefDisplayed, tf_previousTopDefContent, previousVersion);
                    tf_previousTopPvPContent.gameObject.SetActive(false);
                    tf_previousTopAtkContent.gameObject.SetActive(false);
                    tf_previousTopDefContent.gameObject.SetActive(true);
                    tf_previousTopHPContent.gameObject.SetActive(false);
                    tf_previousTopLukContent.gameObject.SetActive(false);
                    break;
                case "TopHP":
                    GetLeaderboard("TopHP", dict_previousLeaderboardTopHpDisplayed, tf_previousTopHPContent, previousVersion);
                    tf_previousTopPvPContent.gameObject.SetActive(false);
                    tf_previousTopAtkContent.gameObject.SetActive(false);
                    tf_previousTopDefContent.gameObject.SetActive(false);
                    tf_previousTopHPContent.gameObject.SetActive(true);
                    tf_previousTopLukContent.gameObject.SetActive(false);
                    break;
                case "TopLUK":
                    GetLeaderboard("TopLUK", dict_previousLeaderboardTopLukDisplayed, tf_previousTopLukContent, previousVersion);
                    tf_previousTopPvPContent.gameObject.SetActive(false);
                    tf_previousTopAtkContent.gameObject.SetActive(false);
                    tf_previousTopDefContent.gameObject.SetActive(false);
                    tf_previousTopHPContent.gameObject.SetActive(false);
                    tf_previousTopLukContent.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void GetRewardsOfLeaderboard()
    {
        GetTitleDataRequest request = new GetTitleDataRequest()
        {
            Keys = new List<string>() { "TopPvPPrizes", "TopATKPrizes", "TopDEFPrizes", "TopHPPrizes", "TopLUKPrizes" }
        };
        PlayFabClientAPI.GetTitleData(request,
            result =>
            {
                if(result != null)
                {
                    foreach (var i in result.Data)
                    {
                        Debug.Log("GetRewardsOfLeaderboard: " + i.Key + " - " + i.Value);
                        var i_value = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(i.Value);
                        foreach (var j in i_value)
                        {
                            var j_value = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(j.Value);
                            Dictionary<CatalogItem, string> dict_catalogItem = new Dictionary<CatalogItem, string>();
                            foreach (var k in j_value)
                            {
                                var k_value = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(k.Value);
                                dict_catalogItem.Add(PlayfabDataManager.instance.catalogItemsDictionary_[k_value["name"]], k_value["quantity"]);
                            }
                            switch (i.Key)
                            {
                                case "TopPvPPrizes":
                                    Debug.Log("ccase TopPvPPrizes" + j.Key);
                                    dict_rewardTopPvP.Add(j.Key, dict_catalogItem);
                                    break;
                                case "TopATKPrizes":
                                    Debug.Log("ccase TopATKPrizes" + j.Key);
                                    dict_rewardTopATK.Add(j.Key, dict_catalogItem);
                                    break;
                                case "TopDEFPrizes":
                                    Debug.Log("ccase TopDEFPrizes" + j.Key);
                                    dict_rewardTopDEF.Add(j.Key, dict_catalogItem);
                                    break;
                                case "TopHPPrizes":
                                    Debug.Log("ccase TopHPPrizes" + j.Key);
                                    dict_rewardTopHP.Add(j.Key, dict_catalogItem);
                                    break;
                                case "TopLUKPrizes":
                                    Debug.Log("ccase TopLUKPrizes" + j.Key);
                                    dict_rewardTopLUK.Add(j.Key, dict_catalogItem);
                                    break;
                            }
                        }
                    }
                    Debug.Log("GetTitleData thành công");
                }
                else
                    Debug.Log("GetTitleData không chứa các key: TopATKPrizes, TopDEFPrizes, TopHPPrizes, TopLUKPrizes");
            },  
            error =>
            {
                Debug.Log("GetTitleData thất bại");
            });
    }

    Dictionary<CatalogItem, string> GetRewardWithRank(int rank, string statisticName)
    {
        switch (rank)
        {
            case 0:
                switch (statisticName)
                {
                    case "TopPvP":
                        return dict_rewardTopPvP["top1"];
                    case "TopATK":
                        return dict_rewardTopATK["top1"];
                    case "TopDEF":
                        return dict_rewardTopDEF["top1"];
                    case "TopHP":
                        return dict_rewardTopHP["top1"];
                    case "TopLUK":
                        return dict_rewardTopLUK["top1"];
                }
                break;
            case 1:
                switch (statisticName)
                {
                    case "TopPvP":
                        return dict_rewardTopPvP["top2"];
                    case "TopATK":
                        return dict_rewardTopATK["top2"];
                    case "TopDEF":
                        return dict_rewardTopDEF["top2"];
                    case "TopHP":
                        return dict_rewardTopHP["top2"];
                    case "TopLUK":
                        return dict_rewardTopLUK["top2"];
                }
                break;
            case 2:
                switch (statisticName)
                {
                    case "TopPvP":
                        return dict_rewardTopPvP["top3"];
                    case "TopATK":
                        return dict_rewardTopATK["top3"];
                    case "TopDEF":
                        return dict_rewardTopDEF["top3"];
                    case "TopHP":
                        return dict_rewardTopHP["top3"];
                    case "TopLUK":
                        return dict_rewardTopLUK["top3"];
                }
                break;
            case 3:
            case 4:
                switch (statisticName)
                {
                    case "TopPvP":
                        return dict_rewardTopPvP["top4_5"];
                    case "TopATK":
                        return dict_rewardTopATK["top4_5"];
                    case "TopDEF":
                        return dict_rewardTopDEF["top4_5"];
                    case "TopHP":
                        return dict_rewardTopHP["top4_5"];
                    case "TopLUK":
                        return dict_rewardTopLUK["top4_5"];
                }
                break;
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
                switch (statisticName)
                {
                    case "TopPvP":
                        return dict_rewardTopPvP["top6_10"];
                    case "TopATK":
                        return dict_rewardTopATK["top6_10"];
                    case "TopDEF":
                        return dict_rewardTopDEF["top6_10"];
                    case "TopHP":
                        return dict_rewardTopHP["top6_10"];
                    case "TopLUK":
                        return dict_rewardTopLUK["top6_10"];
                }
                break;
        }
        return null;
    }
}
