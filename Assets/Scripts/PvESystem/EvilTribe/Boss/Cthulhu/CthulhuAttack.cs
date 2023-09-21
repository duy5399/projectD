using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CthulhuAttack : ObjectPool
{
    public static CthulhuAttack instance { get; private set; }

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
        poolManager = GameObject.Find("BulletPool").transform;
    }

    //tải object từ list data vào pool
    public void LoadBulletToAttack(List<GameObject> _prefabBullet)
    {
        LoadPrefabDB(_prefabBullet);
        LoadObjectToPool();
    }

    //tải object từ list data vào pool
    protected override void LoadObjectToPool()
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            GameObject mob = Instantiate(prefabList[i]);
            mob.transform.parent = poolManager;
            mob.SetActive(false);
            objPool.Add(mob);
        }
    }

    //lấy object từ pool
    protected override GameObject GetObjectFromPool(GameObject _gameObject)
    {
        for (int i = 0; i < objPool.Count; i++)
        {
            if (objPool[i] == _gameObject && !objPool[i].activeInHierarchy)
            {
                return objPool[i];
            }
        }
        GameObject mob = Instantiate(_gameObject);
        mob.transform.parent = poolManager;
        mob.SetActive(false);
        objPool.Add(mob);
        return objPool[objPool.Count - 1];
    }

    public void BeatA_Spear()
    {
        GameObject bullet = GetObjectFromPool(objPool.Find(x => x.name == "Spear(Clone)"));
        if (bullet != null)
        {
            bullet.transform.GetComponent<Spear>().SetDamage(CthulhuController.instance.attackDamage_);
            bullet.transform.GetComponent<Spear>().SetParameter(5f, CthulhuController.instance.attackPoint_, CthulhuController.instance.target_.position);
            bullet.transform.position = CthulhuController.instance.attackPoint_.position;
            bullet.SetActive(true);
        }
    }

    public void BeatB_StoneStatue()
    {
        GameObject bullet = GetObjectFromPool(objPool.Find(x => x.name == "StoneStatue(Clone)"));
        if (bullet != null)
        {
            bullet.transform.GetComponent<StoneStatue>().SetDamage(CthulhuController.instance.attackDamage_*2);
            bullet.transform.GetComponent<StoneStatue>().SetParameter(10f, CthulhuController.instance.attackPoint_, CthulhuController.instance.target_.position);
            bullet.transform.position = CthulhuController.instance.attackPoint_.position;
            bullet.SetActive(true);
        }
    }

    public void BeatC_AOE()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(CthulhuController.instance.areaOfEffectPoint_.position, CthulhuController.instance.areaOfEffectRange_, CthulhuController.instance.layerMask_);
        foreach (Collider2D player in hitPlayer)
        {
            if (player.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                player.GetComponent<PlayerCombat>().TakingDamage(CthulhuController.instance.attackDamage_, null);
                player.GetComponent<PlayerMovement>().SlowEffect(0f, 3f);
            }
        }
    }
}
