using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillMPRecovery", menuName = "ScriptableObjects/Skill/SkillMPRecovery")]
public class SkillMPRecoverySO : SkillSO
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
            MPRecovery(playerID);
        }
        else
            Debug.Log("Không tìm thấy player");
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    public void MPRecovery(int playerID)  //hàm đặt lại máu của nhân vật
    {
        PhotonView player = PhotonView.Find(playerID);
        if (player != null)
        {
            player.GetComponent<PlayerStats>().PercentageRevoveryMP(floatPercentageMPRecovery);
            Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y + 0.8f, player.transform.position.z);
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs/SkillSystem/Skill", goSkillEffect.name), pos, Quaternion.identity);
            obj.GetComponent<SkillEffect>().SetFloatDestroyTime(floatEffectDuration);
            obj.GetComponent<SkillEffect>().SetOwner(player);
            obj.transform.SetParent(player.transform);
        }
    }
}