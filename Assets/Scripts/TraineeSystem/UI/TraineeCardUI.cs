using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TraineeData를 기반으로 제자 카드의 이미지 요소를 갱신합니다.
/// </summary>
public class TraineeCardUI : MonoBehaviour
{
    [Header("아이콘 UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Image tierBadgeImage;
    [SerializeField] private Image typeIconImage;

    [Header("아이콘 매핑")]
    [SerializeField] private Sprite[] tierIcons;
    [SerializeField] private Sprite craftingIcon;
    [SerializeField] private Sprite enhancingIcon;
    [SerializeField] private Sprite sellingIcon;

    [Header("카드 앞 or 뒤 이미지")]
    [SerializeField] private GameObject frontRoot;
    [SerializeField] private GameObject backRoot;

    public void UpdateUI(TraineeData data)
    {
        int tierIndex = data.Personality.Tier - 1;
        if (tierIndex >= 0 && tierIndex < tierIcons.Length)
            tierBadgeImage.sprite = tierIcons[tierIndex];

        typeIconImage.sprite = data.Specialization switch
        {
            SpecializationType.Crafting => craftingIcon,
            SpecializationType.Enhancing => enhancingIcon,
            SpecializationType.Selling => sellingIcon,
            _ => null
        };

        // TODO: 캐릭터 이미지 설정
    }

    public void SetBack()
    {
        frontRoot?.SetActive(false);
        backRoot?.SetActive(true);
    }

    public void SetFront()
    {
        frontRoot?.SetActive(true);
        backRoot?.SetActive(false);
    }
}
