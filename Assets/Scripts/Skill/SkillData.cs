using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public string Key;
    public ForgeUpgradeType Type;
    public float BaseValue;
    public float BaseDuration;
    public float BaseCoolDown;
    public float ValueMultiplier;
    public float DurationMultiplier;
    public float CoolDownMultiplier;
    public string Description;
    public string IconPath;
}

public class SkillDataLoader
{
    public List<SkillData> SkillList { get; private set; }
    public Dictionary<string, SkillData> SkillDict { get; private set; }

    public SkillDataLoader(string path = "Data/skill_data")
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        if (json == null)
        {
            Debug.LogWarning("SkillData가 존재하지않습니다.");
            return;
        }

        SkillList = JsonUtility.FromJson<Wrapper>(json.text).Items;
        SkillDict = new Dictionary<string, SkillData>();

        foreach (var skill in SkillList)
        {
            SkillDict[skill.Key] = skill;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<SkillData> Items;
    }

    public SkillData GetSkillByKey(string key)
    {
        SkillDict.TryGetValue(key, out SkillData data);
        return data;
    }

    public SkillData GetRandomSkill()
    { 
        return SkillList[Random.Range(0, SkillList.Count)];
    }
}