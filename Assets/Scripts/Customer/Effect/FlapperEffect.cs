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
        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        yield return WaitForSecondsCache.Wait(1f);
        onComplete?.Invoke();
    }
}
