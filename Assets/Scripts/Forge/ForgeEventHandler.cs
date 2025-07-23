using System;
using UnityEngine;

public class ForgeEventHandler
{
    public event Action<int> OnGoldChanged;
    public event Action<int> OnDiaChanged;
    public event Action<int, int> OnFameChanged;
    public event Action<int> OnTotalFameChanged;
    public event Action<int> OnLevelChanged;
    public event Action<AssistantInstance, bool> OnAssistantChanged;
    public event Action<float, float> OnCraftProgress;
    public event Action<Sprite> OnCraftStarted;

    // 스킬 관련 이벤트
    public event Action<int, float> OnSkillCooldownStarted;
    public event Action<int, float, float> OnSkillCooldownUpdate;
    public event Action<int> OnSkillCooldownFinished;

    public void RaiseGoldChanged(int gold) => OnGoldChanged?.Invoke(gold);
    public void RaiseDiaChanged(int dia) => OnDiaChanged?.Invoke(dia);
    public void RaiseFameChanged(int currentFame, int maxFame) => OnFameChanged?.Invoke(currentFame, maxFame);
    public void RaiseTotalFameChanged(int totalFame) => OnTotalFameChanged?.Invoke(totalFame);
    public void RaiseLevelChanged(int level) => OnLevelChanged?.Invoke(level);
    public void RaiseAssistantChanged(AssistantInstance assi, bool isActive) => OnAssistantChanged?.Invoke(assi, isActive);
    public void RaiseCraftProgress(float curTime, float totalTime) => OnCraftProgress?.Invoke(curTime, totalTime);
    public void RaiseCraftStarted(Sprite icon) => OnCraftStarted?.Invoke(icon);
    public void RaiseSkillCoolDownStarted(int idx, float curCooldown) => OnSkillCooldownStarted?.Invoke(idx, curCooldown);
    public void RaiseSkillCooldownUpdate(int idx, float curCooldown, float maxCooldown) => OnSkillCooldownUpdate?.Invoke(idx, curCooldown, maxCooldown);
    public void RaiseSkillCooldownFinished(int idx) => OnSkillCooldownFinished?.Invoke(idx);
    
}
