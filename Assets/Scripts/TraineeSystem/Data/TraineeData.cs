using System.Collections.Generic;

using System;

/// <summary>
/// 개별 제자 정보를 담고 있는 데이터 클래스입니다.
/// 이름, 성격, 특화, 능력치 계수 등을 포함합니다.
/// </summary>
[Serializable]
public class TraineeData
{
    public const int MaxLevel = 10;

    public string Name { get; private set; }
    public int Level { get; private set; }
    public PersonalityData Personality { get; private set; }
    public SpecializationType Specialization { get; private set; }
    public List<AbilityMultiplier> Multipliers { get; private set; }

    public bool IsEquipped { get; set; }
    public bool IsInUse { get; set; }

    public int SpecializationIndex { get; set; }

    public TraineeData(
        string name,
        PersonalityData personality,
        SpecializationType specialization,
        List<AbilityMultiplier> multipliers,
        int level = 1,
        bool isEquipped = false,
        bool isInuse = false)
    {
        Name = name;
        Personality = personality;
        Specialization = specialization;
        Multipliers = multipliers ?? new List<AbilityMultiplier>();
        Level = level;
        IsEquipped = isEquipped;
        IsInUse = isInuse;
    }

    /// <summary>
    /// 능력치 계수 정의용 내부 클래스
    /// </summary>
    [Serializable]
    public class AbilityMultiplier
    {
        public string AbilityName;
        public float Multiplier;

        public AbilityMultiplier(string abilityName, float multiplier)
        {
            AbilityName = abilityName;
            Multiplier = multiplier;
        }

        public override string ToString()
        {
            return $"{AbilityName}: x{Multiplier:F2}";
        }
    }

    /// <summary>
    /// 이름을 변경합니다.
    /// </summary>
    public void SetName(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 레벨업 시 능력치 계수를 증가시킵니다.
    /// </summary>
    public void LevelUp(float growthRate = 1.2f)
    {
        if (Level >= MaxLevel) return;

        Level++;

        for (int i = 0; i < Multipliers.Count; i++)
        {
            var m = Multipliers[i];
            float newMultiplier = m.Multiplier * growthRate;
            Multipliers[i] = new AbilityMultiplier(m.AbilityName, newMultiplier);
        }
    }

    /// <summary>
    /// 계수를 외부에서 갱신할 때 사용
    /// </summary>
    public void SetMultipliers(List<AbilityMultiplier> newMultipliers)
    {
        Multipliers = newMultipliers;
    }
}