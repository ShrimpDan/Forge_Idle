using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionBookManager : MonoSingleton<CollectionBookManager>
{
    private DataManager dataManager;

    public event Action<RegualrCustomerData> OnCustomerDiscovered;

    private readonly HashSet<RegualrCustomerData> discovered = new();

    private Dictionary<CustomerJob, List<RegualrCustomerData>> regularDic = new Dictionary<CustomerJob, List<RegualrCustomerData>>();

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
                list = new List<RegualrCustomerData>();
                regularDic.Add(baseData.job, list);
            }
            if (!list.Contains(data))
            {
                list.Add(data);
            }

        }
    }


    public void Discover(RegualrCustomerData data)
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

    public bool IsDiscovered(RegualrCustomerData data)
    {
        return discovered.Contains(data);
    }

    public IEnumerable<RegualrCustomerData> GetAllCustomerData()
    {
        foreach (var list in regularDic.Values)
        {
            foreach (var data in list)
            {
                yield return data;
            }
        }
    }

    public List<RegualrCustomerData> GetByJob(CustomerJob job)
    {
        if (regularDic.TryGetValue(job, out var list))
        {
            return list;
        }

        return new List<RegualrCustomerData>();
        //없을때 새로운 리스트를 만들어야지
    }
}
