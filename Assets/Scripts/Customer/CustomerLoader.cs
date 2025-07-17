using System.Collections.Generic;
using UnityEngine;

public class CustomerLoader
{
    private CustomerManager customerManager;
    private readonly CustomerDataLoader dataLoader;
    private readonly Dictionary<(CustomerJob, CustomerType), Customer> prefabsByJob;
    private readonly Transform spawnPoint;
    private readonly BuyPoint mainBuyPoint;


    public CustomerLoader(CustomerManager customerManager, CustomerDataLoader _dataLoader, Dictionary<(CustomerJob,CustomerType), Customer> _prefabsByJob, Transform _spawnPoint , BuyPoint buyPoint)
    {
        this.customerManager = customerManager;
        dataLoader = _dataLoader;
        prefabsByJob = _prefabsByJob;
        spawnPoint = _spawnPoint;
        mainBuyPoint = buyPoint;
    }


    public Customer SpawnCustomer(string customerkey)
    {
        var data = dataLoader.GetByKey(customerkey);
        if (data == null)
        {
            return null;
        }

        if (!prefabsByJob.TryGetValue((data.job,data.type), out var prefab))
        {
            return null;
        }

        //var customer = Object.Instantiate(prefab, spawnPoint.position, Quaternion.identity); ->오브젝트 풀링함
        GameObject customerobj = customerManager.PoolManager.Get(prefab.gameObject, spawnPoint.position, Quaternion.identity);
        var customer = customerobj.GetComponent<Customer>();
        customer.SetSourcePrefab(prefab.gameObject);

        customer.Init(customerManager, data,mainBuyPoint);
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
    public Customer SpawnRegular(CustomerJob job) => SpawnByJob(job, CustomerType.Regular);
    public Customer SpawnNuisance(CustomerJob job) => SpawnByJob(job, CustomerType.Nuisance);


}
