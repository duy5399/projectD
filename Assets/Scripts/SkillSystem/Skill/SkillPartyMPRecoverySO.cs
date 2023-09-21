using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillPartyMPRecovery", menuName = "ScriptableObjects/Skill/SkillPartyMPRecovery")]
public class SkillPartyMPRecoverySO : SkillSO
{
    public override void Update()
    {
        base.Update();
    }

    public override void Skill(int playerID)
    {
        base.Skill(playerID);
        PhotonView player = PhotonView.Find(playerID);
        if (player.GetComponent<PlayerStats>() != null && player.GetComponent<PlayerStats>().intCurrentMP_ >= intMPCost)
        {
            //player.RPC("Heal", RpcTarget.AllBuffered, player.ViewID);
            player.GetComponent<PlayerStats>().FlatMPRevovery(-intMPCost);
            PartyMPRecovery(playerID);
        }
        else
            Debug.Log("Không tìm thấy player");
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    public void PartyMPRecovery(int playerID)  //hàm đặt lại máu của nhân vật
    {
        PhotonView player = PhotonView.Find(playerID);
        if (player != null)
        {
            string[] arrListStr;
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("hasTeam"))
            {
                var team = PhotonNetwork.CurrentRoom.CustomProperties.Values.FirstOrDefault(kvp => kvp.ToString().Contains(player.Owner.NickName));
                arrListStr = team.ToString().Split(',');
            }
            else
                arrListStr = new string[] { player.Owner.NickName };

            GameObject[] playersInGame = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < arrListStr.Length; i++)
            {
                PhotonView teammate = playersInGame.FirstOrDefault(x => x.GetComponent<PhotonView>().Owner.NickName == arrListStr[i]).GetPhotonView();
                if (teammate != null)
                {
                    teammate.GetComponent<PlayerStats>().PercentageRevoveryMP(floatPercentageMPRecovery);
                    Vector3 pos = new Vector3(teammate.transform.position.x, teammate.transform.position.y + 0.5f, teammate.transform.position.z);
                    GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs/SkillSystem/Skill", goSkillEffect.name), pos, Quaternion.identity);
                    obj.GetComponent<SkillEffect>().SetFloatDestroyTime(floatEffectDuration);
                    obj.GetComponent<SkillEffect>().SetOwner(teammate);
                    obj.transform.SetParent(teammate.transform);
                }
            }
        }
    }
}