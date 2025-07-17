using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBlocker : MonoBehaviour
{
    public Action OnClick;

    
    public void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke();
        }

    }

}
