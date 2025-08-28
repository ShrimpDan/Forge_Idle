using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TutorialRegistry
{
    public static readonly Dictionary<string, ITutorialAction> Actions = new()
    {
        ["ShowMessage"] = new ActShowDialog()
       // ["ClickBlockerClicked"] = new ActClickBlockerOn()


    };
    
}
