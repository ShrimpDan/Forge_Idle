using System.Collections;
using System.Collections.Generic;
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

    /*
     * 
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
        return Customer;

    }
     */


}
