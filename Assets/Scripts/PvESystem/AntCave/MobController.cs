using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour, IDamageable
{
    [Header("ScriptableObjects")]
    [SerializeField] protected MobSO mobSO;
    public MobSO MobSO => mobSO;

    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private AudioSource audioSource;

    [Header("Movement")]
    [SerializeField] private GameObject target;
    [SerializeField] private float moveSpeed;

    [Header("Flip")]
    [SerializeField] private bool facingLeft = true;

    [Header("Attack")]
    [SerializeField] private bool isAttack;
    [SerializeField] private float attackRange;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private int attackDamage;
    [SerializeField] private LayerMask playerLayer;

    [Header("Health")]
    [SerializeField] private HealthBarController healthBar;
    [SerializeField] private int armor;
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isDead;

    public int maxHealth_ => maxHealth;

    void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        attackPoint = transform.GetChild(0);
        healthBar = this.transform.GetChild(1).GetComponent<HealthBarController>();
        LoadMobController();
    }
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    public void LoadMobController()
    {
        string resourcePath = "ScriptableObjects/Map/Mob/" + transform.tag;
        //Debug.Log(resourcePath);
        mobSO = Resources.Load<MobSO>(resourcePath);
        isDead = false;
        moveSpeed = MobSO.moveSpeed;
        attackDamage = MobSO.attackDamage;
        attackRange = MobSO.attackRange;
        armor = MobSO.armor;
        maxHealth = MobSO.maxHealth;
        currentHealth = maxHealth;     
        healthBar.SetHealth(currentHealth, maxHealth);
        boxCollider2D.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            UpdateMovement();
            ModFlip();
            MobAttack();
        }
    }

    public void UpdateMovement()
    {
        Vector2 targetPosition = new Vector2(target.transform.position.x, this.transform.position.y);
        this.transform.position = Vector2.MoveTowards(this.transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
    }

    public void ModFlip()
    {
        if(this.transform.position.x > target.transform.position.x && !facingLeft)
        {
            this.transform.localScale = new Vector3(1, this.transform.localScale.y, this.transform.localScale.z);
            facingLeft = true;
        }
        else if(this.transform.position.x < target.transform.position.x && facingLeft)
        {
            this.transform.localScale = new Vector3(-1, this.transform.localScale.y, this.transform.localScale.z);
            facingLeft = false;
        }
    }

    public void MobAttack()
    {
        if (Vector2.Distance(this.transform.position, target.transform.position) <= 0.5f) //attackRange
        {
            isAttack = true;
        }
        else
        {
            isAttack = false;          
        }
        anim.SetBool("beatA", isAttack);
    }

    public void MobDealDamage()
    {
        var hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerCombat>().TakingDamage(attackDamage, null);
            player.GetComponent<PlayerMovement>().SlowEffect(0f, 0.5f);
        }
    }

    public void TakeDamage(int damage)
    {
        if(currentHealth > 0)
        {
            int dmg = damage * 600 / (600 + armor);
            currentHealth -= dmg;
            healthBar.SetHealth(currentHealth, maxHealth);
            anim.SetTrigger("cry");
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        isDead = true;
        //anim.SetBool("isDead", isDead);
        anim.SetTrigger("isDead01");
    }

    protected void HideMob()
    {
        if (isDead)
        {
            gameObject.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void StartDazedTime()
    {
        moveSpeed = 0f;
    }

    public void EndDazedTime()
    {
        moveSpeed = MobSO.moveSpeed;
    }

    public void ResetIdle()
    {
        isDead = true;
        anim.SetTrigger("idle");
    }

    public void MobAttackSFX()
    {
        AudioManager.instance.MobAttackSFX(audioSource);
    }

    public void MobDieSFX()
    {
        AudioManager.instance.MobDieSFX(audioSource);
    }

    public void MobMove01()
    {
        AudioManager.instance.MobMove01SFX(audioSource);
    }

}
