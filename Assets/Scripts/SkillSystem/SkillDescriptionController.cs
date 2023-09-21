using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDescriptionController : MonoBehaviour
{
    public static SkillDescriptionController instance { get; private set; }
    [SerializeField] private SkillSO skillSO;
    [SerializeField] private TextMeshProUGUI txtSkillName;
    [SerializeField] private TextMeshProUGUI txtSkillType;
    [SerializeField] private TextMeshProUGUI txtSkillDescription;
    [SerializeField] private TextMeshProUGUI txtSkillMPCost;
    [SerializeField] private TextMeshProUGUI txtSkillCooldown;
    [SerializeField] private Button btnSkillEquip;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void LoadSkillDescription(SkillSO skillSO)
    {
        this.skillSO = skillSO;
        txtSkillName.text = skillSO.strSkillName_ + " Lv." + skillSO.strSkillLevel_;
        txtSkillType.text = skillSO.strSkillType_ == "active" ? "KN Chủ động" : "KN Bị động";
        txtSkillDescription.text = skillSO.strSkillDescription_;
        txtSkillMPCost.text = skillSO.intMPCost_.ToString();
        txtSkillCooldown.text = skillSO.floatCooldown_.ToString() + "s";
    }

    public void OnClickSkillEquipBtn()
    {
        if(skillSO != null)
            SkillEquipController.instance.EquipSkill(skillSO);
    }
}
