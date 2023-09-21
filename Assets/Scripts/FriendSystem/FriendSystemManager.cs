using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;

public class FriendSystemManager : MonoBehaviour
{
    public static FriendSystemManager instance;

    private List<FriendInfo> _friends = null;
    private Dictionary<string,GameObject> friendListDisplayed = new Dictionary<string,GameObject>();
    private Dictionary<string, int> dict_friendsStatus = new Dictionary<string, int>();

    [SerializeField] private Transform tfFriendList;
    [SerializeField] private Transform tfRequestList;
    [SerializeField] private Transform tfSearchList;
    [SerializeField] private Transform tfWaitingAcceptList;

    [SerializeField] private GameObject goFriendSlot;

    [SerializeField] private Button btnFriend;
    [SerializeField] private Button btnRequest;
    [SerializeField] private Button btnWaitingAccept;

    [SerializeField] private string friendSearchName;


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        //GetFriends();
    }

    private void OnEnable()
    {
        btnFriend.onClick.Invoke();
    }

    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            XboxToken = null
        }, result => {
            _friends = result.Friends;
            foreach (var friend in _friends)
            {
                PhotonChatManager.instance.AddFriendToPhotonChat(friend.TitleDisplayName);
            }
            PhotonChatManager.instance.AddFriendToPhotonChat();
            DisplayFriends(_friends); // triggers your UI
        }, DisplayPlayFabError);
    }

    public void SetFriendsStatusUpdated(string key, int value)
    {
        if (dict_friendsStatus.ContainsKey(key))
        {
            dict_friendsStatus[key] = value;
        }
        else
        {
            dict_friendsStatus.Add(key, value);
        }
        friendListDisplayed[_friends.Find(x => x.TitleDisplayName == key).FriendPlayFabId].GetComponent<FriendSlotController>().LoadStatus(dict_friendsStatus[_friends.Find(x => x.TitleDisplayName == key).TitleDisplayName]);
        Debug.Log("SetFriendsStatusUpdated: " + key + value);
    }

    //hiển thị danh sách bạn bè
    void DisplayFriends(List<FriendInfo> friendsCache)
    {
        List<string> friendListDisplayed_temp = new List<string>();
        foreach (var friend in friendsCache)
        {
            friendListDisplayed_temp.Add(friend.FriendPlayFabId);
            if (friendListDisplayed.ContainsKey(friend.FriendPlayFabId))
            {
                if(friendListDisplayed[friend.FriendPlayFabId] != null)
                {
                    if (friend.Tags[0] != friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().friendInfo_.Tags[0])
                    {
                        switch (friend.Tags[0])
                        {
                            case "confirmed":
                                friendListDisplayed[friend.FriendPlayFabId].transform.SetParent(tfFriendList);
                                friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadInfo(friend);
                                friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadAvatar(friend);
                                friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadStatus(dict_friendsStatus[_friends.Find(x => x.FriendPlayFabId == friend.FriendPlayFabId).TitleDisplayName]);
                                break;
                            case "waitingAccept":
                                friendListDisplayed[friend.FriendPlayFabId].transform.SetParent(tfWaitingAcceptList);
                                friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadInfo(friend);
                                friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadAvatar(friend);
                                friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadStatus(dict_friendsStatus[_friends.Find(x => x.FriendPlayFabId == friend.FriendPlayFabId).TitleDisplayName]);
                                break;
                            case "newRequest":
                                friendListDisplayed[friend.FriendPlayFabId].transform.SetParent(tfRequestList);
                                friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadInfo(friend);
                                friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadAvatar(friend);
                                friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadStatus(dict_friendsStatus[_friends.Find(x => x.FriendPlayFabId == friend.FriendPlayFabId).TitleDisplayName]);
                                break;
                        }
                    }
                    else
                    {
                        try
                        {
                            friendListDisplayed[friend.FriendPlayFabId].GetComponent<FriendSlotController>().LoadStatus(dict_friendsStatus[_friends.Find(x => x.FriendPlayFabId == friend.FriendPlayFabId).TitleDisplayName]);
                        }
                        catch 
                        {
                            Debug.Log("Không tìm thấy key");
                        }
                    }
                }
                else
                {
                    Debug.Log("friendListDisplayed.ContainsKey(friend.FriendPlayFabId) - friendListDisplayed[friend.FriendPlayFabId] == nul");
                    if (friend.Tags[0] == "confirmed")
                    {
                        GameObject friendSlot = Instantiate(goFriendSlot, tfFriendList);
                        friendSlot.GetComponent<FriendSlotController>().LoadComponent();
                        friendSlot.GetComponent<FriendSlotController>().LoadInfo(friend);
                        friendSlot.GetComponent<FriendSlotController>().LoadAvatar(friend);
                        friendSlot.GetComponent<FriendSlotController>().LoadStatus(dict_friendsStatus[_friends.Find(x => x.FriendPlayFabId == friend.FriendPlayFabId).TitleDisplayName]);
                        friendListDisplayed[friend.FriendPlayFabId] = friendSlot;
                    }
                    else if (friend.Tags[0] == "waitingAccept")
                    {
                        GameObject friendSlot = Instantiate(goFriendSlot, tfWaitingAcceptList);
                        friendSlot.GetComponent<FriendSlotController>().LoadComponent();
                        friendSlot.GetComponent<FriendSlotController>().LoadInfo(friend);
                        friendSlot.GetComponent<FriendSlotController>().LoadAvatar(friend);
                        friendSlot.GetComponent<FriendSlotController>().LoadStatus(dict_friendsStatus[_friends.Find(x => x.FriendPlayFabId == friend.FriendPlayFabId).TitleDisplayName]);
                        friendListDisplayed[friend.FriendPlayFabId] = friendSlot;
                    }
                    else
                    {
                        GameObject friendSlot = Instantiate(goFriendSlot, tfRequestList);
                        friendSlot.GetComponent<FriendSlotController>().LoadComponent();
                        friendSlot.GetComponent<FriendSlotController>().LoadInfo(friend);
                        friendSlot.GetComponent<FriendSlotController>().LoadAvatar(friend);
                        friendSlot.GetComponent<FriendSlotController>().LoadStatus(dict_friendsStatus[_friends.Find(x => x.FriendPlayFabId == friend.FriendPlayFabId).TitleDisplayName]);
                        friendListDisplayed[friend.FriendPlayFabId] = friendSlot;
                    }
                }
            }
            else
            {
                if (friend.Tags[0] == "confirmed")
                {
                    GameObject friendSlot = Instantiate(goFriendSlot, tfFriendList);
                    friendSlot.GetComponent<FriendSlotController>().LoadComponent();
                    friendSlot.GetComponent<FriendSlotController>().LoadInfo(friend);
                    friendSlot.GetComponent<FriendSlotController>().LoadAvatar(friend);
                    friendListDisplayed[friend.FriendPlayFabId] = friendSlot;
                }
                else if (friend.Tags[0] == "waitingAccept")
                {
                    GameObject friendSlot = Instantiate(goFriendSlot, tfWaitingAcceptList);
                    friendSlot.GetComponent<FriendSlotController>().LoadComponent();
                    friendSlot.GetComponent<FriendSlotController>().LoadInfo(friend);
                    friendSlot.GetComponent<FriendSlotController>().LoadAvatar(friend);
                    friendListDisplayed[friend.FriendPlayFabId] = friendSlot;
                }
                else
                {
                    GameObject friendSlot = Instantiate(goFriendSlot, tfRequestList);
                    friendSlot.GetComponent<FriendSlotController>().LoadComponent();
                    friendSlot.GetComponent<FriendSlotController>().LoadInfo(friend);
                    friendSlot.GetComponent<FriendSlotController>().LoadAvatar(friend);
                    friendListDisplayed[friend.FriendPlayFabId] = friendSlot;
                }
            }
        }
        foreach(var i in friendListDisplayed)
        {
            if (!friendListDisplayed_temp.Contains(i.Key))
            {
                Destroy(i.Value);
                friendListDisplayed.Remove(i.Key);
            }
        }
    }

    void DisplayPlayFabError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
    void DisplayError(string error)
    {
        Debug.LogError(error);
    }


    //tìm kiếm bạn bè
    #region tìm kiếm bạn bè
    public void FriendSearchNameOnChangeValue(string valueIn)
    {
        friendSearchName = valueIn;
    }

    public void SearchFriendOnClick()
    {
        for (int i = 0; i < tfSearchList.childCount; i++)
        {
            Destroy(tfSearchList.GetChild(i).gameObject);
        }
        if (string.IsNullOrEmpty(friendSearchName))
            return;

        AsyncLoadingScene.instance.LoadingScreen(true);
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            XboxToken = null
        }, result => {
            var friends = result.Friends;
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
            {
                TitleDisplayName = friendSearchName
            }, result =>
            {
                if (result.AccountInfo.TitleInfo.DisplayName != null && result.AccountInfo.PlayFabId != PlayfabDataManager.instance.playFabID_)
                {
                    Debug.Log("Tìm thấy nhân vật" + result.AccountInfo.TitleInfo.DisplayName);
                    GameObject friendSlot = Instantiate(goFriendSlot, tfSearchList);
                    friendSlot.GetComponent<FriendSlotController>().LoadComponent();
                    foreach (var f in friends)
                    {
                        if (f.TitleDisplayName == friendSearchName)
                        {
                            friendSlot.GetComponent<FriendSlotController>().LoadInfo(f);
                            friendSlot.GetComponent<FriendSlotController>().LoadAvatar(f);
                            //AsyncLoadingScene.instance.LoadingScreen();
                            return;
                        }
                    }
                    friendSlot.GetComponent<FriendSlotController>().LoadInfo(result.AccountInfo);
                    friendSlot.GetComponent<FriendSlotController>().LoadAvatar(result.AccountInfo);
                }
                AsyncLoadingScene.instance.LoadingScreen(false);
            }, error =>
            {
                Debug.Log("Không tìm thấy nhân vật");
                AsyncLoadingScene.instance.LoadingScreen(false);
            });
        }, DisplayPlayFabError);
    }

    #endregion
}
