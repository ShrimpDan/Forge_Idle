using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MineralSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text mineralNameText;
    [SerializeField] private Image mineralIcon;
    [SerializeField] private Button assignAssistantBtn;

    [Header("Assistant")]
    [SerializeField] private GameObject assistantIconRoot;
    [SerializeField] private Image assistantIcon;

    private TraineeData assignedAssistant;
    private Action onClickSlot;

    public void Init(string mineralName, Sprite mineralSprite, Action onClick)
    {
        mineralNameText.text = mineralName;
        mineralIcon.sprite = mineralSprite;
        onClickSlot = onClick;

        assignAssistantBtn.onClick.RemoveAllListeners();
        assignAssistantBtn.onClick.AddListener(() => onClickSlot?.Invoke());

        SetAssistant(null);
    }

    public void SetAssistant(TraineeData assistant)
    {
        assignedAssistant = assistant;
        if (assistant != null)
        {
            assistantIconRoot?.SetActive(true);
            // 아이콘 적용: 현재 구조에선 TraineeData에 IconPath가 없으니, 추후 확장 필요
            // if (assistantIcon != null && !string.IsNullOrEmpty(assistant.IconPath))
            // {
            //     assistantIcon.sprite = IconLoader.GetIcon(assistant.IconPath);
            //     assistantIcon.enabled = true;
            // }
        }
        else
        {
            if (assistantIconRoot != null)
                assistantIconRoot.SetActive(false);
        }
    }
}
