using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// PersonalityAssigner 클래스는 제자 생성을 위한 성격, 특화, 능력 배율을 자동으로 할당하는 클래스입니다.
/// </summary>
public class PersonalityAssigner
{
    // 성격 티어 데이터베이스 참조 (1~5티어별 성격 목록이 포함된 ScriptableObject)
    private PersonalityTierDatabase database;

    /// <summary>
    /// 생성자: 외부에서 PersonalityTierDatabase를 주입받아 초기화
    /// </summary>
    public PersonalityAssigner(PersonalityTierDatabase db)
    {
        database = db;
    }

    // 임시로 제자의 이름에 번호를 등록 시키기 위한 카운터
    private static int traineeCount = 1;

    /// <summary>
    /// 제자 정보를 랜덤으로 생성하고 구성된 TraineeData를 반환합니다.
    /// </summary>
    public TraineeData GenerateTrainee()
    {
        // 1. 성격 데이터를 확률적으로 선택
        PersonalityData personality = GetRandomPersonality();

        // 2. 특화 타입 (제작/강화/판매) 중 하나를 랜덤으로 선택
        SpecializationType specialization = GetRandomSpecialization();

        // 3. TraineeData ScriptableObject 인스턴스를 생성 (에셋으로 저장 전 단계)
        TraineeData trainee = ScriptableObject.CreateInstance<TraineeData>();

        // 4. 임시 이름 자동 지정 (제자1,제자2,제자3)
        trainee.name = $"제자{traineeCount++}_{specialization}";

        // 5. 내부 필드에 성격과 특화 정보를 강제로 주입 (private field 접근용)
        trainee.GetType().GetField("personality", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(trainee, personality);

        trainee.GetType().GetField("specialization", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(trainee, specialization);

        // 6. 특화 능력치 배율 자동 부여
        SetMultipliers(trainee);

        // 최종 완성된 TraineeData 반환
        return trainee;
    }

    /// <summary>
    /// 1~5티어 중 확률 기반으로 성격 하나를 랜덤 선택합니다.
    /// </summary>
    private PersonalityData GetRandomPersonality()
    {
        int tier = GetRandomTier();  // 확률 기반으로 티어 선택
        PersonalityTier selectedTier = database.tiers.Find(t => t.TierLevel == tier);  // 해당 티어 검색

        if (selectedTier != null && selectedTier.Personalities.Count > 0)
        {
            // 해당 티어 내에서 성격 하나 랜덤 선택
            return selectedTier.Personalities[Random.Range(0, selectedTier.Personalities.Count)];
        }

        // 예외 처리: 유효하지 않을 경우 null 반환
        return null;
    }

    /// <summary>
    /// 1~5티어 중 확률에 따라 티어 값을 반환합니다.
    /// </summary>
    private int GetRandomTier()
    {
        float rand = Random.value;  // 0.0 ~ 1.0 사이 무작위 수

        if (rand < 0.05f) return 1;         // 5% 확률
        else if (rand < 0.15f) return 2;    // 10% 확률
        else if (rand < 0.35f) return 3;    // 20% 확률
        else if (rand < 0.65f) return 4;    // 30% 확률
        else return 5;                     // 35% 확률 (나머지)
    }

    /// <summary>
    /// 특화 타입을 무작위로 반환합니다 (Crafting / Enhancing / Selling)
    /// </summary>
    private SpecializationType GetRandomSpecialization()
    {
        return (SpecializationType)Random.Range(0, 3);  // Enum 값 범위: 0 ~ 2
    }

    /// <summary>
    /// 선택된 특화 타입에 따라 능력 배율 리스트를 자동 설정합니다.
    /// 성격에 따라 변화하는 값을 곱하여 최종 능력치를 정해줍니다.
    /// </summary>
    private void SetMultipliers(TraineeData trainee)
    {
        float personalityMultiplier = 1f;

        switch (trainee.Specialization)
        {
            case SpecializationType.Crafting:
                personalityMultiplier = trainee.Personality.CraftingMultiplier;

                trainee.Multipliers.Add(new TraineeData.AbilityMultiplier
                {
                    abilityName = "제작 속도 증가",
                    multiplier = 1.3f * personalityMultiplier
                });

                trainee.Multipliers.Add(new TraineeData.AbilityMultiplier
                {
                    abilityName = "고급 제작 확률 증가",
                    multiplier = 1.5f * personalityMultiplier
                });
                break;

            case SpecializationType.Enhancing:
                personalityMultiplier = trainee.Personality.EnhancingMultiplier;

                trainee.Multipliers.Add(new TraineeData.AbilityMultiplier
                {
                    abilityName = "강화 확률 증가",
                    multiplier = 1.2f * personalityMultiplier
                });

                trainee.Multipliers.Add(new TraineeData.AbilityMultiplier
                {
                    abilityName = "파괴 확률 감소",
                    multiplier = 0.8f * personalityMultiplier
                });

                trainee.Multipliers.Add(new TraineeData.AbilityMultiplier
                {
                    abilityName = "강화 비용 감소",
                    multiplier = 0.9f * personalityMultiplier
                });
                break;

            case SpecializationType.Selling:
                personalityMultiplier = trainee.Personality.SellingMultiplier;

                trainee.Multipliers.Add(new TraineeData.AbilityMultiplier
                {
                    abilityName = "판매 수익 증가",
                    multiplier = 1.4f * personalityMultiplier
                });

                trainee.Multipliers.Add(new TraineeData.AbilityMultiplier
                {
                    abilityName = "손님 수 증가",
                    multiplier = 1.3f * personalityMultiplier
                });
                break;
        }
    }
}
