using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidasController : BossController
{
    public static MidasController instance { get; private set; }
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
        summonPoint = GameObject.Find("MobOfMidas").transform;
        areaOfEffectPoint = transform.GetChild(2);
        areaOfEffectRange = 10f;
        MidasAttack.instance.LoadBulletToAttack(prefabBullet);
        MidasSummon.instance.LoadMobToSummon(prefabMob, numberOfMob);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override IEnumerator BossMechanics(float _intervalNextAction)
    {
        UIController.instance.OnAlertWarning("Bạn đã tiến vào cung điện của Midas, hắn đang rất tức giận, hãy cẩn thận!!!");
        while (!isDead)
        {
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatA();
            yield return new WaitUntil(()=> nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            UIController.instance.OnAlertWarning("Midas chuẩn bị giậm nhảy - gây sát thương cực lớn, hãy cẩn thận tránh né!!!");
            BeatB();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatA();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatC();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            //Debug.Log("turn: " + turnAction);
        }
        yield return null;
    }

    private void BeatA()
    {
        //bullet4
        AudioManager.instance.MidasBeatASFX(audioSource);
        anim.SetTrigger("beatA");
    }

    public void BeatB()
    {
        //jump
        AudioManager.instance.MidasBeatBSFX(audioSource);
        anim.SetTrigger("beatB");
    }

    public void BeatC()
    {
        //hồi máu, tăng giáp và sát thương
        AudioManager.instance.MidasBeatCSFX(audioSource);
        anim.SetTrigger("beatC");
    }

    public override void BuffATK()
    {
        attackDamage = attackDamage + (attackDamage / 10);
    }

    public override void BuffDEF()
    {
        armor = armor + (armor / 10);
    }

    protected void HealHP()
    {
        currentHealth = (currentHealth + (maxHealth*0.05)) < maxHealth ? (currentHealth + (int)(maxHealth * 0.05)) :  maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
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

    public void MidasDieSFX()
    {
        AudioManager.instance.MidasDieSFX(audioSource);
    }
}