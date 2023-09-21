using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrivateChatSlotController : MonoBehaviour
{
    [SerializeField] private FriendInfo friendInfo;

    [SerializeField] private Image imgAvatar;
    [SerializeField] private Image imgAvatarBorder;
    [SerializeField] private TextMeshProUGUI txtDisplayName;
    //[SerializeField] private TextMeshProUGUI txtStatus;
    //[SerializeField] private Image imgStatus;

    [SerializeField] private Button btnPrivateChat;

    [SerializeField] private int unreadMsg = 0;

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        btnPrivateChat.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = unreadMsg == 0 ? "Chat riêng" : "Chat riêng (" + unreadMsg + ")";
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        unreadMsg = 0;
    }

    public void SetUnreadMsg()
    {
        unreadMsg++;
    }

    public void LoadComponent()
    {
        txtDisplayName = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        //imgStatus = transform.GetChild(2).GetChild(0).GetComponent<Image>();
        //txtStatus = transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        btnPrivateChat = transform.GetChild(4).GetComponent<Button>();
        btnPrivateChat.gameObject.SetActive(false);
    }

    public void LoadInfo(FriendInfo friendInfo)
    {
        this.friendInfo = friendInfo;
        txtDisplayName.text = friendInfo.TitleDisplayName;
        LoadAction(friendInfo);
    }

    public void LoadStatus(int status)
    {
        //txtStatus.text = status == 0 ? "0ffline" : "Online";
        //imgStatus.color = status == 0 ? Color.black : new Color32(0, 255, 70, 255);
    }

    public void LoadAction(FriendInfo friendInfo)
    {
        //gừi lời mời kết bạn
        btnPrivateChat.gameObject.SetActive(true);
        btnPrivateChat.onClick.AddListener(PrivateChat);
    }

    void PrivateChat()
    {
        Debug.Log("Private chat to: " + friendInfo.TitleDisplayName);
        PhotonChatManager.instance.SetPrivateReceiver(friendInfo.TitleDisplayName);
        PhotonChatManager.instance.ShowHideWorldChatDisplayBtnOnClick(false);
        PhotonChatManager.instance.ShowHidePrivateChatListBtnOnClick(false);
        PhotonChatManager.instance.ShowHidePrivateChatDisplayBtnOnClick(true);
        PhotonChatManager.instance.LoadHistoryChatAnotherUser(friendInfo);
        PhotonChatManager.instance.InteracableChatInput(true);
    }

    public void DestroyPrivateChartSlot()
    {
        PhotonChatManager.instance.RemoveDictPrivateChatList(friendInfo);
        Destroy(gameObject);
    }

    public void LoadAvatar(FriendInfo friendInfo)
    {
        GetUserDataRequest request = new GetUserDataRequest()
        {
            PlayFabId = friendInfo.FriendPlayFabId,
            Keys = new List<string>() { "infoAvatar" }
        };
        PlayFabClientAPI.GetUserData(request,
            result => {
                if (result.Data == null || !result.Data.ContainsKey("infoAvatar"))
                {
                    Debug.Log("Không lấy được thông tin avatar");
                }
                else
                {
                    var infoAvatar = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.Data["infoAvatar"].Value);
                    imgAvatar.sprite = AvatarSystemManager.instance.avatarDatabase_.avatarDB_.Find(x => x.name == infoAvatar["avatar"]);
                    imgAvatarBorder.sprite = AvatarSystemManager.instance.avatarDatabase_.avatarBorderDB_.Find(x => x.name == infoAvatar["avatarBorder"]);
                    Debug.Log("Tải avatar thành công");
                }
            }, (error) => {
                Debug.Log(error.GenerateErrorReport());
            });
    }
}
