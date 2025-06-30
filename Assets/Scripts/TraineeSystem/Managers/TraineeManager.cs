using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("애니메이터")]
    [SerializeField] private TraineeCardLayoutAnimator layoutAnimator;

    private TraineeFactory factory;
    private TraineeCardSpawner spawner;

    private List<TraineeData> runtimeTrainees = new();
    private List<GameObject> activeTrainees = new();
    private List<TraineeData> currentBatch = new();
    private List<GameObject> spawnedCards = new();
    private List<Vector2> targetPositions = new();

    private GameObject confirmAllButtonInstance;

    private bool isCardInteractionLocked = false;
    public bool IsCardInteractionLocked => isCardInteractionLocked;

    private bool canRecruit = true;
    private int cardsToConfirm = 0;
    private SpecializationType? currentDrawType = null;

    private void Awake()
    {
        factory = new TraineeFactory(personalityDatabase);
        spawner = new TraineeCardSpawner(largeTraineeCardPrefab, miniTraineeCardPrefab, singleDrawParent, multiDrawParent, factory, this);
    }

    /// <summary>
    /// 랜덤 제자를 생성하고 카드로 출력합니다.
    /// 외부에서 호출하기 위한 함수입니다.
    /// </summary>
    public void RecruitAndSpawnTrainee()
    {
        HandleSingleRecruit(() => factory.CreateRandomTrainee());
    }

    // 버튼 이벤트 연결

    public void OnClickRecruitRandomTrainee() => HandleSingleRecruit(() => factory.CreateRandomTrainee());
    public void OnClickRecruitCraftingTrainee() => HandleSingleRecruit(() => factory.CreateFixedTrainee(SpecializationType.Crafting));
    public void OnClickRecruitEnhancingTrainee() => HandleSingleRecruit(() => factory.CreateFixedTrainee(SpecializationType.Enhancing));
    public void OnClickRecruitSellingTrainee() => HandleSingleRecruit(() => factory.CreateFixedTrainee(SpecializationType.Selling));
    public void OnClickRecruit10Random() => HandleMultiRecruit(10);
    public void OnClickRecruit10Crafting() => HandleMultiRecruit(10, SpecializationType.Crafting);
    public void OnClickRecruit10Enhancing() => HandleMultiRecruit(10, SpecializationType.Enhancing);
    public void OnClickRecruit10Selling() => HandleMultiRecruit(10, SpecializationType.Selling);

    private void HandleSingleRecruit(System.Func<TraineeData> recruitFunc)
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = 1;

        var data = recruitFunc.Invoke();
        if (data == null) return;

        spawner.SpawnLargeCard(data);
        ConfirmTrainee(data);
    }

    private void HandleMultiRecruit(int count, SpecializationType? type = null)
    {
        if (!canRecruit) return;
        canRecruit = false;
        cardsToConfirm = count;
        StartMultipleRecruit(count, type);
    }

    public void StartMultipleRecruit(int count, SpecializationType? fixedType = null)
    {
        currentBatch.Clear();
        currentDrawType = fixedType;
        factory.ResetRecruitLock();
        StartCoroutine(RecruitMultipleCoroutine(count));
    }

    private IEnumerator RecruitMultipleCoroutine(int count)
    {
        targetPositions = layoutAnimator.CalculateCardPositions(count);
        spawnedCards.Clear();

        for (int i = 0; i < count; i++)
        {
            var data = currentDrawType == null
                ? factory.CreateRandomTrainee(true)
                : factory.CreateFixedTrainee(currentDrawType.Value, true);

            if (data == null) continue;

            var card = spawner.SpawnMiniCard(data, i);
            spawnedCards.Add(card);
        }

        yield return StartCoroutine(layoutAnimator.SpreadCardsOverTime(spawnedCards, targetPositions, 0.5f, 0.1f));

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
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnClick_ConfirmAllCards());
            }
        }
    }

    public void OnClick_ConfirmAllCards()
    {
        if (isCardInteractionLocked) return;

        bool allFlipped = true;

        foreach (var card in spawnedCards)
        {
            if (card == null) continue;

            var controller = card.GetComponent<TraineeController>();
            if (controller != null && !controller.IsFlipped)
            {
                allFlipped = false;
                break;
            }
        }

        if (!allFlipped)
        {
            StartCoroutine(FlipAllUnflippedCardsSequentially());
        }
        else
        {
            foreach (var card in new List<GameObject>(spawnedCards))
            {
                if (card == null) continue;

                var controller = card.GetComponent<TraineeController>();
                if (controller != null)
                    controller.OnClick_FrontCard();
            }
        }
    }

    private IEnumerator FlipAllUnflippedCardsSequentially()
    {
        isCardInteractionLocked = true;

        if (confirmAllButtonInstance != null)
        {
            var btn = confirmAllButtonInstance.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
                btn.interactable = false;
        }

        foreach (var card in spawnedCards)
        {
            if (card == null) continue;

            var controller = card.GetComponent<TraineeController>();
            if (controller == null || controller.IsFlipped) continue;

            bool finished = false;
            controller.ForceFlipWithCallback(() => finished = true);
            yield return new WaitUntil(() => finished);
        }

        isCardInteractionLocked = false;

        if (confirmAllButtonInstance != null)
        {
            var btn = confirmAllButtonInstance.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
                btn.interactable = true;
        }
    }

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

    public void ConfirmTrainee(TraineeData data)
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

        if (spawnedCards.Contains(obj))
            spawnedCards.Remove(obj);

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
