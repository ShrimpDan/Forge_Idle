using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class AssistantIconView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text specializationText;

    private AssistantInstance data;
    private Action onClick;

    private Button button;

    public AssistantInstance Data => data;

    public void Init(AssistantInstance assistant, Action onClickCallback)
    {
        data = assistant;
        onClick = onClickCallback;

        nameText.text = assistant.Name;
        specializationText.text = GetKoreanSpecialization(assistant.Specialization);

        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke());
        }

        SetSelected(false);
    }

    private string GetKoreanSpecialization(SpecializationType type)
    {
        return type switch
        {
            SpecializationType.Crafting => "제작 특화",
            SpecializationType.Enhancing => "강화 특화",
            SpecializationType.Selling => "판매 특화",
            _ => "알 수 없음"
        };
    }

    /// <summary>
    /// 외부에서 선택 상태를 표시할 수 있게 하는 함수
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        if (iconImage != null)
        {
            iconImage.color = isSelected ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);
        }
    }
}
