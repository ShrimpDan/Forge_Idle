using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// AssistantFactory.cs
// JSON 기반 제자 데이터를 바탕으로 제자를 생성하고, 중복 방지 및 등급 확률을 고려한 팩토리 클래스입니다.

public class AssistantFactory
{
    private readonly Dictionary<SpecializationType, int> specializationCounts = new();
    private readonly AssistantDataLoader assistantLoader;
    private readonly PersonalityDataLoader personalityLoader;
    private readonly SpecializationDataLoader specializationLoader;
    private readonly System.Random rng = new();
    private bool canRecruit = true;

    public AssistantFactory(DataManager dataManager)
    {
        assistantLoader = dataManager.AssistantLoader;
        personalityLoader = dataManager.PersonalityLoader;
        specializationLoader = dataManager.SpecializationLoader;
    }

    public bool CanRecruit => canRecruit;
    public void SetCanRecruit(bool value) => canRecruit = value;
    public void ResetRecruitLock() => canRecruit = true;

    // 스마트 방식으로 중복 없이 제자 생성
    public AssistantInstance CreateSmartRandomTrainee(HashSet<string> ownedKeys, SpecializationType? fixedType = null)
    {
        var remaining = assistantLoader.ItemsList
            .Where(a => !ownedKeys.Contains(a.Key))
            .Where(a => fixedType == null ||
                specializationLoader.GetByKey(a.specializationKey)?.specializationType == fixedType)
            .ToList();

        if (remaining.Count == 0)
        {
            Debug.LogWarning("[Factory] 남은 후보가 없습니다.");
            return null;
        }

        var grouped = remaining
            .GroupBy(a => a.grade)
            .ToDictionary(g => g.Key, g => g.ToList());

        Dictionary<string, float> baseWeights = new()
    {
        { "UR", 0.005f },
        { "SSR", 0.01f },
        { "SR", 0.03f },
        { "R", 0.30f },
        { "N", 0.655f }
    };

        var weightedChances = new Dictionary<string, float>();
        float total = 0f;

        foreach (var kv in grouped)
        {
            float count = kv.Value.Count;
            float weight = baseWeights.TryGetValue(kv.Key, out float baseW) ? baseW : 0f;
            float score = count * baseW;

            weightedChances[kv.Key] = score;
            total += score;
        }

        float rand = UnityEngine.Random.value;
        float cumulative = 0f;
        string selectedGrade = null;

        foreach (var kv in weightedChances.OrderByDescending(kv => kv.Value))
        {
            cumulative += kv.Value / total;
            if (rand <= cumulative)
            {
                selectedGrade = kv.Key;
                break;
            }
        }

        if (selectedGrade == null || !grouped.ContainsKey(selectedGrade))
            return null;

        var pool = grouped[selectedGrade];
        var chosen = pool[UnityEngine.Random.Range(0, pool.Count)];

        return CreateAssistantFromData(chosen);
    }


    public AssistantInstance CreateFixedTrainee(SpecializationType type, bool bypassRecruitCheck = false)
    {
        if (!canRecruit && !bypassRecruitCheck) return null;
        canRecruit = false;

        if (type == SpecializationType.All)
        {
            return CreateSmartRandomTrainee(new HashSet<string>(), null);
        }

        var candidates = assistantLoader.ItemsList.FindAll(t =>
            GetTier(t.grade) >= 2 &&
            specializationLoader.GetByKey(t.specializationKey)?.specializationType == type);

        if (candidates.Count == 0)
        {
            return null;
        }

        var selected = candidates[rng.Next(candidates.Count)];
        return CreateAssistantFromData(selected);
    }

    public List<AssistantInstance> CreateMultiple(int count, SpecializationType? fixedType = null)
    {
        var results = new List<AssistantInstance>();
        var ownedKeys = GameManager.Instance.AssistantInventory.GetAll()
            .Select(a => a.Key)
            .Concat(GameManager.Instance.HeldCandidates.Select(a => a.Key))
            .ToHashSet();

        for (int i = 0; i < count; i++)
        {
            var candidate = CreateSmartRandomTrainee(ownedKeys, fixedType);
            if (candidate != null)
            {
                results.Add(candidate);
                ownedKeys.Add(candidate.Key);
            }
        }

        return results;
    }

