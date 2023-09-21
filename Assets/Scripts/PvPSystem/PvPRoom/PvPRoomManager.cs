using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PvPRoomManager : MonoBehaviourPunCallbacks
{
    public static PvPRoomManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("hasTeam"))
        //{
        //    foreach(var player in PhotonNetwork.CurrentRoom.Players)
        //    {
        //        if (PhotonNetwork.CurrentRoom.CustomProperties["team0"].ToString().Contains(player.Value.NickName))
        //        {
        //            GameObject obj = player.Value.TagObject as GameObject;
        //            obj.GetComponent<PlayerCombat>().tfHealth.GetComponent<Image>().color = Color.blue;
        //            Debug.Log("start pvp");
        //        }
        //        else if (PhotonNetwork.CurrentRoom.CustomProperties["team1"].ToString().Contains(player.Value.NickName))
        //        {
        //            GameObject obj = player.Value.TagObject as GameObject;
        //            obj.GetComponent<PlayerCombat>().tfHealth.GetComponent<Image>().color = Color.red;
        //            Debug.Log("start pvp");
        //        }
        //    }
        //}
    }

    public override void OnEnable()
    {
        base.OnEnable();
        LoadTeamList();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        dictTeamList.Clear();
        dictTeamScore.Clear();
    }

    private void Update()
    {
        CheckPlayersInRoom();
    }

    private Dictionary<string, List<string>> dictTeamList = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> dictTeamList_ => dictTeamList;
    private Dictionary<string, int> dictTeamScore = new Dictionary<string, int>();

    [Header("Scoreboard")]
    [SerializeField] private Transform tfScoreboard;
    [SerializeField] private TextMeshProUGUI txtScoreTeamA;
    [SerializeField] private TextMeshProUGUI txtScoreTeamB;
    [SerializeField] private TextMeshProUGUI txtTimer;
    [SerializeField] private float floatTimer;


    [Header("SpawnPanel")]
    [SerializeField] private Transform tfSpawnPanel;
    [SerializeField] private TextMeshProUGUI txtSpawnTime;
    [SerializeField] private float floatSpawnTime = 5f;

    [Header("ResultPanel")]
    [SerializeField] private bool boolHasResultBattle;
    [SerializeField] private Transform tfResultPanel;

    void LoadTeamList()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("hasTeam"))
        {
            for (int i = 0; i < (int)PhotonNetwork.CurrentRoom.CustomProperties["teamQuantity"]; i++)
            {
                string[] arrListStr = PhotonNetwork.CurrentRoom.CustomProperties["team"+i].ToString().Split(',');
                List<string> listPlayerList = new List<string>();
                foreach (string player in arrListStr)
                {
                    listPlayerList.Add(player);
                }
                dictTeamList.Add("team" + i, listPlayerList);
            }
        }

        foreach(var i in dictTeamList)
        {
            foreach (var j in i.Value)
            {
                Debug.Log("dictTeamList: " + i.Key + " " + j);
            }
        }
    }

    IEnumerator LoadSpawnPanel()
    {
        tfSpawnPanel.gameObject.SetActive(true);
        floatSpawnTime = 5f;
        while (floatSpawnTime > 0)
        {
            floatSpawnTime -= Time.deltaTime;
            txtSpawnTime.text = (int)floatSpawnTime + "s";
            yield return null;
        }
        tfSpawnPanel.gameObject.SetActive(false);
        GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        player.GetComponent<PlayerStats>().LoadStats();
        player.GetComponent<PlayerCombat>().Resurrect();
    }

    public void LoadSpawnPanel(string photonName)
    {
        if (photonName == PhotonNetwork.NickName)
        {
            StartCoroutine(LoadSpawnPanel());
        }
    }

    void UpdateScoreboard()
    {
        foreach (var team in dictTeamList)
        {
            Debug.Log("UpdateScoreboard team " + team.Key + " - " + team.Value.ToStringFull());
            int score = 0;
            Debug.Log("UpdateScoreboard score " + score);
            foreach (var player in team.Value)
            {
                Debug.Log("UpdateScoreboard player " + player);
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("kills"))
                {
                    Dictionary<string, int> dict = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["kills"];
                    if (dict.ContainsKey(player))
                        score += dict[player];
                }
            }
            if(!dictTeamScore.ContainsKey(team.Key))
                dictTeamScore.Add(team.Key, score);
            else
                dictTeamScore[team.Key] = score;
            switch (team.Key)
            {
                case "team0":
                    txtScoreTeamA.text = score.ToString();
                    Debug.Log("team0: " + score);
                    break;
                case "team1":
                    txtScoreTeamB.text = score.ToString();
                    Debug.Log("team1: " + score);
                    break;
            }
        }
    }

    void CheckPlayersInRoom()
    {
        if (!boolHasResultBattle)
        {
            foreach (var team in dictTeamList)
            {
                int numberPlayers = team.Value.Count;
                foreach (var player in team.Value)
                {
                    var playerTemp = PhotonNetwork.CurrentRoom.Players.FirstOrDefault(x => x.Value.NickName == player).Value;
                    if (playerTemp == null)
                        numberPlayers--;
                }
                if (numberPlayers <= 0)
                {
                    boolHasResultBattle = true;
                    tfResultPanel.gameObject.SetActive(true);
                    tfResultPanel.GetComponent<ResultPanelController>().LoadImgResult(dictTeamList.FirstOrDefault(x => x.Value.Contains(PhotonNetwork.CurrentRoom.Players.First().Value.NickName)).Key);
                }
            }
        }
    }

    void CheckingTheWinCondition()
    {
        if (!boolHasResultBattle && dictTeamScore.Values.Contains(2))
        {
            boolHasResultBattle = true;
            tfResultPanel.gameObject.SetActive(true);
            tfResultPanel.GetComponent<ResultPanelController>().LoadImgResult(dictTeamScore.FirstOrDefault(x => x.Value == 2).Key); 
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        UpdateScoreboard();
        CheckingTheWinCondition();
        try
        {
            Dictionary<string, int> dict = (Dictionary<string, int>)PhotonNetwork.CurrentRoom.CustomProperties["kills"];
            foreach (var i in dict)
                Debug.Log("OnRoomPropertiesUpdate fds: " + i.Key + " - " + i.Value);
        }
        catch
        {
            Debug.Log("lỗi");
        }
    }
}
