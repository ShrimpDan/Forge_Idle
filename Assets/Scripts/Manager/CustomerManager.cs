using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoSingleton<CustomerManager>
{

    //스폰 담당을 해야할듯
    [System.Serializable]
    public class CustomerSpawnData
    {
        public Customer prefabs;
        public CustomerType type; 
        
    }

    [Header("SpawnSetting")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay = 2f;
    [SerializeField] private List<CustomerSpawnData> customerPrefabs; //소환 가능 숫자.


    private Dictionary<CustomerJob, CustomerSpawnData> spawnDict = new Dictionary<CustomerJob, CustomerSpawnData>();
    private Dictionary<CustomerJob, int> customerCount = new Dictionary<CustomerJob, int>(); //현재 수

    private void Start()
    {
        foreach (var data in customerPrefabs)
        {
            CustomerJob job = data.prefabs.Job;
            if (!spawnDict.ContainsKey(job))
            {
                spawnDict[job] = data;
                customerCount[job] = 0;
            }        
        }


        StartCoroutine(SpawnLoop());
    }


    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpwanCustomer();
            yield return new WaitForSeconds(spawnDelay);
        }
    
    }

    private void SpwanCustomer()
    {
        //5명 이하인 애들을 골라야함
        List<CustomerJob> availableJobs = new List<CustomerJob>();

        foreach (var pair in customerCount)
        {
            if (pair.Value < Customer.maxCount)
            {
                availableJobs.Add(pair.Key);
            }
        }

        if (availableJobs.Count == 0)
        {
            return;
        }


        //랜덤소환
        CustomerJob selected = availableJobs[UnityEngine.Random.Range(0, availableJobs.Count)];
        CustomerSpawnData selectedData = spawnDict[selected];


        Customer newCusomter = Instantiate(selectedData.prefabs, spawnPoint.position, Quaternion.identity);
        customerCount[selected]++; //인원 증가
      


    }


    //퇴장
    public void CustomerExit(Customer customer)
    {
        //나가니까 제거 나중에 풀링도 해야것다
        CustomerJob job = customer.Job;
        if (customerCount.ContainsKey(job))
        {
            customerCount[job] = Mathf.Max(0, customerCount[job] - 1);
        }
    }

}
