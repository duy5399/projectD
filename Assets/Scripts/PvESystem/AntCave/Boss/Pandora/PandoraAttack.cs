using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PandoraAttack : ObjectPool
{
    public static PandoraAttack instance { get; private set; }

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

    public void BeatD_IceArrow()
    {
        GameObject bullet = GetObjectFromPool(objPool.Find(x => x.name == "IceArrow(Clone)"));
        if (bullet != null)
        {
            bullet.transform.GetComponent<IceArrow>().SetDamage(PandoraController.instance.attackDamage_/10);
            bullet.transform.GetComponent<IceArrow>().SetParameter(5f, PandoraController.instance.attackPoint_, PandoraController.instance.target_.position);
            bullet.transform.position = PandoraController.instance.attackPoint_.position;
            bullet.SetActive(true);
        }
    }

    public void BeatB_FireArrow()
    {
        GameObject bullet = GetObjectFromPool(objPool.Find(x => x.name == "FireArrow(Clone)"));
        if (bullet != null)
        {
            bullet.transform.GetComponent<FireArrow>().SetDamage(PandoraController.instance.attackDamage_);
            bullet.transform.GetComponent<FireArrow>().SetParameter(5f, PandoraController.instance.attackPoint_, PandoraController.instance.target_.position);
            bullet.transform.position = PandoraController.instance.attackPoint_.position;
            bullet.SetActive(true);
        }
    }

    public void BeatC_LightningArrow()
    {
        GameObject bullet = GetObjectFromPool(objPool.Find(x => x.name == "LightningArrow(Clone)"));
        if (bullet != null)
        {
            bullet.transform.GetComponent<LightningArrow>().SetDamage(PandoraController.instance.attackDamage_);
            bullet.transform.GetComponent<LightningArrow>().SetParameter(2f, 3);
            bullet.SetActive(true);
            StartCoroutine(bullet.transform.GetComponent<LightningArrow>().MoveToTargetWithMark());
        }
    }
}
