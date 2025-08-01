using System;

[Serializable]
public class MineAssistantSlot
{
    public AssistantInstance AssignedAssistant { get; private set; }
    public DateTime AssignedTime { get; set; }
    public bool IsAssigned => AssignedAssistant != null;

    // 기존 시그니처
    public void Assign(AssistantInstance assistant, DateTime assignedTime)
    {
        AssignedAssistant = assistant;
        AssignedTime = assignedTime;
    }

    // 추가: 단일 파라미터 오버로드
    public void Assign(AssistantInstance assistant)
    {
        Assign(assistant, DateTime.Now);
    }

    public void Unassign()
    {
        AssignedAssistant = null;
        AssignedTime = DateTime.MinValue;
    }
}