    public AssistantInstance CreateFromSpecAndPersonality(SpecializationType spec, string personalityKey, int minTier = 1)
    {
        var candidates = assistantLoader.ItemsList
            .Where(a =>
                specializationLoader.GetByKey(a.specializationKey)?.specializationType == spec &&
                a.personalityKey == personalityKey &&
                GetTier(a.grade) <= minTier)
            .ToList();

        if (candidates == null || candidates.Count == 0) return null;

        var selected = candidates[rng.Next(candidates.Count)];
        return CreateAssistantFromData(selected);
    }

    private AssistantInstance CreateAssistantFromData(AssistantData assistant)
    {
        int tier = GetTier(assistant.grade);
        var personalityData = personalityLoader.GetByKey(assistant.personalityKey);
        var specializationData = specializationLoader.GetByKey(assistant.specializationKey);

        float m = specializationData.specializationType switch
        {
            SpecializationType.Crafting => personalityData.craftingMultiplier,
            SpecializationType.Mining => personalityData.miningMultiplier,
            SpecializationType.Selling => personalityData.sellingMultiplier,
            _ => 1f
        };

        var multipliers = new List<AssistantInstance.AbilityMultiplier>();
        for (int i = 0; i < specializationData.statNames.Count; i++)
        {
            float raw = specializationData.statValues[i] * m;
            float clamped = Mathf.Max(raw, 0f);
            multipliers.Add(new AssistantInstance.AbilityMultiplier(specializationData.statNames[i], clamped));
        }

        string costKey = string.IsNullOrEmpty(assistant.costKey) ? $"wage_t{tier}" : assistant.costKey;
        var wageData = WageDataManager.Instance.GetByKey(costKey);

        int recruitCost = 0, wage = 0, rehireCost = 0;
        if (wageData != null)
        {
            int minRecruit = Mathf.FloorToInt(wageData.minRecruitCost / 10f) * 10;
            int maxRecruit = Mathf.Max(minRecruit + 10, Mathf.FloorToInt(wageData.maxRecruitCost / 10f) * 10);
            int minWage = Mathf.FloorToInt(wageData.minWage / 10f) * 10;
            int maxWage = Mathf.Max(minWage + 10, Mathf.FloorToInt(wageData.maxWage / 10f) * 10);
            int minRehire = Mathf.FloorToInt(wageData.minRehireCost / 10f) * 10;
            int maxRehire = Mathf.Max(minRehire + 10, Mathf.FloorToInt(wageData.maxRehireCost / 10f) * 10);

            recruitCost = Mathf.FloorToInt(UnityEngine.Random.Range(minRecruit, maxRecruit + 1) / 10f) * 10;
            wage = Mathf.FloorToInt(UnityEngine.Random.Range(minWage, maxWage + 1) / 10f) * 10;
            rehireCost = Mathf.FloorToInt(UnityEngine.Random.Range(minRehire, maxRehire + 1) / 10f) * 10;
        }

        var assistantData = new AssistantInstance(
            assistant.Key, assistant.Name, personalityData,
            specializationData.specializationType, multipliers,
            costKey, assistant.iconPath, 1, false, false,
            assistant.grade, assistant.customerInfo,
            recruitCost, wage, rehireCost
        );

        Debug.Log($"costKey: {costKey}, recruitCost: {recruitCost}, wage: {wage}, rehireCost: {rehireCost}");
        AssignInfo(assistantData);
        return assistantData;
    }

    private void AssignInfo(AssistantInstance data)
    {
        var spec = data.Specialization;
        if (!specializationCounts.ContainsKey(spec))
            specializationCounts[spec] = 0;
        specializationCounts[spec]++;
        data.SpecializationIndex = specializationCounts[spec];
    }

    private int GetTier(string grade)
    {
        return grade switch
        {
            "UR" => 1,
            "SSR" => 2,
            "SR" => 3,
            "R" => 4,
            "N" => 5,
            _ => 5
        };
    }
}
