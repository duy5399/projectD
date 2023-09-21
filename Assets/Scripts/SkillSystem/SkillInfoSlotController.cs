using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SkillInfoSlotController : MonoBehaviour
{
    [SerializeField] private SkillSO skillSO;
    [SerializeField] private Image imgSkillIcon;
    [SerializeField] private TextMeshProUGUI txtSkillLevel;
    [SerializeField] private Button btnSkillUpgrade;
    [SerializeField] private Button btnSkillDowngrade;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }

    public void LoadInfoSkillSO(SkillSO skillSO)
    {
        this.skillSO = skillSO;
        imgSkillIcon.sprite = skillSO.sprSkillIcon_;
        string[] arrSkillID = skillSO.strSkillID_.Split(new char[] { '_' });
        txtSkillLevel.text = skillSO.strSkillLevel_ + "/" + SkillSystemManager.instance.skillDatabase_.skillData[arrSkillID[0]].Count;
        OnChangeSkillUpgradeBtn();
    }

    public void LoadSkillDescription()
    {
        SkillDescriptionController.instance.LoadSkillDescription(skillSO);
    }

    public async void OnClickUpgradeSkill(int upgrade)
    {
        bool checkUpgrade = false;
        switch (upgrade)
        {
            case 1:
                if(SkillSystemManager.instance.intSkillPoint_ > 0)
                    checkUpgrade = true;
                break;
            case -1:
                if (SkillSystemManager.instance.intSkillPoint_ > -1)
                    checkUpgrade = true;
                break;
        }
        if(skillSO != null && checkUpgrade)
        {
            AsyncLoadingScene.instance.LoadingScreen(true);
            string[] arrSkillID = skillSO.strSkillID_.Split(new char[] { '_' });
            int nextSkillLevel = int.Parse(arrSkillID[1].ToString()) + upgrade;
            string nextSkillName = arrSkillID[0] + "_" + nextSkillLevel;
            SkillSO newSkillSO = SkillSystemManager.instance.skillDatabase_.skillData[arrSkillID[0]].Find(x => x.name == nextSkillName);
            LoadInfoSkillSO(newSkillSO);
            SkillSystemManager.instance.UpdateSkillTree(newSkillSO);
            SkillSystemManager.instance.UpdateSkillPoint(-upgrade);
            SkillDescriptionController.instance.LoadSkillDescription(newSkillSO);
            SkillSystemManager.instance.LoadSkillPoint();
            await Task.Delay(500);
            AsyncLoadingScene.instance.LoadingScreen(false);
        }
    }

    private void OnChangeSkillUpgradeBtn()
    {
        if (skillSO.strSkillLevel_ >= 5)
        {
            btnSkillUpgrade.interactable = false;
            Debug.Log("skillSO.strSkillLevel_ >= 5: " + skillSO.strSkillName_ + " - " + skillSO.strSkillLevel_);
        }

        else if(skillSO.strSkillLevel_ <= 1)
        {
            btnSkillDowngrade.interactable = false;
            Debug.Log("skillSO.strSkillLevel_ <=1: " + skillSO.strSkillName_ + " - " + skillSO.strSkillLevel_);
        }
        else
        {
            btnSkillUpgrade.interactable = true;
            btnSkillDowngrade.interactable = true;
        }
    }
}
