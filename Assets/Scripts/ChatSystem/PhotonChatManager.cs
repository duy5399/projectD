using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    public static PhotonChatManager instance { get; private set; }

    [SerializeField] private ChatClient chatClient;
    [SerializeField] private bool isConnected;
    [SerializeField] private List<string> friends = new List<string>();

    [SerializeField] private string username;
    [SerializeField] private string playfabID;

    [SerializeField] private string currentChat;
    [SerializeField] private string currentChannel;

    [SerializeField] private string privateReceiver;
    private Dictionary<string, GameObject> dict_PrivateChatList = new Dictionary<string, GameObject>();
    [SerializeField] private GameObject go_PrivateChatSlot;

    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Transform tf_WorldChatDisplay;
    [SerializeField] private Transform tf_PrivateChatDisplay;
    [SerializeField] private Transform tf_PrivateChatList;

    [SerializeField] private Button btn_worldChat;
    [SerializeField] private Button btn_partyChat;
    [SerializeField] private Button btn_privateChat;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void OnEnable()
    { 
        btn_worldChat.onClick.Invoke();
    }

    public void AddFriendToPhotonChat(string friendName)
    {
        Debug.Log("AddFriendToPhotonChat(string friendName): " + friendName);
        friends.Add(friendName);
    }

    public void AddFriendToPhotonChat()
    {
        Debug.Log("AddFriendToPhotonChat(string friendName)");
        chatClient.AddFriends(friends.ToArray());
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        Debug.Log("Chat Connected");
        chatClient.Subscribe(new string[] { "WorldChannel" });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
        chatClient.SetOnlineStatus(ChatUserStatus.Offline);
    }

    public void SetStstus()
    {
        Debug.Log("SetStstus");
        chatClient.SetOnlineStatus(ChatUserStatus.Offline);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for(int i = 0; i < senders.Length; i++)
        {
            //Debug.Log("senders[i] == username: " + senders[i] + " " + username);
            if (senders[i] == username)
            {
                msgs = "<align=\"right\">" + messages[i] + " :" + "<b><color=#9800FF>" + senders[i] + "</color></b></align>";
            }
            else
            {
                msgs = "<b><color=#00FAFF>[TG]" + senders[i] + "</color></b>" + ": " + messages[i];
            }
            //msgs  = "<b><color=#FF0000>" + senders[i] + "</color></b>" + ": " + messages[i];
            tf_WorldChatDisplay.GetComponent<TextMeshProUGUI>().text += "\n" + msgs;
        }
        Debug.Log("Sender: " + senders);
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if(!dict_PrivateChatList.ContainsKey(sender) && sender != username)
        {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
            {
                XboxToken = null
            }, result => {
                LoadPrivateChatList(result.Friends.Find(x => x.TitleDisplayName == sender));
                Debug.Log("LoadPrivateChatList(result.Friends.Find(x => x.TitleDisplayName == sender));");
            }, error =>
            {
                //Debug.Log(error);
            });
        }

        string msgs = "";
        if (sender == username)
        {
            msgs = "<align=\"right\">" + message + " :" + "<b><color=#E6FF00>" + sender + "</color></b></align>";
        }
        else
        {
            msgs = "<b><color=#ff7abf>[CN]" + sender + "</color></b>" + ": " + message;
        }
        tf_PrivateChatDisplay.GetComponent<TextMeshProUGUI>().text += "\n" + msgs;
        Debug.Log("channelName: " + channelName);
    }
    
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        FriendSystemManager.instance.SetFriendsStatusUpdated(user, status);
        Debug.Log("Status change for: " + user + " to: " + status);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
       //throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isConnected)
        {
            ChatConnect();
        }
        else
        {
            chatClient.Service();
        }
    }

    public void ChatConnect()
    {
        username = PlayfabDataManager.instance.displayname_;
        playfabID = PlayfabDataManager.instance.playFabID_;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(username));
        isConnected = true;
        Debug.Log("Chat Connecting");
    }

    public void UsernameOnValueChange(string username)
    {
        this.username = username;
    }

    public void ChannelChatOnValueChange(string channel)
    {
        currentChannel = channel;
    }

    public void InputChatOnValueChange(string valueInput)
    {
        currentChat = valueInput;
    }

    public void SetPrivateReceiver(string receiver)
    {
        privateReceiver = receiver;
    }

    public void SubmitChatOnClick()
    {
        if (currentChat.Length == 0 || currentChat.Length > 50)
        {
            UIManager.instance.questionDialogUI_.DisplayPurchaseSuccesful("Nội dung tin nhắn giới hạn trong khoảng 0 - 50 kí tự!", () => { }, () => { });
            return;
        }
        else
        {
            if(currentChannel == "WorldChannel")
            {
                chatClient.PublishMessage("WorldChannel", currentChat);
                chatInput.text = "";
                currentChat = "";
            }  
            else if(currentChannel == "PartyChannel")
            {

            }
            else
            {
                if (privateReceiver != "")
                {
                    chatClient.SendPrivateMessage(privateReceiver, currentChat);
                    chatInput.text = "";
                    currentChat = "";
                }
            }
        }
    }

    public void LoadHistoryChatAnotherUser(FriendInfo friendInfo)
    {
        string channelName = username + ":" + friendInfo.TitleDisplayName;
        try
        {
            tf_PrivateChatDisplay.GetComponent<TextMeshProUGUI>().text = "";
            ChatChannel ch = this.chatClient.PrivateChannels[channelName];
            for (int i = 0; i < ch.Messages.Count; i++)
            {
                string msgs = "";
                //msgs = "<b><color=#9800FF>" + username + "</color></b>" + ": " + msg;

                if (ch.Senders[i] == username)
                {
                    msgs = "<align=\"right\">" + ch.Messages[i] + " :" + "<b><color=#E6FF00>[" + ch.Senders[i] + "]</color></b></align>";
                }
                else
                {
                    msgs = "<b><color=#ff7abf>[CN]" + ch.Senders[i] + "</color></b>" + ": " + ch.Messages[i];
                }

                tf_PrivateChatDisplay.GetComponent<TextMeshProUGUI>().text += "\n" + msgs;
                //Debug.Log("msg: " + msg);
            }
        }
        catch
        {
            Debug.Log("Không tìm thấy channel private chat giữa " + username + " và " + friendInfo.TitleDisplayName);
        }
    }

    public void LoadPrivateChatList(FriendInfo friendInfo)
    {
        if (!dict_PrivateChatList.ContainsKey(friendInfo.TitleDisplayName))
        {
            GameObject obj = GameObject.Instantiate(go_PrivateChatSlot, tf_PrivateChatList);
            //obj.GetComponent<PrivateChatSlotController>().LoadComponent();
            obj.GetComponent<PrivateChatSlotController>().LoadInfo(friendInfo);
            obj.GetComponent<PrivateChatSlotController>().LoadAvatar(friendInfo);
            dict_PrivateChatList.Add(friendInfo.TitleDisplayName, obj);
        }
    }

    public void OpenPrivateChat()
    {
        transform.GetComponent<Button>().onClick.Invoke();
        btn_privateChat.onClick.Invoke();
    }

    public void RemoveDictPrivateChatList(FriendInfo friendInfo)
    {
        dict_PrivateChatList.Remove(friendInfo.TitleDisplayName);
    }

    public void ShowHideWorldChatDisplayBtnOnClick(bool boolean)
    {
        tf_WorldChatDisplay.gameObject.SetActive(boolean);
    }

    public void ShowHidePrivateChatDisplayBtnOnClick(bool boolean)
    {
        tf_PrivateChatDisplay.gameObject.SetActive(boolean);
    }

    public void ShowHidePrivateChatListBtnOnClick(bool boolean)
    {
        tf_PrivateChatList.gameObject.SetActive(boolean);
    }

    public void InteracableChatInput(bool boolean)
    {
        chatInput.interactable = boolean;
    }
}
