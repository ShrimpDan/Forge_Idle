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

        if (slot.IsAssigned && slot.AssignedAssistant != null)
        {
            assistantInventory.Add(slot.AssignedAssistant);
        }
        if (assistant != null)
            assistantInventory.Remove(assistant);

        slot.Assign(assistant);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slot != null && slot.IsAssigned && iconImage != null && slot.AssignedAssistant != null)
        {
            // 변경: GetIcon -> GetIconByPath
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
        if (iconImage != null && assistant != null)
        {
            // 변경: GetIcon -> GetIconByPath
            var icon = IconLoader.GetIconByPath(assistant.IconPath);
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
