using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CthulhuTotem : ObjectPool
{
    public static CthulhuTotem instance { get; private set; }

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
        poolManager = GameObject.Find("TotemPool").transform;
    }

    //tải object từ list data vào pool với số lượng truyền vào -> lấy ngẫu nhiên
    protected override void LoadObjectToPool()
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            GameObject mob = Instantiate(prefabList[i]);
            mob.transform.parent = poolManager;
            mob.GetComponent<Totem>().LoadData(CthulhuController.instance.maxHealth_ / 10, CthulhuController.instance.armor_ / 10);
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

    //tải object từ list data vào pool
    public void LoadTotemToSummon(List<GameObject> _prefabTotem)
    {
        LoadPrefabDB(_prefabTotem);
        LoadObjectToPool();
    }

    public void CallA_Totem()
    {
        int rand = Random.Range(0, prefabList.Count);
        GameObject totem = GetObjectFromPool(objPool[rand]);
        if (totem != null)
        {
            totem.GetComponent<Totem>().LoadData(CthulhuController.instance.maxHealth_ / 10, CthulhuController.instance.armor_ / 10);
            float positionX = CthulhuController.instance.facingLeft_ ? transform.position.x - 2f : transform.position.x + 2f;
            totem.transform.position = new Vector3(positionX, -2.2f, transform.position.z);
            totem.SetActive(true);
            StartCoroutine(totem.GetComponent<Totem>().Function());
        }
    }
}
