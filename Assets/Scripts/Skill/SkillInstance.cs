public class SkillInstance
{
    public string skillKey;
    public SkillData skillData;
    public int Level { get; private set; }
    public int CurCount { get; private set; }
    public int NeedCount { get; private set; }
    public bool canUpgrade => CurCount >= NeedCount;

    public SkillInstance(string key, SkillData data, int level = 1, int curCount = 0, int needCount = 5)
    {
        skillKey = key;
        skillData = data;

        Level = level;
        CurCount = curCount;
        NeedCount = needCount;
    }

    public void UpgradeSkill()
    {
        if (!canUpgrade) return;

        CurCount -= NeedCount;
        NeedCount += 2;

        Level += 1;
    }

    public float GetValue()
    {
        float value = skillData.BaseValue;

        for (int i = 1; i < Level; i++)
        {
            value *= skillData.ValueMultiplier;
        }

        return value;
    }

    public float GetDuration()
    {
        float duration = skillData.BaseDuration;

        for (int i = 1; i < Level; i++)
        {
            duration *= skillData.DurationMultiplier;
        }

        return duration;
    }

    public float GetCoolDown()
    {
        float cooldown = skillData.BaseCoolDown;

        for (int i = 1; i < Level; i++)
        {
            cooldown *= skillData.CoolDownMultiplier;
        }

        return cooldown;
    }

    public string GetDescription()
    {
        string description = string.Format(skillData.Description, GetDuration(), skillData.BaseValue, GetValue() - skillData.BaseValue);
        return description;
    }
}
