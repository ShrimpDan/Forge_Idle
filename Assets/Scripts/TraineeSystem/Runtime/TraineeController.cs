using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraineeController : MonoBehaviour
{
    private TraineeData data;
    private TraineeFactory factory;
    private TraineeManager manager;

    [SerializeField] private TraineeCardUI cardUI;
    [SerializeField] private TraineeCardAnimator cardAnimator;

    [SerializeField] private TMP_Text orderText;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button backCardButton;

    private bool isFlipped = false;
    private bool isFlipping = false;
    private bool isPartOfMultipleDraw = false;
    private int currentIndex = 0;

    private System.Action<int> onSkip;
    private System.Action<TraineeData> onConfirm;

    public void Setup(TraineeData traineeData, TraineeFactory traineeFactory, TraineeManager managerRef,
                      int order, System.Action<int> onSkipAction, System.Action<TraineeData> onConfirmAction,
                      bool enableFlipOnStart = false, bool isMultiDrawCard = false)
    {
        data = traineeData;
        factory = traineeFactory;
        manager = managerRef;
        isFlipped = false;
        currentIndex = order;
        onSkip = onSkipAction;
        onConfirm = onConfirmAction;
        isPartOfMultipleDraw = isMultiDrawCard;

        cardUI?.UpdateUI(data);

        if (orderText != null)
            orderText.text = $"{currentIndex + 1}번째 카드";

        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true);
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(() => onSkip?.Invoke(currentIndex));
        }

        if (backCardButton != null)
        {
            backCardButton.interactable = enableFlipOnStart;
            backCardButton.onClick.RemoveAllListeners();
            backCardButton.onClick.AddListener(OnClick_FlipCard);
        }
    }

    public void EnableFlip()
    {
        if (backCardButton != null)
            backCardButton.interactable = true;
    }

    public void OnClick_DeleteSelf()
    {
        Destroy(gameObject);
    }

    public void OnClick_FrontCard()
    {
        onConfirm?.Invoke(data);
        Destroy(gameObject);

        manager?.OnCardConfirmed();
        factory?.SetCanRecruit(true);
    }

    public void OnClick_FlipCard()
    {
        if (isFlipped || isFlipping) return;

        if (manager != null && manager.IsCardInteractionLocked)
        {
            return;
        }

        isFlipping = true;
        cardUI?.UpdateUI(data);

        cardAnimator?.FlipCard(() =>
        {
            isFlipped = true;
            isFlipping = false;
        });
    }

    public void PlaySpawnEffect()
    {
        cardAnimator?.PlaySpawnEffect();
    }

    public void OnClick_LevelUp()
    {
        data.LevelUp();
        cardUI?.UpdateUI(data);
    }
}
