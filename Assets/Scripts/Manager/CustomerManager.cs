using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

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
        public CustomerRarity rarity; //단골만 적용
    }

    [Header("SpawnSetting")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay;
    [SerializeField] private List<SpriteLibraryAsset> normalSpriteAssets;



    [Header("Nuisance")]
    [SerializeField] private float nuisanceSpawnTime = 3f;
    [SerializeField, Range(0f, 1f)] private float nuisanceSpawnChance = 0.5f;

    [Header("Regular")]
    [SerializeField] private int RegularSpawnCount = 1;

    public List<BuyPoint> allBuyPoints;


    private Dictionary<CustomerJob, int> normalcustomerCounter = new Dictionary<CustomerJob, int>(); //현재 수
    private Dictionary<CustomerJob, int> normalVisitedCounter = new Dictionary<CustomerJob, int>();

    //단골손님
    private Dictionary<(CustomerJob,CustomerRarity), Customer> regularPrefabDic = new();
    //Forge
    private Forge forge;


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
    private RegularCustomerLoader regularLoader;
    private CustomerPrefabLoader prefabLoader;
    public CustomerEventHandler CustomerEvent { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        CustomerEvent = new CustomerEventHandler();
    }

    private void Start()
    {
        forge = GameManager.Instance.Forge;
        //프리팹 자동로더
        prefabLoader = new CustomerPrefabLoader();
        prefabLoader.LoadAll();

        var normalDic = new Dictionary<(CustomerJob, CustomerType), Customer>();
        foreach (CustomerJob job in Enum.GetValues(typeof(CustomerJob)))
        {
            var normalPreb = prefabLoader.GetNormal(job);
            if (normalPreb)
            {
                normalDic[(job, CustomerType.Normal)] = normalPreb;
            }
            var nuisancePreb = prefabLoader.GetNuisance();

            if (nuisancePreb)
            {
                normalDic[(job, CustomerType.Nuisance)] = nuisancePreb;
            }
        }
        var regDic = new Dictionary<(CustomerJob, CustomerRarity), Customer>();
        foreach (CustomerJob job in Enum.GetValues(typeof(CustomerJob)))
        {
            foreach (CustomerRarity rarity in Enum.GetValues(typeof(CustomerRarity)))
            {
                var regPreb = prefabLoader.GetRegular(job,rarity);
                if (regPreb)
                {
                    regDic[(job, rarity)] = regPreb;
                }

                
            }
        
        }

        foreach (CustomerJob job in Enum.GetValues(typeof(CustomerJob)))
        {
            if (!normalcustomerCounter.ContainsKey(job))
            {
                normalcustomerCounter[job] = 0;
            }

            if (!normalVisitedCounter.ContainsKey(job))
            { 
                normalVisitedCounter[job] = 0;
            }
        }

        customerLoader = new CustomerLoader(GameManager.Instance.DataManager.CustomerDataLoader, normalDic, spawnPoint);
        regularLoader = new RegularCustomerLoader(GameManager.Instance.DataManager.RegularDataLoader, regDic, spawnPoint, rarityProbabilities);

        

        StartCoroutine(SpawnNormalLoop());
        StartCoroutine(SpawnNuisanceLoop());

    }


    private IEnumerator SpawnNormalLoop()
    {
        while (true)
        {
            SpawnNormalCustomer();
            yield return WaitForSecondsCache.Wait(GameManager.Instance.Forge.FinalCustomerSpawnRate);
        }

    }

    private IEnumerator SpawnNuisanceLoop()
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

            //랜덤 스프라이트 추가하기
            if (normalSpriteAssets.Count > 0)
            {
                var randomAsset = normalSpriteAssets[UnityEngine.Random.Range(0, normalSpriteAssets.Count)];
                customer.ChangeSpriteLibrary(randomAsset);
                
            }
        }
    }

    private void SpawnNuisanceCustomer()
    {
        if (normalcustomerCounter.Count == 0)
        {
            return;
        }

        var jobs = new List<CustomerJob>(normalcustomerCounter.Keys);
        CustomerJob randomJob = jobs[UnityEngine.Random.Range(0, 1)];//랜덤으로 직업 부여하자jobs.Count 진상 다른 직업들 추가되면 jobs.Count로 해도됨
        customerLoader.SpawnNuisance(randomJob);

    }

    public void SpawnRegularCustomer(CustomerJob job)
    {
        regularLoader.SpawnRandomByJob(job);
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
        Debug.Log($"들어옴{normalVisitedCounter[job]}");

        if (!normalcustomerCounter.ContainsKey(job))
        {
            normalVisitedCounter[job] = 0;
        }

        normalVisitedCounter[job]++;
        Debug.Log($"{job}방문 {normalVisitedCounter[job]}/{RegularSpawnCount}");

        if (normalVisitedCounter[job] >= RegularSpawnCount)
        {
            normalVisitedCounter[job] = 0;
            regularLoader.SpawnRandomByJob(job);
            Debug.Log( "단골 손님 소환");
        }
    }
}
