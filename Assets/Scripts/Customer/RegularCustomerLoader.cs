using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class RegularCustomerLoader
{
    private readonly RegularDataLoader dataLoader;
    private readonly Dictionary<CustomerJob, Customer> prefabs;
    private readonly Transform spawnPoint;
    private readonly Dictionary<CustomerRarity, float> rarityPrefabs;


    public RegularCustomerLoader(RegularDataLoader _dataLoader, Dictionary<CustomerJob, Customer> _prefabs, Transform _spawnPoint, Dictionary<CustomerRarity, float> _rarityPrefabs)
    {
        dataLoader = _dataLoader;
        prefabs = _prefabs;
        spawnPoint = _spawnPoint;
        rarityPrefabs = _rarityPrefabs;
    }

    
    public Customer SpawnRandomByJob(CustomerJob job)
    {
        if (!prefabs.TryGetValue(job, out var prefab))
        {
            return null; //없음
        }

        var list = new List<RegualrCustomerData>();
        foreach (var data in dataLoader.ItemsList)
        {
            var baseData = GameManager.Instance.DataManager.CustomerDataLoader.GetByKey(data.customerKey);
            if (baseData != null && baseData.job == job)
            {
                list.Add(data);
            }
        }

        if (list.Count == 0)
        {
            return null;
        }


        float total = 0f;

        float pick = Random.value * total;  
        foreach (var r in list)
        {
            total += rarityPrefabs[r.rarity];
        }
        foreach (var r in list)
        {
            pick -= rarityPrefabs[r.rarity];
            if (pick <= 0f)
            {
                var baseData = GameManager.Instance.DataManager.CustomerDataLoader.GetByKey(r.customerKey);
                var obj = Object.Instantiate(prefab, spawnPoint.position, Quaternion.identity);
                obj.Init(baseData);
                return obj;

            }
        }

        return null;


    }
    


}
