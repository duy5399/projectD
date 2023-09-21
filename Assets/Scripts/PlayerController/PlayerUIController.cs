using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public static PlayerUIController instance { get; private set; }

    [SerializeField] private Button btnAutoAttack;
    [SerializeField] private List<Transform> btnSkill;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Update()
    {
        //if (!PhotonNetwork.IsMasterClient && !photonView.IsMine)
        //{
        //    GameObject newPhotonView = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        //    //PhotonNetwork.LocalPlayer.
        //    //newPhotonView.GetPhotonView().RequestOwnership();
        //    photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        //    Debug.Log("newPhotonView: " + newPhotonView.GetPhotonView().ViewID);
        //}
        //else
        //{
        //    Debug.Log("photonView.IsMine):");
        //}
    }

    public void LoadBtnSkill(List<SkillSO> skillSOs)
    {
        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < skillSOs.Count; i++)
            {
                ChangeSkillSlot(i, skillSOs[i]);
            }
        }
    }

    public void ChangeSkillSlot(int slot, SkillSO skillSO)
    {
        btnSkill[slot].GetComponent<ButtonSkillController>().LoadInfoSkill(skillSO);
    }
}
