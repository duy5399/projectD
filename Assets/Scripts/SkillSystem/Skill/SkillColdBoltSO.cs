using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "SkillColdBolt", menuName = "ScriptableObjects/Skill/SkillColdBolt")]
public class SkillColdBoltSO : SkillSO
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
            ColdBolt(playerID);
        }
        else
            Debug.Log("Không tìm thấy player");
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    public void ColdBolt(int playerID)  //hàm đặt lại máu của nhân vật
    {
        PhotonView player = PhotonView.Find(playerID);
        if (player != null)
        {
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs/SkillSystem/Skill", goSkillEffect.name), player.GetComponent<PlayerCombat>().attackPoint_.position, Quaternion.identity);
            obj.GetComponent<SkillEffect>().SetDamage(floatPercentageDamage);
            obj.GetComponent<SkillEffect>().SetFloatMoveSpeed(3f);
            obj.GetComponent<SkillEffect>().SetFloatDestroyTime(floatEffectDuration);
            obj.GetComponent<SkillEffect>().SetOwner(player);
            obj.GetComponent<SkillColdBoltEffect>().SetBoolFlipX(player.GetComponent<SpriteRenderer>().flipX);
        }
    }
}

