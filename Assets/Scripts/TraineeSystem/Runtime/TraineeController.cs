using UnityEngine;


/// <summary>
/// TraineeData를 받아 카드 오브젝트를 초기화하고, 
/// 카드 삭제 및 카드 뒤집기 등 주요 사용자 상호작용을 처리하는 컨트롤러입니다.
/// </summary>/// <summary>
public class TraineeController : MonoBehaviour
{
    private TraineeData data;
    private TraineeManager manager;

    [SerializeField] private TraineeCardUI cardUI;
    [SerializeField] private TraineeCardAnimator cardAnimator;

    private bool isFlipped = false;

    public void Setup(TraineeData traineeData, TraineeManager traineeManager)
    {
        data = traineeData;
        manager = traineeManager;
        isFlipped = false;

        cardUI?.UpdateUI(data);
    }

    public void OnClick_DeleteSelf()
    {
        manager?.RemoveTrainee(gameObject, data);
    }

    public void OnClick_FrontCard()
    {
        Destroy(gameObject);
        manager?.SetCanRecruit(true);
    }

    public void OnClick_FlipCard()
    {
        if (isFlipped) return;

        var oneTimeTrainee = manager?.GenerateOneTimeTrainee();
        if (oneTimeTrainee == null) return;

        Setup(oneTimeTrainee, manager);
        cardUI?.UpdateUI(oneTimeTrainee);

        cardAnimator?.FlipCard(() =>
        {
            isFlipped = true;
        });
    }

    public void PlaySpawnEffect()
    {
        cardAnimator?.PlaySpawnEffect();
    }
}
