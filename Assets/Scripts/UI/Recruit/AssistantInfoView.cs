using UnityEngine;
using UnityEngine.UI;
using TMPro;

// AssistantInfoView.cs
// 제자(Assistant)의 정보를 UI에 표시하는 역할을 담당하는 뷰 스크립트입니다.
// 아이콘, 성격, 이름, 등급, 특화, 비용, 능력치, 설명 등을 표시하며
// UI 초기화 기능도 함께 제공합니다.

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

    // 제자 데이터를 받아 UI에 반영
    public void SetData(AssistantInstance assistant)
    {
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

        textName.text = assistant.Name;
        textPersonality.text = assistant.Personality?.personalityName ?? "알 수 없음";

        rankIcon.sprite = GetRankSprite(assistant.grade);
        imageSpecialization.sprite = GetSpecializationSprite(assistant.Specialization);

        textWage.text = $"영입 비용 : {assistant.RecruitCost} G";
        textHourlyWage.text = $"시급 : {assistant.Wage} G";

        textAbilityStats.text = GetAbilityStatsString(assistant);

        textDescription.text = assistant.CustomerInfo ?? "";
    }

    // UI 내용을 모두 초기화
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

    // 등급에 따른 아이콘 반환
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

    // 특화 타입에 따른 아이콘 반환
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

    // 능력치 배율 정보를 문자열로 변환
    private string GetAbilityStatsString(AssistantInstance assistant)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var ability in assistant.Multipliers)
        {
            float multiplier = Mathf.Max(ability.Multiplier, 0f);
            float percent = (multiplier - 1f) * 100f;

            if (percent <= 0f)
                sb.AppendLine($"{ability.AbilityName} 증가 : 0%");
            else
                sb.AppendLine($"{ability.AbilityName} 증가 : +{percent:F0}%");
        }
        return sb.ToString();
    }
}
