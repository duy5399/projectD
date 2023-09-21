using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillEquipSlotController : MonoBehaviour
{
    [SerializeField] private SkillSO skillSO;
    [SerializeField] private Image imgSkillIcon;
    [SerializeField] private Button btnSkillUnequip;
    [SerializeField] private bool boolIsEquipped;

    public SkillSO skillSO_ => skillSO;
    public bool boolIsEquipped_ => boolIsEquipped;

    private void OnEnable()
    {
        if(skillSO == null)
            btnSkillUnequip.gameObject.SetActive(false);
    }

    public void EquipSkill(SkillSO skillSO)
    {
        if(skillSO != null)
        {
            this.skillSO = skillSO;
            imgSkillIcon.sprite = skillSO.sprSkillIcon_;
            imgSkillIcon.enabled = true;
            btnSkillUnequip.gameObject.SetActive(true);
            boolIsEquipped = true;
        }
    }

    public void UnequipSkill()
    {
        this.skillSO = null;
        imgSkillIcon.sprite = null;
        imgSkillIcon.enabled = false;
        btnSkillUnequip.gameObject.SetActive(false);
        boolIsEquipped = false;
        int i = SkillEquipController.instance.lstSkillEquipSlot_.FindIndex(x => x == this.transform);
        SkillSystemManager.instance.UpdateSkillEquip(i, skillSO);
        PlayerUIController.instance.ChangeSkillSlot(i, skillSO);
    }

    public void OnClickUnequipSkill()
    {
        if (this.skillSO != null)
            UnequipSkill();
    }
}
