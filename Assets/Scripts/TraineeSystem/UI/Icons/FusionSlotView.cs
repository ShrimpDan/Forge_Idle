using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FusionSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text specializationText;

    private TraineeData currentData;
    private Action onClicked;

    public TraineeData Data => currentData;
    public Action OnClick => onClicked;

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
    public void SetData(TraineeData data, Action onClicked = null)
    {
        currentData = data;
        this.onClicked = onClicked;

        if (data == null)
        {
            nameText.text = "비어 있음";
            specializationText.text = "";
        }
        else
        {
            nameText.text = data.Name;
            specializationText.text = GetKoreanSpecialization(data.Specialization);
        }
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
}
