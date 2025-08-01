using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class RewardPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    [Header("UI Elements")]
    [SerializeField] private Button exitTitle;
    [SerializeField] private RectTransform popupPanel;
    [SerializeField] private RectTransform rewardRoot;
    [SerializeField] private GameObject rewardSlotPrefab;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button overlayButton;
    [SerializeField] private Image titleIcon;

    private List<RewardPopup_Slot> slots = new();

    private float openDuration = 0.35f;
    private float closeDuration = 0.25f;
    private float exitTitleScale = 1.1f;
    private float exitTitleAnimDuration = 0.7f;
    private UIManager uiManager;

    public override void Init(GameManager gm, UIManager um)
    {
        base.Init(gm, um);
        uiManager = um;
    }

    public void Show(List<(string itemKey, int count)> rewardList, ItemDataLoader itemLoader, string title = "획득 보상")
    {
        if (titleText != null)
            titleText.text = title;

        foreach (Transform child in rewardRoot)
            Destroy(child.gameObject);
        slots.Clear();

        foreach (var reward in rewardList)
        {
            var itemData = itemLoader.GetItemByKey(reward.itemKey);
            if (itemData == null) continue;
            var go = Instantiate(rewardSlotPrefab, rewardRoot);
            var slot = go.GetComponent<RewardPopup_Slot>();
            slot.Init(itemData, reward.count);
            slots.Add(slot);
        }

        UIEffect.PopupOpenEffect(popupPanel, openDuration);

        if (exitTitle != null)
            UIEffect.TextScaleEffect(exitTitle.GetComponentInChildren<TMP_Text>(), exitTitleScale, exitTitleAnimDuration);

        PlayTitleIconGlow();

        gameObject.SetActive(true);
    }

    public void Show(Dictionary<ItemData, int> rewardDict, string title = "획득 보상")
    {
        if (titleText != null)
            titleText.text = title;

        foreach (Transform child in rewardRoot)
            Destroy(child.gameObject);
        slots.Clear();

        foreach (var itemData in rewardDict.Keys)
        {
            if (itemData == null) continue;
            var go = Instantiate(rewardSlotPrefab, rewardRoot);
            var slot = go.GetComponent<RewardPopup_Slot>();
            slot.Init(itemData, rewardDict[itemData]);
            slots.Add(slot);
        }

        UIEffect.PopupOpenEffect(popupPanel, openDuration);

        if (exitTitle != null)
            UIEffect.TextScaleEffect(exitTitle.GetComponentInChildren<TMP_Text>(), exitTitleScale, exitTitleAnimDuration);

        PlayTitleIconGlow();

        gameObject.SetActive(true);
    }

    private void PlayTitleIconGlow()
    {
        if (titleIcon == null) return;

        titleIcon.color = new Color(1f, 0.97f, 0.45f, 1f);
        titleIcon.transform.localScale = Vector3.one * 1.15f;

        titleIcon.DOFade(0.85f, 0.15f).From(0f).SetEase(Ease.OutQuad);
        titleIcon.transform.DOScale(1.04f, 0.18f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBack);
    }

    public override void Open()
    {
        base.Open();
        UIEffect.PopupOpenEffect(popupPanel, openDuration);
        if (exitTitle != null)
            UIEffect.TextScaleEffect(exitTitle.GetComponentInChildren<TMP_Text>(), exitTitleScale, exitTitleAnimDuration);
        PlayTitleIconGlow();
    }

    public override void Close()
    {
        UIEffect.PopupCloseEffect(popupPanel, closeDuration);
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (exitTitle != null)
            exitTitle.onClick.AddListener(() => ClosePopup());

        if (overlayButton != null)
        {
            overlayButton.onClick.RemoveAllListeners();
            overlayButton.onClick.AddListener(() => ClosePopup());
        }

        if (popupPanel != null)
        {
            var btn = popupPanel.GetComponent<Button>();
            if (btn == null)
                btn = popupPanel.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(() => ClosePopup());
        }
    }
    public void ShowWithoutManager(List<(string itemKey, int count)> rewardList, ItemDataLoader itemLoader, string title = "획득 보상")
    {
        if (titleText != null)
            titleText.text = title;

        foreach (Transform child in rewardRoot)
            Destroy(child.gameObject);
        slots.Clear();

        foreach (var reward in rewardList)
        {
            var itemData = itemLoader.GetItemByKey(reward.itemKey);
            if (itemData == null) continue;
            var go = Instantiate(rewardSlotPrefab, rewardRoot);
            var slot = go.GetComponent<RewardPopup_Slot>();
            slot.Init(itemData, reward.count);
            slots.Add(slot);
        }

        if (overlayButton != null)
        {
            overlayButton.onClick.RemoveAllListeners();
            overlayButton.onClick.AddListener(() => ClosePopup());
        }
        if (popupPanel != null)
        {
            var btn = popupPanel.GetComponent<Button>();
            if (btn == null)
                btn = popupPanel.gameObject.AddComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => ClosePopup());
        }
        if (exitTitle != null)
        {
            exitTitle.onClick.RemoveAllListeners();
            exitTitle.onClick.AddListener(() => ClosePopup());
        }

        UIEffect.PopupOpenEffect(popupPanel, openDuration);

        if (exitTitle != null)
            UIEffect.TextScaleEffect(exitTitle.GetComponentInChildren<TMP_Text>(), exitTitleScale, exitTitleAnimDuration);

        PlayTitleIconGlow();
        gameObject.SetActive(true);
    }

    private void ClosePopup()
    {
        if (uiManager != null)
        {
            uiManager.CloseUI(UIName.RewardPopup);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
