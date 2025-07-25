using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyQuestLoader 
{
    public DailyQuestData data;
    public int currentAmount;
    public bool isCompleted => currentAmount >= data.goalAmount;
    public bool isAccepted; //수락했는지?
    public bool isRewardClaimed; //보상 수령 했는지


    public DailyQuestLoader(DailyQuestData data)
    {
        this.data = data;
        this.currentAmount = 0;
        this.isAccepted = false;
        this.isRewardClaimed = false;
    }
}
