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
        }
        else if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
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
    }
}
