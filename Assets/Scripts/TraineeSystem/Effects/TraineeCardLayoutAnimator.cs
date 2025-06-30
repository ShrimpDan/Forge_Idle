using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 여러 제자 카드를 화면에 펼쳐 배치하는 애니메이션을 담당하는 클래스입니다.
/// 카드 위치 계산과 DOTween 이동을 순차적으로 처리합니다.
/// </summary>
public class TraineeCardLayoutAnimator : MonoBehaviour
{
    /// <summary>
    /// 카드 개수에 따라 배치될 위치들을 계산합니다. 2줄 5칸 고정 배치입니다.
    /// </summary>
    public List<Vector2> CalculateCardPositions(int count)
    {
        List<Vector2> positions = new();
        Vector2 firstCard = new(-432, 713), sixthCard = new(-432, 419);
        float verticalGap = sixthCard.y - firstCard.y, horizontalGap = 216f;

        for (int i = 0; i < count; i++)
        {
            int row = i / 5, col = i % 5;
            positions.Add(new Vector2(firstCard.x + col * horizontalGap, firstCard.y + row * verticalGap));
        }

        return positions;
    }

    /// <summary>
    /// 카드들을 일정 시간 간격으로 순차적으로 이동시킵니다.
    /// </summary>
    public IEnumerator SpreadCardsOverTime(List<GameObject> cards, List<Vector2> targetPositions, float delay = 1f, float interval = 0.1f)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < cards.Count; i++)
        {
            var rt = cards[i].GetComponent<RectTransform>();
            if (rt != null && i < targetPositions.Count)
                rt.DOAnchorPos(targetPositions[i], 0.4f).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(interval);
        }
    }
}
