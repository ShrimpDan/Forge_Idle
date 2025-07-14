using UnityEngine;
using UnityEngine.UI;
using System;

public class MineAssistantSlotUI : MonoBehaviour
{
    private MineAssistantSlot slot;
    private AssistantInventory assistantInventory;

    public Image iconImage;
    public Button slotButton;

    public Action<MineAssistantSlotUI> OnSlotClicked;

    private void Awake()
    {
        if (slotButton != null)
            slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(this));
    }

    // 인벤토리 연동용 (Start 등에서 반드시 호출)
    public void Init(AssistantInventory inv)
    {
        assistantInventory = inv;
    }

    public void SetSlot(MineAssistantSlot newSlot)
    {
        slot = newSlot;
        UpdateUI();
    }

    // 어시스턴트 할당 
    public void AssignAssistant(AssistantInstance assistant)
    {
        // 이전 할당된 어시스턴트는 인벤토리에 반환
        if (slot != null && slot.IsAssigned && slot.AssignedAssistant != null)
        {
            assistantInventory.Add(slot.AssignedAssistant);
        }

        // 새 어시스턴트는 인벤토리에서 제거
        if (assistant != null)
            assistantInventory.Remove(assistant);

        slot.Assign(assistant);
        UpdateUI();
    }

    // 슬롯 해제 (UI에서 별도 버튼 연결시 사용)
    public void UnassignAssistant()
    {
        if (slot != null && slot.IsAssigned && slot.AssignedAssistant != null)
        {
            assistantInventory.Add(slot.AssignedAssistant);
            slot.Assign(null);
            UpdateUI();
        }
    }

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
