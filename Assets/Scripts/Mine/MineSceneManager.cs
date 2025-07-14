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
    public List<CameraLimit> cameraLimits;

    [Header("채굴 패널 (버튼/채굴량 UI 포함)")]
    public GameObject miningUIPanel;
    public Button collectButton;
    public TMP_Text minedAmountText;

    // Inspector에서 연결 ❌. 반드시 코드로 할당
    private AssistantInventory assistantInventory;

    private MineLoader mineLoader;
    private int currentMineIndex = 0;

    private void Start()
    {
        // 반드시 GameManager에서 받아와야 함!
        if (GameManager.Instance == null || GameManager.Instance.AssistantManager == null)
        {
            Debug.LogError("GameManager.Instance 또는 AssistantManager가 초기화되어 있지 않습니다!");
            return;
        }
        assistantInventory = GameManager.Instance.AssistantManager.AssistantInventory;
        if (assistantInventory == null)
        {
            Debug.LogError("MineSceneManager: AssistantInventory를 GameManager에서 받아오지 못했습니다!");
            return;
        }

        mineLoader = new MineLoader();
        for (int idx = 0; idx < mineGroups.Count; ++idx)
        {
            var group = mineGroups[idx];
            var mineData = mineLoader.GetByKey(group.mineKey);
            if (mineData == null) continue;
            group.mineManager = new MineAssistantManager(mineData);
            group.lastCollectTime = DateTime.Now;

            foreach (var slotUI in group.slotUIs)
            {
                slotUI.Init(assistantInventory); // 인벤토리 주입
                int closureIdx = idx;
                slotUI.OnSlotClicked = (ui) => OnSlotClicked(closureIdx, ui);
            }
        }

        if (collectButton != null)
            collectButton.onClick.AddListener(OnCollectButton);

        if (miningUIPanel != null)
            miningUIPanel.SetActive(true);

        ShowMineDetailMap();
    }

    private void Update() => UpdateMinedAmountUI(currentMineIndex);

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
        if (miningUIPanel != null) miningUIPanel.SetActive(true);
        SetActiveMine(0);
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
        if (miningUIPanel != null) miningUIPanel.SetActive(false);

        SetActiveMine(idx);
    }

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

    // 팝업에 인벤토리 반드시 전달
    void OnSlotClicked(int mineIdx, MineAssistantSlotUI slotUI)
    {
        var prefab = Resources.Load<GameObject>("UI/Popup/AssistantSelectPopup");
        var popupRoot = GameObject.Find("PopupRoot")?.transform;
        if (popupRoot == null)
        {
            Debug.LogError("PopupRoot를 찾을 수 없습니다.");
            return;
        }
        var go = Instantiate(prefab, popupRoot);

        var popup = go.GetComponent<AssistantSelectPopup>();
        if (popup == null)
        {
            Debug.LogError("AssistantSelectPopup 컴포넌트가 없습니다.");
            Destroy(go);
            return;
        }
        popup.Init(assistantInventory);

        popup.OpenForSelection(selected =>
        {
            slotUI.AssignAssistant(selected);
            UpdateMinedAmountUI(mineIdx);
        }, true);
    }


    void OnCollectButton()
    {
        var group = mineGroups[currentMineIndex];
        float amount = group.mineManager.CalcMinedAmount(group.lastCollectTime, DateTime.Now);
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
