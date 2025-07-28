using System;
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

        const int maxRetry = 10;
        int attempts = 0;

        while (attempts < maxRetry)
        {
            string selectedGrade = GetRandomGradeByProbability();
            Debug.Log($"[등급 선택됨]: '{selectedGrade}'");

            var candidates = assistantLoader.ItemsList
                .Where(t =>
                {
                    string gradeClean = t.grade?.Trim();
                    Debug.Log($"[제자 등급 확인]: '{gradeClean}'");
                    return string.Equals(gradeClean, selectedGrade, System.StringComparison.OrdinalIgnoreCase);
                })
                .ToList();

            if (candidates.Count == 0 && attempts == maxRetry - 1)
            {
                Debug.LogWarning($"[Fallback] {selectedGrade} 후보 없음 → 전체에서 아무거나");
                candidates = assistantLoader.ItemsList.ToList();
            }

            if (candidates.Count > 0)
            {
                var selected = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                return CreateAssistantFromData(selected);
            }

            attempts++;
        }

        Debug.LogWarning("[AssistantFactory] 뽑기 실패: 유효한 후보 없음");
        canRecruit = true;
        return null;
    }

    public AssistantInstance CreateFixedTrainee(SpecializationType type, bool bypassRecruitCheck = false)
    {
        if (!canRecruit && !bypassRecruitCheck) return null;
        canRecruit = false;

        string selectedGrade = GetRandomGradeByProbability();

        var candidates = assistantLoader.ItemsList
            .Where(t =>
                string.Equals(t.grade?.Trim(), selectedGrade, StringComparison.OrdinalIgnoreCase) &&
                specializationLoader.GetByKey(t.specializationKey)?.specializationType == type)
            .ToList();

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"[FixedTrainee] 특화: {type}, 등급: {selectedGrade} → 후보 없음");
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
            .ToHashSet();

        var availableCandidates = assistantLoader.ItemsList
            .Where(a => !ownedKeys.Contains(a.Key))
            .ToList();

        if (availableCandidates.Count < count)
        {
            Debug.LogWarning("[AssistantFactory] 보유하지 않은 제자가 부족합니다.");
            return results;
        }

        var usedKeys = new HashSet<string>();
        int attempts = 0;
        const int maxAttempts = 100;

        while (results.Count < count && attempts < maxAttempts)
        {
            attempts++;

            string selectedGrade = GetRandomGradeByProbability();

            var gradeFiltered = availableCandidates
                .Where(a => string.Equals(a.grade?.Trim(), selectedGrade, StringComparison.OrdinalIgnoreCase))
                .Where(a => !usedKeys.Contains(a.Key))
                .ToList();

            if (gradeFiltered.Count == 0)
                continue;

            var selected = gradeFiltered[UnityEngine.Random.Range(0, gradeFiltered.Count)];
            var instance = CreateAssistantFromData(selected);
            results.Add(instance);
            usedKeys.Add(selected.Key);
        }

        if (results.Count < count)
        {
            Debug.LogWarning($"[AssistantFactory] {count}명 중 {results.Count}명만 생성됨 (후보 부족)");
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
        int rehireCost = 0;

        if (wageData != null)
        {
            int minRecruit = Mathf.FloorToInt(wageData.minRecruitCost / 10f) * 10;
            int maxRecruit = Mathf.FloorToInt(wageData.maxRecruitCost / 10f) * 10;
            int minWage = Mathf.FloorToInt(wageData.minWage / 10f) * 10;
            int maxWage = Mathf.FloorToInt(wageData.maxWage / 10f) * 10;
            int minRehire = Mathf.FloorToInt(wageData.minRehireCost / 10f) * 10;
            int maxRehire = Mathf.FloorToInt(wageData.maxRehireCost / 10f) * 10;

            if (maxRecruit <= minRecruit) maxRecruit = minRecruit + 10;
            if (maxWage <= minWage) maxWage = minWage + 10;
            if (maxRehire <= minRehire) maxRehire = minRehire + 10;

            recruitCost = Mathf.FloorToInt(UnityEngine.Random.Range(minRecruit, maxRecruit + 1) / 10f) * 10;
            wage = Mathf.FloorToInt(UnityEngine.Random.Range(minWage, maxWage + 1) / 10f) * 10;
            rehireCost = Mathf.FloorToInt(UnityEngine.Random.Range(minRehire, maxRehire + 1) / 10f) * 10;
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
            rehireCost: rehireCost,
            grade: assistant.grade
        );

        Debug.Log($"costKey: {costKey}, recruitCost: {recruitCost}, wage: {wage}, rehireCost: {rehireCost}");
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

    private string GetRandomGradeByProbability()
    {
        float rand = UnityEngine.Random.value; // 0.0 ~ 1.0

        if (rand < 0.005f) return "UR";        // 0.5%
        else if (rand < 0.015f) return "SSR";  // 1%
        else if (rand < 0.045f) return "SR";   // 3%
        else if (rand < 0.345f) return "R";    // 30%
        else return "N";                       // 65.5%
    }


}
