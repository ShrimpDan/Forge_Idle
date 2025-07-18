using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CustomerManager : MonoBehaviour
{
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
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private BuyPoint mainBuyPoint;
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
    private Dictionary<(CustomerJob, CustomerRarity), Customer> regularPrefabDic = new();
    //Forge
    private Forge forge;

    public PoolManager PoolManager { get => poolManager; }

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
    public CustomerEventHandler CustomerEvent { get; private set; } = new CustomerEventHandler();

    private void Start()
    {
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
                var regPreb = prefabLoader.GetRegular(job, rarity);
                if (regPreb)
                {
                    regDic[(job, rarity)] = regPreb;
                }


            }

        }
        var uniquePrefabs = new HashSet<GameObject>();
        foreach (var prefab in normalDic.Values)
        {
            if (prefab != null)
            {
                uniquePrefabs.Add(prefab.gameObject);
            }

        }
        foreach (var prefab in regDic.Values)
        {
            if (prefab != null)
            {
                uniquePrefabs.Add(prefab.gameObject);
            }
        }
        foreach (var prefab in uniquePrefabs)
        {
            // 각 프리팹 당 최대 손님 수(maxCount)만큼 미리 생성해 둡니다.
            poolManager.CreatePool(prefab, Customer.maxCount);
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

        customerLoader = new CustomerLoader(this, GameManager.Instance.DataManager.CustomerDataLoader, normalDic, spawnPoint, mainBuyPoint);
        regularLoader = new RegularCustomerLoader(this, GameManager.Instance.DataManager.RegularDataLoader, regDic, spawnPoint, rarityProbabilities, mainBuyPoint);
    }

    public void StartSpawnCustomer(Forge forge)
    {
        this.forge = forge;

        StartCoroutine(SpawnNormalLoop());
        StartCoroutine(SpawnNuisanceLoop());
    }

    public void StopSpawnCustomer()
    {
        StopCoroutine(SpawnNormalLoop());
        StopCoroutine(SpawnNuisanceLoop());
    }

    private IEnumerator SpawnNormalLoop()
    {


        while (true)
        {
            yield return WaitForSecondsCache.Wait(forge.StatHandler.FinalCustomerSpawnInterval);
            SpawnNormalCustomer();
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
            if (pair.Value < Customer.maxCount)
            {
                availableJobs.Add(pair.Key);
            }
        }

        if (availableJobs.Count == 0)
        {
            return;
        }

        WeaponType weaponType = forge.GetRandomWeaponType();

        if (forge.SellingSystem.CanOrder(weaponType))
        {
            //랜덤소환
            CustomerJob selected = availableJobs[UnityEngine.Random.Range(0, availableJobs.Count)];
            Customer customer = customerLoader.SpawnNormal(selected);//만들고

            if (customer != null)
            {
                customer.SetWeaponType(weaponType);
                normalcustomerCounter[selected]++; //카운트 증가

                //랜덤 스프라이트 추가하기
                if (normalSpriteAssets.Count > 0)
                {
                    var randomAsset = normalSpriteAssets[UnityEngine.Random.Range(0, normalSpriteAssets.Count)];
                    customer.ChangeSpriteLibrary(randomAsset);

                }
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
        WeaponType weaponType = forge.GetRandomWeaponType();
        regularLoader.SpawnRandomByJob(job, weaponType);
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

            WeaponType weaponType = forge.GetRandomWeaponType();
            regularLoader.SpawnRandomByJob(job, weaponType);

            Debug.Log("단골 손님 소환");
        }
    }
}
