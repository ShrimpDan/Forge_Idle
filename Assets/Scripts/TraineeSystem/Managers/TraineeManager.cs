using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TraineeManager : MonoBehaviour
{
    [Header("카드 프리팹")]
    [SerializeField] private GameObject largeTraineeCardPrefab;
    [SerializeField] private GameObject miniTraineeCardPrefab;

    [Header("카드 출력 위치")]
    [SerializeField] private Transform singleDrawParent;
    [SerializeField] private Transform multiDrawParent;

    [Header("성격 데이터베이스")]
    [SerializeField] private PersonalityTierDatabase personalityDatabase;

    private List<TraineeData> runtimeTrainees = new();
    private List<GameObject> activeTrainees = new();
    private List<TraineeData> currentBatch = new();

    private List<GameObject> spawnedCards = new();
    private List<Vector2> targetPositions = new();

    private TraineeFactory factory;

    private bool isCardInteractionLocked = false;
    public bool IsCardInteractionLocked => isCardInteractionLocked;

    private bool canRecruit = true;
    private int cardsToConfirm = 0;

    private SpecializationType? currentDrawType = null;

    private void Awake()
    {
        factory = new TraineeFactory(personalityDatabase);
    }

    public void OnClickRecruitRandomTrainee()
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 1;
        RecruitAndSpawnTrainee();
    }

    public void OnClickRecruitCraftingTrainee()
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 1;
        RecruitAndSpawnFixed(SpecializationType.Crafting);
    }

    public void OnClickRecruitEnhancingTrainee()
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 1;
        RecruitAndSpawnFixed(SpecializationType.Enhancing);
    }

    public void OnClickRecruitSellingTrainee()
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 1;
        RecruitAndSpawnFixed(SpecializationType.Selling);
    }

    public void OnClickRecruit10Random()
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 10;
        StartMultipleRecruit(10);
    }

    public void OnClickRecruit10Crafting()
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 10;
        StartMultipleRecruit(10, SpecializationType.Crafting);
    }

    public void OnClickRecruit10Enhancing()
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 10;
        StartMultipleRecruit(10, SpecializationType.Enhancing);
    }

    public void OnClickRecruit10Selling()
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 10;
        StartMultipleRecruit(10, SpecializationType.Selling);
    }

    public void OnCardConfirmed()
    {
        cardsToConfirm--;
        if (cardsToConfirm <= 0)
        {
            canRecruit = true;
            Debug.Log(">> 다음 뽑기 가능");
        }
    }

    public void RecruitAndSpawnTrainee()
    {
        TraineeData data = factory.CreateRandomTrainee();
        if (data == null) return;
        SpawnSingleCard(data);
        ConfirmTrainee(data);
    }

    public void RecruitAndSpawnFixed(SpecializationType type)
    {
        TraineeData data = factory.CreateFixedTrainee(type);
        if (data == null) return;
        SpawnSingleCard(data);
        ConfirmTrainee(data);
    }

    private void SpawnSingleCard(TraineeData data)
    {
        if (largeTraineeCardPrefab == null || singleDrawParent == null) return;
        GameObject obj = Instantiate(largeTraineeCardPrefab, singleDrawParent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
        SetupCardObject(obj, data, 0, true);
    }

    private void SpawnMiniCard(TraineeData data, int index)
    {
        if (miniTraineeCardPrefab == null || multiDrawParent == null) return;
        GameObject obj = Instantiate(miniTraineeCardPrefab, multiDrawParent);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
        SetupCardObject(obj, data, index, enableFlipImmediately: false);
        spawnedCards.Add(obj);
    }

    private void SetupCardObject(GameObject obj, TraineeData data, int index, bool enableFlipImmediately = false)
    {
        TraineeController controller = obj.GetComponent<TraineeController>();
        if (controller == null)
        {
            Debug.LogError("TraineeController 없음");
            Destroy(obj);
            return;
        }
        controller.Setup(data, factory, this, index, null, ConfirmTrainee, enableFlipImmediately, false);
        controller.PlaySpawnEffect();
        activeTrainees.Add(obj);
    }

    private List<Vector2> CalculateCardPositions(int count)
    {
        List<Vector2> positions = new();
        Vector2 firstCardPos = new Vector2(-432, 713);
        Vector2 sixthCardPos = new Vector2(-432, 419);
        float verticalGap = sixthCardPos.y - firstCardPos.y;
        float horizontalGap = 216f;
        for (int i = 0; i < count; i++)
        {
            int row = i / 5;
            int col = i % 5;
            float x = firstCardPos.x + (col * horizontalGap);
            float y = firstCardPos.y + (row * verticalGap);
            positions.Add(new Vector2(x, y));
        }
        return positions;
    }

    private IEnumerator SpreadCardsOverTime(float delay = 1f, float interval = 0.1f)
    {
        isCardInteractionLocked = true;
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            RectTransform rt = spawnedCards[i].GetComponent<RectTransform>();
            if (rt != null && i < targetPositions.Count)
            {
                rt.DOAnchorPos(targetPositions[i], 0.4f).SetEase(Ease.OutQuad);
            }
            yield return new WaitForSeconds(interval);
        }
        isCardInteractionLocked = false;
    }

    public void StartMultipleRecruit(int count, SpecializationType? fixedType = null)
    {
        currentBatch.Clear();
        currentDrawType = fixedType;
        factory.SetCanRecruit(true);
        StartCoroutine(RecruitMultipleCoroutine(count));
    }

    private IEnumerator RecruitMultipleCoroutine(int count)
    {
        targetPositions = CalculateCardPositions(count);
        spawnedCards.Clear();
        for (int i = 0; i < count; i++)
        {
            TraineeData data = currentDrawType == null
                ? factory.CreateRandomTrainee(bypassRecruitCheck: true)
                : factory.CreateFixedTrainee(currentDrawType.Value, bypassRecruitCheck: true);
            if (data == null) continue;
            SpawnMiniCard(data, i);
        }
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SpreadCardsOverTime(0f, 0.1f));
        foreach (var obj in spawnedCards)
        {
            var controller = obj.GetComponent<TraineeController>();
            controller?.EnableFlip();
        }
    }

    private void ConfirmTrainee(TraineeData data)
    {
        currentBatch.Add(data);
        AddAndReindex(data);
    }

    private void AddAndReindex(TraineeData data)
    {
        runtimeTrainees.Add(data);
        var sameType = runtimeTrainees.FindAll(t => t.Specialization == data.Specialization);
        for (int i = 0; i < sameType.Count; i++)
            sameType[i].SpecializationIndex = i + 1;
    }

    public void RemoveTrainee(GameObject obj, TraineeData data)
    {
        runtimeTrainees.Remove(data);
        activeTrainees.Remove(obj);
        Destroy(obj);
        var sameType = runtimeTrainees.FindAll(t => t.Specialization == data.Specialization);
        for (int i = 0; i < sameType.Count; i++)
            sameType[i].SpecializationIndex = i + 1;
    }

    public void DebugPrintAllTrainees()
    {
        Debug.Log($"[전체 제자 수]: {runtimeTrainees.Count}");
        for (int i = 0; i < runtimeTrainees.Count; i++)
        {
            var t = runtimeTrainees[i];
            Debug.Log($"[{i + 1}] 이름: {t.Name} / 특화: {t.Specialization} / 번호: {t.SpecializationIndex}");
        }
    }

    public List<TraineeData> GetAllTrainees() => runtimeTrainees;
    public List<TraineeData> GetTraineesByType(SpecializationType type) => runtimeTrainees.FindAll(t => t.Specialization == type);
    public List<TraineeData> GetLastDrawResult() => currentBatch;
}