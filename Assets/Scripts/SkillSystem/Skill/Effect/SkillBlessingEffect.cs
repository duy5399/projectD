using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBlessingEffect : SkillEffect
{
    protected override void OnEnable()
    {
        base.OnEnable();
        animator.SetBool("SkillBlessingEffect", true);
    }

    protected override void Update()
    {
        DestroyGameObject("SkillBlessingEffect");
    }

    protected override void DestroyGameObject(string condition)
    {
        base.DestroyGameObject(condition);
    }
}

