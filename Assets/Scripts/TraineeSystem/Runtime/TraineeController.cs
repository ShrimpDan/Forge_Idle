using UnityEngine;

public class TraineeController : MonoBehaviour
{
    private TraineeData data;
    private TraineeFactory factory;

    [SerializeField] private TraineeCardUI cardUI;
    [SerializeField] private TraineeCardAnimator cardAnimator;

    private bool isFlipped = false;
    private bool isFlipping = false;

    private System.Action<TraineeController> onClickCard;

    public bool IsFlipped => isFlipped;

    public void Setup(TraineeData data, TraineeFactory factory, System.Action<TraineeController> onClickCard)
    {
        this.data = data;
        this.factory = factory;
        this.onClickCard = onClickCard;

        isFlipped = false;
        isFlipping = false;
        cardUI?.SetBack();
    }

    public void OnClick_Card()
    {
        if (isFlipped || isFlipping) return;
        onClickCard?.Invoke(this);
    }

    public void FlipImmediately()
    {
        if (isFlipped || isFlipping) return;

        isFlipping = true;

        cardAnimator?.FlipCard(() =>
        {
            isFlipped = true;
            isFlipping = false;

            cardUI?.UpdateUI(data);
            cardUI?.SetFront();
        });
    }
}
