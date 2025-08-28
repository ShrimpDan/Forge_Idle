using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class TutorialStepData
{
    public string id;
    public List<ActionData> actionOnEnter;
    public List<ConditionData> completeWhen;
    public List<ActionData> actionsOnComplete;

    
}


[System.Serializable]
public class TutorialQuestData
{
    public string id;
    public List<TutorialStepData> steps;

}




[System.Serializable]
public class ActionData
{
    public string type;
    public string text;
    public string target;
    public float x, y;
}


[System.Serializable]
public class ConditionData
{
    public string type;   // "ClickBlockerClicked", "UiClicked", ...
    public string target; // 버튼/UI 이름
}

public class TutorialContext //튜토리얼 매니저, UI, GameState등에 접근하기 위한 핸들
{
    public readonly TutorialManager mgr;  // 니가 쓰던 기본 매니저 가져와서 써
    public TutorialContext(TutorialManager m) { mgr = m; }
}   