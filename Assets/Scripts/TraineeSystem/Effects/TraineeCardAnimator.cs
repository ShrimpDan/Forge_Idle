using DG.Tweening;
using UnityEngine;

/// <summary>
/// 카드의 시각적 애니메이션(뒤집기, 소환 연출 등)을 담당하는 클래스입니다.
/// DOTween을 활용하여 위치/회전 애니메이션을 제어합니다.
/// </summary>
public class TraineeCardAnimator : MonoBehaviour
{
    [SerializeField] private GameObject frontSide;
    [SerializeField] private GameObject backSide;

    public void PlaySpawnEffect()
    {
        RectTransform rect = backSide.GetComponent<RectTransform>();
        if (rect == null) return;

        Vector2 basePos = rect.anchoredPosition;
        rect.anchoredPosition = basePos + new Vector2(0, 500f);

        backSide.SetActive(true);
        frontSide.SetActive(false);

        rect.DOAnchorPos(basePos, 0.4f).SetEase(Ease.OutBounce);
    }

    public void FlipCard(System.Action onFlipped = null)
    {
        frontSide.transform.localEulerAngles = new Vector3(0, 270, 0);

        backSide.transform.DORotate(new Vector3(0, 90, 0), 0.3f).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                frontSide.SetActive(true);
                frontSide.transform.DORotate(new Vector3(0, 360, 0), 0.3f).SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        backSide.SetActive(false);
                        onFlipped?.Invoke();
                    });
            });
    }
}
