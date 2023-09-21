using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected BoxCollider2D boxCollider2D;
    [SerializeField] protected AudioSource audioSource;

    [Header("Health")]
    [SerializeField] protected HealthBarController healthBar;
    [SerializeField] protected int armor;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected bool isDead;

    public int maxHealth_ => maxHealth;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        healthBar = this.transform.GetChild(0).GetComponent<HealthBarController>();
        isDead = false;
    }

    public void LoadData(int _maxHealth, int _armor)
    {
        isDead = false;
        boxCollider2D.enabled = true;
        maxHealth = _maxHealth;
        armor = _armor;
        currentHealth = _maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
    }

    public virtual IEnumerator Function()
    {
        yield return null;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth > 0)
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
        anim.SetTrigger("isDead");
    }

    protected void HideTorem()
    {
        if (isDead)
        {
            gameObject.SetActive(false);
        }
    }

    protected void TotemBornSFX()
    {
        AudioManager.instance.TotemBornSFX(audioSource);
    }

    protected void TotemDieSFX()
    {
        AudioManager.instance.TotemDieSFX(audioSource);
    }
}
