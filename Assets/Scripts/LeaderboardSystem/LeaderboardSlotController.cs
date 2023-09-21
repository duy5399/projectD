using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeaderboardSlotController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlayerLeaderboardEntry playerLeaderboardEntry;

    [SerializeField] private TextMeshProUGUI txt_ranking;
    [SerializeField] private Image img_avatar;
    [SerializeField] private Image img_avatarBorder;
    [SerializeField] private TextMeshProUGUI txt_name;
    [SerializeField] private TextMeshProUGUI txt_score;
    [SerializeField] private Transform tf_reward1;
    [SerializeField] private Transform tf_reward2;

    public void LoadInfo(PlayerLeaderboardEntry playerLeaderboardEntry)
    {
        this.playerLeaderboardEntry = playerLeaderboardEntry;
        GetUserDataRequest request = new GetUserDataRequest()
        {
            PlayFabId = playerLeaderboardEntry.PlayFabId,
            Keys = new List<string>() { "infoAvatar" }
        };
        PlayFabClientAPI.GetUserData(request,
            result => {
                if (result.Data == null || !result.Data.ContainsKey("infoAvatar"))
                {
                    img_avatar.enabled = false;
                    img_avatarBorder.enabled = false;
                    Debug.Log("No Ancestor");
                }
                else
                {
                    var infoAvatar = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.Data["infoAvatar"].Value);
                    try{
                        img_avatar.sprite = AvatarSystemManager.instance.avatarDatabase_.avatarDB_.Find(x => x.name == infoAvatar["avatar"]);
                        img_avatarBorder.sprite = AvatarSystemManager.instance.avatarDatabase_.avatarBorderDB_.Find(x => x.name == infoAvatar["avatarBorder"]);
                        img_avatar.enabled = true;
                        img_avatarBorder.enabled = true;
                        Debug.Log("Tải avatar thành công");
                    }
                    catch
                    {
                        img_avatar.enabled = false;
                        img_avatarBorder.enabled = false;
                        Debug.Log("No Ancestor");
                    }
                }
            }, (error) => {
                Debug.Log(error.GenerateErrorReport());
            });
        txt_ranking.text = (playerLeaderboardEntry.Position + 1).ToString();
        txt_name.text = playerLeaderboardEntry.DisplayName;
        txt_score.text = playerLeaderboardEntry.StatValue.ToString();
    }

    public void LoadRewards(Dictionary<CatalogItem, string> rewards)
    {
        tf_reward1.GetComponent<LeaderboardRewardSlotController>().LoadInforItemSlot(rewards.ElementAt(0).Key, rewards.ElementAt(0).Value);
        tf_reward2.GetComponent<LeaderboardRewardSlotController>().LoadInforItemSlot(rewards.ElementAt(1).Key, rewards.ElementAt(1).Value);
        tf_reward1.gameObject.SetActive(true);
        tf_reward2.gameObject.SetActive(true);
    }

    public void NotInLeaderboard(PlayerLeaderboardEntry playerLeaderboardEntry)
    {
        txt_ranking.text = "-";
        txt_name.text = playerLeaderboardEntry.DisplayName;
        txt_score.text = playerLeaderboardEntry.StatValue.ToString();
    }

    public void LoadInfo(string name)
    {
        txt_ranking.text = "-";
        txt_name.text = name;
        txt_score.text = "Chưa vào BXH";
        tf_reward1.gameObject.SetActive(false);
        tf_reward2.gameObject.SetActive(false);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick"); 
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("playerLeaderboardEntry.PlayFabId: " + playerLeaderboardEntry.PlayFabId);
            LeaderboardInfoCharacterController.instance.GetUserData(playerLeaderboardEntry.PlayFabId, playerLeaderboardEntry.DisplayName);
        }
    }
}
