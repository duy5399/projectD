using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class AvatarSystemManager : MonoBehaviour
{
    public static AvatarSystemManager instance { get; private set; }

    [SerializeField] private AvatarDatabaseSO avatarDatabase;

    [SerializeField] private Image imgCurrentAvatar;
    [SerializeField] private Image imgCurrentBorderAvatar;

    [SerializeField] private Transform tfContentAvatar;
    [SerializeField] private Transform tfContentAvataBorder;

    [SerializeField] private GameObject goAvatarSlot;
    [SerializeField] private GameObject goBorderAvatarSlot;

    private Dictionary<string, string> dictAvatarInfo = new Dictionary<string, string>();
    public AvatarDatabaseSO avatarDatabase_ => avatarDatabase;
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        GetAvatar();
        GetDatabaseAvatar();
    }

    private void GetAvatar()
    {
        GetUserDataRequest request = new GetUserDataRequest()
        {
            Keys = new List<string>() { "infoAvatar" }
        };
        PlayFabClientAPI.GetUserData(request,
            result => {
                if (result.Data == null || !result.Data.ContainsKey("infoAvatar"))
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    Dictionary<string, string> infoAvatar = new Dictionary<string, string>();
                    infoAvatar.Add("avatar", "avatar-01");
                    infoAvatar.Add("avatarBorder", "border-01");
                    string json = JsonConvert.SerializeObject(infoAvatar);
                    data.Add("infoAvatar", json);
                    UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest()
                    {
                        Data = data,
                        Permission = UserDataPermission.Public
                    };
                    PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
                        result =>
                        {
                            GetAvatar();
                        },
                        error =>
                        {

                        });
                }
                else
                {
                    var infoAvatar = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.Data["infoAvatar"].Value);
                    foreach (var i in infoAvatar)
                        dictAvatarInfo.Add(i.Key, i.Value);
                    LoadCurrentAvatar(avatarDatabase.avatarDB_.Find(x => x.name == infoAvatar["avatar"]));
                    LoadCurrentAvatarBorder(avatarDatabase.avatarBorderDB_.Find(x => x.name == infoAvatar["avatarBorder"]));
                }
            }, (error) => {
                Debug.Log(error.GenerateErrorReport());
            });
    }

    public void UpdateAvatar(Sprite avatar)
    {
        dictAvatarInfo["avatar"] = avatar.name;
        Dictionary<string, string> data = new Dictionary<string, string>();
        string json = JsonConvert.SerializeObject(dictAvatarInfo);
        data.Add("infoAvatar", json);
        UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest()
        {
            Data = data,
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
            result =>
            {
                Debug.Log("UpdateAvatar thành công: " + avatar);
            },
            error =>
            {
                Debug.Log("UpdateAvatar thất bại " + avatar);
            });
    }
    public void UpdateAvatarBorder(Sprite avatarBorder)
    {
        dictAvatarInfo["avatarBorder"] = avatarBorder.name;
        Dictionary<string, string> data = new Dictionary<string, string>();
        string json = JsonConvert.SerializeObject(dictAvatarInfo);
        data.Add("infoAvatar", json);
        UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest()
        {
            Data = data,
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
            result =>
            {
                Debug.Log("UpdateAvatar thành công: " + avatarBorder);
            },
            error =>
            {
                Debug.Log("UpdateAvatar thất bại " + avatarBorder);
            });
    }

    public void GetDatabaseAvatar()
    {
        foreach(var sprite in avatarDatabase.avatarDB_)
        {
            GameObject obj = Instantiate(goAvatarSlot, tfContentAvatar);
            obj.GetComponent<AvatarSlotController>().LoadInfoAvatar(sprite);
        }
        foreach (var sprite in avatarDatabase.avatarBorderDB_)
        {
            GameObject obj = Instantiate(goBorderAvatarSlot, tfContentAvataBorder);
            obj.GetComponent<AvatarSlotController>().LoadInfoAvatarBorder(sprite);
        }
    }

    public void LoadCurrentAvatar(Sprite avatar)
    {
        imgCurrentAvatar.sprite = avatar;
        HealthPlayerController.instance.SetAvatar(avatar);
    }

    public void LoadCurrentAvatarBorder(Sprite avatarBorder)
    {
        imgCurrentBorderAvatar.sprite = avatarBorder;
        HealthPlayerController.instance.SetAvatarBorder(avatarBorder);
    }

    public void OnChangeAvatarCurrent(Sprite newAvatar)
    {
        imgCurrentAvatar.sprite = newAvatar;
    }

    public void OnChangeAvatarBorderCurrent(Sprite newAvatarBorder)
    {
        imgCurrentBorderAvatar.sprite = newAvatarBorder;
    }
}
