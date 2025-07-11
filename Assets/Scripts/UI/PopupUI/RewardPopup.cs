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

    private List<RewardPopup_Slot> slots = new();

    // 애니메이션 시간
    private float openDuration = 0.35f;
    private float closeDuration = 0.25f;
    private float exitTitleScale = 1.1f;
    private float exitTitleAnimDuration = 0.7f;

    public void Show(List<(string itemKey, int count)> rewardList, ItemDataLoader itemLoader, string title = "획득")
    {
        // 타이틀 세팅
        if (titleText != null)
            titleText.text = title;

        // 기존 슬롯 제거
        foreach (Transform child in rewardRoot)
            Destroy(child.gameObject);
        slots.Clear();

        // 슬롯 생성
        foreach (var reward in rewardList)
        {
            var itemData = itemLoader.GetItemByKey(reward.itemKey);
            if (itemData == null) continue;

            var go = Instantiate(rewardSlotPrefab, rewardRoot);
            var slot = go.GetComponent<RewardPopup_Slot>();
            slot.Init(itemData, reward.count);
            slots.Add(slot);
        }

        // 오픈 효과
        UIEffect.PopupOpenEffect(popupPanel, openDuration);

        // Exit 버튼 효과 (확대-축소)
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
        // PopupCloseEffect가 자동으로 꺼줌
    }

    private void Awake()
    {
        if (exitTitle != null)
            exitTitle.onClick.AddListener(() => Close());
        // 터치시 닫기
        if (popupPanel != null)
        {
            var btn = popupPanel.GetComponent<Button>();
            if (btn == null)
                btn = popupPanel.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(() => Close());
        }
    }
}
