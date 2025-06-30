using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TraineeManager : MonoBehaviour
{
    [Header("카드 프리팹")]
    [SerializeField] private GameObject largeTraineeCardPrefab;
    [SerializeField] private GameObject miniTraineeCardPrefab;

    [Header("버튼 프리팹")]
    [SerializeField] private GameObject confirmAllButtonPrefab;

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

    private GameObject confirmAllButtonInstance;

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

    // 단일 뽑기 버튼

    public void OnClickRecruitRandomTrainee() => HandleSingleRecruit(() => factory.CreateRandomTrainee());
    public void OnClickRecruitCraftingTrainee() => HandleSingleRecruit(() => factory.CreateFixedTrainee(SpecializationType.Crafting));
    public void OnClickRecruitEnhancingTrainee() => HandleSingleRecruit(() => factory.CreateFixedTrainee(SpecializationType.Enhancing));
    public void OnClickRecruitSellingTrainee() => HandleSingleRecruit(() => factory.CreateFixedTrainee(SpecializationType.Selling));

    // 10연속 뽑기 버튼

    public void OnClickRecruit10Random() => HandleMultiRecruit(10);
    public void OnClickRecruit10Crafting() => HandleMultiRecruit(10, SpecializationType.Crafting);
    public void OnClickRecruit10Enhancing() => HandleMultiRecruit(10, SpecializationType.Enhancing);
    public void OnClickRecruit10Selling() => HandleMultiRecruit(10, SpecializationType.Selling);

    // 뽑기 공통 처리 함수

    private void HandleSingleRecruit(System.Func<TraineeData> recruitFunc)
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 1;
        var data = recruitFunc.Invoke();
        if (data == null) return;
        SpawnSingleCard(data);
        ConfirmTrainee(data);
    }

    private void HandleMultiRecruit(int count, SpecializationType? type = null)
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = count;
        StartMultipleRecruit(count, type);
    }

    // 카드 확정 처리 함수

    public void OnCardConfirmed()
    {
        cardsToConfirm--;
        if (cardsToConfirm <= 0)
        {
            canRecruit = true;
            if (confirmAllButtonInstance != null)
                Destroy(confirmAllButtonInstance);
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

    // 카드 생성 함수

    private void SpawnSingleCard(TraineeData data)
    {
        if (largeTraineeCardPrefab == null || singleDrawParent == null) return;
        var obj = Instantiate(largeTraineeCardPrefab, singleDrawParent);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.transform.localScale = Vector3.one;
        SetupCardObject(obj, data, 0, true);
    }

    private void SpawnMiniCard(TraineeData data, int index)
    {
        if (miniTraineeCardPrefab == null || multiDrawParent == null) return;
        var obj = Instantiate(miniTraineeCardPrefab, multiDrawParent);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.transform.localScale = Vector3.one;
        SetupCardObject(obj, data, index, false);
        spawnedCards.Add(obj);
    }

    private void SetupCardObject(GameObject obj, TraineeData data, int index, bool enableFlipImmediately)
    {
        var controller = obj.GetComponent<TraineeController>();
        if (controller == null)
        {
            Destroy(obj);
            return;
        }
        controller.Setup(data, factory, this, index, null, ConfirmTrainee, enableFlipImmediately, true);
        controller.PlaySpawnEffect();
        activeTrainees.Add(obj);
    }

    // 카드 애니메이션 함수

    private List<Vector2> CalculateCardPositions(int count)
    {
        List<Vector2> positions = new();
        Vector2 firstCard = new(-432, 713), sixthCard = new(-432, 419);
        float verticalGap = sixthCard.y - firstCard.y, horizontalGap = 216f;
        for (int i = 0; i < count; i++)
        {
            int row = i / 5, col = i % 5;
            positions.Add(new Vector2(firstCard.x + col * horizontalGap, firstCard.y + row * verticalGap));
        }
        return positions;
    }

    private IEnumerator SpreadCardsOverTime(float delay = 1f, float interval = 0.1f)
    {
        isCardInteractionLocked = true;
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            var rt = spawnedCards[i].GetComponent<RectTransform>();
            if (rt != null && i < targetPositions.Count)
                rt.DOAnchorPos(targetPositions[i], 0.4f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(interval);
        }
        isCardInteractionLocked = false;
    }

    // 10연속 뽑기 시작 처리 함수

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
            var data = currentDrawType == null
                ? factory.CreateRandomTrainee(bypassRecruitCheck: true)
                : factory.CreateFixedTrainee(currentDrawType.Value, bypassRecruitCheck: true);
            if (data == null) continue;
            SpawnMiniCard(data, i);
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(SpreadCardsOverTime(0f, 0.1f));

        foreach (var obj in spawnedCards)
            obj.GetComponent<TraineeController>()?.EnableFlip();

        if (confirmAllButtonPrefab != null)
        {
            confirmAllButtonInstance = Instantiate(confirmAllButtonPrefab, multiDrawParent);
            var rt = confirmAllButtonInstance.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;

            var btn = confirmAllButtonInstance.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
                btn.onClick.AddListener(OnClick_ConfirmAllCards);
        }
    }

    // 확인 버튼 함수

    public void OnClick_ConfirmAllCards()
    {
        bool allFlipped = true;

        foreach (var card in spawnedCards)
        {
            var controller = card.GetComponent<TraineeController>();
            if (controller != null && !controller.IsFlipped)
            {
                controller.ForceFlip();
                allFlipped = false;
            }
        }

        if (allFlipped)
        {
            foreach (var card in new List<GameObject>(spawnedCards))
                card.GetComponent<TraineeController>()?.OnClick_FrontCard();
        }
    }

    // 기타

    public void RecruitAndSpawnTrainee()
    {
        HandleSingleRecruit(() => factory.CreateRandomTrainee());
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
