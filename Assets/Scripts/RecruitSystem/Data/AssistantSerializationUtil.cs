using System.Collections.Generic;

// AssistantSerializationUtil.cs
// AssistantInstance와 SerializableAssistantInstance 간의 변환 기능을 제공하는 유틸리티 클래스입니다.
// 저장/불러오기 시스템에서 사용되는 데이터 직렬화 및 역직렬화를 담당합니다.

public static class AssistantSerializationUtil
{
    // 런타임 제자 데이터를 저장용 구조로 직렬화
    public static SerializableAssistantInstance ToSerializable(AssistantInstance instance)
    {
        return new SerializableAssistantInstance
        {
            Key = instance.Key,
            Name = instance.Name,
            Level = instance.Level,
            PersonalityKey = instance.Personality?.Key,
            PersonalityName = instance.Personality?.personalityName,
            PersonalityTier = instance.Personality?.tier ?? 0,
            CraftingMultiplier = instance.Personality?.craftingMultiplier ?? 1f,
            MiningMultiplier = instance.Personality?.miningMultiplier ?? 1f,
            SellingMultiplier = instance.Personality?.sellingMultiplier ?? 1f,
            Specialization = instance.Specialization,
            CostKey = instance.CostKey,
            IconPath = instance.IconPath,
            Grade = instance.grade,
            CustomerInfo = instance.CustomerInfo,
            RecruitCost = instance.RecruitCost,
            Wage = instance.Wage,
            Multipliers = instance.Multipliers,

            IsFired = instance.IsFired,
            RehireCost = instance.RehireCost
        };
    }

    // 저장용 제자 데이터를 런타임 구조로 역직렬화
    public static AssistantInstance ToRuntime(SerializableAssistantInstance data)
    {
        var personality = new PersonalityData
        {
            Key = data.PersonalityKey,
            personalityName = data.PersonalityName,
            tier = data.PersonalityTier,
            craftingMultiplier = data.CraftingMultiplier,
            miningMultiplier = data.MiningMultiplier,
            sellingMultiplier = data.SellingMultiplier
        };

        var instance = new AssistantInstance(
            key: data.Key,
            name: data.Name,
            personality: personality,
            specialization: data.Specialization,
            multipliers: data.Multipliers,
            costKey: data.CostKey,
            iconPath: data.IconPath,
            level: data.Level,
            grade: data.Grade,
            customerInfo: data.CustomerInfo,
            recruitCost: data.RecruitCost,
            wage: data.Wage,
            rehireCost: data.RehireCost
        );

        instance.IsFired = data.IsFired;

        return instance;
    }
}
