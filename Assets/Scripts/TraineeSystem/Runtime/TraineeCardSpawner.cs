using UnityEngine;

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
    private readonly TraineeDrawController drawController;

    public TraineeCardSpawner(
        GameObject largeCardPrefab,
        GameObject miniCardPrefab,
        Transform singleDrawParent,
        Transform multiDrawParent,
        TraineeFactory factory,
        TraineeDrawController drawController)
    {
        this.largeCardPrefab = largeCardPrefab;
        this.miniCardPrefab = miniCardPrefab;
        this.singleDrawParent = singleDrawParent;
        this.multiDrawParent = multiDrawParent;
        this.factory = factory;
        this.drawController = drawController;
    }

    /// <summary>
    /// 단일 큰 카드 프리팹을 생성합니다.
    /// </summary>
    public GameObject SpawnLargeCard(
    TraineeData data,
    Transform parentOverride = null,
    System.Action<TraineeData> onConfirmAction = null,
    bool enableFlipImmediately = true,
    bool playSpawnEffect = true)
    {
        if (largeCardPrefab == null) return null;

        var parent = parentOverride != null ? parentOverride : singleDrawParent;
        if (parent == null) return null;

        var obj = Object.Instantiate(largeCardPrefab, parent);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.transform.localScale = Vector3.one;

        SetupCard(obj, data, 0, enableFlipImmediately, onConfirmAction, playSpawnEffect);
        return obj;
    }

    /// <summary>
    /// 미니 카드 프리팹을 생성합니다.
    /// </summary>
    public GameObject SpawnMiniCard(TraineeData data, int index)
    {
        if (miniCardPrefab == null || multiDrawParent == null) return null;

        var obj = Object.Instantiate(miniCardPrefab, multiDrawParent);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.transform.localScale = Vector3.one;

        SetupCard(obj, data, index, false, d => drawController.OnTraineeConfirmed?.Invoke(d), true);
        return obj;
    }

    /// <summary>
    /// 카드 오브젝트를 설정하고 이펙트를 실행합니다.
    /// </summary>
    private void SetupCard(
        GameObject obj,
        TraineeData data,
        int index,
        bool enableFlipImmediately,
        System.Action<TraineeData> onConfirmAction = null,
        bool playSpawnEffect = true)
    {
        var controller = obj.GetComponent<TraineeController>();
        if (controller == null)
        {
            Object.Destroy(obj);
            return;
        }

        controller.Setup(
            data,
            factory,
            drawController,
            onConfirmAction,
            enableFlipImmediately,
            isMultiDrawCard: index > 0,
            playSpawnEffect: playSpawnEffect
        );
    }
}
