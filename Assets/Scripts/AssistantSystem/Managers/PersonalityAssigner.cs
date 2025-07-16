using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// PersonalityAssigner 클래스는 제자 생성을 위한 성격, 특화, 능력 배율, 지출 금액을 자동으로 할당하는 클래스입니다.
/// </summary>
public class PersonalityAssigner
{
    private DataManager dataManger;

    public PersonalityAssigner(DataManager dataManger)
    {
        this.dataManger = dataManger;
    }

    private static int assistantCount = 1;

    public AssistantInstance GenerateTrainee()
    {
        var personality = GetRandomPersonality();
        var specialization = GetRandomSpecialization();
        var multipliers = GenerateMultipliers(personality, specialization);
        string costKey = $"wage_t{personality.tier}";
        string name = $"제자{assistantCount++}_{specialization}";
        string key = "What The Fuck";
        
        return new AssistantInstance(
            key,
            name,
            personality,
            specialization,
            multipliers,
            costKey,
            level: 1,
            isEquipped: false,
            isInuse: false
        );
    }


    /// <summary>
    /// 1~5티어 중 확률 기반으로 성격 하나를 랜덤 선택합니다.
    /// </summary>
    private PersonalityData GetRandomPersonality()
    {
        int tier = GetRandomTier();

        List<PersonalityData> personalityDatas = dataManger.PersonalityLoader.DataList.FindAll(t => t.tier == tier);

        if (personalityDatas != null)
        {
            return personalityDatas[Random.Range(0, personalityDatas.Count)];
        }
        
        return null;
    }

    /// <summary>
    /// 1~5티어 중 확률에 따라 티어 값을 반환합니다.
    /// </summary>
    private int GetRandomTier()
    {
        float rand = Random.value;

        if (rand < 0.05f) return 1;
        else if (rand < 0.15f) return 2;
        else if (rand < 0.35f) return 3;
        else if (rand < 0.65f) return 4;
        else return 5;
    }

    /// <summary>
    /// 특화 타입을 무작위로 반환합니다 (Crafting / Mining / Selling)
    /// </summary>
    private SpecializationType GetRandomSpecialization()
    {
        return (SpecializationType)Random.Range(0, 3);
    }

    /// <summary>
    /// 특정 특화 및 티어의 성격을 가진 제자를 생성합니다.
    /// 티어는 1(최상위) ~ 5(최하위)이며, 존재하는 데이터 중 랜덤 선택.
    /// </summary>
    public AssistantInstance GenerateTrainee(SpecializationType fixedType, int targetTier)
    {
        List<PersonalityData> personalityDatas = dataManger.PersonalityLoader.DataList.FindAll(t => t.tier == targetTier);

        if (personalityDatas == null || personalityDatas.Count == 0)
            return null;

        var personality = personalityDatas[Random.Range(0, personalityDatas.Count)];
        var multipliers = GenerateMultipliers(personality, fixedType);
        string costKey = $"wage_t{personality.tier}";
        string name = $"제자{assistantCount++}_{fixedType}";
        string key = "What The Fuck";

        return new AssistantInstance(
            key,
            name,
            personality,
            fixedType,
            multipliers,
            costKey,
            level: 1,
            isEquipped: false,
            isInuse: false
        );
    }

    public List<AssistantInstance.AbilityMultiplier> GenerateMultipliers(PersonalityData personality, SpecializationType spec)
    {
        var list = new List<AssistantInstance.AbilityMultiplier>();

        SpecializationData data = dataManger.SpecializationLoader.GetByTierAndType(personality.tier, spec);
        float m;

        switch (spec)
        {
            case SpecializationType.Crafting:
                m = personality.craftingMultiplier;
                break;
            case SpecializationType.Mining:
                m = personality.miningMultiplier;
                break;
            case SpecializationType.Selling:
                m = personality.sellingMultiplier;
                break;
            default:
                m = 1f;
                break;
        }

        for (int i = 0; i < data.statNames.Count; i++)
        {
            list.Add(new(data.statNames[i], data.statValues[i] * m));
        }

        return list;
    }

    public AssistantInstance GenerateTrainee(SpecializationType fixedType)
    {
        var personality = GetRandomPersonality();
        if (personality == null)
            return null;

        var multipliers = GenerateMultipliers(personality, fixedType);
        string costKey = $"wage_t{personality.tier}";
        string name = $"제자{assistantCount++}_{fixedType}";
        string key = "What The Fuck";

        return new AssistantInstance(
            key,
            name,
            personality,
            fixedType,
            multipliers,
            costKey,
            level: 1,
            isEquipped: false,
            isInuse: false
        );
    }
}

