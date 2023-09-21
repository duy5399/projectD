using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MidasAttack : ObjectPool
{
    public static MidasAttack instance { get; private set; }

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

    public void BeatA_Bullet4()
    {
        GameObject bullet = GetObjectFromPool(objPool.Find(x => x.name == "Bullet4(Clone)"));
        if (bullet != null)
        {
            bullet.transform.GetComponent<Bullet4>().SetDamage(MidasController.instance.attackDamage_);
            bullet.transform.GetComponent<Bullet4>().SetParameter(5f, MidasController.instance.attackPoint_, MidasController.instance.target_.position);
            bullet.transform.position = MidasController.instance.attackPoint_.position;
            bullet.SetActive(true);
        }
    }

    public void BeatB_Jump()
    {
        float positionX = MidasController.instance.facingLeft_ ? MidasController.instance.target_.position.x + 3f : MidasController.instance.target_.position.x - 3f;
        MidasController.instance.transform.position = new Vector3(positionX, MidasController.instance.transform.position.y, MidasController.instance.transform.position.z);
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(MidasController.instance.areaOfEffectPoint_.position, MidasController.instance.areaOfEffectRange_, MidasController.instance.layerMask_);
        foreach (Collider2D player in hitPlayer)
        {
            if (player.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                player.GetComponent<PlayerCombat>().TakingDamage(MidasController.instance.attackDamage_, null);
                player.GetComponent<PlayerMovement>().SlowEffect(0f, 3f);
            }
        }
    }
}
