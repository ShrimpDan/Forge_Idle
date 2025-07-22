using UnityEngine;
using DG.Tweening;
using System;

public class AssistantPaperAnimator : MonoBehaviour
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 종이를 왼쪽 상단 바깥에서 중앙으로 회전하며 입장시킴
    /// </summary>
    public void AnimateEnterFromTopLeft(float duration = 0.7f, Action onComplete = null)
    {
        rectTransform.anchoredPosition = new Vector2(-Screen.width, Screen.height);
        rectTransform.localRotation = Quaternion.Euler(0, 0, -30f);

        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.OutBack));
        seq.Join(rectTransform.DOLocalRotate(Vector3.zero, duration).SetEase(Ease.OutBack));
        seq.OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 종이를 우측 상단으로 회전시키며 퇴장시키고 오브젝트 제거
    /// </summary>
    public void AnimateExitToTopRight(float distance = 800f, float duration = 0.5f, Action onComplete = null)
    {
        Vector2 exitTarget = new Vector2(Screen.width + distance, Screen.height + distance);

        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOAnchorPos(exitTarget, duration).SetEase(Ease.InBack));
        seq.Join(rectTransform.DOLocalRotate(new Vector3(0, 0, 45f), duration).SetEase(Ease.InBack));
        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }
}
