using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoSingleton<CustomerManager>
{

    //Test
    public int Reputation = 0;

    //스폰 담당을 해야할듯
    [System.Serializable]
    public class CustomerSpawnData
    {
        public Customer prefabs;
        public CustomerType type;
        public CustomerJob job;
        [HideInInspector]
        public int curCount;
        
    }

    [Header("SpawnSetting")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay = 2f;
    [SerializeField] private List<CustomerSpawnData> customerPrefabs; //소환 가능 숫자.
    //진상
    [SerializeField] private float nuisanceSpawnTime = 3f;
    [SerializeField] private float nuisnaceSpawnChance = 0.5f;


    public List<BuyPoint> allBuyPoints;

    private Dictionary<CustomerJob, CustomerSpawnData> spawnDict = new Dictionary<CustomerJob, CustomerSpawnData>();
    private Dictionary<CustomerJob, int> customerCount = new Dictionary<CustomerJob, int>(); //현재 수

    private Dictionary<CustomerJob, int> jobPurchaseCounts = new Dictionary<CustomerJob, int>();
    [SerializeField] private int callingCount = 5;//단골 호출 카운트



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
        StartCoroutine(SpawnNunsanceLoop());
    }


    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpwanNormalCustomer();
            yield return WaitForSecondsCache.Wait(spawnDelay);
        }
    
    }

    private IEnumerator SpawnNunsanceLoop()
    {
        while (true)
        {
            yield return WaitForSecondsCache.Wait(nuisanceSpawnTime);
            if (UnityEngine.Random.value < nuisnaceSpawnChance)
            {
                Debug.Log("진상 소환");
                SpawnCustomer(CustomerType.Nuisance);
                
            }
        }
    
    }


    private void SpwanNormalCustomer() //생성
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

    private void SpawnCustomer(CustomerType type , CustomerJob job)
    {
        var data = customerPrefabs.Find(d => d.type == type && d.job == job);
        Spawn(data);

    }
    private void SpawnCustomer(CustomerType type)
    {
        var data = customerPrefabs.Find(d => d.type == type);
        Spawn(data);
    }


    private void Spawn(CustomerSpawnData data)
    {
        if (data == null)
        {
            return;
        }
        customerCount.TryGetValue(data.job, out int cur);
        if (cur >= Customer.maxCount)
        {
            return;
        }

        Instantiate(data.prefabs, spawnPoint.position, Quaternion.identity);
        data.curCount++;
        customerCount[data.job] = cur + 1;

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

    public void RegualrCounting(CustomerJob job) //해당 직업 수치 달성 되면 소환해주는 메서드
    {
        if (!jobPurchaseCounts.ContainsKey(job))
        {
            jobPurchaseCounts[job] = 0;
        }

        jobPurchaseCounts[job]++;
        if (jobPurchaseCounts[job] >= callingCount)
        {
            jobPurchaseCounts[job] = 0;//초기화 
            SpawnRegularCusomter(job);
        }
    }


    private void SpawnRegularCusomter(CustomerJob job)
    {
        var regular = customerPrefabs.Find(data => data.type == CustomerType.Regualr && data.job == job);
        if (regular != null)
        {
            Instantiate(regular.prefabs, spawnPoint.position, Quaternion.identity);
        }
        
    }

}
