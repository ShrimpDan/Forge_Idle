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

    // �κ��丮 ������ (Start ��� �ݵ�� ȣ��)
    public void Init(AssistantInventory inv)
    {
        assistantInventory = inv;
    }

    public void SetSlot(MineAssistantSlot newSlot)
    {
        slot = newSlot;
        UpdateUI();
    }

    // ��ý���Ʈ �Ҵ� 
    public void AssignAssistant(AssistantInstance assistant)
    {
        // ���� �Ҵ�� ��ý���Ʈ�� �κ��丮�� ��ȯ
        if (slot != null && slot.IsAssigned && slot.AssignedAssistant != null)
        {
            assistantInventory.Add(slot.AssignedAssistant);
        }

        // �� ��ý���Ʈ�� �κ��丮���� ����
        if (assistant != null)
            assistantInventory.Remove(assistant);

        slot.Assign(assistant);
        UpdateUI();
    }

    // ���� ���� (UI���� ���� ��ư ����� ���)
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
