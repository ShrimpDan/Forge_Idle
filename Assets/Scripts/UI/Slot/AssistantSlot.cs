using UnityEngine;
using UnityEngine.UI;
using System;

public class AssistantSlot : MonoBehaviour
{
    private UIManager uIManager;
    public AssistantInstance AssistantData { get; private set; }
    public AssistantInstance Assistant => AssistantData;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image icon;
    [SerializeField] private Button slotBtn;
    [SerializeField] private GameObject equippedIndicator;
    [SerializeField] private GameObject firedIndicator;
    [SerializeField] private Image checkmark;

    private Action<AssistantInstance> clickCallback;
    private bool preventPopup = false;
    private bool isSelected = false;
    private bool isDismissMode = false;

    public event Action OnClicked;

    public void Init(AssistantInstance data, Action<AssistantInstance> onClick, bool preventPopup = false)
    {
        AssistantData = data;
        clickCallback = onClick;
        this.preventPopup = preventPopup;

        if (slotBtn != null)
        {
            slotBtn.onClick.RemoveAllListeners();
            slotBtn.onClick.AddListener(OnClickSlot);
        }

        UpdateVisuals();

        if (uIManager == null)
            uIManager = GameManager.Instance.UIManager;

        if (checkmark != null)
            checkmark.gameObject.SetActive(false);
    }

    private void OnClickSlot()
    {
        SoundManager.Instance.Play("ClickSound");

        if (DismissManager.Instance != null && DismissManager.Instance.IsDismissMode())
        {
            DismissManager.Instance.ToggleSelect(this);
            return;
        }

        OnClicked?.Invoke();
        clickCallback?.Invoke(AssistantData);

        if (AssistantData == null || preventPopup) return;

        var popup = uIManager.OpenUI<AssistantPopup>(UIName.AssistantPopup);
        popup.SetAssistant(AssistantData);
    }

    public void RefreshEquippedState()
    {
        if (AssistantData == null) return;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (AssistantData == null) return;

        bool isEquipped = AssistantData.IsEquipped;
        bool isFired = AssistantData.IsFired;

        if (equippedIndicator != null)
            equippedIndicator.SetActive(isEquipped && !isFired);

        if (firedIndicator != null)
            firedIndicator.SetActive(isFired);

        if (icon != null)
        {
            icon.sprite = !string.IsNullOrEmpty(AssistantData.IconPath)
                ? IconLoader.GetIconByPath(AssistantData.IconPath)
                : null;

            icon.enabled = icon.sprite != null;
            icon.color = isFired ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white;
        }

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

    public void SetDismissMode(bool value)
    {
        isDismissMode = value;
    }

    public void SetSelectedForDismiss(bool selected)
    {
        SetSelected(selected);
    }
}
