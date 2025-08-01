using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class DailyQuestSaveData
{
    public string questId;
    public int currentAmount;
    public bool isAccepted;
    public bool isRewardClaimed;
}

[Serializable]
public class DailyQuestSaveWrapper
{
    public List<DailyQuestSaveData> quests = new();
    public string lastResetTime;
}


