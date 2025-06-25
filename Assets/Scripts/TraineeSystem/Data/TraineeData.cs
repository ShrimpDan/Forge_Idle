using System.Collections.Generic;

public class TraineeData
{
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
}
