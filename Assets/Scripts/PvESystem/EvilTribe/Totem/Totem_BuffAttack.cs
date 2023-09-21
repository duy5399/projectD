using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem_BuffAttack : Totem
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override IEnumerator Function()
    {
        while (!isDead && TimerInBattle.instance.timerIsRunning_)
        {
            CthulhuController.instance.BuffATK();
            yield return new WaitForSeconds(2f);
        }
    }
}
