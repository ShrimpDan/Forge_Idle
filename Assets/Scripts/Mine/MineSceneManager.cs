using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

[Serializable]
public class MineGroup
{
    public string mineKey; // 예: "CopperBronzeMine", "IronSilverMine" 등
    public List<MineAssistantSlotUI> slotUIs;
    public Button collectButton;
    public TMP_Text minedAmountText;

    [NonSerialized] public MineAssistantManager mineManager;
    [NonSerialized] public DateTime lastCollectTime;
}

public class MineSceneManager : MonoBehaviour
{
    [Header("마인별 그룹(Inspector에서 4개 mineKey, 슬롯, 버튼, 텍스트 연결)")]
    public List<MineGroup> mineGroups; // 각 그룹을 드래그로 세팅

    private MineLoader mineLoader;

    private void Start()
    {
        mineLoader = new MineLoader(); // json 데이터 로드

        foreach (var group in mineGroups)
        {
            var mineData = mineLoader.GetByKey(group.mineKey);
            if (mineData == null)
            {
                Debug.LogError($"MineData not found for key: {group.mineKey}");
                continue;
            }
            group.mineManager = new MineAssistantManager(mineData);
            group.lastCollectTime = DateTime.Now;

            for (int i = 0; i < group.slotUIs.Count; ++i)
            {
                group.slotUIs[i].SetSlot(group.mineManager.Slots[i]);
                group.slotUIs[i].OnSlotClicked = (slotUI) => OnSlotClicked(group, slotUI);
            }

            if (group.collectButton != null)
                group.collectButton.onClick.AddListener(() => OnCollectButton(group));
        }
    }

    private void Update()
    {
        foreach (var group in mineGroups)
        {
            UpdateMinedAmountUI(group);
        }
    }

    void OnSlotClicked(MineGroup group, MineAssistantSlotUI slotUI)
    {
        UIManager ui = FindObjectOfType<UIManager>();
        ui.OpenUI<AssistantSelectPopup>("AssistantSelectPopup").OpenForSelection(selected =>
        {
            slotUI.AssignAssistant(selected);
            UpdateMinedAmountUI(group);
        }, true);
    }

    void OnCollectButton(MineGroup group)
    {
        float amount = group.mineManager.CalcMinedAmount(group.lastCollectTime, DateTime.Now);
        Debug.Log($"{group.mineKey} 채굴 수령: {amount:F0}개");
        group.lastCollectTime = DateTime.Now;
        UpdateMinedAmountUI(group);
    }

    void UpdateMinedAmountUI(MineGroup group)
    {
        if (group.mineManager == null) return;
        float amount = group.mineManager.CalcMinedAmount(group.lastCollectTime, DateTime.Now);
        if (group.minedAmountText != null)
            group.minedAmountText.text = $"{amount:F0}개";
    }
}
