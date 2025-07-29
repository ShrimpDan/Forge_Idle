using System;
using System.Collections.Generic;
using UnityEngine;

public class ForgeStatHandler
{
    private Forge forge;
    private ForgeManager forgeManager;
    private ForgeUpgradeDataLoader upgradeDataLoader;

    // 업그레이드 레벨
    public Dictionary<ForgeUpgradeType, int> UpgradeLevels { get; private set; }

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
    public float FinalPerfectCr3aftingChance => (upgradePerfectCraftingChance + assistantPerfectCraftingChance + skillPerfectCraftingChance) * 100;
    public float FinalBadCustomerAutoKickChance => assistantBadCustomerAutoKickChance;
    public float FinalReduceWeaponCraftingTime => assistantReduceWeaponCraftingTime;
    public float FinalRareCraftChance => assistantRareCraftChance;
    public float FinalResourcePerMinuteBonus => assistantResourcePerMinuteBonus;
    public float FinalMaxResourceCapacityBonus => assistantMaxResourceCapacityBonus;
    public int ForgeVisualLevel { get; private set; }

    public ForgeStatHandler(Forge forge, DataManager dataManager)
    {
        this.forge = forge;
        forgeManager = forge.ForgeManager;

        upgradeDataLoader = dataManager.UpgradeDataLoader;
    }

