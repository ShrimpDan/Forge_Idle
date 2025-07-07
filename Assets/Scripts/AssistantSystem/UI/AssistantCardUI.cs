using UnityEngine;
using UnityEngine.UI;

public class AssistantCardUI : MonoBehaviour
{
    [Header("아이콘 오브젝트")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private Image tierIcon;
    [SerializeField] private Image specializationIcon;

    [Header("아이콘 스프라이트")]
    [SerializeField] private Sprite[] tierSprites; // 성격 티어 1~5 (인덱스 0~4)
    [SerializeField] private Sprite craftingIcon;
    [SerializeField] private Sprite enhancingIcon;
    [SerializeField] private Sprite sellingIcon;

    public void UpdateUI(AssistantInstance data)
    {
        if (!string.IsNullOrEmpty(data.Personality.Key))
        {
            string iconPath = data.Personality.Key;
            characterIcon.sprite = LoadCharacterIcon(data);
        }

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

    private Sprite LoadCharacterIcon(AssistantInstance data)
    {
        string assumedIconPath = $"Icons/{data.Name}";
        return Resources.Load<Sprite>(assumedIconPath);
    }
}
