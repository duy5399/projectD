using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PvPSystemManager : MonoBehaviourPunCallbacks
{
    public static PvPSystemManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        AsyncLoadingScene.instance.LoadingScreen(true);
        if (!PhotonNetwork.IsConnected)
            ConnectToPhotonPUN();
    }

    private void Update()
    {
        if (boolInLobby && !boolIsSpawnPlayer)
        {
            Debug.Log("CreateOrJoinRoom");
            CreateOrJoinRoom();
        }
    }

    [SerializeField] private GameObject goPlayer;

    [SerializeField] private bool boolInLobby;
    [SerializeField] private bool boolIsSpawnPlayer;

    [SerializeField] private Button btnReconnect;
    [SerializeField] private Transform tfWaitingRoom;
    [SerializeField] private GameObject goRoomSlot;
    [SerializeField] private Transform tfRoomListContent;
    private Dictionary<RoomInfo, GameObject> dictRoomList = new Dictionary<RoomInfo, GameObject>();


    public void OnClick_OpenPvPSystem()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("OnClick_OpenPvPSystem");
    }

    public void OnClick_ClosePvPSystem()
    {
        boolIsSpawnPlayer = false;
        Debug.Log("OnClick_OpenPvPSystem");
    }

    public void SetBoolIsSpawnPlayer(bool boolIsSpawnPlayer)
    {
        this.boolIsSpawnPlayer = boolIsSpawnPlayer;
    }

    public void JoinedRoom()
    {
        //print("Room Joned Sucess");
        Debug.Log("OnJoinedRoom");
        tfWaitingRoom.gameObject.SetActive(true);
        WaitingRoomController.instance.SetRoomName(PhotonNetwork.CurrentRoom.Name);
        WaitingRoomController.instance.AddPlayerToRoom(PlayfabDataManager.instance.playFabID_);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach (var room in roomList)
        {
            if (!dictRoomList.ContainsKey(room) && !room.RemovedFromList)
            {
                GameObject obj = Instantiate(goRoomSlot, tfRoomListContent);
                obj.GetComponent<RoomSlotController>().LoadInfoRoom(room);
                dictRoomList.Add(room, obj);
            }
            else
            {
                if (room.RemovedFromList)
                {
                    try
                    {
                        Destroy(dictRoomList[room]);
                        dictRoomList.Remove(room);
                    }
                    catch
                    {
                        Debug.Log("Lỗi xóa phòng");
                    }
                }
                else
                {
                    dictRoomList[room].GetComponent<RoomSlotController>().LoadInfoRoom(room);
                }
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------

    private void SpawnPlayer()
    {
        Vector3 position = new Vector3(UnityEngine.Random.Range(-10f, 10f), -2, 0);
        PhotonNetwork.Instantiate(Path.Combine("Prefabs/Player", goPlayer.name), position, goPlayer.transform.rotation);
    }

    public void SetNamePlayer()
    {
        PhotonNetwork.NickName = PlayfabDataManager.instance.displayname_;
    }

    private void CreateOrJoinRoom()
    {
        boolIsSpawnPlayer = true;
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = false,
            IsOpen = true,
            MaxPlayers = (byte)10,
        };
        ExitGames.Client.Photon.Hashtable RoomCustomProps = new ExitGames.Client.Photon.Hashtable();
        RoomCustomProps.Add("mapPvP", false);
        roomOptions.CustomRoomProperties = RoomCustomProps;

        string[] customPropsForLobby = { "mapPvP" };
        PhotonNetwork.JoinOrCreateRoom("Homepage", roomOptions, null);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gameMode"))
        {
            JoinedRoom();
        }
        else
        {
            SpawnPlayer();
            SetNamePlayer();
            AsyncLoadingScene.instance.LoadingScreen(false);
        }
    }

    public void CreateRoom(string roomName, RoomOptions roomOptions)
    {
        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }
    //-------------------------------------------------------------------------------

    private void ConnectToPhotonPUN()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("ConnectToPhotonPUN");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby(TypedLobby.Default); //khi kết nối thành công tới Photon Server (Master) thì sẽ kết nối vào Lobby (sảnh chờ) 
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (!boolInLobby && !boolIsSpawnPlayer)
            ConnectToPhotonPUN();
    }

    public override void OnJoinedLobby() //hàm được gọi khi dòng PhotonNetwork.JoinLobby(TypedLobby.Default); thực hiện thành công
    {
        Debug.Log("OnJoinedLobby");
        boolInLobby = true;
    }

    public override void OnCreatedRoom()
    {
        //SpawnPlayer();
        //SetNamePlayer();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //CreateOrJoinRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //CreateOrJoinRoom();
    }
}
