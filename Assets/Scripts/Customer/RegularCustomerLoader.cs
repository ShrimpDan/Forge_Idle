using System.Collections.Generic;
using UnityEngine;

public class RegularCustomerLoader
{
    private readonly RegularDataLoader dataLoader;
    private readonly Dictionary<(CustomerJob, CustomerRarity), Customer> prefabs;
    private readonly Transform spawnPoint;
    private readonly Dictionary<CustomerRarity, float> rarityPrefabs;


    public RegularCustomerLoader(RegularDataLoader _dataLoader, Dictionary<(CustomerJob, CustomerRarity), Customer> _prefabs, Transform _spawnPoint, Dictionary<CustomerRarity, float> _rarityPrefabs)
    {
        dataLoader = _dataLoader;
        prefabs = _prefabs;
        spawnPoint = _spawnPoint;
        rarityPrefabs = _rarityPrefabs;
    }


    public Customer SpawnRandomByJob(CustomerJob job)
    {
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

        foreach (var r in list)
        {
            total += rarityPrefabs[r.rarity];
        }
        float pick = Random.value * total;


        RegualrCustomerData choice = null; // 확률 선택

        foreach (var r in list)
        {
            pick -= rarityPrefabs[r.rarity];
            if (pick <= 0f)
            {
                choice = r;
                break;
            }

            if (choice == null)
            {
                choice = list[list.Count - 1];
            }
        }

        if (!prefabs.TryGetValue((job, choice.rarity), out var prefab))
        {
            Debug.LogWarning($"[RegularLoader] 프리팹 없음: {job}/{choice.rarity}");
            return null;
        }

        var baseCustomerData = GameManager.Instance.DataManager.CustomerDataLoader.GetByKey(choice.customerKey);
        var obj = Object.Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        obj.Init(baseCustomerData);
        Debug.Log($"<color=lime>[Regular]</color> {choice.customerName} ({choice.rarity}) 등장!");
        return obj;
    }

}
