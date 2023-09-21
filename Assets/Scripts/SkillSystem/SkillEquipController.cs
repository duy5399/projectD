using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillEquipController : MonoBehaviour
{
    public static SkillEquipController instance { get; private set; }

    [SerializeField] private List<Transform> lstSkillEquipSlot;
    public List<Transform> lstSkillEquipSlot_ => lstSkillEquipSlot;
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public void EquipSkill(SkillSO skillSO)
    {
        try
        {
            string[] arrSkillID = skillSO.strSkillID_.Split(new char[] { '_' });
            if (lstSkillEquipSlot.Find(x => x.GetComponent<SkillEquipSlotController>().skillSO_ != null && x.GetComponent<SkillEquipSlotController>().skillSO_.strSkillID_.Contains(arrSkillID[0])))
                return;
        }
        catch
        {
            Debug.Log("Lỗi kiểm tra lstSkillEquipSlot");
        }
        for(int  i = 0; i < lstSkillEquipSlot.Count; i++)
        {
            if (!lstSkillEquipSlot[i].GetComponent<SkillEquipSlotController>().boolIsEquipped_)
            {
                lstSkillEquipSlot[i].GetComponent<SkillEquipSlotController>().EquipSkill(skillSO);
                SkillSystemManager.instance.UpdateSkillEquip(i, skillSO);
                PlayerUIController.instance.ChangeSkillSlot(i, skillSO);
                return;
            }
        }
        Debug.Log("Không thể trang bị skill");
    }
}
