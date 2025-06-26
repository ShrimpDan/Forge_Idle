using UnityEngine;

public class TraineeController : MonoBehaviour
{
    private TraineeData data;
    private TraineeFactory factory;

    [SerializeField] private TraineeCardUI cardUI;
    [SerializeField] private TraineeCardAnimator cardAnimator;

    private bool isFlipped = false;
    private bool isFlipping = false;

    public void Setup(TraineeData traineeData, TraineeFactory traineeFactory)
    {
        data = traineeData;
        factory = traineeFactory;
        isFlipped = false;

        cardUI?.UpdateUI(data);
    }

    public void OnClick_DeleteSelf()
    {
        Destroy(gameObject);
    }

    public void OnClick_FrontCard()
    {
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