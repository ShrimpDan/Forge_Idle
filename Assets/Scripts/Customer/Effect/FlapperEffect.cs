using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapperEffect : MonoBehaviour , IPoolable
{
    private Action onComplete;

    private GameObject sourcePrefab;

    public GameObject SourcePrefab
    {
        get => sourcePrefab;
        set => sourcePrefab = value;
    }

    public void Init(Vector3 pos, Action onCompleteCallBack)
    {
        transform.position = pos;
        onComplete = onCompleteCallBack;

     //   SoundManager.Instance.Play(); 사운드 입력 주말에 작업하겟움
        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        yield return WaitForSecondsCache.Wait(1f);
        onComplete?.Invoke();
    }
}
