using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class PlayerSkills : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private List<SkillSO> lstSkillSO;

    public List<SkillSO> lstSkillSO_ => lstSkillSO;

    private void OnEnable()
    {
        GetPlayerSkill();
    }

    public void GetPlayerSkill()
    {
        GetUserDataRequest request = new GetUserDataRequest()
        {
            Keys = new List<string>() { "skillEquip" }
        };
        PlayFabClientAPI.GetUserData(request,
            result => {
                if (result.Data != null && result.Data.ContainsKey("skillEquip"))
                {
                    var skillEquip = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer).DeserializeObject<Dictionary<string, string>>(result.Data["skillEquip"].Value);
                    if(skillEquip != null)
                    {
                        foreach (var skill in skillEquip)
                        {
                            if (skill.Value != "")
                            {
                                string[] arrSkillID = skill.Value.Split(new char[] { '_' });
                                int skillSlot = int.Parse(skill.Key);
                                SkillSO skillSO = SkillSystemManager.instance.skillDatabase_.skillData[arrSkillID[0]].Find(x => x.strSkillID_ == skill.Value);
                                lstSkillSO.Add(skillSO);
                            }
                            else
                                lstSkillSO.Add(null);
                        }
                    }
                    PlayerUIController.instance.LoadBtnSkill(lstSkillSO);
                    Debug.Log("GetPlayerSkill thành công");   
                }
            }, (error) => {
                Debug.Log(error.GenerateErrorReport());
            });
    }

    private void Skill()
    {
        photonView.RPC("RPC_Skill", RpcTarget.All, photonView.ViewID);
    }

    [PunRPC]
    private void RPC_Skill(string skillID , int playerID)
    {
        Debug.Log("GetSkill PlayerSkills: " + skillID + " - " + playerID);
        string[] arrSkillID = skillID.Split(new char[] { '_' });
        SkillSO skillSO = SkillSystemManager.instance.skillDatabase_.skillData[arrSkillID[0]].Find(x => x.strSkillID_ == skillID);
        //photonView.RPC("skillSO.Skill", RpcTarget.All, goOwner.GetPhotonView().ViewID);
        skillSO.Skill(playerID);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
