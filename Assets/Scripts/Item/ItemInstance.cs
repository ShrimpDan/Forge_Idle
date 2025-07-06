using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public string ItemKey;
    public int Quantity;

    public int CurrentEnhanceLevel;
    public bool IsEquipped;
    public bool CanEnhance => CurrentEnhanceLevel < Data.UpgradeInfo.MaxEnhanceLevel;

    // gem 데이터 효과 연산
    public List<ItemInstance> GemSockets = new List<ItemInstance>() { null, null, null };

    [System.NonSerialized] public ItemData Data;

    public CraftingData CraftingData { get; private set; }




    public ItemInstance(string key, ItemData data, CraftingData craftingData = null)
    {
        ItemKey = key;
        Data = data;
        Quantity = 1;
        CurrentEnhanceLevel = 0;
        IsEquipped = false;
        CraftingData = craftingData;
        GemSockets = new List<ItemInstance>() { null, null, null };
    }

   

    public void EquipItem()
    {
        IsEquipped = true;
    }

    public void UnEquipItem()
    {
        IsEquipped = false;
    }

    public void AddItem(int count)
    {
        Quantity += count;
    }

    public void UseItem()
    {
        Debug.Log("Use Item");
        Quantity -= 1;
    }

    public void EnhanceItem()
    {
        if (CanEnhance)
            CurrentEnhanceLevel++;
    }

    public float GetTotalAttack()
    {
        if (Data == null || Data.ItemType != ItemType.Weapon || Data.WeaponStats == null)
            return 0;

        // 기본/강화 공격력 계산
        float baseAttack = Data.WeaponStats.Attack;
        float enhancedAttack = (CurrentEnhanceLevel == 0)
            ? baseAttack
            : baseAttack * (CurrentEnhanceLevel * Data.UpgradeInfo.AttackMultiplier);

        // 젬 소켓 효과 적용
        var (atkMul, _) = GetGemMultipliers();
        return enhancedAttack * atkMul;
    }

    public float GetTotalInterval()
    {
        if (Data == null || Data.ItemType != ItemType.Weapon || Data.WeaponStats == null)
            return 0;

        // 기본/강화 속도 계산
        float baseInterval = Data.WeaponStats.AttackInterval;
        float enhancedInterval = (CurrentEnhanceLevel == 0)
            ? baseInterval
            : Mathf.Max(0.1f, baseInterval - (CurrentEnhanceLevel * Data.UpgradeInfo.IntervalReductionPerLevel));

        // 젬 소켓 효과 적용
        var (_, speedMul) = GetGemMultipliers();
        return Mathf.Max(0.05f, enhancedInterval / speedMul);
    }


    // 장착된 젬 소켓 효과 계산
    private (float atkMul, float speedMul) GetGemMultipliers()
    {
        float atkMul = 1f;
        float speedMul = 1f;
        foreach (var gem in GemSockets)
        {
            if (gem == null || gem.Data?.GemStats == null)
                continue;

            string key = gem.ItemKey;

            // 공격력 계수 누적 (루비, 에메랄드, 사파이어)
            if (key == "gem_ruby" || key == "gem_emerald")
                atkMul *= gem.Data.GemStats.GemMultiplier;

            // 속도 계수 누적 (아메시스트, 사파이어)
            if (key == "gem_amethyst")
                speedMul *= gem.Data.GemStats.GemMultiplier;

            if (key == "gem_sapphire")
            {
                atkMul *= gem.Data.GemStats.GemMultiplier;
                speedMul *= gem.Data.GemStats.GemMultiplier;
            }
        }
        return (atkMul, speedMul);
    }

}
