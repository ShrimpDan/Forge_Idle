using System.Collections.Generic;

// SerializableAssistantInstance.cs
// AssistantInstance 데이터를 저장/불러오기용으로 직렬화하기 위한 구조체입니다.
// JSON 저장 시스템과 연동되며, 런타임에서 AssistantInstance로 변환됩니다.

[System.Serializable]
public class SerializableAssistantInstance
{
    // 기본 정보
    public string Key;
    public string Name;
    public int Level;
    public string Grade;
    public SpecializationType Specialization;
    public string IconPath;
    public string CustomerInfo;

    // 성격 정보
    public string PersonalityKey;
    public string PersonalityName;
    public int PersonalityTier;
    public float CraftingMultiplier;
    public float MiningMultiplier;
    public float SellingMultiplier;

    // 비용 정보
    public string CostKey;
    public int RecruitCost;
    public int Wage;
    public int RehireCost;

    // 상태
    public bool IsFired;

    // 능력치
    public List<AssistantInstance.AbilityMultiplier> Multipliers;
}
