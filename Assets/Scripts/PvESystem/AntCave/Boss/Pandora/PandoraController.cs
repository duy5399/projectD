using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandoraController : BossController
{
    public static PandoraController instance { get; private set; }

    void Awake()
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

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        summonPoint = GameObject.Find("MobOfPandora").transform;
        PandoraAttack.instance.LoadBulletToAttack(prefabBullet);
        PandoraSummon.instance.LoadMobToSummon(prefabMob, numberOfMob);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override IEnumerator BossMechanics(float _intervalNextAction)
    {
        while (!isDead)
        {
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            UIController.instance.OnAlertWarning("Pandora tiến hành triệu hồi các chiến binh kiến, hãy cẩn thận!!!");
            BeatA();
            yield return new WaitUntil(()=> nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatB();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatC();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatD();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatE();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            turnAction++;
            if (turnAction < 5)
            {
                if (turnAction == 1)
                {
                    StartCoroutine(Movement());
                }
            }
            //Debug.Log("turn: " + turnAction);
        }
        yield return null;
    }

    private void BeatA()
    {
        //summon mobs
        AudioManager.instance.PandoraBeatASFX(audioSource);
        anim.SetTrigger("beatA");
    }

    public void BeatB()
    {
        //fire arrow
        AudioManager.instance.PandoraBeatB_DSFX(audioSource);
        anim.SetTrigger("beatB");
    }

    public void BeatC()
    {
        //lightning arrow
        AudioManager.instance.PandoraBeatCSFX(audioSource);
        anim.SetTrigger("beatC");
    }

    public void BeatD()
    {
        //ice arrow
        AudioManager.instance.PandoraBeatASFX(audioSource);
        anim.SetTrigger("beatD");
    }

    public void BeatE()
    {
        //ice arrow
        AudioManager.instance.PandoraBeatESFX(audioSource);
        anim.SetTrigger("beatE");
    }

    public override void BuffATK()
    {
        attackDamage = attackDamage + (attackDamage / 10);
    }

    public override void BuffDEF()
    {
        armor = armor + (armor / 10);
    }

    private IEnumerator Movement()
    {
        UIController.instance.OnAlertWarning("Pandora chuẩn bị ra khỏi tổ, hãy sẵn sàng tấn công!!!");
        AllowNextAction(0);
        GameObject teleport = transform.GetChild(2).gameObject;
        teleport.SetActive(true);
        yield return new WaitForSeconds(2f);
        Vector2 newPosition = new Vector2(6f, -1.8f);
        this.transform.position = newPosition;
        teleport.SetActive(false);
        gameObject.layer = LayerMask.NameToLayer("Boss");
        AllowNextAction(1);
    }

    public void PandoraDieSFX()
    {
        AudioManager.instance.PandoraDieSFX(audioSource);
    }
}