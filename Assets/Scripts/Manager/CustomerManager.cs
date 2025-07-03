using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoSingleton<CustomerManager>
{

    //Test
    public int Reputation = 0;

    //스폰 담당을 해야할듯
    [System.Serializable]
    public struct CustomerSpawnData
    {
        public Customer prefabs;
        public CustomerType type;
        public CustomerJob job;
    }




    [Header("SpawnSetting")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay = 2f;
    [SerializeField] private List<CustomerSpawnData> customerPrefabs = new();


    [Header("Nuisance")]
    [SerializeField] private float nuisanceSpawnTime = 1f;
    [SerializeField, Range(0f, 1f)] private float nuisanceSpawnChance = 0.5f;

    [Header("Regular")]
    [SerializeField] private int RegularSpawnCount = 10;

    public List<BuyPoint> allBuyPoints;

    private Dictionary<CustomerJob, int> normalcustomerCounter = new Dictionary<CustomerJob, int>(); //현재 수
    private Dictionary<CustomerJob, int> normalVisitedCounter = new Dictionary<CustomerJob, int>();

    private readonly Dictionary<CustomerRarity, float> rarityProbabilities = new()
    {
        {CustomerRarity.Common, 0.5f },
        {CustomerRarity.Rare , 0.3f },
        {CustomerRarity.Epic , 0.1f },
        {CustomerRarity.Unique,0.07f },
        {CustomerRarity.Legendary,0.03f }
    };



    //Loader
    private CustomerLoader customerLoader;
    private RegularDataLoader regularLoader;

    // 현재 방문 중인 손님
    public List<Customer> visitCustomers = new List<Customer>();

    public CustomerEventHandler CustomerEvent { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        CustomerEvent = new CustomerEventHandler();
    }

    private void Start()
    {
        var prefabDic = new Dictionary<(CustomerJob, CustomerType), Customer>();

        foreach (var data in customerPrefabs)
        {

            var key = (data.job, data.type);

            if (!prefabDic.ContainsKey(key))
                prefabDic[key] = data.prefabs;

            if (data.type == CustomerType.Normal)
            {
                if (!normalcustomerCounter.ContainsKey(data.job))
                {
                    normalcustomerCounter[data.job] = 0;
                }
                if (!normalVisitedCounter.ContainsKey(data.job))
                {
                    normalVisitedCounter[data.job] = 0;
                }
            }
        }
        customerLoader = new CustomerLoader(GameManager.Instance.DataManager.CustomerDataLoader, prefabDic, spawnPoint);


        StartCoroutine(SpawnNormalLoop());
        StartCoroutine(SpawnNunsanceLoop());

    }


    private IEnumerator SpawnNormalLoop()
    {
        while (true)
        {
            SpawnNormalCustomer();
            yield return WaitForSecondsCache.Wait(spawnDelay);
        }

    }

    private IEnumerator SpawnNunsanceLoop()
    {
        while (true)
        {
            yield return WaitForSecondsCache.Wait(nuisanceSpawnTime);
            if (UnityEngine.Random.value < nuisanceSpawnChance)
            {
                Debug.Log("진상 소환");
                SpawnNuisanceCustomer();
            }
        }

    }


    private void SpawnNormalCustomer() //생성
    {
        //5명 이하인 애들을 골라야함
        List<CustomerJob> availableJobs = new List<CustomerJob>();

        foreach (var pair in normalcustomerCounter)
        {
            if (pair.Value < Customer.maxCount && GameManager.Instance.Forge.SellingSystem.CraftingWeapon[pair.Key] != null)
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
        Customer customer = customerLoader.SpawnNormal(selected);//만들고
        if (customer != null)
        {
            normalcustomerCounter[selected]++; //카운트 증가
        }
    }

    private void SpawnNuisanceCustomer()
    {
        if (normalcustomerCounter.Count == 0)
        {
            return;
        }

        var jobs = new List<CustomerJob>(normalcustomerCounter.Keys);
        CustomerJob randomJob = jobs[Random.Range(0, 1)];//랜덤으로 직업 부여하자jobs.Count 진상 다른 직업들 추가되면 jobs.Count로 해도됨
        customerLoader.SpawnNuisance(randomJob);

    }

    public void SpawnRegularCustomer(CustomerJob job)
    {
        customerLoader.SpawnRegular(job);
    }

    //퇴장
    public void CustomerExit(Customer customer)
    {
        if (customer.Type == CustomerType.Normal && normalcustomerCounter.ContainsKey(customer.Job))
        {
            normalcustomerCounter[customer.Job] = Mathf.Max(0, normalcustomerCounter[customer.Job] - 1);
        }
    }


    public void NotifyNormalCustomerPurchased(CustomerJob job) //일반손님 구매 알림
    {
        if (!normalcustomerCounter.ContainsKey(job))
        {
            normalVisitedCounter[job] = 0;
        }

        if (++normalVisitedCounter[job] >= RegularSpawnCount)
        {
            normalVisitedCounter[job] = 0;
            SpawnRegularCustomer(job);
        }
    }
}
