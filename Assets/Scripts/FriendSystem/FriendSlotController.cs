using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FriendSlotController : MonoBehaviour
{
    [SerializeField] private Image imgAvatar;
    [SerializeField] private Image imgAvatarBorder;
    [SerializeField] private TextMeshProUGUI txtDisplayName;
    [SerializeField] private TextMeshProUGUI txtStatus;
    [SerializeField] private Image imgStatus;
    [SerializeField] private Button btnAddFriend;
    [SerializeField] private Button btnAcceptFriend;
    [SerializeField] private Button btnDeclineFriend;
    [SerializeField] private Button btnPrivateChat;
    [SerializeField] private Button btnDeleteFriend;

    [SerializeField] private FriendInfo friendInfo;
    [SerializeField] private UserAccountInfo userAccountInfo;
    public FriendInfo friendInfo_ => friendInfo;
    private void Awake()
    {
        
    }

    public void LoadComponent()
    {
        btnAddFriend.gameObject.SetActive(false);
        btnAcceptFriend.gameObject.SetActive(false);
        btnDeclineFriend.gameObject.SetActive(false);
        btnPrivateChat.gameObject.SetActive(false);
        btnDeleteFriend.gameObject.SetActive(false);
    }

    public void LoadInfo(FriendInfo friendInfo)
    {
        this.friendInfo = friendInfo;
        txtDisplayName.text = friendInfo.TitleDisplayName;
        LoadAction(friendInfo);
    }

    public void LoadStatus(int status)
    {
        txtStatus.text = status == 0 ? "Offline" : "Online";
        imgStatus.color = status == 0 ? Color.black : new Color32(0, 255, 70, 255);
    }

    public void LoadInfo(UserAccountInfo userAccountInfo)
    {
        this.userAccountInfo = userAccountInfo;
        txtDisplayName.text = userAccountInfo.TitleInfo.DisplayName;
        imgStatus.enabled = false;
        txtStatus.enabled = false;
        LoadAction(userAccountInfo);
    }

    public void LoadAction(FriendInfo friendInfo)
    {
        if(friendInfo.Tags[0] == "confirmed")
        {
            //chat riêng
            btnPrivateChat.gameObject.SetActive(true);
            btnPrivateChat.onClick.AddListener(PrivateChat);
            //xóa bạn
            btnDeleteFriend.gameObject.SetActive(true);
            btnDeleteFriend.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Xóa bạn";
            btnDeleteFriend.onClick.AddListener(RemoveFriend);
        }
        else if (friendInfo.Tags[0] == "newRequest")
        {
            //chấp nhận lời mời
            btnAcceptFriend.gameObject.SetActive(true);
            btnAcceptFriend.onClick.AddListener(AcceptFriend);
            //từ chối lời mời
            btnDeclineFriend.gameObject.SetActive(true);
            btnDeclineFriend.onClick.AddListener(RemoveFriend);
        }
        else if(friendInfo.Tags[0] == "waitingAccept")
        {
            //hủy lời mời
            btnDeleteFriend.gameObject.SetActive(true);
            btnDeleteFriend.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Hủy kết bạn";
            btnDeleteFriend.onClick.AddListener(RemoveFriend);
        }
    }

    public void LoadAction(UserAccountInfo userAccountInfo)
    {
        //gừi lời mời kết bạn
        btnAddFriend.gameObject.SetActive(true);
        btnAddFriend.onClick.AddListener(SendFriendRequest);
    }

    //gừi lời mời kết bạn
    void SendFriendRequest()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "SendFriendRequest",
            FunctionParameter = new { friendPlayFabId = userAccountInfo.PlayFabId }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("Gửi lời mời kết bạn thành công");
                Destroy(gameObject);
            },
            error =>
            {
                Debug.Log("Gửi lời mời kết bạn thất bại");
            });
    }

    //chấp nhận lời mời
    void AcceptFriend()
    {
        Debug.Log("AcceptFriend onclick");
        AsyncLoadingScene.instance.LoadingScreen(true);
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "AcceptFriendRequest",
            FunctionParameter = new { friendPlayFabId = friendInfo.FriendPlayFabId }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("Chấp nhận lời mời kết bạn thành công");
                AsyncLoadingScene.instance.LoadingScreen(false);
                Destroy(gameObject);
            },
            error =>
            {
                Debug.Log("Chấp nhận lời mời kết bạn thất bại");
                AsyncLoadingScene.instance.LoadingScreen(false);
            });
    }

    //xóa bạn hoặc từ chối lời mời kết bạn
    void RemoveFriend()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "DeclineFriendRequest",
            FunctionParameter = new { friendPlayFabId = friendInfo.FriendPlayFabId }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("Xóa bạn thành công");
                Destroy(gameObject);
            },
            error =>
            {
                Debug.Log("Xóa bạn thất bại");
            });
    }

    void PrivateChat()
    {
        Debug.Log("Private chat to: " + friendInfo.TitleDisplayName);
        PhotonChatManager.instance.LoadPrivateChatList(friendInfo);
        PhotonChatManager.instance.OpenPrivateChat();
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

    public void LoadAvatar(UserAccountInfo userAccountInfo)
    {
        GetUserDataRequest request = new GetUserDataRequest()
        {
            PlayFabId = userAccountInfo.PlayFabId,
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
