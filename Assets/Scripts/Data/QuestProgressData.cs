using System.Collections.Generic;
using System;

[Serializable]
public class QuestProgressData
{
    public bool IsRunning = false;
    public bool IsCompleted = false;
    public float Timer = 0f;
    public int FinalRewardAmount = 0;
    public List<AssistantInstance> AssignedAssistants = new List<AssistantInstance>();
    public bool RewardReceived = false;
}
