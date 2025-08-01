using System;

[Serializable]
public class MineAssistantSlot
{
    public AssistantInstance AssignedAssistant { get; private set; }
    public DateTime AssignedTime { get; set; }
    public bool IsAssigned => AssignedAssistant != null;
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
        AssignedAssistant = null;
        AssignedTime = DateTime.MinValue;
    }
}
