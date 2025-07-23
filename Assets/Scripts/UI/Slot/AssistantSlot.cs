using UnityEngine;
using UnityEngine.UI;
using System;

public class AssistantSlot : MonoBehaviour
{
    private UIManager uIManager;
    public AssistantInstance AssistantData { get; private set; }
    [SerializeField] private Image icon;
    [SerializeField] private Button slotBtn;
    [SerializeField] private GameObject equippedIndicator;
    private Action<AssistantInstance> clickCallback;

    private bool preventPopup = false;

    public void Init(AssistantInstance data, Action<AssistantInstance> onClick, bool preventPopup = false)
    {
        AssistantData = data;
        clickCallback = onClick;
        this.preventPopup = preventPopup;

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(OnClickSlot);

        if (icon != null)
        {
            string iconPath = data?.IconPath;
            icon.sprite = !string.IsNullOrEmpty(iconPath)
                ? IconLoader.GetIconByPath(iconPath)
                : null;
            icon.enabled = icon.sprite != null;
        }

        if (equippedIndicator != null)
            equippedIndicator.SetActive(data.IsEquipped);

        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;
    }

    private void OnClickSlot()
    {
        SoundManager.Instance.Play("SFX_SystemClick");

        clickCallback?.Invoke(AssistantData);

        // 다른곳에서 안킴
        if (AssistantData == null || preventPopup) return;

        var ui = uIManager.OpenUI<AssistantPopup>(UIName.AssistantPopup);
        ui.SetAssistant(AssistantData);
    }

    public void RefreshEquippedState()
    {
        if (equippedIndicator != null && AssistantData != null)
            equippedIndicator.SetActive(AssistantData.IsEquipped);
    }
}
