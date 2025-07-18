using UnityEngine;
using UnityEngine.UI;
using System;

public class MineAssistantSlotUI : MonoBehaviour
{
    private MineAssistantSlot slot; // �ھ��� �����ϴ� ���Ը� ������ �Ҵ�
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

    // ���ξ����� Init �ʼ�!
    public void Init(AssistantInventory inv)
    {
        assistantInventory = inv;
    }

    // ���� ������ ���� (�� ���Ը�! �˾� ������ ȣ�� ����)
    public void SetSlot(MineAssistantSlot newSlot)
    {
        slot = newSlot;
        UpdateUI();
    }

    // ��ý���Ʈ �Ҵ� (�� ���Ը� ���)
    public void AssignAssistant(AssistantInstance assistant)
    {
        if (slot == null)
        {
            Debug.LogError("MineAssistantSlotUI: slot is null! AssignAssistant�� ���� ����UI������ ȣ��Ǿ�� �մϴ�.");
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

    // ������ �� UI ����
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

    // �ӽ� ���� UI (�˾������� ���, slot ���� ���� ���)
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
