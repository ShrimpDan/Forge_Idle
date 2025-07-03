using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSellingSystem : MonoBehaviour
{
    private Forge forge;
    private CustomerManager customerManager;

    public Dictionary<CustomerJob, CraftingData> CraftingWeapon { get; private set; }
    private Queue<CraftingData> craftingQueue;
    private Queue<Customer> customerQueue;

    private Coroutine craftingCoroutine;

    public void Init(Forge forge)
    {
        this.forge = forge;
        customerManager = CustomerManager.Instance;

        InitDictionary();
        craftingQueue = new Queue<CraftingData>();
        customerQueue = new Queue<Customer>();

        customerManager.CustomerEvent.OnCustomerArrived += CraftItem;
    }

    private void OnDisable()
    {
        customerManager.CustomerEvent.OnCustomerArrived -= CraftItem;
    }

    private void InitDictionary()
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
        CraftingData preData = CraftingWeapon[data.jobType];
        if (preData != null)
        {

        }

        CraftingWeapon[data.jobType] = data;
    }

    private void CraftItem(Customer customer)
    {
        customerQueue.Enqueue(customer);
        
        if (CraftingWeapon[customer.Job] != null)
            craftingQueue.Enqueue(CraftingWeapon[customer.Job]);

        if (craftingCoroutine == null)
        {
            Debug.Log($"[무기 판매 시스템] {customer.Job} 무기 제작 시작!!");
            craftingCoroutine = StartCoroutine(CraftingWeaponCoroutine());
        }
    }

    IEnumerator CraftingWeaponCoroutine()
    {
        while (craftingQueue.Count > 0 && customerQueue.Count > 0)
        {
            float time = 0f;

            CraftingData weapon = craftingQueue.Dequeue();
            float duration = weapon.craftTime * forge.FinalCraftSpeedMultiplier;

            while (time < duration)
            {
                time += 0.1f;
                yield return WaitForSecondsCache.Wait(0.1f);
            }

            // 손님에게 알림
            var customer = customerQueue.Dequeue();
            customer.NotifiedCraftWeapon();

            // 골드 지급
            int price = (int)(weapon.sellCost * forge.FinalSellPriceMultiplier);
            forge.AddGold(price);

            Debug.Log($"[무기 판매 시스템] {weapon.jobType} 무기 제작 완료!");
        }

        craftingCoroutine = null;
    }
}
