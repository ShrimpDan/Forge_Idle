using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FusionAnimator : MonoBehaviour
{
    [Header("슬롯이 모일 중심 위치")]
    [SerializeField] private RectTransform fusionCenterPoint;

    [Header("연출 설정")]
    public float mergeDuration = 1.0f;
    public float radius = 150f;
    public int spiralTurns = 2;

    public void PlayMergeAnimation(List<FusionSlotView> slotViews, Action onComplete = null)
    {
        if (slotViews == null || slotViews.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        if (fusionCenterPoint == null)
        {
            Debug.LogError("[FusionAnimator] fusionCenterPoint가 설정되지 않았습니다.");
            onComplete?.Invoke();
            return;
        }

        Vector3 localCenter = fusionCenterPoint.localPosition + new Vector3(0, -300f, 0);

        for (int i = 0; i < slotViews.Count; i++)
        {
            var slot = slotViews[i];
            var rect = slot.transform as RectTransform;
            if (rect == null) continue;

            int points = 30;
            Vector3[] path = new Vector3[points];
            for (int j = 0; j < points; j++)
            {
                float t = j / (float)(points - 1);
                float angle = Mathf.Lerp(0, spiralTurns * 360f, t) * Mathf.Deg2Rad;
                float r = Mathf.Lerp(radius, 0, t);
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * r;
                path[j] = localCenter + offset;
            }

            rect.DOLocalPath(path, mergeDuration, PathType.CatmullRom)
                .SetEase(Ease.InOutSine);

            rect.localScale = Vector3.one * 0.6f;
            rect.DOScale(1.3f, mergeDuration).SetEase(Ease.OutBack);

            rect.DOLocalRotate(new Vector3(0, 0, -360f * spiralTurns), mergeDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);
        }

        DOVirtual.DelayedCall(mergeDuration, () => onComplete?.Invoke());
    }

    public void PlayEmphasizeEmptySlots(List<FusionSlotView> slotViews)
    {
        if (slotViews == null || slotViews.Count == 0)
            return;

        foreach (var slot in slotViews)
        {
            if (slot.Data == null)
            {
                var rect = slot.transform as RectTransform;
                if (rect == null) continue;

                rect.DOPunchScale(Vector3.one * 0.25f, 0.4f, 6, 0.8f);
            }
        }
    }
}
