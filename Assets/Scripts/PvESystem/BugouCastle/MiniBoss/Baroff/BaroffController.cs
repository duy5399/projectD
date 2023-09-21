using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaroffController : BossController
{
    public static BaroffController instance { get; private set; }
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

    [SerializeField] private Transform areaOfEffectPoint;
    [SerializeField] private float areaOfEffectRange;
    [SerializeField] private LayerMask layerMask;
    public Transform areaOfEffectPoint_ => areaOfEffectPoint;
    public float areaOfEffectRange_ => areaOfEffectRange;
    public LayerMask layerMask_ => layerMask;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        summonPoint = GameObject.Find("MobOfBaroff").transform;
        areaOfEffectPoint = transform.GetChild(2);
        areaOfEffectRange = 10f;
        BaroffAttack.instance.LoadBulletToAttack(prefabBullet);
        BaroffSummon.instance.LoadMobToSummon(prefabMob, numberOfMob);
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
            UIController.instance.OnAlertWarning("Baroff tiến hành triệu hồi các chiến binh gà dũng cảm, hãy cẩn thận!!!");
            BeatC();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            UIController.instance.OnAlertWarning("Baroff chuẩn bị giậm nhảy - gây sát thương cực lớn, hãy cẩn thận tránh né!!!");
            BeatB();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatA();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatD();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            turnAction++;
            if (turnAction == 1)
            {
                CanAttack();
            }
            Debug.Log("turn: " + turnAction);
        }
        yield return null;
    }

    private void BeatA()
    {
        //bullet4
        AudioManager.instance.BaroffBeatASFX(audioSource);
        anim.SetTrigger("beatA");
    }

    public void BeatB()
    {
        //jump
        AudioManager.instance.BaroffBeatBSFX(audioSource);
        anim.SetTrigger("beatB");
    }


    public void BeatC()
    {
        //summon mobs
        AudioManager.instance.BaroffBeatCSFX(audioSource);
        anim.SetTrigger("beatC");
    }

    public void BeatD()
    {
        //buff def
        anim.SetTrigger("beatD");
    }

    public override void BuffATK()
    {
        attackDamage = attackDamage + (attackDamage / 10);
    }

    public override void BuffDEF()
    {
        armor = armor + (armor / 10);
    }

    private void CanAttack()
    {
        UIController.instance.OnAlertWarning("Baroff đã xuất hiện điểm yếu, hãy dồn lực tấn công!!!");
        anim.SetTrigger("born");
        gameObject.layer = LayerMask.NameToLayer("Boss");
    }

    private void SetLayer(string _layer)
    {
        gameObject.layer = LayerMask.NameToLayer(_layer);
    }

    void OnDrawGizmosSelected()
    {
        if (areaOfEffectPoint == null)
            return;
        Gizmos.DrawWireSphere(areaOfEffectPoint.position, areaOfEffectRange);
    }

    public void BaroffDieSFX()
    {
        AudioManager.instance.BaroffDieSFX(audioSource);
    }
}