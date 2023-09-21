using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningArrow : Bullet
{
    [SerializeField] private Animator anim;
    [SerializeField] private float attackInterval;
    [SerializeField] private int numberOfAttack;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetParameter(float _attackInterval, int _numberOfAttack)
    {
        attackInterval = _attackInterval;
        numberOfAttack = _numberOfAttack;
    }

    //spawn bullet on position of player
    public IEnumerator MoveToTargetWithMark()
    {
        for (int i = 0; i < numberOfAttack; i++)
        {
            if (!anim.enabled)
            {
                SetAnim(1);
            }
            Vector3 targetPoint = GameObject.FindGameObjectWithTag("Player").transform.position;
            transform.position = new Vector3(targetPoint.x, -2.246191f, 0);
            yield return new WaitForSeconds(attackInterval);
        }
        gameObject.SetActive(false);
    }

    public void SetAnim(int  _enable)
    {
        if(_enable == 1)
        {
            anim.enabled = true;
        }
        else
        {
            anim.enabled = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collider.gameObject.GetComponent<PlayerCombat>().TakingDamage(damage, null);
            collider.GetComponent<PlayerMovement>().SlowEffect(0f, 0.5f);
            //Debug.Log("Hit player");
        }
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //Debug.Log("Hit ground");
        }
        //GameObject frost_effect = Instantiate(blastOut, transform.position, Quaternion.identity);
        //Destroy(gameObject);
    }
}
