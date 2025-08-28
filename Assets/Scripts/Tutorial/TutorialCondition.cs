using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CondClickBlockerClicked : ITutorialCondition
{
    bool hit;

    public void Bind(TutorialContext ctx, ConditionData data)
    {
        ClickBlocker.OnBlockClick += OnClick;
    }
    void OnClick() => hit = true;

    public void Unbind(TutorialContext ctx)
    {
        ClickBlocker.OnBlockClick -= OnClick;
    }

    public bool IsSatisfied()
    {
        return hit;
    }
}


public class CondUIOpen : ITutorialCondition
{

    bool hit;
    string target;
    
    public void Bind(TutorialContext ctx, ConditionData data)
    {
        target = data.target;
        ForgeTab.onClickButton += OnButton;
    }

    void OnButton(string name)
    {
        if (name == target)
        {
            hit = true;
        }
    }
    public void Unbind(TutorialContext ctx)
    { 
        ForgeTab.onClickButton -= OnButton;
    }
    public bool IsSatisfied()
    {
        return hit;
    }
}

public class CondUIClose : ITutorialCondition
{
    bool hit; string target;
    public void Bind(TutorialContext ctx, ConditionData data)
    {
        target = data.target;
        GameManager.Instance.UIManager.CloseUIName += OnClose;
    }
    void OnClose(string name) { if (name == target) hit = true; }
    public void Unbind(TutorialContext ctx)
    {
        GameManager.Instance.UIManager.CloseUIName -= OnClose;
    }
    public bool IsSatisfied()
    {
        return hit;
    }
}

