using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using System.Linq;
using Newtonsoft.Json;
using TMPro;

public class SkillSystemManager : MonoBehaviour
{
    public static SkillSystemManager instance { get; private set; }

    [SerializeField] private SkillDatabaseSO skillDatabase;
    [SerializeField] private GameObject goSkillInfo;

    [Header("Skill Tree")]
    [SerializeField] private Transform tfSkillListContent;
    private Dictionary<string, string> dictListSkillTree = new Dictionary<string, string>();
    private Dictionary<string, GameObject> dictListSkillTreeDisplayed = new Dictionary<string, GameObject>();

    [Header("Skill Equip")]
    [SerializeField] private Transform tfSkillEquipContent;
    private Dictionary<string, string> dictListSkillEquip = new Dictionary<string, string>();
    private Dictionary<string, GameObject> dictListSkillEquipDisplayed = new Dictionary<string, GameObject>();

    [Header("Skill Point")]
    [SerializeField] private TextMeshProUGUI txtSkillPoint;
    [SerializeField] private int intSkillPoint;

    public SkillDatabaseSO skillDatabase_ => skillDatabase;
    public int intSkillPoint_ => intSkillPoint;

    private void Awake()
    {
        if(instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void OnEnable()
    {
        GetPlayerSkillData();
    }

    public void LoadSkillPoint()
    {
        Debug.Log("LoadSkillPoint");
        txtSkillPoint.text = intSkillPoint.ToString();
    }

    public void LoadSkillTree()
    {
        Debug.Log("LoadSkillTree");
        foreach (var skill in dictListSkillTree)
        {
            if (!dictListSkillTreeDisplayed.ContainsKey(skill.Key))
            {
                var skillInfo = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(skill.Value);
                SkillSO skillSO = skillDatabase.skillData[skill.Key].Find(x => x.strSkillID_ == skillInfo["skillID"]);
                GameObject obj = Instantiate(goSkillInfo, tfSkillListContent);
                obj.GetComponent<SkillInfoSlotController>().LoadInfoSkillSO(skillSO);
                dictListSkillTreeDisplayed.Add(skill.Key, obj);
            }
        }
    }

    public void LoadSkillEquip()
    {
        Debug.Log("LoadSkillEquip");
        foreach (var skill in dictListSkillEquip)
        {
            if(skill.Value != "")
            {
                string[] arrSkillID = skill.Value.Split(new char[] { '_' });
                int skillSlot = int.Parse(skill.Key);
                SkillSO skillSO = skillDatabase.skillData[arrSkillID[0]].Find(x => x.strSkillID_ == skill.Value);
                SkillEquipController.instance.lstSkillEquipSlot_[skillSlot].GetComponent<SkillEquipSlotController>().EquipSkill(skillSO);
            }
        }
    }

    private void GetPlayerSkillData()
    {
        GetUserDataRequest request = new GetUserDataRequest()
        {
            Keys = new List<string>() { "skillTree", "skillPoint" , "skillEquip" }
        };
        PlayFabClientAPI.GetUserData(request,
            result => {
                if (result.Data == null || !result.Data.ContainsKey("skillTree") || !result.Data.ContainsKey("skillPoint") || !result.Data.ContainsKey("skillEquip"))
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    Dictionary<string, string> skillTree = new Dictionary<string, string>();
                    foreach (var skill in skillDatabase.skillData)
                    {
                        Dictionary<string, string> temp = new Dictionary<string, string>();
                        temp.Add("skillID", skill.Value[0].name);
                        string json0 = JsonConvert.SerializeObject(temp);
                        skillTree.Add(skill.Key, json0);
                    }
                    string json1 = JsonConvert.SerializeObject(skillTree);
                    data.Add("skillTree", json1);
                    data.Add("skillPoint", "15");
                    data.Add("skillEquip", "");
                    UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest()
                    {
                        Data = data,
                        Permission = UserDataPermission.Public
                    };
                    PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
                        result =>
                        {
                            GetPlayerSkillData();
                        },
                        error =>
                        {

                        });
                }
                else
                {
                    var skillPoint = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<int>(result.Data["skillPoint"].Value);
                    intSkillPoint = skillPoint;
                    Debug.Log("intSkillPoint: " + intSkillPoint.ToString());
                    var skillTree = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.Data["skillTree"].Value);
                    foreach (var i in skillTree)
                    {
                        dictListSkillTree.Add(i.Key, i.Value);
                        Debug.Log("dictListSkillTree: " + i.Key + " - " + i.Value);
                    }
                    var skillEquip = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.Data["skillEquip"].Value);
                    if(skillEquip != null)
                    {
                        foreach (var i in skillEquip)
                            dictListSkillEquip.Add(i.Key, i.Value);
                    }
                }
            }, (error) => {
                Debug.Log(error.GenerateErrorReport());
            });
    }

    public void UpdateSkillPoint(int skillPoint)
    {
        intSkillPoint += skillPoint;
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("skillPoint", intSkillPoint.ToString());
        UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest()
        {
            Data = data,
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
            result =>
            {
                Debug.Log("UpdateSkillPoint thành công: " + skillPoint);
            },
            error =>
            {
                Debug.Log("UpdateSkillPoint thất bại " + skillPoint);
            });
    }

    public void UpdateSkillTree(SkillSO skillSO)
    {
        string[] arrSkillID = skillSO.strSkillID_.Split(new char[] { '_' });
        var skillInfo = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(dictListSkillTree[arrSkillID[0]]);

        skillInfo["skillID"] = skillSO.name;
        dictListSkillTree[arrSkillID[0]] = JsonConvert.SerializeObject(skillInfo);
        string newData = JsonConvert.SerializeObject(dictListSkillTree);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("skillTree", newData);
        UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest()
        {
            Data = data,
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
            result =>
            {
                Debug.Log("UpdateSkillTree thành công: " + newData);
            },
            error =>
            {
                Debug.Log("UpdateSkillTree thất bại");
            });
    }
    public void UpdateSkillEquip(int skillSlot, SkillSO skillSO)
    {
        dictListSkillEquip[skillSlot.ToString()] = skillSO == null? "" : skillSO.name;
        string newData = JsonConvert.SerializeObject(dictListSkillEquip);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("skillEquip", newData);
        UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest()
        {
            Data = data,
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
            result =>
            {
                Debug.Log("UpdateSkillEquip thành công: " + newData);
            },
            error =>
            {
                Debug.Log("UpdateSkillEquip thất bại");
            });
    }
}
