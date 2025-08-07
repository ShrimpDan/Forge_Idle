using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeSkillSystem : MonoBehaviour
{
    private ForgeManager forgeManager;
    private SkillManager skillManager;
    public SkillInstance[] ActiveSkills { get; private set; } = new SkillInstance[3];
    private List<Coroutine> activeCoroutines = new List<Coroutine>();

    public void Init(ForgeManager forgeManager, SkillManager skillManager)
    {
        this.forgeManager = forgeManager;
        this.skillManager = skillManager;
    }

    public void SetSkill(int idx, SkillInstance skill)
    {
        if (skill == null) return;

        if (ActiveSkills[idx] != null)
        {
            if (ActiveSkills[idx].IsCoolDown)
            {
                // 쿹타임 중에는 스킬 교체 불가능 알람 표시
                return;
            }

            UnSetSkill(ActiveSkills[idx]);
        }

        skill.EquipSkill();
        ActiveSkills[idx] = skill;
        forgeManager.Events.RaiseSkillChanged(idx, skill);
    }

    public void UnSetSkill(SkillInstance skill)
    {
        if (skill.IsCoolDown)
        {
            // 쿨타임 중에는 해제 불가능 알람
            return;
        }

        skill.UnEquipSkill();
        int idx = Array.IndexOf(ActiveSkills, skill);
        ActiveSkills[idx] = null;
        forgeManager.Events.RaiseSkillChanged(idx, null);
    }

    public void UseSkill(int idx)
    {
        SkillInstance skill = ActiveSkills[idx];
        if (skill == null || skill.IsCoolDown) return;

        Coroutine effectCoroutine = null;
        effectCoroutine = StartCoroutine(SkillEffectCoroutine(skill, effectCoroutine));
        activeCoroutines.Add(effectCoroutine);

        StartCoroutine(SkillCooldownCoroutine(idx));
        forgeManager.CurrentForge.BlackSmith.PlayTextEffect(skill.SkillData.Name);
    }

    public void StopAllSkillEffectCoroutine()
    {
        foreach (var coroutine in activeCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        foreach (var skill in ActiveSkills)
        {
            if (skill != null) // null 스킬 방지
            {
                forgeManager.CurrentForge.StatHandler.SetSkillEffect(skill.SkillData.Type, skill.GetValue(), false);
            }
        }

        activeCoroutines.Clear();
    }

    private IEnumerator SkillEffectCoroutine(SkillInstance skill, Coroutine coroutine)
    {
        forgeManager.CurrentForge.StatHandler.SetSkillEffect(skill.SkillData.Type, skill.GetValue(), true);
        forgeManager.CurrentForge.BlackSmith.CreateSkillSlot(skill, forgeManager);

        float maxDuration = skill.GetDuration();
        float time = 0;

        while (time / maxDuration <= 1f)
        {
            yield return WaitForSecondsCache.Wait(0.1f);
            time += 0.1f;

            forgeManager.Events.RaiseSkillDurationUpdate(skill, time / maxDuration);
        }

        forgeManager.CurrentForge.StatHandler.SetSkillEffect(skill.SkillData.Type, skill.GetValue(), false);

        if (activeCoroutines.Contains(coroutine))
            activeCoroutines.Remove(coroutine);
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

    public List<ActiveSkillSaveData> GetSaveData()
    {
        List<ActiveSkillSaveData> activeSkills = new List<ActiveSkillSaveData>();

        for (int i = 0; i < ActiveSkills.Length; i++)
        {
            SkillInstance skill = ActiveSkills[i];

            if (skill != null)
            {
                ActiveSkillSaveData saveData = new ActiveSkillSaveData()
                {
                    SkillKey = skill.SkillKey,
                    Idx = i
                };

                activeSkills.Add(saveData);
            }
        }

        return activeSkills;
    }

    public void LoadFromData(List<ActiveSkillSaveData> saveData)
    {
        if (saveData == null) return;

        foreach (var data in saveData)
        {
            SkillInstance skill = skillManager.GetSkillByKey(data.SkillKey);
            SetSkill(data.Idx, skill);
        }
    }
}
