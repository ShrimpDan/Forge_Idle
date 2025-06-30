using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제자 뽑기(단일, 다중), 연출 실행 및 버튼 처리 등을 담당하는 컨트롤러입니다.
/// </summary>
public class TraineeDrawController : MonoBehaviour
{
    [SerializeField] private TraineeCardLayoutAnimator layoutAnimator;
    [SerializeField] private Transform multiDrawParent;
    [SerializeField] private GameObject confirmAllButtonPrefab;

    private TraineeFactory factory;
    private TraineeCardSpawner spawner;

    private List<GameObject> spawnedCards = new();
    private List<Vector2> targetPositions = new();

    private bool isCardInteractionLocked = false;
    public bool IsCardInteractionLocked => isCardInteractionLocked;

    private GameObject confirmAllButtonInstance;
    private int cardsToConfirm = 0;

    public Action<TraineeData> OnTraineeConfirmed;
    public event Action OnRecruitingFinished;

    /// <summary>
    /// 컨트롤러 초기화: 팩토리와 스포너 설정
    /// </summary>
    public void Init(TraineeFactory factory, TraineeCardSpawner spawner)
    {
        this.factory = factory;
        this.spawner = spawner;
    }

    /// <summary>
    /// 다중 제자 뽑기 시작. 뽑을 카드 수와 고정 직업 타입(optional)을 설정함.
    /// </summary>
    public void StartMultipleRecruit(int count, SpecializationType? fixedType = null)
    {
        factory.ResetRecruitLock();
        cardsToConfirm = count;
        StartCoroutine(RecruitMultipleCoroutine(count, fixedType));
    }

    /// <summary>
    /// 여러 장의 제자 카드를 생성하고, 애니메이션으로 배치하고, 뒤집기 활성화 및 확인 버튼을 생성하는 코루틴
    /// </summary>
    private IEnumerator RecruitMultipleCoroutine(int count, SpecializationType? fixedType)
    {
        targetPositions = layoutAnimator.CalculateCardPositions(count);
        spawnedCards.Clear();

        for (int i = 0; i < count; i++)
        {
            var data = fixedType == null
                ? factory.CreateRandomTrainee(true)
                : factory.CreateFixedTrainee(fixedType.Value, true);

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
                btn.onClick.AddListener(OnClick_ConfirmAllCards);
            }
        }
    }

    /// <summary>
    /// "확인" 버튼 클릭 시 호출, 카드가 모두 뒤집혔는지 확인 후 처리 분기
    /// </summary>
    private void OnClick_ConfirmAllCards()
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
                controller?.OnClick_FrontCard();
            }
        }
    }

    /// <summary>
    /// 뒤집지 않은 카드들을 순차적으로 자동 뒤집기 처리하는 코루틴
    /// </summary>
    private IEnumerator FlipAllUnflippedCardsSequentially()
    {
        isCardInteractionLocked = true;
        SetConfirmButtonInteractable(false);

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
        SetConfirmButtonInteractable(true);
    }

    /// <summary>
    /// "전체 확인" 버튼의 활성화 여부를 설정합니다.
    /// </summary>
    private void SetConfirmButtonInteractable(bool interactable)
    {
        if (confirmAllButtonInstance != null)
        {
            var btn = confirmAllButtonInstance.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
                btn.interactable = interactable;
        }
    }

    /// <summary>
    /// 개별 카드가 확인될 때 호출되며, 모든 카드가 확인되었는지 검사 후 최종 종료 처리
    /// </summary>
    public void NotifyCardConfirmed()
    {
        cardsToConfirm--;
        if (cardsToConfirm <= 0)
        {
            if (confirmAllButtonInstance != null)
                Destroy(confirmAllButtonInstance);

            OnRecruitingFinished?.Invoke();
        }
    }

    /// <summary>
    /// 외부에서 전체 카드 확인을 강제로 트리거함 (버튼 클릭 시 사용)
    /// </summary>
    public void ConfirmAllCards()
    {
        OnClick_ConfirmAllCards();
    }

    /// <summary>
    /// 외부에서 개별 카드가 확인되었음을 알림
    /// </summary>
    public void OnCardConfirmed()
    {
        NotifyCardConfirmed();
    }
}
