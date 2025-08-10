using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartEffect : MonoBehaviour, IPoolable
{
    private Action onComplete;    
    private GameObject sourcePrefab;

    public GameObject SourcePrefab
    {
        get => sourcePrefab;
        set => sourcePrefab = value;
    }
    public void Init(Vector3 pos, System.Action onCompleteCallBack)
    {
        transform.position = pos;
        StartCoroutine(HeartEffectCoroutine());
        onComplete = onCompleteCallBack;
        // SoundManager.Instance.Play(); 사운드 입력 주말에 작업하겟움
    }

    void Update()
    {
        this.transform.position += Vector3.up * Time.deltaTime * 4.0f;

    }

    private IEnumerator HeartEffectCoroutine()
    {
      
        yield return WaitForSecondsCache.Wait(3.0f);
        onComplete?.Invoke(); //끝났다고 알림

    }
}
