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
    public int rewardCount;


}
