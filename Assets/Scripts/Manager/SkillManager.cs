using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private GameManager gameManager;
    private SkillDataLoader skillDataLoader;
    public List<SkillInstance> SkillList { get; private set; }

    public void Init(GameManager gameManager, SkillDataLoader skillDataLoader)
    {
        this.gameManager = gameManager;
        this.skillDataLoader = skillDataLoader;
    }

    public void AddSkill()
    {
        SkillData skillData = skillDataLoader.GetRandomSkill();

        var existSkill = SkillList.Find(s => s.skillKey == skillData.Key);

        if (existSkill != null)
        {
            existSkill.AddSkill();
            return;
        }

        SkillInstance skill = new SkillInstance(skillData.Key, skillData);
        SkillList.Add(skill);
    }
}
