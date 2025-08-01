using UnityEngine;
using UnityEngine.UI;
using System;

public class AssistantSlot : MonoBehaviour
{
    private UIManager uIManager;
    public AssistantInstance AssistantData { get; private set; }
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image icon;
    [SerializeField] private Button slotBtn;
    [SerializeField] private GameObject equippedIndicator;
    [SerializeField] private GameObject firedIndicator;
    [SerializeField] private Image checkmark;

    private Action<AssistantInstance> clickCallback;

    private bool preventPopup = false;

    private bool isSelected = false;

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
            icon.color = data.IsFired ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white;
        }

        if (equippedIndicator != null)
            equippedIndicator.SetActive(data.IsEquipped && !data.IsFired);

        if (firedIndicator != null)
            firedIndicator.SetActive(data.IsFired);

        if (canvasGroup != null)
            canvasGroup.alpha = data.IsFired ? 0.5f : 1f;

        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;

        if (checkmark != null)
            checkmark.gameObject.SetActive(false);
    }


    private void OnClickSlot()
    {
        SoundManager.Instance.Play("ClickSound");

        clickCallback?.Invoke(AssistantData);

        // 다른곳에서 안킴
        if (AssistantData == null || preventPopup) return;

        var ui = uIManager.OpenUI<AssistantPopup>(UIName.AssistantPopup);
        ui.SetAssistant(AssistantData);
    }

    public void RefreshEquippedState()
    {
        if (AssistantData == null) return;

        bool isEquipped = AssistantData.IsEquipped;
        bool isFired = AssistantData.IsFired;

        if (equippedIndicator != null)
            equippedIndicator.SetActive(isEquipped && !isFired);

        if (firedIndicator != null)
            firedIndicator.SetActive(isFired);

        if (icon != null)
            icon.color = isFired ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white;

        if (canvasGroup != null)
            canvasGroup.alpha = isFired ? 0.5f : 1f;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (checkmark != null)
            checkmark.gameObject.SetActive(selected);
    }

    public bool IsSelected() => isSelected;
}
