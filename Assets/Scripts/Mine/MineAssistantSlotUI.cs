using UnityEngine;
using UnityEngine.UI;
using System;

public class MineAssistantSlotUI : MonoBehaviour
{
    private MineAssistantSlot slot;
    public Image iconImage;
    public Button slotButton;

    public Action<MineAssistantSlotUI> OnSlotClicked;

    private void Awake()
    {
        if (slotButton != null)
            slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(this));
    }

    public void SetSlot(MineAssistantSlot newSlot)
    {
        slot = newSlot;
        UpdateUI();
    }

    public void AssignAssistant(AssistantInstance assistant)
    {
        slot.Assign(assistant);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slot != null && slot.IsAssigned && iconImage != null)
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
}
