using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 합성 슬롯 하나를 구성하고, 클릭 시 해당 제자를 선택 또는 제거할 수 있도록 함.
/// </summary>
public class FusionSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text specializationText;

    private AssistantInstance currentData;
    private System.Action onClicked;

    public AssistantInstance Data => currentData;
    public System.Action OnClick => onClicked;

    private void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnSlotClicked);
        }
    }

    /// <summary>
    /// 제자 데이터를 슬롯에 세팅하고, 클릭 시 호출할 콜백을 등록
    /// </summary>
    public void SetData(AssistantInstance data, System.Action onClicked = null)
    {
        currentData = data;
        this.onClicked = onClicked;

        if (data == null)
        {
            nameText.text = "비어 있음";
            specializationText.text = "";
            return;
        }

        nameText.text = data.Name;
        specializationText.text = GetKoreanSpecialization(data.Specialization);
    }

    /// <summary>
    /// 클릭 시 호출됨. 등록된 콜백이 있다면 실행.
    /// </summary>
    private void OnSlotClicked()
    {
        onClicked?.Invoke();
    }

    /// <summary>
    /// 슬롯 초기화
    /// </summary>
    public void Clear()
    {
        SetData(null, null);
    }

    /// <summary>
    /// 특화 타입을 한글로 반환
    /// </summary>
    public static string GetKoreanSpecialization(SpecializationType type)
    {
        return type switch
        {
            SpecializationType.Crafting => "제작 특화",
            SpecializationType.Mining => "채광 특화",
            SpecializationType.Selling => "판매 특화",
            _ => "알 수 없음"
        };
    }
}
