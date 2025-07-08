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

    private AssistantInstance assignedAssistant;
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

    public void SetAssistant(AssistantInstance assistant)
    {
        assignedAssistant = assistant;
        if (assistant != null)
        {
            assistantIconRoot?.SetActive(true);
            if (assistantIcon != null)
            {
                string iconPath = assistant?.IconPath;
                assistantIcon.sprite = !string.IsNullOrEmpty(iconPath)
                    ? IconLoader.GetIcon(iconPath)
                    : null;
                assistantIcon.enabled = assistantIcon.sprite != null;
            }
        }
        else
        {
            if (assistantIconRoot != null)
                assistantIconRoot.SetActive(false);
        }
    }

}
