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

    public IEnumerator StartQuest()
    {
        yield return MoveNext();
    }


    private IEnumerator MoveNext()
    {
        currentStep++;
        if (quest == null || currentStep >= quest.steps.Count)
        {
            yield break; //퀘스트 종료 
        }


        var ctx = new TutorialContext(mananger);
        var step = quest.steps[currentStep];


        //Enter Action 실행
        foreach (var a in step.actionOnEnter)
        {
            if (TutorialRegistry.Actions.TryGetValue(a.type, out var action))
            {
                yield return mananger.StartCoroutine(action.Execute(ctx, a));
            }
        }


    }

}
