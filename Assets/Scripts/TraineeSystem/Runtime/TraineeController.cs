using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개별 제자 카드의 동작을 담당하는 컨트롤러. UI 설정, 뒤집기, 확인, 삭제 등을 처리.
/// </summary>
public class TraineeController : MonoBehaviour
{
    private TraineeData data;
    private TraineeFactory factory;
    private TraineeDrawController drawController;

    [SerializeField] private TraineeCardUI cardUI;
    [SerializeField] private TraineeCardAnimator cardAnimator;

    [SerializeField] private Button backCardButton;

    private bool isFlipped = false;
    private bool isFlipping = false;
    private int currentIndex = 0;
    public bool IsFlipped => isFlipped;

    private System.Action<int> onSkip;
    private System.Action<TraineeData> onConfirm;

    /// <summary>
    /// 카드 설정 및 UI 초기화
    /// </summary>
    public void Setup(TraineeData traineeData, TraineeFactory traineeFactory, TraineeDrawController drawCtrl,
                      int order, System.Action<int> onSkipAction, System.Action<TraineeData> onConfirmAction,
                      bool enableFlipOnStart = false, bool isMultiDrawCard = false)
    {
        data = traineeData;
        factory = traineeFactory;
        drawController = drawCtrl;
        isFlipped = false;
        currentIndex = order;
        onSkip = onSkipAction;
        onConfirm = onConfirmAction;

        cardUI?.UpdateUI(data);
        cardAnimator?.Setup(data);

        if (backCardButton != null)
        {
            backCardButton.interactable = enableFlipOnStart;
            backCardButton.onClick.RemoveAllListeners();
            backCardButton.onClick.AddListener(OnClick_FlipCard);
        }
    }

    /// <summary>
    /// 외부에서 강제로 카드를 뒤집게 하며 완료 콜백 제공
    /// </summary>
    public void ForceFlipWithCallback(System.Action onComplete)
    {
        if (isFlipped || isFlipping)
        {
            onComplete?.Invoke();
            return;
        }

        isFlipping = true;
        cardUI?.UpdateUI(data);

        cardAnimator?.PlayTieredFlip(() =>
        {
            isFlipped = true;
            isFlipping = false;
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 뒤집기 버튼을 사용 가능하게 설정
    /// </summary>
    public void EnableFlip()
    {
        if (backCardButton != null)
            backCardButton.interactable = true;
    }

    /// <summary>
    /// 게임 오브젝트 제거 (직접 삭제)
    /// </summary>
    public void OnClick_DeleteSelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 카드 앞면 클릭 시 실행. 제자 확인 처리 + 카드 제거
    /// </summary>
    public void OnClick_FrontCard()
    {
        if (drawController != null && drawController.IsCardInteractionLocked) return;

        onConfirm?.Invoke(data);
        Destroy(gameObject);
        drawController?.OnCardConfirmed();
        factory?.SetCanRecruit(true);
    }

    /// <summary>
    /// 카드 뒤집기 버튼 클릭 시 실행
    /// </summary>
    public void OnClick_FlipCard()
    {
        if (isFlipped || isFlipping) return;
        if (drawController != null && drawController.IsCardInteractionLocked) return;

        isFlipping = true;
        cardUI?.UpdateUI(data);

        cardAnimator?.PlayTieredFlip(() =>
        {
            isFlipped = true;
            isFlipping = false;
        });
    }

    /// <summary>
    /// 외부에서 강제로 카드 뒤집기 실행 (콜백 없음)
    /// </summary>
    public void ForceFlip()
    {
        if (isFlipped || isFlipping) return;

        isFlipping = true;
        cardUI?.UpdateUI(data);

        cardAnimator?.PlayTieredFlip(() =>
        {
            isFlipped = true;
            isFlipping = false;
        });
    }

    /// <summary>
    /// 카드 등장 연출 재생
    /// </summary>
    public void PlaySpawnEffect()
    {
        cardAnimator?.PlaySpawnEffect();
    }

    /// <summary>
    /// 레벨 업 버튼 클릭 시 실행
    /// </summary>
    public void OnClick_LevelUp()
    {
        data.LevelUp();
        cardUI?.UpdateUI(data);
    }
}
