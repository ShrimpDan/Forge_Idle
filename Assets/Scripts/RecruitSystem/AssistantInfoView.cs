using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AssistantInfoView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image imageIcon;
    [SerializeField] private Image rankIcon;
    [SerializeField] private Image imageSpecialization;
    [SerializeField] private TMP_Text textPersonality;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textWage;
    [SerializeField] private TMP_Text textHourlyWage;
    [SerializeField] private TMP_Text textAbilityStats;
    [SerializeField] private TMP_Text textDescription;

    [Header("데이터 참조용")]
    [SerializeField] private Sprite[] rankIcons;
    [SerializeField] private Sprite[] specializationIcons; 

    public void SetData(AssistantInstance assistant)
    {
        // 아이콘 로딩
        if (!string.IsNullOrEmpty(assistant.IconPath))
        {
            var sprite = Resources.Load<Sprite>(assistant.IconPath);
            if (sprite != null)
            {
                imageIcon.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"[AssistantInfoView] 아이콘 경로 오류: '{assistant.IconPath}' 에 해당하는 스프라이트를 찾을 수 없습니다.");
            }
        }

        // 이름 및 성격
        textName.text = assistant.Name;
        textPersonality.text = assistant.Personality?.Name;

        // 등급 아이콘
        rankIcon.sprite = GetRankSprite(assistant.grade);

        // 특화 아이콘
        imageSpecialization.sprite = GetSpecializationSprite(assistant.Specialization);

        // 비용
        var wageData = assistant.WageData;
        textWage.text = $"영입 비용 : {assistant.RecruitCost} G";
        textHourlyWage.text = $"시급 : {assistant.Wage} G";

        // 능력 배율
        textAbilityStats.text = GetAbilityStatsString(assistant);

        // 설명
        textDescription.text = assistant.CustomerInfo ?? "";
    }

    private Sprite GetRankSprite(string grade)
    {
        return grade switch
        {
            "UR" => rankIcons[0],
            "SSR" => rankIcons[1],
            "SR" => rankIcons[2],
            "R" => rankIcons[3],
            "N" => rankIcons[4],
            _ => null
        };
    }

    private Sprite GetSpecializationSprite(SpecializationType type)
    {
        return type switch
        {
            SpecializationType.Crafting => specializationIcons.Length > 0 ? specializationIcons[0] : null,
            SpecializationType.Mining => specializationIcons.Length > 1 ? specializationIcons[1] : null,
            SpecializationType.Selling => specializationIcons.Length > 2 ? specializationIcons[2] : null,
            _ => null
        };
    }

    private string GetAbilityStatsString(AssistantInstance assistant)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var ability in assistant.Multipliers)
        {
            sb.AppendLine($"{ability.AbilityName} 증가 : x{ability.Multiplier:F2}");
        }
        return sb.ToString();
    }

    public void ClearView()
    {
        imageIcon.sprite = null;
        rankIcon.sprite = null;
        imageSpecialization.sprite = null;
        textName.text = "";
        textPersonality.text = "";
        textWage.text = "";
        textHourlyWage.text = "";
        textAbilityStats.text = "";
        textDescription.text = "";
    }
}
