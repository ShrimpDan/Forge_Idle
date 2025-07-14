using System;

[Serializable]
public class MineAssistantSlot
{
    public AssistantInstance AssignedAssistant;
    public DateTime AssignedTime;

    public bool IsAssigned => AssignedAssistant != null;

    public void Assign(AssistantInstance assistant)
    {
        AssignedAssistant = assistant;
        AssignedTime = DateTime.Now;
    }

    public void Unassign()
    {
        AssignedAssistant = null;
    }
}
