using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

[Serializable]
public class MineGroup
{
    public string mineKey;
    public List<MineAssistantSlotUI> slotUIs;
    [NonSerialized] public MineAssistantManager mineManager;
    [NonSerialized] public DateTime lastCollectTime;
}

public class MineSceneManager : MonoBehaviour
{
    [Header("마인별 그룹(Inspector에서 세팅)")]
    public List<MineGroup> mineGroups;

    [Header("맵 프리팹/카메라")]
    public GameObject mineDetailMap;
    public List<GameObject> minePrefabs;
    public CameraTouchDrag cameraTouchDrag;
    public List<CameraLimit> cameraLimits; // 0=상세맵, 1~4=각 마인

    [Header("채굴 패널 (버튼/채굴량 UI 포함)")]
    public GameObject miningUIPanel;
    public Button collectButton;
    public TMP_Text minedAmountText;

    private MineLoader mineLoader;
    private int currentMineIndex = 0;

    private void Start()
    {
        mineLoader = new MineLoader();
        for (int idx = 0; idx < mineGroups.Count; ++idx)
        {
            var group = mineGroups[idx];
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
                int closureIdx = idx;
                group.slotUIs[i].OnSlotClicked = (slotUI) => OnSlotClicked(closureIdx, slotUI);
            }
        }

        if (collectButton != null)
            collectButton.onClick.AddListener(OnCollectButton);

        // 패널은 항상 ON(초기화 시 상세맵이니까)
        if (miningUIPanel != null)
            miningUIPanel.SetActive(true);

        ShowMineDetailMap();
    }

    private void Update()
    {
        UpdateMinedAmountUI(currentMineIndex);
    }

    // 프리팹/지도만 끄고, 패널은 상황따라
    private void SetAllInactive()
    {
        if (mineDetailMap != null) mineDetailMap.SetActive(false);
        foreach (var obj in minePrefabs)
            if (obj != null) obj.SetActive(false);
    }

    public void ShowMineDetailMap()
    {
        SetAllInactive();
        if (mineDetailMap != null)
            mineDetailMap.SetActive(true);

        if (cameraTouchDrag != null && cameraLimits.Count > 0)
        {
            cameraTouchDrag.SetCameraLimit(cameraLimits[0]);
            cameraTouchDrag.enabled = true;
        }

        // 채굴 패널은 **상세맵에서만 ON**
        if (miningUIPanel != null) miningUIPanel.SetActive(true);

        SetActiveMine(0); // 상세맵(0번 기준)
    }

    public void ShowMine(int idx)
    {
        SetAllInactive();
        if (idx < 0 || idx >= minePrefabs.Count) return;
        minePrefabs[idx].SetActive(true);

        if (cameraTouchDrag != null && cameraLimits.Count > idx + 1)
        {
            cameraTouchDrag.SetCameraLimit(cameraLimits[idx + 1]);
            cameraTouchDrag.enabled = true;
        }

        // 채굴 패널은 **마인 진입시 OFF**
        if (miningUIPanel != null) miningUIPanel.SetActive(false);

        SetActiveMine(idx);
    }

    // 버튼용 함수 (Inspector에서 직접 연결)
    public void OnCopperBronzeMineBtn() => ShowMine(0);
    public void OnIronSilverMineBtn() => ShowMine(1);
    public void OnGoldmithrilMineBtn() => ShowMine(2);
    public void OnGemMineBtn() => ShowMine(3);
    public void OnBackToDetailMap() => ShowMineDetailMap();

    public void SetActiveMine(int mineIdx)
    {
        if (mineIdx < 0 || mineIdx >= mineGroups.Count) return;
        currentMineIndex = mineIdx;
        UpdateMinedAmountUI(currentMineIndex);
    }

    void OnSlotClicked(int mineIdx, MineAssistantSlotUI slotUI)
    {
        UIManager ui = FindObjectOfType<UIManager>();
        ui.OpenUI<AssistantSelectPopup>("AssistantSelectPopup").OpenForSelection(selected =>
        {
            slotUI.AssignAssistant(selected);
            UpdateMinedAmountUI(mineIdx);
        }, true);
    }

    void OnCollectButton()
    {
        var group = mineGroups[currentMineIndex];
        float amount = group.mineManager.CalcMinedAmount(group.lastCollectTime, DateTime.Now);
        Debug.Log($"{group.mineKey} 채굴 수령: {amount:F0}개");
        group.lastCollectTime = DateTime.Now;
        UpdateMinedAmountUI(currentMineIndex);
    }

    void UpdateMinedAmountUI(int mineIdx)
    {
        var group = mineGroups[mineIdx];
        if (group.mineManager == null) return;
        float amount = group.mineManager.CalcMinedAmount(group.lastCollectTime, DateTime.Now);
        if (minedAmountText != null)
            minedAmountText.text = $"{amount:F0}개";
    }
}
