using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BaroffAttack : ObjectPool
{
    public static BaroffAttack instance { get; private set; }

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
            bullet.transform.GetComponent<Bullet4>().SetDamage(BaroffController.instance.attackDamage_);
            bullet.transform.GetComponent<Bullet4>().SetParameter(5f, BaroffController.instance.attackPoint_, BaroffController.instance.target_.position);
            bullet.transform.position = BaroffController.instance.attackPoint_.position;
            bullet.SetActive(true);
        }
    }

    public void BeatB_Jump()
    {
        float positionX = BaroffController.instance.facingLeft_ ? BaroffController.instance.target_.position.x + 3f : BaroffController.instance.target_.position.x - 3f;
        BaroffController.instance.transform.position = new Vector3(positionX, BaroffController.instance.transform.position.y, BaroffController.instance.transform.position.z);
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(BaroffController.instance.areaOfEffectPoint_.position, BaroffController.instance.areaOfEffectRange_, BaroffController.instance.layerMask_);
        foreach (Collider2D player in hitPlayer)
        {
            if (player.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                player.GetComponent<PlayerCombat>().TakingDamage(BaroffController.instance.attackDamage_, null);
                player.GetComponent<PlayerMovement>().SlowEffect(0f, 3f);
            }
        }
    }
}
