using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMPRecoveryEffect : SkillEffect
{
    protected override void OnEnable()
    {
        base.OnEnable();
        animator.SetBool("SkillMPRecoveryEffect", true);
    }

    protected override void Update()
    {
        DestroyGameObject("SkillMPRecoveryEffect");
    }

    protected override void DestroyGameObject(string condition)
    {
        base.DestroyGameObject(condition);
    }
}
