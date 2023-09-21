using Photon.Pun.Demo.Procedural;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillColdBoltEffect : SkillEffect
{
    protected override void OnEnable()
    {
        base.OnEnable();
        animator.SetBool("SkillColdBoltEffect", true);
    }

    protected override void Update()
    {
        UpdatePosition(boolFlipX);
        DealingDamagePvE();
        DestroyGameObject("SkillColdBoltEffect");
    }

    protected override void DestroyGameObject(string condition)
    {
        base.DestroyGameObject(condition);
    }

    public void SetBoolFlipX(bool boolFlipX)
    {
        this.boolFlipX = boolFlipX;
        if (boolFlipX)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void UpdatePosition(bool flipX)
    {
        if (flipX)
            transform.Translate(Vector2.left * floatMoveSpeed * Time.deltaTime); //Translation là sự di chuyển object trong trục tọa độ X,Y hoặc Z
        else
            transform.Translate(Vector2.right * floatMoveSpeed * Time.deltaTime); //Translation là sự di chuyển object trong trục tọa độ X,Y hoặc Z
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["mapPvP"] == true)
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Boss" || collision.gameObject.tag == "MobA" || collision.gameObject.tag == "MobB")          //đối tượng va chạm là đạn
            {
                if (collision.gameObject.GetPhotonView() != null && photonViewOwner != null)
                {
                    if (collision.gameObject.GetPhotonView().Owner != photonViewOwner.Owner)
                    {
                        var team = PhotonNetwork.CurrentRoom.CustomProperties.Values.FirstOrDefault(kvp => kvp.ToString().Contains(photonViewOwner.Owner.NickName));
                        if (!team.ToString().Contains(collision.gameObject.GetPhotonView().Owner.NickName))
                        {
                            floatDamage = collision.gameObject.GetComponent<PlayerStats>().intHitPoint_ * floatDamage;
                            DealingDamage(collision.gameObject);
                            Destroy(gameObject);
                        }
                    }
                }
                else
                {
                    if (photonViewOwner == null)
                        Debug.Log("photonViewOwner == null");
                    if (collision.gameObject.GetPhotonView() == null)
                        Debug.Log("collision.gameObject.GetPhotonView() == null");
                }
            }
        }
    }
    protected override void DealingDamage(GameObject gameObject)
    {
        if(gameObject.tag == "Player")
            gameObject.GetComponent<PlayerCombat>().TakingTrueDamage((int)floatDamage, photonViewOwner);
        else
        {
            switch(gameObject.tag)
            {
                case "MobA":
                case "MobB":
                    gameObject.GetComponent<MobController>().TakeDamage((int)floatDamage);
                    break;
                case "Totem":
                    gameObject.GetComponent<Totem>().TakeDamage((int)floatDamage);
                    break;
                case "MiniBoss":
                case "Boss":
                    gameObject.GetComponent<BossController>().TakeDamage((int)floatDamage);
                    break;
            }
        }
    }

    public void DealingDamagePvE()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.gameObject.layer == LayerMask.NameToLayer("Mob"))
            {
                floatDamage = enemy.gameObject.GetComponent<MobController>().maxHealth_ * floatDamage;
                DealingDamage(enemy.gameObject);
            }
            else if (enemy.gameObject.layer == LayerMask.NameToLayer("Totem"))
            {
                floatDamage = enemy.gameObject.GetComponent<Totem>().maxHealth_ * floatDamage;
                DealingDamage(enemy.gameObject);
            }
            else if (enemy.gameObject.layer == LayerMask.NameToLayer("Boss") || enemy.gameObject.layer == LayerMask.NameToLayer("MiniBoss"))
            {
                floatDamage = enemy.gameObject.GetComponent<BossController>().maxHealth_ * (floatDamage * 0.5f);
                DealingDamage(enemy.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
