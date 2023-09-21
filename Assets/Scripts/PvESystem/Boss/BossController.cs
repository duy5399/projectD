using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour, IDamageable
{
    [SerializeField] protected BossSO bossSO;

    [Header("Components")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected Transform summonPoint;
    [SerializeField] protected AudioSource audioSource;
    public Transform attackPoint_ => attackPoint;
    public Transform summonPoint_ => summonPoint;

    [Header("Target")]
    [SerializeField] protected Transform target;
    public Transform target_ => target;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed; 

    [Header("Flip")]
    [SerializeField] protected bool facingLeft; 
    public bool facingLeft_ => facingLeft;

    [Header("Attack")]
    [SerializeField] protected int attackDamage; 
    [SerializeField] protected List<GameObject> prefabBullet; 
    public int attackDamage_ => attackDamage; 

    [Header("Health")]
    [SerializeField] protected int maxHealth; 
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int armor; 
    [SerializeField] protected HealthBarController healthBar;
    [SerializeField] protected bool isDead;
    public int maxHealth_ => maxHealth;
    public int currentHealth_ => currentHealth;
    public int armor_ => armor; 
    public bool isDead_ => isDead;

    [Header("SummonMob")]
    [SerializeField] protected List<GameObject> prefabMob; 
    [SerializeField] protected int numberOfMob; 
    public List<GameObject> prefabMob_ => prefabMob; 
    public int numberOfMob_ => numberOfMob; 

    [Header("Mechanic")]
    [SerializeField] protected IEnumerator bossMechanics;
    [SerializeField] protected bool nextAction;
    [SerializeField] protected float intervalNextAction; 
    [SerializeField] protected int turnAction;

    public IEnumerator bossMechanics_ => bossMechanics;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        attackPoint = transform.GetChild(0).transform;
        audioSource = GetComponent<AudioSource>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        gameObject.layer = LayerMask.NameToLayer("CantAttack");
        healthBar = transform.GetChild(1).GetComponent<HealthBarController>();
        isDead = false;
        currentHealth = maxHealth;
        turnAction = 0;
        //UpdateHealth();
        bossMechanics = BossMechanics(intervalNextAction);
        nextAction = true;
    }

    protected virtual void FixedUpdate()
    {
        if(!isDead)
        {
            Flip();
        }
    }

    public void SetDataForBoss(BossSO _bossSO)
    {
        this.bossSO = _bossSO;
        this.moveSpeed = _bossSO.moveSpeed_;
        this.facingLeft = _bossSO.facingLeft_;
        this.attackDamage = _bossSO.attackDamage_;
        this.prefabBullet = _bossSO.prefabBullet_;
        this.maxHealth = _bossSO.maxHealth_;
        this.armor = _bossSO.armor_;
        this.prefabMob = _bossSO.prefabMob_;
        this.numberOfMob = _bossSO.numberOfMob_;
        this.intervalNextAction = _bossSO.intervalNextAction_;
    }

    protected virtual IEnumerator BossMechanics(float _interval)
    {
        yield return null;
    }

    public void UpdateHealth()
    {
        healthBar.SetHealth(currentHealth, maxHealth);
        UIController.instance.SetHealth(currentHealth, maxHealth);
    }

    protected virtual void Attack()
    {

    }

    protected virtual IEnumerator SummonMobs()
    {
        yield return null;
    }


    public virtual void BuffATK()
    {
       
    }

    public virtual void BuffDEF()
    {
        
    }

    protected void Flip()
    {
        if (this.transform.position.x > target.transform.position.x && !facingLeft)
        {
            this.transform.localScale = new Vector3(1, this.transform.localScale.y, this.transform.localScale.z);
            facingLeft = true;
        }
        else if (this.transform.position.x < target.transform.position.x && facingLeft)
        {
            this.transform.localScale = new Vector3(-1, this.transform.localScale.y, this.transform.localScale.z);
            facingLeft = false;
        }
    }

    public virtual void TakeDamage(int _damage)
    {
        int dmg = _damage * 100 / (100 + armor);
        currentHealth -= dmg;
        UpdateHealth();
        anim.SetTrigger("cry");
        if (currentHealth <= 0)
        {
            isDead = true;
            StopMechanics();
            anim.SetBool("isDead", isDead);
        }
    }

    protected void AllowNextAction(int  _nextAction)
    {
        if(_nextAction == 1)
            nextAction = true;
        else
            nextAction = false;
    }

    public void StartMechanics()
    {
        if (bossMechanics != null)
        {
            StartCoroutine(bossMechanics);
        }
    }

    public void StopMechanics()
    {
        if (bossMechanics != null)
        {
            StopCoroutine(bossMechanics);
            bossMechanics = null;
        }
    }

    protected void HideBoss()
    {
        if (isDead)
        {
            gameObject.SetActive(false);
        }
    }
}
