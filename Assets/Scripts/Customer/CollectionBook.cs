using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionBookManager : MonoSingleton<CollectionBookManager>
{
    private DataManager dataManager;
    [SerializeField] private CollectionBookData bookData;

    public event Action<RegualrCustomerData> OnCustomerDiscovered;

    private readonly HashSet<RegualrCustomerData> discovered = new();

    private Dictionary<CustomerJob, List<RegualrCustomerData>> regularDic = new Dictionary<CustomerJob, List<RegualrCustomerData>>();

    private void InitDic()
    {
        dataManager = GameManager.Instance.DataManager;
        
        discovered.Clear();
        foreach (var rc in bookData.regularCustomers)
        {
            if (rc == null) continue;

            CustomerData data = dataManager.CustomerDataLoader.GetByKey(rc.customerKey);
            if (!regularDic.TryGetValue(data.job, out var list))
            {
                list = new List<RegualrCustomerData>();
                regularDic[data.job] = list;
            }

            list.Add(rc);
            //rc.isDiscovered = false; // 항상 초기화
        }


        foreach (var list in regularDic.Values)
        {
            list.Sort((a, b) => a.rarity.CompareTo(b.rarity)); //등급 비교스
        }


    }

    public void Discover(RegualrCustomerData data)
    {
        if (data == null || discovered.Contains(data))
        {
            //data.isDiscovered = true;
            discovered.Add(data);

            OnCustomerDiscovered?.Invoke(data);
        }
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

    public List<RegualrCustomerData> GetByJob(CustomerJob job)
    {
        if (regularDic.TryGetValue(job, out var list))
        {
            return list;
        }

        return new List<RegualrCustomerData>();
        //없을때 새로운 리스트를 만들어야지
    }

    public List<RegualrCustomerData> GetAllRegularCutsomer()
    {
        List<RegualrCustomerData> result = new();

        foreach (var data in regularDic)
        {
            result.AddRange(data.Value);
        }

        return result;
    }

    public bool IsDiscovered(RegualrCustomerData data)
    {
        return data != null; //&& data.isDiscovered;

    }

    public bool IsDiscovered(CustomerJob job, CustomerRarity rarity)
    {
        if (!regularDic.TryGetValue(job, out var list))
        {
            return false;
        }

        foreach (var data in list)
        {
            if (data.rarity == rarity) //&& data.isDiscovered)
            {
                return true;
            }
        }

        return false;
    }
    
}
