using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

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

    public void Show(List<(string itemKey, int count)> rewardList, ItemDataLoader itemLoader, string title = "È¹µæ")
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

        gameObject.SetActive(true);
    }

    public override void Open()
    {
        base.Open();
        UIEffect.PopupOpenEffect(popupPanel, openDuration);
        if (exitTitle != null)
            UIEffect.TextScaleEffect(exitTitle.GetComponentInChildren<TMP_Text>(), exitTitleScale, exitTitleAnimDuration);
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
