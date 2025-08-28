using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAction : ITutorialAction
{
    public IEnumerator Execute(TutorialContext ctx, ActionData data)
    {
        throw new System.NotImplementedException();
    }
}


public class ActShowDialog : ITutorialAction
{ 
    public IEnumerator Execute(TutorialContext ctx, ActionData data)
    {
        ctx.mgr.ShowTextWithTyping(data.text);
        yield break;
    }
}

public class ActClickBlockerOn : ITutorialAction
{
    public IEnumerator Execute(TutorialContext ctx, ActionData data)
    {
       // ctx.mgr.SetClickBlocker(true);
        yield break;
    }
}
