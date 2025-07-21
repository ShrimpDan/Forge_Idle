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

            var ownedKeys = GameManager.Instance.AssistantInventory.GetAll().Select(a => a.Key);
            var heldKeys = GameManager.Instance.HeldCandidates.Select(h => h.Key);
            var blockedKeys = new HashSet<string>(ownedKeys.Concat(heldKeys));

            var candidates = assistantLoader.ItemsList
                .Where(t => t.grade == selectedGrade && !blockedKeys.Contains(t.Key))
                .ToList();

            if (candidates.Count > 0)
            {
                var selected = candidates[rng.Next(candidates.Count)];
                return CreateAssistantFromData(selected);
            }

            attempts++;
        }

        Debug.LogWarning("[AssistantFactory] 뽑기 실패: 유효한 등급의 제자 후보가 없습니다.");
        return null;
    }

    public AssistantInstance CreateFixedTrainee(SpecializationType type, bool bypassRecruitCheck = false)
    {
        if (!canRecruit && !bypassRecruitCheck) return null;
        canRecruit = false;

        var ownedKeys = GameManager.Instance.AssistantInventory.GetAll().Select(a => a.Key);
        var heldKeys = GameManager.Instance.HeldCandidates.Select(h => h.Key);
        var blockedKeys = new HashSet<string>(ownedKeys.Concat(heldKeys));

        var candidates = assistantLoader.ItemsList.FindAll(t =>
            GetTier(t.grade) >= 2 &&
            specializationLoader.GetByKey(t.specializationKey)?.specializationType == type &&
            !blockedKeys.Contains(t.Key)
        );

        if (candidates.Count == 0) return null;

        var selected = candidates[rng.Next(candidates.Count)];
        return CreateAssistantFromData(selected);
    }


    public List<AssistantInstance> CreateMultiple(int count, SpecializationType? fixedType = null)
    {
        var results = new List<AssistantInstance>();
        const int maxAttemptsPerSlot = 10;

        for (int i = 0; i < count; i++)
        {
            int attempts = 0;
            AssistantInstance data = null;

            while (attempts < maxAttemptsPerSlot)
            {
                data = fixedType == null
                    ? CreateRandomTrainee(true)
                    : CreateFixedTrainee(fixedType.Value, true);

                if (data != null)
                {
                    results.Add(data);
                    break;
                }

                attempts++;
            }

            if (data == null)
            {
                Debug.LogWarning($"[AssistantFactory] 제자 {i + 1}명 생성 실패 (고정 타입: {fixedType?.ToString() ?? "랜덤"})");
            }
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

        if (rand < 0.01f) return "UR";       // 1%
        else if (rand < 0.05f) return "SSR"; // 4%
        else if (rand < 0.15f) return "SR";  // 10%
        else if (rand < 0.40f) return "R";   // 25%
        else return "N";                     // 60%
    }
}
