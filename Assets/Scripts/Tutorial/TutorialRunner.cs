using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRunner 
{
    private readonly TutorialManager mananger;
    private TutorialQuestData quest;
    private int currentStep = -1;
    private readonly List<ITutorialCondition> conds = new();



    public TutorialRunner(TutorialManager manager) { this.mananger = manager; }


    public void LoadQuest(TutorialQuestData questData)
    {
        quest = questData;
        currentStep = -1;
       
    }

}
