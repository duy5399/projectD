using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHealEffect : SkillEffect
{
    protected override void OnEnable()
    {
        base.OnEnable();
        animator.SetBool("SkillHealEffect", true);
    }

    protected override void Update()
    {
        DestroyGameObject("SkillHealEffect");
    }

    protected override void DestroyGameObject(string condition)
    {
        base.DestroyGameObject(condition);
    }
}
