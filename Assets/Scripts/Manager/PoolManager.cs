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
        Debug.Log($"[Get] '{prefabs.name}' 풀 요청. 현재 개수: {pool.Count}");

        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            Debug.Log($"{prefabs.name} 재생성 현재 개수: {{pool.Count}} ");
        }
        else
        {
            obj = Instantiate(prefabs, position, rotation);
            Debug.Log($"{prefabs.name} 새로생성 현재 개수: {pool.Count}");
        }
        return obj;
    }

    public void Return(GameObject obj, GameObject sourcePrefabs)
    {
        if (sourcePrefabs == null)
        {
            Debug.LogError($"[PoolManager] {obj.name} 반납 실패! 원본 프리팹(sourcePrefab) 정보가 없습니다(null). 오브젝트를 파괴합니다.");

            Destroy(obj);
            return;
        }

        if (poolDictionary.ContainsKey(sourcePrefabs))
        {
            obj.SetActive(false);
            poolDictionary[sourcePrefabs].Enqueue(obj);
            Debug.Log($"<color=lime>{obj.name} 반납 완료.</color> '{sourcePrefabs.name}' 풀에 추가됨.");
        }
        else
        {
            Debug.LogError($"[PoolManager] {obj.name} 반납 실패! '{sourcePrefabs.name}'에 해당하는 풀이 없습니다. 오브젝트를 파괴합니다.");
            // 풀이 없는 경우 그냥 파괴
            Destroy(obj);
        }
    }

}
