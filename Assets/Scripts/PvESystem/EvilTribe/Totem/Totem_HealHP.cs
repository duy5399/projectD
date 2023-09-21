using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem_HealHP : Totem
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override IEnumerator Function()
    {
        while (!isDead && TimerInBattle.instance.timerIsRunning_)
        {
            //Debug.Log("Totem_HealHP");
            CthulhuController.instance.HealHP();
            yield return new WaitForSeconds(2f);
        }
    }
}
