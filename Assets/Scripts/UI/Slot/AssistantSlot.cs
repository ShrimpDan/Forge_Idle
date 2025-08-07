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

    [Header("Rank Icon")]
    [SerializeField] private Image rankIconImage;
    [SerializeField] private Sprite rankN;
    [SerializeField] private Sprite rankR;
    [SerializeField] private Sprite rankSR;
    [SerializeField] private Sprite rankSSR;
    [SerializeField] private Sprite rankUR;

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

        if (AssistantData != null)
        {
            DismissManager.Instance?.RegisterSlot(this);
            Debug.Log($"[AssistantSlot] 등록 완료: {AssistantData.Name}");
        }
        else
        {
            Debug.LogWarning("[AssistantSlot] AssistantData가 null이어서 등록되지 않음");
        }

        SetRankIcon(data?.grade); // 등급 아이콘 설정
    }

    private void SetRankIcon(string grade)
    {
        if (rankIconImage == null) return;

        switch (grade)
        {
            case "N": rankIconImage.sprite = rankN; break;
            case "R": rankIconImage.sprite = rankR; break;
            case "SR": rankIconImage.sprite = rankSR; break;
            case "SSR": rankIconImage.sprite = rankSSR; break;
            case "UR": rankIconImage.sprite = rankUR; break;
            default: rankIconImage.sprite = null; break;
        }

        rankIconImage.enabled = rankIconImage.sprite != null;
    }

    private void OnClickSlot()
    {
        SoundManager.Instance.Play("ClickSound");

        Debug.Log($"[AssistantSlot] 슬롯 클릭됨: {AssistantData?.Name}, 해고 모드: {DismissManager.Instance?.IsDismissMode()}");

        if (DismissManager.Instance?.IsDismissMode() == true)
        {
            Debug.Log($"[AssistantSlot] 해고 선택 처리됨 → {AssistantData?.Name}");
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

        SetRankIcon(AssistantData?.grade);
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

        if (!value)
        {
            SetSelected(false);
        }
    }

    public void SetSelectedForDismiss(bool selected)
    {
        SetSelected(selected);
    }
}
