using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// PersonalityAssigner 클래스는 제자 생성을 위한 성격, 특화, 능력 배율을 자동으로 할당하는 클래스입니다.
/// </summary>
public class PersonalityAssigner
{
    private DataManger dataManger;

    public PersonalityAssigner(DataManger dataManger)
    {
        this.dataManger = dataManger;
    }

    private static int traineeCount = 1;

    public TraineeData GenerateTrainee()
    {
        var personality = GetRandomPersonality();
        var specialization = GetRandomSpecialization();
        var multipliers = GenerateMultipliers(personality, specialization);
        string name = $"제자{traineeCount++}_{specialization}";

        return new TraineeData(
            name,
            personality,
            specialization,
            multipliers,
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
    /// 특화 타입을 무작위로 반환합니다 (Crafting / Enhancing / Selling)
    /// </summary>
    private SpecializationType GetRandomSpecialization()
    {
        return (SpecializationType)Random.Range(0, 3);
    }

    private List<TraineeData.AbilityMultiplier> GenerateMultipliers(PersonalityData personality, SpecializationType spec)
    {
        var list = new List<TraineeData.AbilityMultiplier>();

        SpecializationData data = dataManger.SpecializationLoader.GetByTierAndType(personality.tier, spec);
        float m;

        switch (spec)
        {
            case SpecializationType.Crafting:
                m = personality.craftingMultiplier;
                break;
            case SpecializationType.Enhancing:
                m = personality.enhancingMultiplier;
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

    public TraineeData GenerateTrainee(SpecializationType fixedType)
    {
        var personality = GetRandomPersonality();
        if (personality == null)
            return null;

        var multipliers = GenerateMultipliers(personality, fixedType);
        string name = $"제자{traineeCount++}_{fixedType}";

        return new TraineeData(
            name,
            personality,
            fixedType,
            multipliers,
            level: 1,
            isEquipped: false,
            isInuse: false
        );
    }
}

