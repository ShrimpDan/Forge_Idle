using System;
using System.Collections.Generic;

[Serializable]
public class AssistantInstance
{
    public const int MaxLevel = 10;

    public string Key { get; private set; }
    public string Name { get; private set; }
    public int Level { get; private set; }
    public PersonalityData Personality { get; private set; }
    public SpecializationType Specialization { get; private set; }
    public List<AbilityMultiplier> Multipliers { get; private set; }
    public string grade { get; private set; }


    public bool IsEquipped { get; set; }
    public bool IsInUse { get; set; }

    public int SpecializationIndex { get; set; }

    // 추가: 런타임 아이콘 경로 (AssistantData에서 복사)
    public string IconPath { get; set; }

    public AssistantInstance(
        string key,
        string name,
        PersonalityData personality,
        SpecializationType specialization,
        List<AbilityMultiplier> multipliers,
        string iconPath = null,
        int level = 1,
        bool isEquipped = false,
        bool isInuse = false,
        string grade = "N")
    {
        Key = key;
        Name = name;
        Personality = personality;
        Specialization = specialization;
        Multipliers = multipliers ?? new List<AbilityMultiplier>();
        IconPath = iconPath;
        Level = level;
        IsEquipped = isEquipped;
        IsInUse = isInuse;
        this.grade = grade;
    }

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

    public void SetName(string name) => Name = name;

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

    public void SetMultipliers(List<AbilityMultiplier> newMultipliers)
    {
        Multipliers = newMultipliers;
    }
}
