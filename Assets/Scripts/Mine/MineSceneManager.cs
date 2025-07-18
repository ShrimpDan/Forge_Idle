using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

[Serializable]
public class MineGroup
{
    public string mineKey;
    public List<MineAssistantSlotUI> slotUIs; // 씬에 배치된 슬롯UI들 (Inspector 연결)
    public List<MineAssistantSlot> slots;     // 실제 데이터 슬롯 (동일 개수로 Inspector 연결 or 코드에서 동적생성)
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

    [Header("마인씬 카메라")]
    public Camera mineCamera;

    [Header("팝업 루트 (Inspector에서 연결하세요)")]
    public Transform popupRoot; // Inspector에서 PopupRoot 연결

    private AssistantInventory assistantInventory;
    private MineLoader mineLoader;
    private int currentMineIndex = 0;

    private void Awake()
    {
        var mainCamObj = GameObject.FindWithTag("MainCamera");
        if (mainCamObj != null) mainCamObj.SetActive(false);
        if (mineCamera != null) mineCamera.gameObject.SetActive(true);
        else Debug.LogError("MineSceneManager: mineCamera 연결 안 됨!");
    }

    private void OnDestroy()
    {
        var mainCamObj = GameObject.FindWithTag("MainCamera");
        if (mainCamObj != null) mainCamObj.SetActive(true);
        if (mineCamera != null) mineCamera.gameObject.SetActive(false);
    }

    private void Start()
    {
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

        // 반드시 slotUIs, slots 개수가 동일해야 함!
        for (int idx = 0; idx < mineGroups.Count; ++idx)
        {
            var group = mineGroups[idx];
            var mineData = mineLoader.GetByKey(group.mineKey);
            if (mineData == null) continue;
            group.mineManager = new MineAssistantManager(mineData);
            group.lastCollectTime = DateTime.Now;

            // 씬 슬롯 UI와 데이터 슬롯을 1:1로 연결
            for (int slotIdx = 0; slotIdx < group.slotUIs.Count; ++slotIdx)
            {
                var slotUI = group.slotUIs[slotIdx];
                slotUI.Init(assistantInventory);

                // 여기 반드시 slot 세팅!
                if (group.slots.Count > slotIdx)
                {
                    slotUI.SetSlot(group.slots[slotIdx]);
                }
                else
                {
                    Debug.LogError($"MineSceneManager: MineGroup[{idx}]의 슬롯 데이터 개수 불일치!");
                }

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

        if (cameraTouchDrag == null)
        {
            Debug.LogError("cameraTouchDrag가 연결되어 있지 않습니다!");
        }
        if (cameraLimits == null || cameraLimits.Count == 0)
        {
            Debug.LogError("cameraLimits가 비어있거나 0번 인덱스가 없습니다!");
        }
        else
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

    void OnSlotClicked(int mineIdx, MineAssistantSlotUI slotUI)
    {
        Debug.Log("OnSlotClicked called!");

        var prefab = Resources.Load<GameObject>("UI/Popup/AssistantSelectPopup");
        if (prefab == null)
        {
            Debug.LogError("AssistantSelectPopup prefab 로드 실패!");
            return;
        }
        if (popupRoot == null)
        {
            Debug.LogError("MineSceneManager: popupRoot 연결 안 됨! (Inspector에서 연결해야 함)");
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
            if (slotUI != null && slotUI.IsSceneSlot())
            {
                slotUI.AssignAssistant(selected);
                UpdateMinedAmountUI(mineIdx);
            }
            else
            {
                Debug.LogError("AssignAssistant 시도: slotUI가 씬 슬롯이 아님!");
            }
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
