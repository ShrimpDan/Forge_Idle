using UnityEngine;
using UnityEngine.UI;
using System;

public class MineAssistantSlotUI : MonoBehaviour
{
    private MineAssistantSlot slot; // ★씬에 존재하는 슬롯만 데이터 할당
    private AssistantInventory assistantInventory;

    public Image iconImage;
    public Button slotButton;

    public Action<MineAssistantSlotUI> OnSlotClicked;
    public bool IsSceneSlot() => slot != null;

    public bool IsTemporarySlot()
    {
        return slot == null;
    }

    private void Awake()
    {
        if (slotButton != null)
            slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(this));
    }

    // 마인씬에서 Init 필수!
    public void Init(AssistantInventory inv)
    {
        assistantInventory = inv;
    }

    // 슬롯 데이터 세팅 (씬 슬롯만! 팝업 슬롯은 호출 금지)
    public void SetSlot(MineAssistantSlot newSlot)
    {
        slot = newSlot;
        UpdateUI();
    }

    // 어시스턴트 할당 (씬 슬롯만 사용)
    public void AssignAssistant(AssistantInstance assistant)
    {
        if (slot == null)
        {
            Debug.LogError("MineAssistantSlotUI: slot is null! AssignAssistant는 실제 슬롯UI에서만 호출되어야 합니다.");
            return;
        }
        if (slot.IsAssigned && slot.AssignedAssistant != null)
        {
            assistantInventory.Add(slot.AssignedAssistant);
        }
        if (assistant != null)
            assistantInventory.Remove(assistant);

        slot.Assign(assistant);
        UpdateUI();
    }

    // 아이콘 등 UI 갱신
    public void UpdateUI()
    {
        if (slot != null && slot.IsAssigned && iconImage != null && slot.AssignedAssistant != null)
        {
            var icon = IconLoader.GetIcon(slot.AssignedAssistant.IconPath);
            iconImage.sprite = icon;
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    // 임시 슬롯 UI (팝업에서만 사용, slot 세팅 없이 사용)
    public void SetTempAssistant(AssistantInstance assistant, Action<AssistantInstance> onClick)
    {
        if (iconImage != null && assistant != null)
        {
            var icon = IconLoader.GetIcon(assistant.IconPath);
            iconImage.sprite = icon;
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => onClick?.Invoke(assistant));
        }
    }
}
