using System;

[Serializable]
public class MineAssistantSlot
{
    public AssistantInstance AssignedAssistant { get; private set; }
    public DateTime AssignedTime { get; set; }
    public bool IsAssigned => AssignedAssistant != null;

    // === 버프/쿨타임 상태 저장용 필드 추가 ===
    public bool LastIsBuffActive;
    public float LastBuffRemain;
    public bool LastIsCooldown;
    public float LastCooldownRemain;

    public void Assign(AssistantInstance assistant, DateTime assignedTime)
    {
        AssignedAssistant = assistant;
        AssignedTime = assignedTime;
    }

    public void Assign(AssistantInstance assistant)
    {
        Assign(assistant, DateTime.Now);
    }

    public void Unassign()
    {
        if (AssignedAssistant != null)
            AssignedAssistant.IsInUse = false;

        AssignedAssistant = null;
        AssignedTime = DateTime.MinValue;
    }
}
