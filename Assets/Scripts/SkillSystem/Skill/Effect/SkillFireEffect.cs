using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillFireEffect : SkillEffect
{
    [SerializeField] private List<GameObject> listEnemy = new List<GameObject>();
    [SerializeField] private float floatTimeDelay = 1f;
    [SerializeField] private float floatTimer = 0f;
    [SerializeField] private float i = 0;
    protected override void OnEnable()
    {
        base.OnEnable();
        animator.SetBool("SkillFireEffect", true);
    }

    protected override void Update()
    {
        DestroyGameObject("SkillFireEffect");
        BurnEnemyAfterTime();
    }

    protected override void DestroyGameObject(string condition)
    {
        base.DestroyGameObject(condition);
    }

    private void BurnEnemyAfterTime()
    {
        floatTimer += Time.deltaTime;
        if (floatTimer > floatTimeDelay)
        {
            floatTimeDelay++;
            i++;
            Debug.Log("i: " + i);
            foreach (var enemy in listEnemy)
            {
                Debug.Log("BurnEnemyAfterTime: " + enemy.GetPhotonView().Owner);
                DealingDamage(enemy);
            }
        }
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
                            listEnemy.Add(collision.gameObject);
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

    void OnTriggerExit2D(Collider2D collision)
    {
        try
        {
            listEnemy.Remove(collision.gameObject);
            Debug.Log("OnTriggerExit2D: " + collision.gameObject.GetPhotonView().Owner);
        }
        catch 
        {
            Debug.Log("Lỗi OnTriggerExit2D: SkillFireEffect");
        }
    }
}
