using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNormalAttackController : MonoBehaviour
{
    public void OnClickBtnNormalAttack()
    {
        GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        player.GetComponent<PlayerCombat>().UpdateAttack();
    }

    public void OnClickBtnJump()
    {
        GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        player.GetComponent<PlayerMovement>().UpdateJump();
    }

    public void OnClickBtnDash()
    {
        GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        player.GetComponent<PlayerMovement>().UpdateDash();
    }
}
