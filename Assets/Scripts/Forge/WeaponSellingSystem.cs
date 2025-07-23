using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSellingSystem : MonoBehaviour
{
    private Forge forge;
    private ForgeManager forgeManager;
    private InventoryManager inventory;
    private CustomerManager customerManager;
    private BlackSmith blackSmith;

    private Queue<CraftingData> craftingQueue;
    private Queue<Customer> customerQueue;

    private Coroutine craftingCoroutine;

    public void Init(Forge forge, DataManager dataManager, InventoryManager inventory)
    {
        this.forge = forge;
        this.inventory = inventory;
        forgeManager = forge.ForgeManager;
        blackSmith = forge.BlackSmith;
        customerManager = forge.CustomerManager;

        craftingQueue = new Queue<CraftingData>();
        customerQueue = new Queue<Customer>();

        customerManager.CustomerEvent.OnCustomerArrived += OrderItem;

        forgeManager.Events.RaiseCraftStarted(null);
        forgeManager.Events.RaiseCraftProgress(0, 1);
    }

    private void OnDisable()
    {
        customerManager.CustomerEvent.OnCustomerArrived -= OrderItem;
    }

    public bool CanOrder(WeaponType type)
    {
        var weaponList = inventory.GetWeaponInstancesByType(type);

        if (weaponList != null)
            return true;

        return false;
    }

    private void OrderItem(Customer customer)
    {
        customerQueue.Enqueue(customer);

        var craftingData = SelectWeaponByType(customer.WeaponType);
        craftingQueue.Enqueue(craftingData);

        if (craftingCoroutine == null)
        {
            Debug.Log($"[무기 판매 시스템] 무기 제작 시작!!");
            craftingCoroutine = StartCoroutine(CraftingWeaponCoroutine());
        }
    }

    private CraftingData SelectWeaponByType(WeaponType type)
    {
        var weaponList = inventory.GetWeaponInstancesByType(type);
        int chance = Random.Range(0, 101);

        if (chance <= forge.StatHandler.FinalExpensiveWeaponSellChance)
        {
            CraftingData data = weaponList[0].CraftingData;

            foreach (var weapon in weaponList)
            {
                if (weapon.CraftingData.sellCost > data.sellCost)
                {
                    data = weapon.CraftingData;
                }
            }

            return data;
        }

        return weaponList[Random.Range(0, weaponList.Count)].CraftingData;
    }

    IEnumerator CraftingWeaponCoroutine()
    {
        blackSmith.SetCraftingAnimation(true);

        while (craftingQueue.Count > 0 && customerQueue.Count > 0)
        {
            float time = 0f;

            CraftingData weapon = craftingQueue.Dequeue();
            float duration = weapon.craftTime * (1 - forge.StatHandler.FinalAutoCraftingTimeReduction);

            Customer customer = customerQueue.Dequeue();

            // 어떤 무기를 만드는지 아이콘 이벤트 호출
            forgeManager.Events.RaiseCraftStarted(IconLoader.GetIconByKey(weapon.ItemKey));

            while (time < duration && !customer.IsAngry)
            {
                time += 0.1f;
                forgeManager.Events.RaiseCraftProgress(time, duration);

                yield return WaitForSecondsCache.Wait(0.1f);
            }

            if (customer.IsAngry)
                continue;

            // 손님에게 알림
            customer.NotifiedCraftWeapon();

            // 골드 지급
            int price = (int)(weapon.sellCost * forge.StatHandler.FinalSellPriceBonus);
            price = CheckPerfectCrafting(price);

            forgeManager.AddGold(price);
            forgeManager.AddFame(5);
            blackSmith.PlayBuyEffect(price, customer.transform.position);

            Debug.Log($"[무기 판매 시스템] {weapon.weaponType} 무기 제작 완료!");
        }

        blackSmith.SetCraftingAnimation(false);
        craftingCoroutine = null;

        forgeManager.Events.RaiseCraftStarted(null);
        forgeManager.Events.RaiseCraftProgress(0, 1);
    }

    private int CheckPerfectCrafting(int price)
    {
        int chance = Random.Range(0, 101);

        // 대성공 시 가격 2배
        if (chance <= forge.StatHandler.FinalPerfectCr3aftingChance)
        {
            // 대성공 시 효과도 추가
            return price * 2;
        }
        
        return price;
    }
}
