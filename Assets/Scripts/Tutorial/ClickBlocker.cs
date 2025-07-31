using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBlocker : MonoBehaviour
{
    public static Action OnBlockClick;

    private float lastClickTime = 0f;
    private float clickDelay = 0.1f; // 연타 방지 딜레이(초)

    void Update()
    {
        MouseClick();
    }
    private void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 마지막 클릭 후 일정 시간 이내면 무시
            if (Time.time - lastClickTime < clickDelay)
                return;

            lastClickTime = Time.time;

            Debug.Log("클릭블록커 눌림");
            OnBlockClick?.Invoke();
        }
    }


}
