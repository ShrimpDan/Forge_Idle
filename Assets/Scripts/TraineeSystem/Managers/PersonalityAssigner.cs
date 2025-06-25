using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// PersonalityAssigner 클래스는 제자 생성을 위한 성격, 특화, 능력 배율을 자동으로 할당하는 클래스입니다.
/// </summary>
public class PersonalityAssigner
{
    private PersonalityTierDatabase database;

    public PersonalityAssigner(PersonalityTierDatabase db)
    {
        database = db;
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
        PersonalityTier selectedTier = database.tiers.Find(t => t.TierLevel == tier); 

        if (selectedTier != null && selectedTier.Personalities.Count > 0)
        {
            return selectedTier.Personalities[Random.Range(0, selectedTier.Personalities.Count)];
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
        float m;

        switch (spec)
        {
            case SpecializationType.Crafting:
                m = personality.CraftingMultiplier;
                list.Add(new("제작 속도 증가", 1.3f * m));
                list.Add(new("고급 제작 확률 증가", 1.5f * m));
                break;
            case SpecializationType.Enhancing:
                m = personality.EnhancingMultiplier;
                list.Add(new("강화 확률 증가", 1.2f * m));
                list.Add(new("파괴 확률 감소", 0.8f * m));
                list.Add(new("강화 비용 감소", 0.9f * m));
                break;
            case SpecializationType.Selling:
                m = personality.SellingMultiplier;
                list.Add(new("판매 수익 증가", 1.4f * m));
                list.Add(new("손님 수 증가", 1.3f * m));
                break;
        }

        return list;
    }
}

