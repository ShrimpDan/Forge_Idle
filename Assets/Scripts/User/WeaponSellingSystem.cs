using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSellingSystem : MonoBehaviour
{
    private Forge forge;
    private CraftingDataLoader craftingLoader;
    private ItemDataLoader itemLoader;
    private CustomerManager customerManager;
    private BlackSmith blackSmith;

    public Dictionary<CustomerJob, CraftingData> CraftingWeapon { get; private set; }
    private Queue<CraftingData> craftingQueue;
    private Queue<Customer> customerQueue;

    private Coroutine craftingCoroutine;

    public void Init(Forge forge, DataManager dataManager)
    {
        this.forge = forge;
        craftingLoader = dataManager.CraftingLoader;
        itemLoader = dataManager.ItemLoader;
        customerManager = CustomerManager.Instance;
        blackSmith = forge.BlackSmith;

        InitDictionary();
        craftingQueue = new Queue<CraftingData>();
        customerQueue = new Queue<Customer>();

        customerManager.CustomerEvent.OnCustomerArrived += CraftItem;
    }

    private void OnDisable()
    {
        customerManager.CustomerEvent.OnCustomerArrived -= CraftItem;
    }

    public void InitDictionary()
    {
        CraftingWeapon = new Dictionary<CustomerJob, CraftingData>();

        CraftingWeapon[CustomerJob.Woodcutter] = null;
        CraftingWeapon[CustomerJob.Farmer] = null;
        CraftingWeapon[CustomerJob.Miner] = null;
        CraftingWeapon[CustomerJob.Warrior] = null;
        CraftingWeapon[CustomerJob.Archer] = null;
        CraftingWeapon[CustomerJob.Tanker] = null;
        CraftingWeapon[CustomerJob.Assassin] = null;
    }

    public void SetCraftingItem(CraftingData data)
    {
        CraftingWeapon[data.jobType] = data;
    }

    private void CraftItem(Customer customer)
    {
        customerQueue.Enqueue(customer);

        if (CraftingWeapon[customer.Job] != null)
            craftingQueue.Enqueue(CraftingWeapon[customer.Job]);

        if (craftingCoroutine == null)
        {
            Debug.Log($"[무기 판매 시스템] 무기 제작 시작!!");
            craftingCoroutine = StartCoroutine(CraftingWeaponCoroutine());
        }
    }

    IEnumerator CraftingWeaponCoroutine()
    {
        blackSmith.SetCraftingAnimation(true);

        while (craftingQueue.Count > 0 && customerQueue.Count > 0)
        {
            float time = 0f;

            CraftingData weapon = craftingQueue.Dequeue();
            float duration = weapon.craftTime * forge.FinalCraftSpeedMultiplier;

            // 어떤 무기를 만드는지 아이콘 이벤트 호출
            string iconPath = itemLoader.GetItemByKey(weapon.ItemKey).IconPath;
            forge.Events.RaiseCraftStarted(IconLoader.GetIcon(iconPath));

            while (time < duration)
            {
                time += 0.1f;
                forge.Events.RaiseCraftProgress(time, duration);

                yield return WaitForSecondsCache.Wait(0.1f);
            }

            // 손님에게 알림
            var customer = customerQueue.Dequeue();
            customer.NotifiedCraftWeapon();

            // 골드 지급
            int price = (int)(weapon.sellCost * forge.FinalSellPriceMultiplier);
            forge.AddGold(price);
            forge.AddFame(5);
            blackSmith.PlayBuyEffect(price, customer.transform.position);

            Debug.Log($"[무기 판매 시스템] {weapon.jobType} 무기 제작 완료!");
        }

        blackSmith.SetCraftingAnimation(false);
        craftingCoroutine = null;
        
        forge.Events.RaiseCraftStarted(null);
        forge.Events.RaiseCraftProgress(0, 1);
    }

    public WeaponSellingSaveData SaveToData()
    {
        var data = new WeaponSellingSaveData();

        data.CraftingKeys = new List<string>();

        foreach (var craftingKey in CraftingWeapon.Values)
        {
            if (craftingKey != null)
            {
                data.CraftingKeys.Add(craftingKey.ItemKey);
                continue;
            }

            data.CraftingKeys.Add(null);
        }

        return data;
    }

    public void LoadFromSaveData(WeaponSellingSaveData data)
    {
        CraftingWeapon = new Dictionary<CustomerJob, CraftingData>();

        for (int i = 0; i < data.CraftingKeys.Count; i++)
        {
            if (data.CraftingKeys[i] != null)
            {
                var craftData = craftingLoader.GetDataByKey(data.CraftingKeys[i]);
                CraftingWeapon[(CustomerJob)i] = craftData;
                continue;
            }

            CraftingWeapon[(CustomerJob)i] = null;
        }
    }
}
