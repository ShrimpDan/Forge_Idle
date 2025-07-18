using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public AssistantInstance CreateRandomTrainee(bool bypassRecruitCheck = false)
    {
        if (!canRecruit && !bypassRecruitCheck) return null;
        canRecruit = false;

        var candidates = assistantLoader.ItemsList.FindAll(t => GetTier(t.grade) >= 2);
        if (candidates.Count == 0) return null;

        var selected = candidates[rng.Next(candidates.Count)];
        return CreateAssistantFromData(selected);
    }

    public AssistantInstance CreateFixedTrainee(SpecializationType type, bool bypassRecruitCheck = false)
    {
        if (!canRecruit && !bypassRecruitCheck) return null;
        canRecruit = false;

        var candidates = assistantLoader.ItemsList.FindAll(t => GetTier(t.grade) >= 2 &&
            specializationLoader.GetByKey(t.specializationKey)?.specializationType == type);
        if (candidates.Count == 0) return null;

        var selected = candidates[rng.Next(candidates.Count)];
        return CreateAssistantFromData(selected);
    }

    public List<AssistantInstance> CreateMultiple(int count, SpecializationType? fixedType = null)
    {
        var results = new List<AssistantInstance>();
        for (int i = 0; i < count; i++)
        {
            var data = fixedType == null
                ? CreateRandomTrainee(true)
                : CreateFixedTrainee(fixedType.Value, true);

            if (data != null)
                results.Add(data);
        }
        return results;
    }

    public void ResetRecruitLock() => canRecruit = true;

    private AssistantInstance CreateAssistantFromData(AssistantData assistant)
    {
        int tier = GetTier(assistant.grade);
        PersonalityData personalityData = personalityLoader.GetByKey(assistant.personalityKey);
        SpecializationData specializationData = specializationLoader.GetByKey(assistant.specializationKey);

        float m = 1f;
        switch (specializationData.specializationType)
        {
            case SpecializationType.Crafting: m = personalityData.craftingMultiplier; break;
            case SpecializationType.Mining: m = personalityData.miningMultiplier; break;
            case SpecializationType.Selling: m = personalityData.sellingMultiplier; break;
        }

        var multipliers = new List<AssistantInstance.AbilityMultiplier>();
        for (int i = 0; i < specializationData.statNames.Count; i++)
        {
            multipliers.Add(new AssistantInstance.AbilityMultiplier(
                abilityName: specializationData.statNames[i],
                multiplier: specializationData.statValues[i] * m));
        }

        string costKey = string.IsNullOrEmpty(assistant.costKey)
            ? $"wage_t{tier}"
            : assistant.costKey;

        WageData wageData = WageDataManager.Instance.GetByKey(costKey);

        int recruitCost = 0;
        int wage = 0;

        if (wageData != null)
        {
            int minRecruit = Mathf.FloorToInt(wageData.minRecruitCost / 10f) * 10;
            int maxRecruit = Mathf.FloorToInt(wageData.maxRecruitCost / 10f) * 10;
            int minWage = Mathf.FloorToInt(wageData.minWage / 10f) * 10;
            int maxWage = Mathf.FloorToInt(wageData.maxWage / 10f) * 10;

            if (maxRecruit <= minRecruit) maxRecruit = minRecruit + 10;
            if (maxWage <= minWage) maxWage = minWage + 10;

            recruitCost = Mathf.FloorToInt(UnityEngine.Random.Range(minRecruit, maxRecruit + 1) / 10f) * 10;
            wage = Mathf.FloorToInt(UnityEngine.Random.Range(minWage, maxWage + 1) / 10f) * 10;
        }

        AssistantInstance assistantData = new AssistantInstance(
            key: assistant.Key,
            name: assistant.Name,
            personality: personalityData,
            specialization: specializationData.specializationType,
            multipliers: multipliers,
            costKey: costKey,
            iconPath: assistant.iconPath,
            customerInfo: assistant.customerInfo,
            recruitCost: recruitCost,
            wage: wage,
            grade: assistant.grade
        );

        Debug.Log($"costKey: {costKey}, recruitCost: {recruitCost}, wage: {wage}");
        AssignInfo(assistantData);
        return assistantData;
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
