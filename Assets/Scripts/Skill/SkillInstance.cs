public class SkillInstance
{
    public string skillKey;
    public SkillData SkillData { get; private set; }
    public int Level { get; private set; }
    public int CurCount { get; private set; }
    public int NeedCount { get; private set; }
    public bool CanUpgrade => CurCount >= NeedCount;
    public bool IsCoolDown { get; private set; }

    public SkillInstance(string key, SkillData data, int level = 1, int curCount = 0, int needCount = 5)
    {
        skillKey = key;
        SkillData = data;

        Level = level;
        CurCount = curCount;
        NeedCount = needCount;
        IsCoolDown = false;
    }

    public void AddSkill() => CurCount += 1;
    
    public void UpgradeSkill()
    {
        if (!CanUpgrade) return;

        CurCount -= NeedCount;
        NeedCount += 2;

        Level += 1;
    }

    public float GetValue()
    {
        float value = SkillData.BaseValue;

        for (int i = 1; i < Level; i++)
        {
            value *= SkillData.ValueMultiplier;
        }

        return value;
    }

    public float GetDuration()
    {
        float duration = SkillData.BaseDuration;

        for (int i = 1; i < Level; i++)
        {
            duration *= SkillData.DurationMultiplier;
        }

        return duration;
    }

    public float GetCoolDown()
    {
        float cooldown = SkillData.BaseCoolDown;

        for (int i = 1; i < Level; i++)
        {
            cooldown *= SkillData.CoolDownMultiplier;
        }

        return cooldown;
    }

    public string GetDescription()
    {
        string description = string.Format(SkillData.Description, GetDuration(), SkillData.BaseValue, GetValue() - SkillData.BaseValue);
        return description;
    }

    public void SetCoolDown(bool isCoolDown)
    {
        IsCoolDown = isCoolDown;
    }
}
