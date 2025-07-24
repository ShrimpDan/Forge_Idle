using System.Collections;
using UnityEngine;

public class ForgeSkillSystem : MonoBehaviour
{
    private ForgeManager forgeManager;
    public SkillInstance[] ActiveSkills { get; private set;  } = new SkillInstance[3];
    private Coroutine[] skillCoroutine = new Coroutine[3];

    public void Init(ForgeManager forgeManager)
    {
        this.forgeManager = forgeManager;
    }

    public void SetSkill(int idx, SkillInstance skill)
    {
        if (skillCoroutine[idx] != null)
        {
            if (ActiveSkills[idx].IsCoolDown)
            {
                // 쿹타임 중에는 스킬 교체 불가능 알람 표시
                return;
            }
        }

        ActiveSkills[idx] = skill;
        forgeManager.Events.RaiseSkillChanged(idx);
    }

    public void UseSkill(int idx)
    {
        SkillInstance skill = ActiveSkills[idx];
        if (skill == null || skill.IsCoolDown) return;

        skillCoroutine[idx] = StartCoroutine(SkillEffectCoroutine(skill));
        StartCoroutine(SkillCooldownCoroutine(idx));
    }

    private IEnumerator SkillEffectCoroutine(SkillInstance skill)
    {
        forgeManager.CurrentForge.StatHandler.SetSkillEffect(skill.SkillData.Type, skill.GetValue(), true);
        yield return WaitForSecondsCache.Wait(skill.GetDuration());
        forgeManager.CurrentForge.StatHandler.SetSkillEffect(skill.SkillData.Type, skill.GetValue(), false);
    }

    private IEnumerator SkillCooldownCoroutine(int idx)
    {
        SkillInstance skill = ActiveSkills[idx];

        float curCooldown = skill.GetCoolDown();
        skill.SetCoolDown(true);
        forgeManager.Events.RaiseSkillCoolDownStarted(idx, curCooldown);

        while (curCooldown > 0f)
        {
            yield return WaitForSecondsCache.Wait(0.1f);
            curCooldown -= 0.1f;

            if (curCooldown < 0f) curCooldown = 0f;

            forgeManager.Events.RaiseSkillCooldownUpdate(idx, curCooldown, skill.GetCoolDown());
        }

        skill.SetCoolDown(false);
        forgeManager.Events.RaiseSkillCooldownFinished(idx);
    }
}
