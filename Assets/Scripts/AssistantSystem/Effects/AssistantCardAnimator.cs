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

    private AssistantInstance data;
    private RectTransform backRect;

    private void Awake()
    {
        backRect = backSide.GetComponent<RectTransform>();
    }

    public void Setup(AssistantInstance assistantData)
    {
        data = assistantData;
    }

    public void PlaySpawnEffect()
    {
        if (backRect == null) return;

        Vector2 basePos = backRect.anchoredPosition;
        backRect.anchoredPosition = basePos + new Vector2(0, 500f);

        backSide.SetActive(true);
        frontSide.SetActive(false);

        backRect.DOAnchorPos(basePos, 0.4f).SetEase(Ease.OutBounce);
    }

    /// <summary>
    /// 티어에 따라 다른 연출 → 기존 Flip 실행
    /// </summary>
    public void PlayTieredFlip(System.Action onFlipped = null)
    {
        if (data == null)
        {
            FlipCard(onFlipped);
            return;
        }

        switch ((AssistantTier)data.Personality.tier)
        {
            case AssistantTier.Tier1:
                FlipWithShakeAndPop(onFlipped);
                break;
            case AssistantTier.Tier2:
                FlipWithShake(onFlipped);
                break;
            default:
                FlipCard(onFlipped);
                break;
        }
    }

    private void FlipWithShake(System.Action onFlipped)
    {
        SoundManager.Instance.Play("SFX_CardFlipRevealSSR");

        backRect.DOShakeRotation(0.77f, new Vector3(0, 0, 20f), 20, 90)
            .OnComplete(() => FlipCard(onFlipped));
    }

    private void FlipWithShakeAndPop(System.Action onFlipped)
    {
        SoundManager.Instance.Play("SFX_CardFlipRevealUR");

        Sequence seq = DOTween.Sequence();
        seq.Append(backRect.DOShakeRotation(0.77f, new Vector3(0, 0, 20f), 20, 90));
        seq.Append(backRect.DOScale(0.8f, 0.2f));
        seq.Append(backRect.DOScale(1.6f, 0.12f));
        seq.Append(backRect.DOScale(1f, 0.1f));
        seq.AppendCallback(() => FlipCard(onFlipped));
    }

    /// <summary>
    /// 카드 뒤집기 연출
    /// </summary>
    public void FlipCard(System.Action onFlipped = null)
    {
        frontSide.transform.localEulerAngles = new Vector3(0, 270, 0);

        backSide.transform.DORotate(new Vector3(0, 90, 0), 0.13f).SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                frontSide.SetActive(true);
                frontSide.transform.DORotate(new Vector3(0, 360, 0), 0.13f).SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        backSide.SetActive(false);
                        onFlipped?.Invoke();
                    });
            });
    }
}
