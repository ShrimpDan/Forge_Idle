using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerLoader
{
    private readonly CustomerDataLoader dataLoader;
    private readonly Dictionary<CustomerJob, Customer> prefabsByJob;
    private readonly Transform spawnPoint;


    public CustomerLoader(CustomerDataLoader _dataLoader, Dictionary<CustomerJob, Customer> _prefabsByJob, Transform _spawnPoint)
    {
        dataLoader = _dataLoader;
        prefabsByJob = _prefabsByJob;
        spawnPoint = _spawnPoint;
    
    }


    public Customer SpawnCustomer(string customerkey)
    {
        var data = dataLoader.GetByKey(customerkey);
        if (data == null)
        {
            return null;
        }

        if (!prefabsByJob.TryGetValue(data.job, out var prefab))
        {
            return null;
        }

        var customer = Object.Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        customer.Init(data);
        return customer; //반환

    }

    public Customer SpawnByJob(CustomerJob _job ,CustomerType _type = CustomerType.Normal) //특정 직업으로 손님 생성 (랜덤 없이 사용할때)
    {
        foreach (var data in dataLoader.ItemsList)
        {
            if (data.job == _job && data.type == _type)
            {
                return SpawnCustomer(data.Key);
            }
        }

        return null;
    }


    public Customer SpawnNormal(CustomerJob job) => SpawnByJob(job, CustomerType.Normal);
    public Customer SpawnRegular(CustomerJob job) => SpawnByJob(job, CustomerType.Regualr);
    public Customer SpawnNuisance(CustomerJob job) => SpawnByJob(job, CustomerType.Nuisance);


}
