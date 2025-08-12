using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public static class UIEffect
{
    public static void TextScaleEffect(TMP_Text text, float targetScale, float duration)
    {
        text.rectTransform.DOScale(targetScale, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public static void PopupOpenEffect(RectTransform panel, float duration)
    {
        panel.localScale = Vector3.zero;
        panel.gameObject.SetActive(true);
        panel.DOScale(1f, duration).SetEase(Ease.OutBack);
    }

    public static void PopupCloseEffect(RectTransform panel, float duration, Action onCompleted = null)
    {
        panel.DOScale(0f, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                panel.gameObject.SetActive(false);
                onCompleted?.Invoke();
            });
    }
}
