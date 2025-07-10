using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PoolInfo
{
    public GameObject prefabs;
    public int initialSize;
}

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField] private List<PoolInfo> poolList;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
        InitializePool();
    }

    private void InitializePool()
    {
        foreach (var poolInfo in poolList)
        {
            var objectQueue = new Queue<GameObject>();
            for (int i = 0; i < poolInfo.initialSize; i++)
            {
                GameObject obj = Instantiate(poolInfo.prefabs, transform);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }
            poolDictionary.Add(poolInfo.prefabs, objectQueue);
        }
    }

    public GameObject Get(GameObject prefabs, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefabs))
        {
            poolDictionary.Add(prefabs, new Queue<GameObject>());
        }
        Queue<GameObject> pool = poolDictionary[prefabs];
       

        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
      
        }
        else
        {
            obj = Instantiate(prefabs, position, rotation);
     
        }
        return obj;
    }

    public void Return(GameObject obj, GameObject sourcePrefabs)
    {
        if (sourcePrefabs == null)
        {
  

            Destroy(obj);
            return;
        }

        if (poolDictionary.ContainsKey(sourcePrefabs))
        {
            obj.SetActive(false);
            poolDictionary[sourcePrefabs].Enqueue(obj);

        }
        else
        {
     
            // 풀이 없는 경우 그냥 파괴
            Destroy(obj);
        }
    }

}
