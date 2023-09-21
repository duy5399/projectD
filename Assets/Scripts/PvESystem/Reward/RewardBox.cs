using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using PlayFab.MultiplayerModels;
using Newtonsoft.Json;
using Photon.Pun;

public class RewardBox : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string mapIdRewards;
    private Dictionary<string,string> rewards = new Dictionary<string, string>();
    [SerializeField] private bool beenClicked;

    [Header("Display Item Drop From BOSS")]
    [SerializeField] private Transform rewardPanel;
    [SerializeField] private Transform rewardContent;
    [SerializeField] private GameObject rewardItem;
    private Dictionary<string, GameObject> rewardSlotDisplayed = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        rewardPanel.gameObject.SetActive(false);
        beenClicked = false;
    }

    private void OnEnable()
    {
        GetRandomRewardsPvE(mapIdRewards);
    }

    public void GetRandomRewardsPvE(string mapName)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetRandomRewardsPvE",
            FunctionParameter = new { mapName = MapInfo.instance.mapInfo_.mapIdRewards_}
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("GetRandomRewardsPvE success: " + result.FunctionResult);
                rewards = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
            },
            error =>
            {
                Debug.Log("GetRandomRewardsPvE error: " + error.Error);
            });
    }

    private void DisplayRewardsList()
    {
        foreach(var i in rewards)
        {
            Dictionary<string,string> item = JsonConvert.DeserializeObject<Dictionary<string, string>>(i.Value);
            GameObject obj = Instantiate(rewardItem, rewardContent);
            obj.GetComponent<RewardItem>().LoadInforItemSlot(item["iconImg"], item["tier"], item["quantity"]);
            //InventoryManager.instance.AddItem(equipment, reward.quantity_);
            //Debug.Log("Vật phẩm thứ [ " + i + "]: " + equipment.itemName_ + " - " + reward.quantity_);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown Detected");
        if (!beenClicked)
        {
            transform.GetComponent<Animator>().SetTrigger("OpenBox");
            rewardPanel.gameObject.SetActive(true);
            DisplayRewardsList();
            beenClicked = true;
        }
    }

    public void OnClickOpen()
    {
        Debug.Log("OnClickOpen Detected");
        if (!beenClicked)
        {
            transform.GetComponent<Animator>().SetTrigger("OpenBox");
            rewardPanel.gameObject.SetActive(true);
            DisplayRewardsList();
            beenClicked = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick Detected");
        if (!beenClicked)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                transform.GetComponent<Animator>().SetTrigger("OpenBox");
                rewardPanel.gameObject.SetActive(true);
                DisplayRewardsList();
                beenClicked = true;
            }
        }
    }

    public void DisplayItemDescription()
    {
        //UIManager.instance.inventoryDesciptionController_.SetDescription(equipment);
    }

    public void onClickConfirmBtn()
    {
        rewardPanel.gameObject.SetActive(false);
        StartCoroutine(AutoReturnHome(3f));
    }

    IEnumerator AutoReturnHome(float _interval)
    {
        UIController.instance.OnAlertWarning("Tự động về lại trang chủ sau " + _interval + " giây!!!");
        yield return new WaitForSeconds(_interval);
        MapInfo.instance.Destroy();
        AudioManager.instance.Destroy();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Homepage");
    }
}
