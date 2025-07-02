using UnityEngine;
using UnityEngine.UI;

public class TraineeCardUI : MonoBehaviour
{
    [Header("아이콘 오브젝트")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private Image tierIcon;
    [SerializeField] private Image specializationIcon;

    [Header("아이콘 스프라이트")]
    [SerializeField] private Sprite[] tierSprites; // 성격 티어 1~5 ( 인덱스 0 ~ 4)
    [SerializeField] private Sprite craftingIcon;
    [SerializeField] private Sprite enhancingIcon;
    [SerializeField] private Sprite sellingIcon;

    public void UpdateUI(TraineeData data)
    {
        characterIcon.sprite = GetRandomCharacterSprite(); // 랜덤 캐릭터 이미지 지정 (추후 구현)

        int tierIndex = Mathf.Clamp(data.Personality.tier - 1, 0, tierSprites.Length - 1);
        tierIcon.sprite = tierSprites[tierIndex];

        specializationIcon.sprite = data.Specialization switch
        {
            SpecializationType.Crafting => craftingIcon,
            SpecializationType.Enhancing => enhancingIcon,
            SpecializationType.Selling => sellingIcon,
            _ => null
        };
    }

    private Sprite GetRandomCharacterSprite()
    {
        return null;
    }
}
