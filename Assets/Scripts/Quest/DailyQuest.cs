using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyQuest", menuName = "Data/DailyQuestData", order = 1)]
public class DailyQuestData : ScriptableObject

{
    public string questId;
    public string title;
    public string questInfo;
    public int goalAmount;
    public int currentAmount;
    public bool isCompleted;
    public bool isClaimed; //수락했는지?
    public int rewardCount;

}
