using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public static class UIEffect
{
    /// 텍스트 확대/축소 효과 (무한 반복)
    /// 사용예시: UIEffect.TextScaleEffect(Text, 타겟크기, 한사이클당 지속시간);

    public static void TextScaleEffect(TMP_Text text, float targetScale, float duration)
    {
        text.rectTransform.DOScale(targetScale, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }


    /// 팝업이 open효과
    /// UIEffect.PopupOpenEffect(Panel, 효과 시간 );
    
    public static void PopupOpenEffect(RectTransform panel, float duration)
    {
        panel.localScale = Vector3.zero;
        panel.gameObject.SetActive(true);
        panel.DOScale(1f, duration).SetEase(Ease.OutBack);
    }

    /// 팝업이 close 효과
    /// 사용예시: UIEffect.PopupCloseEffect(Panel, 효과 시간 );
    public static void PopupCloseEffect(RectTransform panel, float duration)
    {
        panel.DOScale(0f, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => panel.gameObject.SetActive(false));
    }
}
