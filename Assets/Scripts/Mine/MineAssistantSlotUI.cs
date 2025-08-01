using UnityEngine;
using UnityEngine.UI;
using System;

public class MineAssistantSlotUI : MonoBehaviour
{
    private MineAssistantSlot slot;
    private AssistantInventory assistantInventory;

    public Image iconImage;
    public Button slotButton;

    // ▼ 등급 아이콘용 필드 추가 (Inspector에서 할당)
    [Header("Rank Icon")]
    [SerializeField] private Image rankIconImage;
    [SerializeField] private Sprite rankN;
    [SerializeField] private Sprite rankR;
    [SerializeField] private Sprite rankSR;
    [SerializeField] private Sprite rankSSR;
    [SerializeField] private Sprite rankUR;

    public Action<MineAssistantSlotUI> OnSlotClicked;

    public bool IsSceneSlot() => slot != null;
    public bool IsTemporarySlot() => slot == null;

    private void Awake()
    {
        if (slotButton != null)
            slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(this));
    }

    public void Init(AssistantInventory inv)
    {
        assistantInventory = inv;
    }

    public void SetSlot(MineAssistantSlot newSlot)
    {
        slot = newSlot;
        UpdateUI();
    }

    public void AssignAssistant(AssistantInstance assistant)
    {
        if (slot == null) return;

        if (assistant != null && assistant.IsInUse)
        {
            slot.Assign(null);
            UpdateUI();
            return;
        }

        if (slot.IsAssigned && slot.AssignedAssistant != null)
            slot.AssignedAssistant.IsInUse = false;

        if (assistant != null)
            assistant.IsInUse = true;

        slot.Assign(assistant);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slot != null && slot.IsAssigned && iconImage != null && slot.AssignedAssistant != null)
        {
            var icon = IconLoader.GetIconByPath(slot.AssignedAssistant.IconPath);
            iconImage.sprite = icon;
            iconImage.enabled = true;
            // 등급 아이콘 표시
            SetRankIcon(slot.AssignedAssistant.grade);
        }
        else if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            // 등급 아이콘 비활성화
            if (rankIconImage != null) rankIconImage.enabled = false;
        }
    }

    public void SetTempAssistant(AssistantInstance assistant, Action<AssistantInstance> onClick)
    {
        if (iconImage != null)
        {
            iconImage.sprite = assistant != null ? IconLoader.GetIconByPath(assistant.IconPath) : null;
            iconImage.enabled = assistant != null;
        }
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => onClick?.Invoke(assistant));
        }
        // 임시 슬롯에도 등급 표시
        SetRankIcon(assistant?.grade);
    }

    private void SetRankIcon(string grade)
    {
        if (rankIconImage == null) return;
        switch (grade)
        {
            case "N": rankIconImage.sprite = rankN; break;
            case "R": rankIconImage.sprite = rankR; break;
            case "SR": rankIconImage.sprite = rankSR; break;
            case "SSR": rankIconImage.sprite = rankSSR; break;
            case "UR": rankIconImage.sprite = rankUR; break;
            default: rankIconImage.sprite = null; break;
        }
        rankIconImage.enabled = rankIconImage.sprite != null;
    }
}
