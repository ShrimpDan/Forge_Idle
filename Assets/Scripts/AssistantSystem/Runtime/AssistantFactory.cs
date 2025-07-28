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

    // 리크루트 락 설정
    public void SetCanRecruit(bool value) => canRecruit = value;

    // 리크루트 락 해제
    public void ResetRecruitLock() => canRecruit = true;

    // 랜덤 제자 생성 (등급 확률 기반, 중복 필터 없음)
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
                    return string.Equals(gradeClean, selectedGrade, StringComparison.OrdinalIgnoreCase);
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

    // 고정 특화 제자 생성 (등급 우선, 없으면 전체 특화에서 뽑기)
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
            candidates = assistantLoader.ItemsList
                .Where(t => specializationLoader.GetByKey(t.specializationKey)?.specializationType == type)
                .ToList();

            if (candidates.Count == 0)
            {
                Debug.LogWarning($"[FixedTrainee] 특화: {type} → 후보 전혀 없음");
                return null;
            }

            Debug.LogWarning($"[FixedTrainee] 원하는 등급 없음 → 해당 특화 전체에서 임의 선택");
        }

        var selected = candidates[rng.Next(candidates.Count)];
        return CreateAssistantFromData(selected);
    }

    // 중복 없이 다수의 제자를 생성
    public List<AssistantInstance> CreateMultiple(int count, SpecializationType? fixedType = null)
    {
        var results = new List<AssistantInstance>();
        var ownedKeys = GameManager.Instance.AssistantInventory.GetAll()
            .Select(a => a.Key)
            .ToHashSet();

        var availableCandidates = assistantLoader.ItemsList
            .Where(a => !ownedKeys.Contains(a.Key))
            .Where(a => fixedType == null ||
                specializationLoader.GetByKey(a.specializationKey)?.specializationType == fixedType)
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

    // 성격 + 특화 조건으로 제자 생성 (티어 이하 필터)
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

    // 제자 데이터 → 런타임 AssistantInstance 변환 및 스탯 계산
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

    // 생성된 제자에 특화 인덱스 부여
    private void AssignInfo(AssistantInstance data)
    {
        var spec = data.Specialization;
        if (!specializationCounts.ContainsKey(spec))
            specializationCounts[spec] = 0;
        specializationCounts[spec]++;
        data.SpecializationIndex = specializationCounts[spec];
    }

    // 등급 문자열을 티어 숫자로 변환
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

    // 등급별 확률 분포 반환
    private string GetRandomGradeByProbability()
    {
        float rand = UnityEngine.Random.value;

        if (rand < 0.005f) return "UR";        // 0.5%
        else if (rand < 0.015f) return "SSR";  // 1%
        else if (rand < 0.045f) return "SR";   // 3%
        else if (rand < 0.345f) return "R";    // 30%
        else return "N";                       // 65.5%
    }
}
