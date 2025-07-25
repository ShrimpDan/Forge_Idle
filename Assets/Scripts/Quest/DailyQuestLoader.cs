using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyQuestLoader 
{
    public DailyQuestData data;
    public int currentAmount;
    public bool isCompleted => currentAmount >= data.goalAmount;
    public bool isClaimed; //수락했는지?


    public DailyQuestLoader(DailyQuestData data)
    {
        this.data = data;
        this.currentAmount = 0;
        this.isClaimed = false;
    }
}
