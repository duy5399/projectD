using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CthulhuController : BossController
{
    public static CthulhuController instance { get; private set; }

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

    [SerializeField] protected List<GameObject> prefabTotem;

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
        summonPoint = GameObject.Find("MobOfCthulhu").transform;
        areaOfEffectPoint = transform.GetChild(2);
        areaOfEffectRange = 30f;
        CthulhuAttack.instance.LoadBulletToAttack(prefabBullet);
        CthulhuSummon.instance.LoadMobToSummon(prefabMob, numberOfMob);
        CthulhuTotem.instance.LoadTotemToSummon(prefabTotem);
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
            UIController.instance.OnAlertWarning("Cthulhu tiến hành triệu hồi các dũng sĩ bộ tộc, hãy cẩn thận!!!");
            CallB();
            yield return new WaitUntil(()=> nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatA();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            UIController.instance.OnAlertWarning("Cthulhu chuẩn bị giải phóng kỹ năng sát thương diện rộng, hãy cẩn thận tránh né!!!");
            BeatC();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            BeatB();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            UIController.instance.OnAlertWarning("Cthulhu triệu hồi các tổ vật kỳ lạ , hãy chú ý!!!");
            CallA();
            yield return new WaitUntil(() => nextAction);
            yield return new WaitForSeconds(_intervalNextAction);
            turnAction++;
            if (turnAction < 5)
            {
                if (turnAction == 1)
                {
                    gameObject.layer = LayerMask.NameToLayer("Boss");
                    //StartCoroutine(Movement());
                }
            }
            //Debug.Log("turn: " + turnAction);
        }
        yield return null;
    }

    private void BeatA()
    {
        //spear
        AudioManager.instance.CthulhuBeatA_BSFX(audioSource);
        anim.SetTrigger("beatA");
    }

    private void BeatB()
    {
        //stone statue
        AudioManager.instance.CthulhuBeatA_BSFX(audioSource);
        anim.SetTrigger("beatB");
    }

    private void BeatC()
    {
        //skill aoe
        anim.SetTrigger("beatC");
    }

    private void CallA()
    {
        //summon totem
        AudioManager.instance.CthulhuCallASFX(audioSource);
        anim.SetTrigger("callA");
    }

    private void CallB()
    {
        //summon mob
        AudioManager.instance.CthulhuCallBSFX(audioSource);
        anim.SetTrigger("callB");
    }

    public override void BuffATK()
    {
        attackDamage = attackDamage + (attackDamage / 10);
    }

    public override void BuffDEF()
    {
        armor = armor + (armor / 10);
    }

    public void HealHP()
    {
        if(currentHealth < maxHealth)
        {
            currentHealth = (currentHealth + (maxHealth * 0.05f)) < maxHealth ? currentHealth + (int)(maxHealth*0.05f) : maxHealth;
            UpdateHealth();
        }        
    }

    public void CthulhuBeatCSFX()
    {
        AudioManager.instance.CthulhuBeatCSFX(audioSource);
    }

    public void CthulhuDieSFX()
    {
        AudioManager.instance.CthulhuDieSFX(audioSource);
    }
}