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
    [SerializeField] private Image titleIcon; // 인스펙터에 TitleIcon 등록

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

    public void Show(List<(string itemKey, int count)> rewardList, ItemDataLoader itemLoader, string title = "획득")
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

        PlayTitleIconGlow(); // 효과 발동

        gameObject.SetActive(true);
    }

    // 타이틀 아이콘 빛나는 효과
    private void PlayTitleIconGlow()
    {
        if (titleIcon == null) return;

        titleIcon.color = new Color(1f, 0.97f, 0.45f, 0.0f); // 밝은 노란색, 투명
        titleIcon.transform.localScale = Vector3.one * 1.15f;

        titleIcon.DOFade(0.85f, 0.15f).From(0f).SetEase(Ease.OutQuad);
        titleIcon.transform.DOScale(1.04f, 0.18f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBack)
            .OnComplete(() => {
                titleIcon.DOFade(0f, 0.5f).SetDelay(0.22f);
            });
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
    }

    private void Awake()
    {
        if (exitTitle != null)
            exitTitle.onClick.AddListener(() => uiManager?.CloseUI("RewardPopup"));

        if (overlayButton != null)
        {
            overlayButton.onClick.RemoveAllListeners();
            overlayButton.onClick.AddListener(() => uiManager?.CloseUI("RewardPopup"));
        }

        if (popupPanel != null)
        {
            var btn = popupPanel.GetComponent<Button>();
            if (btn == null)
                btn = popupPanel.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(() => uiManager?.CloseUI("RewardPopup"));
        }
    }
}
