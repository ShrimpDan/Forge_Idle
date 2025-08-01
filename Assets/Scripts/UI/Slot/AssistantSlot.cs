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
    private Action<AssistantInstance> clickCallback;
    private bool preventPopup = false;

    
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

        // 등급 아이콘 표시
        SetRankIcon(data?.grade);
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
        clickCallback?.Invoke(AssistantData);

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

        // 상태 갱신시 등급도 다시 표시
        SetRankIcon(AssistantData?.grade);
    }
}
