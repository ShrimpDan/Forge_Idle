using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// TraineeData를 기반으로 UI 텍스트 요소(Name, 능력치 등)를 갱신하는 역할을 담당합니다.
/// UI 요소에 직접 접근하여 데이터를 시각적으로 표시합니다.
/// </summary>
public class TraineeCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text specializationText;
    [SerializeField] private TMP_Text personalityText;
    [SerializeField] private TMP_Text abilitiesText;

    public void UpdateUI(TraineeData data)
    {
        nameText.text = data.Name;
        specializationText.text = GetSpecializationKorean(data.Specialization);
        personalityText.text = $"{data.Personality.PersonalityName} (티어 {data.Personality.Tier})";
        abilitiesText.text = GetFormattedAbilities(data);
    }

    private string GetFormattedAbilities(TraineeData data)
    {
        return string.Join("\n", data.Multipliers.Select(m => $"- {m.AbilityName} x{m.Multiplier:F2}"));
    }

    private string GetSpecializationKorean(SpecializationType spec)
    {
        return spec switch
        {
            SpecializationType.Crafting => "제작 특화",
            SpecializationType.Enhancing => "강화 특화",
            SpecializationType.Selling => "판매 특화",
            _ => "알 수 없음"
        };
    }
}
