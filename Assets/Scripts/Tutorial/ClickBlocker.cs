using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBlocker : MonoBehaviour
{
    public static Action OnBlockClick;

    void Update()
    {
        MouseClick();
        
    }


    public void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("클릭블록커 눌림");
            OnBlockClick?.Invoke();
        }

    }

}
