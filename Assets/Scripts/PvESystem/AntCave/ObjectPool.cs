using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] protected List<GameObject> prefabList;
    [SerializeField] protected List<GameObject> objPool;
    [SerializeField] protected Transform poolManager;

    //tải tất cả prefab vào danh sách dữ liệu để sử dụng
    protected void LoadPrefabDB(List<GameObject> _prefabDB)
    {
        foreach (GameObject prefab in _prefabDB)
        {
            prefabList.Add(prefab.gameObject);
        }
    }

    //tải object từ list data vào pool
    protected virtual void LoadObjectToPool()
    {

    }

    //tải object từ list data vào pool với số lượng truyền vào
    protected virtual void LoadObjectToPool(int _sizeOfPool)
    {
        
    }

    //lấy object từ pool
    protected virtual GameObject GetObjectFromPool()
    {
        return null;
    }

    //lấy object từ pool
    protected virtual GameObject GetObjectFromPool(GameObject _gameObject)
    {
        return null;
    }
}



