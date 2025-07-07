using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionBookManager : MonoSingleton<CollectionBookManager>
{
    private DataManager dataManager;

    public event Action<RegularCustomerData> OnCustomerDiscovered;

    private readonly HashSet<RegularCustomerData> discovered = new();

    private Dictionary<CustomerJob, List<RegularCustomerData>> regularDic = new Dictionary<CustomerJob, List<RegularCustomerData>>();

    public void Initialize()
    {
        dataManager = GameManager.Instance.DataManager;
        regularDic.Clear();

        RegularDataLoader regularDataLoader = dataManager.RegularDataLoader;
        CustomerDataLoader dataLoader = dataManager.CustomerDataLoader;

        foreach (var data in regularDataLoader.ItemsList)
        {
            if (data == null)
            {
                continue;
            }
            CustomerData baseData = dataLoader.GetByKey(data.customerKey); //키를 사용해서 찾은다음

            if (baseData == null)
            {
                continue; //기본 데이터가 없으면 건너뛰기
            }

            if (!regularDic.TryGetValue(baseData.job, out var list))
            {
                list = new List<RegularCustomerData>();
                regularDic.Add(baseData.job, list);
            }

            if (!list.Contains(data))
            {
                list.Add(data);
            }
        }

        Debug.Log("StopPoint");
    }


    public void Discover(RegularCustomerData data)
    {
        if (data == null)
        {
            return;
        }
        if (discovered.Contains(data))
        {
            return;
        }

        discovered.Add(data);
        OnCustomerDiscovered?.Invoke(data);

        //나중에 여기 Save()추가
    }

    public void Discover(CustomerJob job, CustomerRarity rarity)
    {
        if (!regularDic.TryGetValue(job, out var list))
        {
            return;
        }

        var target = list.FirstOrDefault(rc => rc.rarity == rarity);
        if (target != null)
        {
            Discover(target);

        }
    }

    public bool IsDiscovered(RegularCustomerData data)
    {
        return discovered.Contains(data);
    }

    public IEnumerable<RegularCustomerData> GetAllCustomerData()
    {
        foreach (var list in regularDic.Values)
        {
            foreach (var data in list)
            {
                yield return data;
            }
        }
    }

    public List<RegularCustomerData> GetByJob(CustomerJob job)
    {
        if (regularDic.TryGetValue(job, out var list))
        {
            return list;
        }

        return new List<RegularCustomerData>();
        //없을때 새로운 리스트를 만들어야지
    }

    public CollectionBookSaveData ToSaveData()
    {
        var data = new CollectionBookSaveData();
        data.discoveredKeys = new List<string>();

        foreach (var discover in discovered)
        {
            data.discoveredKeys.Add(discover.Key);
        }
        
        return data;
    }

    public void LoadFromSaveData(CollectionBookSaveData saveData)
    {
        foreach (var key in saveData.discoveredKeys)
        {
            discovered.Add(dataManager.RegularDataLoader.GetByKey(key));
        }
    }
}
