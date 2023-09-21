using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidasSummon : ObjectPool
{
    public static MidasSummon instance { get; private set; }

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
        poolManager = GameObject.Find("MobPool").transform;
    }

    //tải object từ list data vào pool với số lượng truyền vào -> lấy ngẫu nhiên
    protected override void LoadObjectToPool(int _sizeOfPool)
    {
        for (int i = 0; i < _sizeOfPool; i++)
        {
            int rand = Random.Range(0, prefabList.Count);
            GameObject mob = Instantiate(prefabList[rand]);
            mob.transform.parent = poolManager;
            mob.SetActive(false);
            objPool.Add(mob);
        }
    }

    //lấy object từ pool
    protected override GameObject GetObjectFromPool()
    {
        for (int i = 0; i < objPool.Count; i++)
        {
            if (!objPool[i].activeInHierarchy)
            {
                return objPool[i];
            }
        }
        LoadObjectToPool(1);
        return objPool[objPool.Count - 1];
    }

    public void LoadMobToSummon(List<GameObject> _prefabMob, int _numberOfMob)
    {
        LoadPrefabDB(_prefabMob);
        LoadObjectToPool(_numberOfMob);
    }

    //public IEnumerator BeatC_SummonMobs()
    //{
    //    for (int i = 0; i < BaroffController.instance.numberOfMob_; i++)
    //    {
    //        GameObject mob = GetObjectFromPool();
    //        if (mob != null)
    //        {
    //            mob.GetComponent<MobController>().LoadMobController();
    //            mob.transform.position = BaroffController.instance.summonPoint_.position;
    //            mob.SetActive(true);
    //        }
    //        yield return new WaitForSeconds(0.75f);
    //    }
    //}
}
