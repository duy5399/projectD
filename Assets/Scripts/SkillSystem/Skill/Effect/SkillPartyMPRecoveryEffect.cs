using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPartyMPRecoveryEffect : SkillEffect
{
    protected override void OnEnable()
    {
        base.OnEnable();
        animator.SetBool("SkillPartyMPRecoveryEffect", true);
    }

    protected override void Update()
    {
        DestroyGameObject("SkillPartyMPRecoveryEffect");
    }

    protected override void DestroyGameObject(string condition)
    {
        base.DestroyGameObject(condition);
    }
}