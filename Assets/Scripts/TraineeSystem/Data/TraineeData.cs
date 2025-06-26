using System.Collections.Generic;

/// <summary>
/// 개별 제자 정보를 담고 있는 데이터 클래스입니다.
/// 이름, 성격, 특화, 능력치 계수 등을 포함합니다.
/// </summary>
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

    public class AbilityMultiplier
    {
        public string AbilityName;
        public float Multiplier;

        public AbilityMultiplier(string abilityName, float multiplier)
        {
            AbilityName = abilityName;
            Multiplier = multiplier;
        }
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void LevelUp()
    {
        if (Level >= MaxLevel)
        {
            return;
        }

        Level++;

        for (int i = 0; i < Multipliers.Count; i++)
        {
            var multiplier = Multipliers[i];
            float newMultiplier = multiplier.Multiplier * 1.2f;
            Multipliers[i] = new AbilityMultiplier(multiplier.AbilityName, newMultiplier);
        }
    }
}
