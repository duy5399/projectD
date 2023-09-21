using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSO : ScriptableObject
{
    [SerializeField] protected string strSkillID;
    [SerializeField] protected Sprite sprSkillIcon;
    [SerializeField] protected string strSkillName;
    [SerializeField] protected string strSkillDescription;
    [SerializeField] protected int strSkillLevel;
    [SerializeField] protected int intMPCost;
    [SerializeField] protected string strSkillType;
    [SerializeField] protected float floatCooldown;
    [SerializeField] protected float floatEffectDuration;
    [SerializeField] protected GameObject goSkillEffect;

    [Header("Attribute Healing")]
    [SerializeField] protected float floatFlatHealingHP;
    [SerializeField] protected float floatPercentageHealingHP;
    [Header("Attribute MpRecovery")]
    [SerializeField] protected float floatFlatMPRecovery;
    [SerializeField] protected float floatPercentageMPRecovery;
    [Header("Attribute Damage")]
    [SerializeField] protected float floatFlatDamage;
    [SerializeField] protected float floatPercentageDamage;
    [Header("Attribute Attack")]
    [SerializeField] protected float floatFlatAttack;
    [SerializeField] protected float floatPercentageAttack;
    [Header("Attribute Shield")]
    [SerializeField] protected float floatFlatShield;
    [SerializeField] protected float floatPercentageShield;

    public string strSkillID_ => strSkillID;
    public Sprite sprSkillIcon_ => sprSkillIcon;
    public string strSkillName_ => strSkillName;
    public string strSkillDescription_ => strSkillDescription;
    public int strSkillLevel_ => strSkillLevel;
    public int intMPCost_ => intMPCost;
    public string strSkillType_ => strSkillType;
    public float floatCooldown_ => floatCooldown;

    public virtual void Skill()
    {
        
    }
    public virtual void Skill(int playerID)
    {

    }

    public virtual void Update() 
    { 
    
    }

    public virtual void Destroy()
    {

    }

    enum SkillType{
        passsive,
        active
    }
}
