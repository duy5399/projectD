using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomController : MonoBehaviour
{
    public static WaitingRoomController instance { get; private set; }

    [SerializeField] private PhotonView photonView;
    [SerializeField] private GameModPvPSO gameModPvPSO;
    [SerializeField] private TextMeshProUGUI txtRoomName;
    [SerializeField] private Transform btnReady;
    [SerializeField] private List<Transform> tfPlayerSlot = new List<Transform>();

    private Dictionary<int, string> dictPlayerReady = new Dictionary<int, string>();
    private Dictionary<int, string> dictListPlayer = new Dictionary<int, string>();

    private Dictionary<string, List<string>> dictListPlayerInTeam = new Dictionary<string, List<string>>();
    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        photonView = this.gameObject.GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        if (photonView != null && photonView.IsMine)
        {
            btnReady.GetComponent<Animator>().SetBool("readyBtn", false);
            btnReady.GetComponent<Button>().onClick.AddListener(OnClickStartBtn);
            photonView.RPC(nameof(RPC_ReadyGame), RpcTarget.AllBufferedViaServer, 0);
        }
        else
        {
            btnReady.GetComponent<Animator>().SetBool("readyBtn", true);
            btnReady.GetComponent<Button>().onClick.AddListener(OnClickReadyBtn);
        }
    }

    private void OnDisable()
    {
        if (photonView != null && photonView.IsMine)
        {
            btnReady.GetComponent<Button>().onClick.RemoveListener(OnClickStartBtn);
        }
        else
        {
            btnReady.GetComponent<Button>().onClick.RemoveListener(OnClickReadyBtn);
        }
        dictPlayerReady.Clear();
        dictListPlayer.Clear();
        dictListPlayerInTeam.Clear();
    }


    public void SetRoomName(string roomName)
    {
        txtRoomName.text = "Tên phòng: " + roomName;
    }

    public void AddPlayerToRoom(string playfabID)
    {
        photonView.RPC(nameof(RPC_LoadInfoPlayer), RpcTarget.AllBufferedViaServer, PlayfabDataManager.instance.displayname_, playfabID);
    }

    [PunRPC]
    private void RPC_LoadInfoPlayer(string displayname, string playfabID)
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            if (!dictListPlayer.ContainsKey(i))
            {
                dictListPlayer.Add(i, playfabID);
                tfPlayerSlot[i].GetComponent<PlayerSlotController>().GetUserData(displayname, playfabID);
                break;
            }
        }
        for (int i = 0; i < (int)PhotonNetwork.CurrentRoom.CustomProperties["teamQuantity"] - 1; i++)
        {
            if (!dictListPlayerInTeam.ContainsKey("team" + i))
                dictListPlayerInTeam.Add("team" + i, new List<string> { displayname });
            else
            {
                for (int j = i + 1; j < (int)PhotonNetwork.CurrentRoom.CustomProperties["teamQuantity"]; j++)
                {
                    if (!dictListPlayerInTeam.ContainsKey("team" + j))
                    {
                        dictListPlayerInTeam.Add("team" + j, new List<string> { displayname });
                        return;
                    }
                    else
                    {
                        if (dictListPlayerInTeam["team" + j].Count < dictListPlayerInTeam["team" + i].Count)
                        {
                            dictListPlayerInTeam["team" + j].Add(displayname);
                            return;
                        }
                    }
                }
                dictListPlayerInTeam["team" + i].Add(displayname);
            }
        }
    }

    private void OnClickReadyBtn()
    {
        //photonView.TransferOwnership(PhotonNetwork.PlayerList.FirstOrDefault(x => x.NickName == dictListPlayerInTeam["team0"][0]));
        Debug.Log("dictListPlayerInTeam[\"team0\"][0]: " + dictListPlayerInTeam["team0"][0]);
        int i = dictListPlayer.FirstOrDefault(x => x.Value == PlayfabDataManager.instance.playFabID_).Key;
        photonView.RPC(nameof(RPC_ReadyGame), RpcTarget.AllBufferedViaServer, i);
    }

    private void OnClickStartBtn()
    {
        photonView.RPC(nameof(RPC_StartGame), RpcTarget.AllBufferedViaServer, null);
    }

    [PunRPC]
    public void RPC_ReadyGame(int i)
    {
        foreach (var a in dictListPlayerInTeam)
        {
            foreach (var j in a.Value)
            {
                Debug.Log(a.Key + " : " + j);
            }
        }
        if (!tfPlayerSlot[i].GetComponent<PlayerSlotController>().isReady_)
        {
            tfPlayerSlot[i].GetComponent<PlayerSlotController>().SetReady(true);
            try
            {
                dictPlayerReady.Add(i, "ready");
            }
            catch
            {
                dictPlayerReady[i] = "ready";
            }
            Debug.Log(i + " đã sẵn sàng");
        }
        else
        {
            tfPlayerSlot[i].GetComponent<PlayerSlotController>().SetReady(false);
            try
            {
                dictPlayerReady.Add(i, "notready");
            }
            catch
            {
                dictPlayerReady[i] = "notready";
            }
            Debug.Log(i + " chưa sẵn sàng");
        }
    }

    [PunRPC]
    public void RPC_StartGame()
    {
        foreach(var i in dictListPlayerInTeam)
        {
            foreach(var j in i.Value)
            {
                Debug.Log(i.Key + " : " + j);
            }
        }
        if (dictListPlayer.Count == PhotonNetwork.CurrentRoom.MaxPlayers && dictPlayerReady.Count == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            for (int i = 0; i < dictPlayerReady.Count; i++)
            {
                if (dictPlayerReady[i] != "ready")
                {
                    Debug.Log("Người chơi " + i + " chưa sẵn sàng");
                    return;
                }
            }
            ExitGames.Client.Photon.Hashtable setValue = PhotonNetwork.CurrentRoom.CustomProperties;
            foreach (var i in dictListPlayerInTeam)
            {
                string value = string.Join(",", i.Value);
                setValue.Add(i.Key, value);
                Debug.Log("i.Value " + value);
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(setValue);
            Debug.Log("Bắt đầu trận đấu " + dictListPlayer.Count + " - " + dictPlayerReady.Count);
            photonView.RPC(nameof(RPC_LoadMapPvP), RpcTarget.All, null);
        }
        else
            Debug.Log("Chưa đủ người chơi " + dictListPlayer.Count + " - " + dictPlayerReady.Count);
    }
    [PunRPC]
    private void RPC_LoadMapPvP()
    {
        PhotonNetwork.LoadLevel("PvPMap");
    }

    public void OnClickLeaveBtn()      //nút Leave
    {
        if (photonView != null && photonView.IsMine)
        {
            int i = dictListPlayer.FirstOrDefault(x => x.Value == PlayfabDataManager.instance.playFabID_).Key;
            photonView.RPC(nameof(RPC_ResetDisplayRoom), RpcTarget.AllBufferedViaServer, i);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            photonView.RPC(nameof(RPC_LeaveRoom), RpcTarget.AllBufferedViaServer, null);
        }
        else
        {
            int i = dictListPlayer.FirstOrDefault(x => x.Value == PlayfabDataManager.instance.playFabID_).Key;
            string playfabid = PlayfabDataManager.instance.playFabID_;
            string displayname = PlayfabDataManager.instance.displayname_;
            photonView.RPC(nameof(RPC_RemoveDictListPlayerInTeam), RpcTarget.All, displayname);
            photonView.RPC(nameof(RPC_RemoveDictListPlayer), RpcTarget.All, i);
            photonView.RPC(nameof(RPC_RemoveDictPlayerReady), RpcTarget.All, i);
            photonView.RPC(nameof(RPC_ResetDisplayRoom), RpcTarget.AllBufferedViaServer, i);
            PhotonNetwork.LeaveRoom();
            transform.gameObject.SetActive(false);  
        }
    }

    [PunRPC]
    private void RPC_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        transform.gameObject.SetActive(false);
    }

    [PunRPC]
    private void RPC_ResetDisplayRoom(int i)
    {
        tfPlayerSlot[i].GetComponent<PlayerSlotController>().ResetImgShow();
    }

    [PunRPC]
    private void RPC_RemoveDictListPlayer(int i)
    {
        dictListPlayer.Remove(i);
    }

    [PunRPC]
    private void RPC_RemoveDictPlayerReady(int i)
    {
        dictPlayerReady.Remove(i);
    }

    [PunRPC]
    private void RPC_RemoveDictListPlayerInTeam(string displayname)
    {
        foreach (var list in dictListPlayerInTeam.Values)
        {
            list.Remove(displayname);
        }
    }
}
