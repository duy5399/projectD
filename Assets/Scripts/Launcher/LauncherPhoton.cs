using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LauncherPhoton : MonoBehaviourPunCallbacks
{
    public static LauncherPhoton instance;
    [SerializeField] private GameObject goPlayer;
    [SerializeField] private PhotonView pv;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
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
        if (PhotonNetwork.IsMasterClient)
        {
              pv.RPC("RPC_LoadHome", RpcTarget.AllBufferedViaServer, null);
        }
        //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gameMode"))
        //{

        //}
        //else
        //{
        //    SpawnPlayer();
        //    SetNamePlayer();
        //}
    }

    public void CreateRoom(string roomName, RoomOptions roomOptions)
    {
        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }
    //-------------------------------------------------------------------------------

    public void ConnectToPhotonPUN()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
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
        ConnectToPhotonPUN();
    }

    public override void OnJoinedLobby() //hàm được gọi khi dòng PhotonNetwork.JoinLobby(TypedLobby.Default); thực hiện thành công
    {
        Debug.Log("OnJoinedLobby");
        CreateOrJoinRoom();
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

    [PunRPC]
    public void RPC_LoadHome()
    {
        PhotonNetwork.LoadLevel("Homepage");
    }
}
