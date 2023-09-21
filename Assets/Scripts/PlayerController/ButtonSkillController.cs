using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSkillController : MonoBehaviour
{
    //[SerializeField] private PhotonView photonView;
    [SerializeField] private SkillSO skillSO;
    [SerializeField] private Image imgSkillIcon;
    [SerializeField] private Image imgCooldown;
    [SerializeField] private TextMeshProUGUI txtCooldown;
    [SerializeField] private GameObject goOwner;

    [SerializeField] private float floatCooldown = -1;

    private void Update()
    {
        if (skillSO != null)
            GetSkillCooldown();
        if(goOwner ==  null)
            GetOwner();
    }

    private void OnEnable()
    {
        imgSkillIcon.enabled = false;
        imgCooldown.enabled = false;
        txtCooldown.enabled = false;
        //GetOwner();
    }
    private void OnDisable()
    {
        //transform.GetComponent<Button>().onClick.RemoveListener(skillSO.Skill);
    }

    public void LoadInfoSkill(SkillSO skillSO)
    {
        if(skillSO != null)
        {
            this.skillSO = skillSO;
            imgSkillIcon.sprite = skillSO.sprSkillIcon_;
            imgSkillIcon.enabled = true;
            //transform.GetComponent<Button>().onClick.AddListener(OnClickBtnSkill);
        }
        else
        {
            imgSkillIcon.enabled = false;
            this.skillSO = null;
        }
    }

    public void OnClickBtnSkill()
    {
        if(skillSO != null && goOwner.GetComponent<PlayerStats>().intCurrentMP_ >= skillSO.intMPCost_)
        {
            goOwner.GetPhotonView().RPC("RPC_Skill", RpcTarget.All, skillSO.strSkillID_, goOwner.GetPhotonView().ViewID);
            floatCooldown = skillSO.floatCooldown_;
        }
    }

    private void GetSkillCooldown()
    {
        if (floatCooldown >= 0)
        {
            transform.GetComponent<Button>().interactable = false;
            floatCooldown -= Time.deltaTime;
            txtCooldown.enabled = true;
            txtCooldown.text = ((int)floatCooldown).ToString();
            imgCooldown.enabled = true;
            imgCooldown.fillAmount = floatCooldown / skillSO.floatCooldown_;
        }
        else
        {
            imgCooldown.enabled = false;
            txtCooldown.enabled = false;
            transform.GetComponent<Button>().interactable = true;
        }
    }

    private void GetOwner()
    {
        //Player p = photonView.Owner;
        //goOwner = p.TagObject as GameObject;
        goOwner = PhotonNetwork.LocalPlayer.TagObject as GameObject;
    }
}
