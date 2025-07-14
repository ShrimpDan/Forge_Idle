using System;
using System.Collections.Generic;

public class MineAssistantManager
{
    public List<MineAssistantSlot> Slots = new();

    public MineData Mine { get; private set; }

    public MineAssistantManager(MineData mine)
    {
        Mine = mine;
        for (int i = 0; i < 5; ++i)
            Slots.Add(new MineAssistantSlot());
    }

    public float CalcMinedAmount(DateTime since, DateTime now)
    {
        float mined = 0f;
        float hours = (float)(now - since).TotalHours;
        foreach (var slot in Slots)
        {
            if (slot.IsAssigned)
            {
                string grade = slot.AssignedAssistant.grade; 
                float multiplier = GetGradeMultiplier(grade);
                mined += Mine.CollectRatePerHour * multiplier * hours;
            }
        }
        return mined;
    }

    private float GetGradeMultiplier(string grade)
    {
        return grade switch
        {
            "UR" => 1.4f,
            "SSR" => 1.3f,
            "SR" => 1.2f,
            "R" => 1.1f,
            "N" => 1.0f,
            _ => 1.0f
        };
    }
}
