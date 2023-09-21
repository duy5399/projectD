using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ItemSO;

public class MapDisplay : MonoBehaviour
{
    public static MapDisplay instance { get; private set; }

    [SerializeField] private Image mapImage;
    [SerializeField] private TextMeshProUGUI bossName;
    [SerializeField] private Image bossImage;
    [SerializeField] private List<Transform> mapReward;
    [SerializeField] private Button startBtn;

    [SerializeField] private UnityAction startAction;

    [SerializeField] private TextMeshProUGUI txt_energy;
    [SerializeField] private TextMeshProUGUI txt_energyRecharge;
    [SerializeField] private float timeToRechargeEnergy = 1;

    private Dictionary<string, int> userVirtualCurrency = new Dictionary<string, int>();
    private Dictionary<string, VirtualCurrencyRechargeTime> userVirtualCurrencyRechargeTime = new Dictionary<string, VirtualCurrencyRechargeTime>();

    void Awake()
    {
        if(instance !=  null && instance != this)
            Destroy(this);
        else
            instance = this;
        mapImage = transform.GetChild(1).GetComponent<Image>();
        bossName = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        bossImage = transform.GetChild(2).GetChild(1).GetComponent<Image>();
        foreach(Transform t in transform.GetChild(2).GetChild(2))
        {
            mapReward.Add(t);
        }
        startBtn = transform.GetChild(3).GetComponent<Button>();
    }

    private void OnEnable()
    {
        GetVirtualCurrencyUser();
    }

    private void FixedUpdate()
    {
        try
        {
            if (userVirtualCurrency["EN"] < 50)
            {
                timeToRechargeEnergy -= Time.fixedDeltaTime;
                TimeSpan time = TimeSpan.FromSeconds(timeToRechargeEnergy);
                txt_energyRecharge.text = time.ToString("mm':'ss");
                if (timeToRechargeEnergy < 0)
                {
                    GetVirtualCurrencyUser();
                }
            }
        }
        catch
        {
            Debug.Log("Đang tải dữ liệu");
        }
    }

    public void GetVirtualCurrencyUser()
    {
        GetUserInventoryRequest request = new GetUserInventoryRequest { };
        PlayFabClientAPI.GetUserInventory(request,
            result =>
            {
                userVirtualCurrency = result.VirtualCurrency;
                userVirtualCurrencyRechargeTime = result.VirtualCurrencyRechargeTimes;
                txt_energy.text = userVirtualCurrency["EN"].ToString() + "/50";
                timeToRechargeEnergy = userVirtualCurrencyRechargeTime["EN"].SecondsToRecharge;
            },
            error => {
                Debug.LogError(error);
            });
    }

    public void DisplayMap(MapSO _map)
    {
        mapImage.sprite = _map.mapImage_;
        bossName.text = _map.mapBoss_[_map.mapBoss_.Count - 1].bossPrefab_.name + " (" + _map.mapEnergy_ + " Energy - " + _map.mapTime_+"s)";
        bossImage.sprite = _map.mapBoss_[_map.mapBoss_.Count - 1].bossPrefab_.GetComponent<SpriteRenderer>().sprite;
        for(int i = 0; i < mapReward.Count; i++)
        {
            if (mapReward[i] != null)
            {
                mapReward[i].GetChild(1).GetComponent<Image>().sprite = _map.mapRewards_[i].itemIcon_;
                mapReward[i].GetChild(2).GetComponent<TextMeshProUGUI>().text = _map.mapRewards_[i].quantity_.ToString();

                switch (_map.mapDifficulty_)
                {
                    case MapDifficulty.easy:
                        mapReward[i].GetChild(0).GetComponent<Image>().color = new Color32(209, 213, 216, 255);
                        break;
                    case MapDifficulty.normal:
                        mapReward[i].GetChild(0).GetComponent<Image>().color = new Color32(65, 168, 95, 255);
                        break;
                    case MapDifficulty.difficult:
                        mapReward[i].GetChild(0).GetComponent<Image>().color = new Color32(44, 130, 201, 255);
                        break;
                    case MapDifficulty.hero:
                        mapReward[i].GetChild(0).GetComponent<Image>().color = new Color32(147, 101, 184, 255);
                        break;
                    default:
                        Debug.Log("Not found mapDifficulty_: " + _map.mapDifficulty_);
                        break;
                }
            }
        }
        startBtn.onClick.RemoveAllListeners();
        
        startBtn.onClick.AddListener(delegate { OnClickStartBtn(_map); });
        //startBtn.onClick.AddListener(delegate { onClickStartBtn(_map); });
    }

    private void OnClickStartBtn(MapSO _map)
    {
        if (userVirtualCurrency["EN"] < _map.mapEnergy_)
        {
            UIManager.instance.questionDialogUI_.DisplayPurchaseFailed("Không đủ năng lượng để khiêu chiến!", () => { }, () => { });
            return;
        }
        //Debug.Log(_map.sceneToLoad_.ToString());
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "SubtractCurrencyForUser",
            FunctionParameter = new { currency  = "EN", amount = _map.mapEnergy_ }
            //GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
            result =>
            {
                Debug.Log("SubtractCurrencyForUser success" + result.FunctionResult);
            },
            SetUserDataError =>
            {
                Debug.Log("SubtractCurrencyForUser Error: " + SetUserDataError.Error);
            });
        SceneManager.LoadScene(_map.sceneToLoad_.ToString());
    }
}
