using System.Collections.Generic;

public class SkillManager
{
    private GameManager gameManager;
    private SkillDataLoader skillDataLoader;
    public List<SkillInstance> SkillList { get; private set; } = new();

    public SkillManager(GameManager gameManager, SkillDataLoader skillDataLoader)
    {
        this.gameManager = gameManager;
        this.skillDataLoader = skillDataLoader;
    }

    public void AddRandomSkill()
    {
        SkillData skillData = skillDataLoader.GetRandomSkill();

        var existSkill = SkillList.Find(s => s.SkillKey == skillData.Key);

        if (existSkill != null)
        {
            existSkill.AddSkill();
            return;
        }

        SkillInstance skill = new SkillInstance(skillData.Key, skillData);
        SkillList.Add(skill);
    }

    public SkillInstance GetSkillByType(ForgeUpgradeType type)
    {
        SkillInstance skill = SkillList.Find(s => s.SkillData.Type == type);
        return skill;
    }

    public SkillInstance GetSkillByKey(string key)
    {
        SkillInstance skill = SkillList.Find(s => s.SkillKey == key);
        return skill;
    }

    public SkillSaveDataList ToSaveData()
    {
        SkillSaveDataList dataList = new SkillSaveDataList();
        dataList.savedSkills = new List<SkillSaveData>();

        foreach (var skill in SkillList)
        {
            SkillSaveData saveData = new SkillSaveData()
            {
                Key = skill.SkillKey,
                Level = skill.Level,
                CurCount = skill.CurCount,
                NeedCount = skill.NeedCount
            };

            dataList.savedSkills.Add(saveData);
        }

        return dataList;
    }

    public void LoadFromSaveData(SkillSaveDataList saveData)
    {
        foreach (var savedSkill in saveData.savedSkills)
        {
            SkillData skillData = skillDataLoader.GetSkillByKey(savedSkill.Key);
            SkillInstance skill = new SkillInstance(savedSkill.Key, skillData, savedSkill.Level, savedSkill.CurCount, savedSkill.NeedCount);

            SkillList.Add(skill);
        }
    }

    public void ClearSkill()
    {
        SkillList.Clear();
    }
}
