using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillHeal", menuName = "ScriptableObjects/Skill/SkillHeal")]
public class SkillHealSO : SkillSO
{
    public override void Update()
    {
        base.Update();
    }

    public override void Skill(int playerID)
    {
        base.Skill(playerID);
        PhotonView player = PhotonView.Find(playerID);
        Debug.Log("player id: " + player.ViewID);
        Debug.Log("player.GetComponent<PlayerStats>().currentMP_: " + player.GetComponent<PlayerStats>().intCurrentMP_);
        Debug.Log("intMPCost: " + intMPCost);
        if (player.GetComponent<PlayerStats>().intCurrentMP_ >= intMPCost && player.GetComponent<PlayerStats>().intCurrentHP_ > 0)
        {
            //player.RPC("Heal", RpcTarget.AllBuffered, player.ViewID);
            player.GetComponent<PlayerStats>().FlatMPRevovery(-intMPCost);
            Heal(playerID);
        }
        else
            Debug.Log("Không tìm thấy player");
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    public void Heal(int playerID)  //hàm đặt lại máu của nhân vật
    {
        PhotonView player = PhotonView.Find(playerID);
        if(player != null)
        {
            player.GetComponent<PlayerCombat>().PercentageHealingHP(floatPercentageHealingHP, player);
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs/SkillSystem/Skill", goSkillEffect.name), player.transform.position, Quaternion.identity);
            obj.GetComponent<SkillEffect>().SetFloatDestroyTime(floatEffectDuration);
            obj.GetComponent<SkillEffect>().SetOwner(player);
            Debug.Log("player: " + player.Owner);
            obj.transform.SetParent(player.transform);
        }
    }
}
