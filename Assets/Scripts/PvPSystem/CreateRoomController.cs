using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomController : MonoBehaviour
{
    [SerializeField] private string strRoomName;
    [SerializeField] private GameModPvPSO gameModPvPSO;

    [SerializeField] private TMP_InputField inputFieldRoomName;
    [SerializeField] private Button btnCreateRoom;
    [SerializeField] private Button btnDefaultRoom;

    private void OnEnable()
    {
        btnDefaultRoom.onClick.Invoke();
    }

    public void OnChangeRoomName(string roomName)
    {
        strRoomName = roomName;
    }

    public void OnClickCreateRoom()
    {
        if (!string.IsNullOrEmpty(strRoomName) && !PhotonNetwork.InRoom)
        {
            Debug.Log("CreateRoom");
            CreateRoom();
        }
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)gameModPvPSO.intMaxPlayers_,
        };

        ExitGames.Client.Photon.Hashtable RoomCustomProps = new ExitGames.Client.Photon.Hashtable();
        RoomCustomProps.Add("mapPvP", true);
        RoomCustomProps.Add("gameMode", gameModPvPSO.strName_);
        RoomCustomProps.Add("hasTeam", gameModPvPSO.boolHasTeam_);
        RoomCustomProps.Add("teamQuantity", gameModPvPSO.intTeamQuantity_);
        RoomCustomProps.Add("teamMember", gameModPvPSO.intTeamMember_);
        roomOptions.CustomRoomProperties = RoomCustomProps;

        string[] customPropsForLobby = { "mapPvP", "gameMode", "hasTeam", "teamQuantity", "teamSize" };

        roomOptions.CustomRoomPropertiesForLobby = customPropsForLobby;
        Debug.Log(" (byte)gameModPvPSO.intMaxPlayers_: " + gameModPvPSO.intMaxPlayers_);
        PhotonNetwork.CreateRoom(strRoomName, roomOptions, null);
        inputFieldRoomName.text = "";
        transform.gameObject.SetActive(false);
    }

    public void OnClickGameMode(GameModPvPSO gameModPvPSO)
    {
        this.gameModPvPSO = gameModPvPSO;
    }
}
