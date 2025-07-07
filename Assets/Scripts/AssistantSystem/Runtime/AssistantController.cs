using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개별 제자 카드의 동작을 담당하는 컨트롤러. UI 설정, 뒤집기, 확인, 삭제 등을 처리.
/// </summary>
public class AssistantController : MonoBehaviour
{
    private AssistantInstance data;
    private AssistantFactory factory;
    private AssistantDrawController drawController;

    [SerializeField] private AssistantCardUI cardUI;
    [SerializeField] private TraineeCardAnimator cardAnimator;
    [SerializeField] private Button backCardButton;

    private bool isFlipped = false;
    private bool isFlipping = false;

    public bool IsFlipped => isFlipped;

    private System.Action<AssistantInstance> onConfirm;

    /// <summary>
    /// 카드 설정 및 UI 초기화
    /// </summary>
    public void Setup(
        AssistantInstance assistantData,
        AssistantFactory assistantFactory,
        AssistantDrawController drawCtrl,
        System.Action<AssistantInstance> onConfirmAction,
        bool enableFlipOnStart = false,
        bool isMultiDrawCard = false,
        bool playSpawnEffect = true)
    {
        data = assistantData;
        factory = assistantFactory;
        drawController = drawCtrl;
        onConfirm = onConfirmAction;
        isFlipped = false;

        // UI 및 애니메이터 초기화
        cardUI?.UpdateUI(data);
        cardAnimator?.Setup(data);

        if (backCardButton != null)
        {
            backCardButton.interactable = enableFlipOnStart;
            backCardButton.onClick.RemoveAllListeners();
            backCardButton.onClick.AddListener(OnClick_FlipCard);
        }

        if (playSpawnEffect)
            StartCoroutine(DelayedSpawnEffect());
    }

    /// <summary>
    /// 카드 등장 연출을 1프레임 뒤에 실행
    /// </summary>
    private IEnumerator DelayedSpawnEffect()
    {
        yield return null;
        PlaySpawnEffect();
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
    /// 외부에서 연출 없이 즉시 앞면으로 강제 Flip
    /// </summary>
    public void ForceInstantFlip()
    {
        if (isFlipped) return;

        isFlipped = true;
        isFlipping = false;

        cardUI?.UpdateUI(data);
        cardAnimator?.FlipCard();
    }

    /// <summary>
    /// 카드 뒤집기 버튼을 사용 가능하게 설정
    /// </summary>
    public void EnableFlip()
    {
        if (backCardButton != null)
            backCardButton.interactable = true;
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
