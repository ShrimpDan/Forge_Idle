using System.Collections.Generic;
using UnityEngine;

public class ForgeStatHandler
{
    private Forge forge;
    private ForgeManager forgeManager;
    private ForgeUpgradeDataLoader upgradeDataLoader;

    // 업그레이드 레벨
    private Dictionary<ForgeUpgradeType, int> upgradeLevels;

    // 판매 가격 증가
    private float upgradeSellPriceBonus;
    private float assistantSellPriceBonus;
    private float skillSellPriceBonus;

    // 대성공 확률 증가 (자동 제작)
    private float upgradePerfectCraftingChance;
    private float assistantPerfectCraftingChance;
    private float skillPerfectCraftingChance;

    // 제작 시간 감소 (자동 제작)
    private float upgradeAutoCraftingTimeReduction;
    private float assistantAutoCraftingTimeReduction;
    private float skillAutoCraftingTimeReduction;

    // 손님 소환 딜레이 감소
    private float upgradeCustomerSpawnInterval;
    private float assistantCustomerSpawnIntervalReduction;
    private float skillCustomerSpawnIntervalReduction;

    // 상위 무기 판매 확률 증가
    private float upgradeExpensiveWeaponSellChance;
    private float skillExpensiveWeaponSellChance;

    private float assistantBadCustomerAutoKickChance; // 진상 손님 자동 내쫓기 확률
    private float assistantReduceWeaponCraftingTime; // 무기 제작 시간 감소 (던전용 무기) 
    private float assistantRareCraftChance; // 상위 등급 무기 제작확률 증가 (던전용 무기)

    private float assistantResourcePerMinuteBonus; // 분당 자원 획득량 증가 (광산)
    private float assistantMaxResourceCapacityBonus; // 최대 자원 수집량 증가 (광산)

    private const float MaxAutoCraftingTimeReduction = 0.99f;

    // 최종 스탯 프로퍼티
    public float FinalSellPriceBonus => 1 + upgradeSellPriceBonus + assistantSellPriceBonus + skillSellPriceBonus;
    public float FinalExpensiveWeaponSellChance => upgradeExpensiveWeaponSellChance + skillExpensiveWeaponSellChance;
    public float FinalCustomerSpawnInterval => upgradeCustomerSpawnInterval + assistantCustomerSpawnIntervalReduction + skillCustomerSpawnIntervalReduction;
    public float FinalAutoCraftingTimeReduction => Mathf.Min(upgradeAutoCraftingTimeReduction + assistantAutoCraftingTimeReduction + skillAutoCraftingTimeReduction, MaxAutoCraftingTimeReduction);
    public float FinalPerfectCr3aftingChance => upgradePerfectCraftingChance + assistantPerfectCraftingChance + skillPerfectCraftingChance;
    public float FinalBadCustomerAutoKickChance => assistantBadCustomerAutoKickChance;
    public float FinalReduceWeaponCraftingTime => assistantReduceWeaponCraftingTime;
    public float FinalRareCraftChance => assistantRareCraftChance;
    public float FinalResourcePerMinuteBonus => assistantResourcePerMinuteBonus;
    public float FinalMaxResourceCapacityBonus => assistantMaxResourceCapacityBonus;

    public ForgeStatHandler(Forge forge, DataManager dataManager)
    {
        this.forge = forge;
        forgeManager = forge.ForgeManager;

        upgradeDataLoader = dataManager.UpgradeDataLoader;
    }

    private void ApplyUpgradeStats()
    {
        upgradeSellPriceBonus = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.IncreaseSellPrice, upgradeLevels[ForgeUpgradeType.IncreaseSellPrice]);
        upgradeExpensiveWeaponSellChance = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.IncreaseExpensiveRecipeChance, upgradeLevels[ForgeUpgradeType.IncreaseExpensiveRecipeChance]);
        upgradePerfectCraftingChance = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.IncreasePerfectCraftChance, upgradeLevels[ForgeUpgradeType.IncreasePerfectCraftChance]);
        upgradeAutoCraftingTimeReduction = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.ReduceAutoCraftingTime, upgradeLevels[ForgeUpgradeType.ReduceAutoCraftingTime]);
        upgradeCustomerSpawnInterval = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.ReduceCustomerSpawnDelay, upgradeLevels[ForgeUpgradeType.ReduceCustomerSpawnDelay]);
    }

    public int GetUpgradeCost(ForgeUpgradeType type)
    {
        if (!upgradeLevels.ContainsKey(type)) return -1;

        return upgradeDataLoader.GetCost(forge.ForgeType, type, upgradeLevels[type]);
    }

    public bool CanUpgrade(ForgeUpgradeType type)
    {
        if (!upgradeLevels.ContainsKey(type)) return false;

        int curLevel = upgradeLevels[type];
        int maxLevel = upgradeDataLoader.GetMaxLevel(forge.ForgeType, type);

        return curLevel < maxLevel;
    }

    public bool TryUpgradeStat(ForgeUpgradeType type)
    {
        if (!CanUpgrade(type)) return false;

        int cost = GetUpgradeCost(type);
        if (!forgeManager.UseGold(cost)) return false;

        upgradeLevels[type]++;
        ApplyUpgradeStats(); // 업그레이드 후 스탯 재계산
        return true;
    }

    public void ApplyAssistantStat(AssistantInstance assi)
    {
        foreach (var stat in assi.Multipliers)
        {
            switch (stat.AbilityName)
            {

            }
        }
    }

    public void DeApplyAssistantStat(AssistantInstance assi)
    {
        foreach (var stat in assi.Multipliers)
        {
            switch (stat.AbilityName)
            {

            }
        }
    }

    public void ResetAssistantStats()
    {
        assistantSellPriceBonus = 0;
        assistantPerfectCraftingChance = 0;
        assistantAutoCraftingTimeReduction = 0;
        assistantBadCustomerAutoKickChance = 0;
        assistantReduceWeaponCraftingTime = 0;
        assistantRareCraftChance = 0;
        assistantCustomerSpawnIntervalReduction = 0;
        assistantResourcePerMinuteBonus = 0;
        assistantMaxResourceCapacityBonus = 0;
    }

    public List<ForgeUpgradeSaveData> GetSaveData()
    {
        var dataList = new List<ForgeUpgradeSaveData>();

        foreach (var key in upgradeLevels.Keys)
        {
            var saveData = new ForgeUpgradeSaveData()
            {
                UpgradeType = key,
                Level = upgradeLevels[key]
            };

            dataList.Add(saveData);
        }

        return dataList;
    }

    public void LoadFromData(List<ForgeUpgradeSaveData> datas)
    {
        if (datas == null)
        {
            upgradeLevels = new Dictionary<ForgeUpgradeType, int>()
            {
                { ForgeUpgradeType.IncreaseExpensiveRecipeChance, 1 },
                { ForgeUpgradeType.IncreasePerfectCraftChance, 1 },
                { ForgeUpgradeType.IncreaseSellPrice, 1 },
                { ForgeUpgradeType.ReduceAutoCraftingTime, 1 },
                { ForgeUpgradeType.ReduceCustomerSpawnDelay, 1 },
            };
        }

        else
        {
            upgradeLevels = new Dictionary<ForgeUpgradeType, int>();
            foreach (var data in datas)
            {
                upgradeLevels[data.UpgradeType] = data.Level;
            }
        }

        ApplyUpgradeStats();
    }
}