    private void ApplyUpgradeStats()
    {
        upgradeSellPriceBonus = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.IncreaseSellPrice, UpgradeLevels[ForgeUpgradeType.IncreaseSellPrice]);
        upgradeExpensiveWeaponSellChance = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.IncreaseExpensiveRecipeChance, UpgradeLevels[ForgeUpgradeType.IncreaseExpensiveRecipeChance]);
        upgradePerfectCraftingChance = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.IncreasePerfectCraftChance, UpgradeLevels[ForgeUpgradeType.IncreasePerfectCraftChance]);
        upgradeAutoCraftingTimeReduction = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.ReduceAutoCraftingTime, UpgradeLevels[ForgeUpgradeType.ReduceAutoCraftingTime]);
        upgradeCustomerSpawnInterval = upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.ReduceCustomerSpawnDelay, UpgradeLevels[ForgeUpgradeType.ReduceCustomerSpawnDelay]);
        ForgeVisualLevel = Mathf.RoundToInt(upgradeDataLoader.GetValue(forge.ForgeType, ForgeUpgradeType.UpgradeInterior, UpgradeLevels[ForgeUpgradeType.UpgradeInterior]));
    }

    public int GetUpgradeCost(ForgeUpgradeType type)
    {
        if (!UpgradeLevels.ContainsKey(type)) return -1;

        return upgradeDataLoader.GetCost(forge.ForgeType, type, UpgradeLevels[type]);
    }

    public bool CanUpgrade(ForgeUpgradeType type)
    {
        if (!UpgradeLevels.ContainsKey(type)) return false;

        int curLevel = UpgradeLevels[type];
        int maxLevel = upgradeDataLoader.GetMaxLevel(forge.ForgeType, type);

        return curLevel < maxLevel;
    }

    public bool TryUpgradeStat(ForgeUpgradeType type)
    {
        if (!CanUpgrade(type)) return false;

        int cost = GetUpgradeCost(type);
        if (!forgeManager.UseGold(cost)) return false;

        UpgradeLevels[type]++;
        ApplyUpgradeStats(); // 업그레이드 후 스탯 재계산

        if (type == ForgeUpgradeType.UpgradeInterior)
        {
            forge.VisualHandler.SetInterior(UpgradeLevels[type]);
        }

        if (IsAllUpgradesMaxed())
        {
            forgeManager.UnlockForge(GetNextForgeType(forge.ForgeType));
        }

        return true;
    }

    public bool IsAllUpgradesMaxed()
    {
        if (UpgradeLevels == null || UpgradeLevels.Count == 0)
        {
            return false;
        }

        foreach (var kvp in UpgradeLevels)
        {
            ForgeUpgradeType upgradeType = kvp.Key;
            int currentLevel = kvp.Value;
            int maxLevel = upgradeDataLoader.GetMaxLevel(forge.ForgeType, upgradeType);

            if (currentLevel < maxLevel)
            {
                return false;
            }
        }

        return true;
    }

    private ForgeType GetNextForgeType(ForgeType currentForgeType)
    {
        switch (currentForgeType)
        {
            case ForgeType.Weapon:
                return ForgeType.Armor;
            case ForgeType.Armor:
                return ForgeType.Magic;
            case ForgeType.Magic:
                return ForgeType.None; // 마지막 대장간 이후에는 더 이상 없음
            default:
                return ForgeType.None;
        }
    }

    public void ApplyAssistantStat(AssistantInstance assi, bool isApply)
    {
        foreach (var stat in assi.Multipliers)
        {
            float raw = stat.Multiplier;
            if (!isApply) raw *= -1;

            float statValue = Mathf.Max(raw, 0f);

            switch (stat.AbilityName)
            {
                case AssistantStatNames.IncreaseCraftSpeed:
                    assistantReduceWeaponCraftingTime += statValue;
                    break;

                case AssistantStatNames.IncreaseAdvancedCraftChance:
                    assistantRareCraftChance += statValue;
                    break;

                case AssistantStatNames.IncreaseAutoCraftSpeed:
                    assistantAutoCraftingTimeReduction += statValue;
                    break;

                case AssistantStatNames.IncreaseGreatSuccessChance:
                    assistantPerfectCraftingChance += statValue;
                    break;

                case AssistantStatNames.IncreaseMiningYieldPerMinute:
                    assistantResourcePerMinuteBonus += statValue;
                    break;

                case AssistantStatNames.IncreaseMaxMiningCapacity:
                    assistantMaxResourceCapacityBonus += statValue;
                    break;

                case AssistantStatNames.IncreaseSellPrice:
                    assistantSellPriceBonus += statValue;
                    break;

                case AssistantStatNames.IncreaseCustomerCount:
                    assistantCustomerSpawnIntervalReduction += statValue;
                    break;

                case AssistantStatNames.IncreaseAutoCustomerRepelChance:
                    assistantBadCustomerAutoKickChance += statValue;
                    break;
            }
        }
    }

    public void SetSkillEffect(ForgeUpgradeType type, float value, bool isActive)
    {
        float multiplier = isActive ? 1 : -1;
        float effectValue = (float)Math.Round(value * multiplier, 2);

        switch (type)
        {
            case ForgeUpgradeType.IncreaseSellPrice:
                skillSellPriceBonus += effectValue;
                break;

            case ForgeUpgradeType.IncreaseExpensiveRecipeChance:
                skillExpensiveWeaponSellChance += effectValue;
                break;

            case ForgeUpgradeType.ReduceCustomerSpawnDelay:
                skillCustomerSpawnIntervalReduction += effectValue;
                break;

            case ForgeUpgradeType.ReduceAutoCraftingTime:
                skillAutoCraftingTimeReduction += effectValue;
                break;

            case ForgeUpgradeType.IncreasePerfectCraftChance:
                skillPerfectCraftingChance += effectValue;
                break;
        }
    }

    public List<ForgeUpgradeSaveData> GetSaveData()
    {
        var dataList = new List<ForgeUpgradeSaveData>();

        foreach (var key in UpgradeLevels.Keys)
        {
            var saveData = new ForgeUpgradeSaveData()
            {
                UpgradeType = key,
                Level = UpgradeLevels[key]
            };

            dataList.Add(saveData);
        }

        return dataList;
    }

    public void LoadFromData(List<ForgeUpgradeSaveData> datas)
    {
        if (datas == null)
        {
            UpgradeLevels = new Dictionary<ForgeUpgradeType, int>()
            {
                { ForgeUpgradeType.IncreaseExpensiveRecipeChance, 1 },
                { ForgeUpgradeType.IncreasePerfectCraftChance, 1 },
                { ForgeUpgradeType.IncreaseSellPrice, 1 },
                { ForgeUpgradeType.ReduceAutoCraftingTime, 1 },
                { ForgeUpgradeType.ReduceCustomerSpawnDelay, 1 },
                { ForgeUpgradeType.UpgradeInterior, 1 }
            };
        }

        else
        {
            UpgradeLevels = new Dictionary<ForgeUpgradeType, int>();
            foreach (var data in datas)
            {
                UpgradeLevels[data.UpgradeType] = data.Level;
            }
        }

        ApplyUpgradeStats();
        forge.VisualHandler.SetInterior(UpgradeLevels[ForgeUpgradeType.UpgradeInterior]);
    }
}
