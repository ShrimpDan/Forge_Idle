using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistantDrawController : MonoBehaviour
{
    [SerializeField] private AssistantCardLayoutAnimator layoutAnimator;
    [SerializeField] private Transform multiDrawParent;
    [SerializeField] private GameObject confirmAllButtonPrefab;
    [SerializeField] private GameObject backgroundPanel;

    private AssistantFactory factory;
    private AssistantCardSpawner spawner;

    private List<GameObject> spawnedCards = new();
    private List<Vector2> targetPositions = new();

    private GameObject confirmAllButtonInstance;
    private int cardsToConfirm = 0;

    private Coroutine cardSpreadCoroutine = null;
    private bool isCardInteractionLocked = false;
    private bool isSpreadingCards = false;
    private bool isFlippingCards = false;

    public bool IsCardInteractionLocked => isCardInteractionLocked;

    public Action<AssistantInstance> OnTraineeConfirmed;
    public event Action OnRecruitingFinished;

    public void Init(AssistantFactory factory, AssistantCardSpawner spawner)
    {
        this.factory = factory;
        this.spawner = spawner;
    }

    public void StartMultipleRecruit(int count, SpecializationType? fixedType = null)
    {
        factory.ResetRecruitLock();
        cardsToConfirm = count;

        StartCoroutine(StartRecruitFlowWithBackground(count, fixedType));
    }

    private IEnumerator StartRecruitFlowWithBackground(int count, SpecializationType? fixedType)
    {
        if (backgroundPanel != null)
            backgroundPanel.SetActive(true);

        // 버튼 미리 생성
        if (confirmAllButtonPrefab != null && confirmAllButtonInstance == null)
        {
            confirmAllButtonInstance = Instantiate(confirmAllButtonPrefab, multiDrawParent);
            var rt = confirmAllButtonInstance.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -400f);
            rt.localScale = Vector3.one;

            var btn = confirmAllButtonInstance.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(OnClick_ConfirmAllCards);
            }
        }

        yield return null;
        yield return StartCoroutine(RecruitMultipleCoroutine(count, fixedType));
    }

    private IEnumerator RecruitMultipleCoroutine(int count, SpecializationType? fixedType)
    {
        targetPositions = layoutAnimator.CalculateCardPositions(count);
        spawnedCards.Clear();

        isSpreadingCards = true;
        isFlippingCards = false;

        for (int i = 0; i < count; i++)
        {
            var data = fixedType == null
                ? factory.CreateRandomTrainee(true)
                : factory.CreateFixedTrainee(fixedType.Value, true);

            if (data == null) continue;

            var card = spawner.SpawnMiniCard(data, i);
            spawnedCards.Add(card);
        }

        cardSpreadCoroutine = StartCoroutine(layoutAnimator.SpreadCardsOverTime(
            spawnedCards, targetPositions, 0.5f, 0.1f
        ));

        yield return cardSpreadCoroutine;
        cardSpreadCoroutine = null;
        isSpreadingCards = false;

        foreach (var obj in spawnedCards)
            obj.GetComponent<AssistantController>()?.EnableFlip();
    }

    private void OnClick_ConfirmAllCards()
    {
        if (isSpreadingCards)
        {
            if (cardSpreadCoroutine != null)
                StopCoroutine(cardSpreadCoroutine);

            for (int i = 0; i < spawnedCards.Count; i++)
            {
                var rect = spawnedCards[i]?.GetComponent<RectTransform>();
                if (rect != null)
                    rect.anchoredPosition = targetPositions[i];
            }

            isSpreadingCards = false;
            StartCoroutine(FlipAllUnflippedCardsSequentially());
            return;
        }

        if (!isFlippingCards && AnyCardUnflipped())
        {
            StartCoroutine(FlipAllUnflippedCardsSequentially());
            return;
        }

        if (isFlippingCards)
        {
            foreach (var card in spawnedCards)
            {
                var controller = card?.GetComponent<AssistantController>();
                if (controller != null && !controller.IsFlipped)
                {
                    controller.ForceInstantFlip();
                }
            }
            return;
        }

        ConfirmAllFlippedCards();
    }

    private IEnumerator FlipAllUnflippedCardsSequentially()
    {
        isFlippingCards = true;
        isCardInteractionLocked = true;

        foreach (var card in spawnedCards)
        {
            var controller = card?.GetComponent<AssistantController>();
            if (controller == null || controller.IsFlipped) continue;

            bool finished = false;
            controller.ForceFlipWithCallback(() => finished = true);
            yield return new WaitUntil(() => finished);
        }

        isFlippingCards = false;
        isCardInteractionLocked = false;
    }

    private void ConfirmAllFlippedCards()
    {
        var remainingCards = new List<GameObject>(spawnedCards);

        foreach (var card in remainingCards)
        {
            if (card == null) continue;

            var controller = card.GetComponent<AssistantController>();
            if (controller != null)
            {
                controller.OnClick_FrontCard();
            }
        }

        spawnedCards.RemoveAll(card => card == null);
    }

    public void ConfirmAllCards() => OnClick_ConfirmAllCards();
    public void OnCardConfirmed() => NotifyCardConfirmed();

    private void NotifyCardConfirmed()
    {
        cardsToConfirm--;
        if (cardsToConfirm <= 0)
        {
            if (confirmAllButtonInstance != null)
                Destroy(confirmAllButtonInstance);

            if (backgroundPanel != null)
                backgroundPanel.SetActive(false);

            OnRecruitingFinished?.Invoke();
        }
    }

    private bool AnyCardUnflipped()
    {
        foreach (var card in spawnedCards)
        {
            if (card == null) continue;

            var controller = card.GetComponent<AssistantController>();
            if (controller != null && !controller.IsFlipped)
                return true;
        }
        return false;
    }
}
