using TMPro;
using UnityEngine;
using DG.Tweening;

public class GoldChangeEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private CanvasGroup canvasGroup;

    public void Play(int amount, Vector3 screenPos)
    {
        transform.position = screenPos;

        goldText.text = $"{(amount >= 0 ? "+ " : "- ")}{Mathf.Abs(amount)}";
        goldText.color = amount >= 0 ? Color.yellow : Color.red;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(1f);

        if (canvasGroup != null)
            seq.Append(canvasGroup.DOFade(0f, 1f));

        seq.OnComplete(() => Destroy(gameObject));
    }
}
