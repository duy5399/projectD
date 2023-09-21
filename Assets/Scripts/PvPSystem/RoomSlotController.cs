using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomSlotController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RoomInfo roomInfo;
    [SerializeField] private TextMeshProUGUI txtRoomName;
    [SerializeField] private TextMeshProUGUI txtTypeRoom;
    [SerializeField] private TextMeshProUGUI txtCurrentPlayer;

    public void LoadInfoRoom(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        txtRoomName.text = roomInfo.Name;
        txtTypeRoom.text = roomInfo.CustomProperties["gameMode"].ToString();
        txtCurrentPlayer.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PhotonNetwork.JoinRoom(roomInfo.Name, null);
        }
    }
}
