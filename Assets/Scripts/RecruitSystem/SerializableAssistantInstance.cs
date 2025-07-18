using System.Collections.Generic;

[System.Serializable]
public class SerializableAssistantInstance
{
    public string Key;
    public string Name;
    public int Level;
    public string PersonalityKey;
    public string PersonalityName;
    public int PersonalityTier;
    public float CraftingMultiplier;
    public float MiningMultiplier;
    public float SellingMultiplier;
    public SpecializationType Specialization;
    public string CostKey;
    public string IconPath;
    public string Grade;
    public string CustomerInfo;
    public int RecruitCost;
    public int Wage;
    public List<AssistantInstance.AbilityMultiplier> Multipliers;
}
