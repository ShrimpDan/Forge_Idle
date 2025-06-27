using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraineeController : MonoBehaviour
{
    private TraineeData data;
    private TraineeFactory factory;

    [SerializeField] private TraineeCardUI cardUI;
    [SerializeField] private TraineeCardAnimator cardAnimator;

    [SerializeField] private TMP_Text orderText;
    [SerializeField] private Button skipButton;

    private bool isFlipped = false;
    private bool isFlipping = false;
    private int currentIndex = 0;

    private System.Action<int> onSkip;
    private System.Action<TraineeData> onConfirm;

    public void Setup(TraineeData traineeData, TraineeFactory traineeFactory, int order,
                      System.Action<int> onSkipAction, System.Action<TraineeData> onConfirmAction)
    {
        data = traineeData;
        factory = traineeFactory;
        isFlipped = false;
        currentIndex = order;
        onSkip = onSkipAction;
        onConfirm = onConfirmAction;

        cardUI?.UpdateUI(data);

        if (orderText != null)
            orderText.text = $"{currentIndex + 1}번째 카드";

        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true);
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(() => onSkip?.Invoke(currentIndex));
        }
    }

    public void OnClick_DeleteSelf()
    {
        Destroy(gameObject);
    }

    public void OnClick_FrontCard()
    {
        onConfirm?.Invoke(data);
        Destroy(gameObject);
        factory?.SetCanRecruit(true);
    }

    public void OnClick_FlipCard()
    {
        if (isFlipped || isFlipping) return;

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
