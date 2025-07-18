using System.Collections.Generic;

[System.Serializable]
public class WageData
{
    public string Key;
    public int minWage;
    public int maxWage;
    public int minRecruitCost;
    public int maxRecruitCost;
    public int minRehireCost;
    public int maxRehireCost;
}

[System.Serializable]
public class WageDataList
{
    public List<WageData> Items;
}
