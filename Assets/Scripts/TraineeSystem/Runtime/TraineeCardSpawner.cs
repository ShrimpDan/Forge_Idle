using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 제자 카드를 프리팹 기반으로 생성하고 초기화하는 책임을 갖는 클래스입니다.
/// </summary>
public class TraineeCardSpawner
{
    private readonly GameObject largeCardPrefab;
    private readonly GameObject miniCardPrefab;
    private readonly Transform singleDrawParent;
    private readonly Transform multiDrawParent;
    private readonly TraineeFactory factory;
    private readonly TraineeManager manager;

    public TraineeCardSpawner(
        GameObject largeCardPrefab,
        GameObject miniCardPrefab,
        Transform singleDrawParent,
        Transform multiDrawParent,
        TraineeFactory factory,
        TraineeManager manager)
    {
        this.largeCardPrefab = largeCardPrefab;
        this.miniCardPrefab = miniCardPrefab;
        this.singleDrawParent = singleDrawParent;
        this.multiDrawParent = multiDrawParent;
        this.factory = factory;
        this.manager = manager;
    }

    /// <summary>
    /// 단일 큰 카드 프리팹을 생성합니다.
    /// </summary>
    public GameObject SpawnLargeCard(TraineeData data)
    {
        if (largeCardPrefab == null || singleDrawParent == null) return null;

        var obj = UnityEngine.Object.Instantiate(largeCardPrefab, singleDrawParent);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.transform.localScale = Vector3.one;

        SetupCard(obj, data, 0, true);
        return obj;
    }

    /// <summary>
    /// 미니 카드 프리팹을 생성합니다.
    /// </summary>
    public GameObject SpawnMiniCard(TraineeData data, int index)
    {
        if (miniCardPrefab == null || multiDrawParent == null) return null;

        var obj = UnityEngine.Object.Instantiate(miniCardPrefab, multiDrawParent);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.transform.localScale = Vector3.one;

        SetupCard(obj, data, index, false);
        return obj;
    }

    /// <summary>
    /// 카드 오브젝트를 설정하고 이펙트를 실행합니다.
    /// </summary>
    private void SetupCard(GameObject obj, TraineeData data, int index, bool enableFlipImmediately)
    {
        var controller = obj.GetComponent<TraineeController>();
        if (controller == null)
        {
            UnityEngine.Object.Destroy(obj);
            return;
        }

        controller.Setup(data, factory, manager, index, null, manager.ConfirmTrainee, enableFlipImmediately, true);
        controller.PlaySpawnEffect();
    }
}
