using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSellingSystem : MonoBehaviour
{
    private Forge forge;
    public Dictionary<CustomerJob, CraftingData> CraftingWeapon { get; private set; }
    private Queue<CraftingData> craftingQueue;

    private Coroutine craftingCoroutine;

    public void Init(Forge forge)
    {
        this.forge = forge;

        InitDictionary();
        craftingQueue = new Queue<CraftingData>();
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

    private void CraftItem(CustomerJob jobType)
    {
        if (CraftingWeapon[jobType] != null)
            craftingQueue.Enqueue(CraftingWeapon[jobType]);

        if (craftingCoroutine == null)
        {
            craftingCoroutine = StartCoroutine(CraftingWeaponCoroutine());
        }
    }

    IEnumerator CraftingWeaponCoroutine()
    {
        while (craftingQueue.Count > 0)
        {
            float time = 0f;

            CraftingData weapon = craftingQueue.Dequeue();
            float duration = weapon.craftTime * (1 - forge.FinalCraftSpeedMultiplier);

            while (time < duration)
            {
                time += 0.1f;
                yield return WaitForSecondsCache.Wait(0.1f);
            }

            int price = (int)(weapon.sellCost * forge.FinalSellPriceMultiplier);
            forge.AddGold(price);
        }
    }
}
