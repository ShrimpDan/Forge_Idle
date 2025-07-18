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
    public List<MineAssistantSlot> slots;
    [NonSerialized] public MineAssistantManager mineManager;
    [NonSerialized] public DateTime lastCollectTime;
}

public class MineSceneManager : MonoBehaviour
{
    [Header("마인 어시스턴트 프리팹")]
    public GameObject mineAssistantPrefab;

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
    public Transform popupRoot;

    private List<List<GameObject>> spawnedAssistants;
    private AssistantInventory assistantInventory;
    private MineLoader mineLoader;
    private int currentMineIndex = 0;

    private void Awake()
    {
        NuisanceCustomer.SetBlockClick(true);

        var mainCamObj = GameObject.FindWithTag("MainCamera");
        if (mainCamObj != null) mainCamObj.SetActive(false);
        if (mineCamera != null) mineCamera.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        NuisanceCustomer.SetBlockClick(false);

        var mainCamObj = GameObject.FindWithTag("MainCamera");
        if (mainCamObj != null) mainCamObj.SetActive(true);
        if (mineCamera != null) mineCamera.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.AssistantManager == null) return;
        assistantInventory = GameManager.Instance.AssistantManager.AssistantInventory;
        if (assistantInventory == null) return;

        mineLoader = new MineLoader();

        spawnedAssistants = new List<List<GameObject>>();
        for (int idx = 0; idx < mineGroups.Count; ++idx)
        {
            spawnedAssistants.Add(new List<GameObject>());
            var group = mineGroups[idx];
            var mineData = mineLoader.GetByKey(group.mineKey);
            if (mineData == null) continue;
            group.mineManager = new MineAssistantManager(mineData);
            group.lastCollectTime = DateTime.Now;

            for (int slotIdx = 0; slotIdx < group.slotUIs.Count; ++slotIdx)
            {
                var slotUI = group.slotUIs[slotIdx];
                slotUI.Init(assistantInventory);

                if (group.slots.Count > slotIdx)
                {
                    slotUI.SetSlot(group.slots[slotIdx]);
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

        if (cameraTouchDrag != null && cameraLimits != null && cameraLimits.Count > 0)
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
        var prefab = Resources.Load<GameObject>("UI/Popup/AssistantSelectPopup");
        if (prefab == null || popupRoot == null) return;

        var go = Instantiate(prefab, popupRoot);
        var popup = go.GetComponent<AssistantSelectPopup>();
        if (popup == null)
        {
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
                ClearSlotAssistant(mineIdx, slotUI);
                SpawnAssistantInMine(mineIdx, slotUI, selected);
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

    void SpawnAssistantInMine(int mineIdx, MineAssistantSlotUI slotUI, AssistantInstance assistant)
    {
        var mineRoot = minePrefabs[mineIdx];
        var group = mineGroups[mineIdx];
        Transform spawn = mineRoot.transform.Find("Spawnpoint") ?? mineRoot.transform;
        GameObject go = Instantiate(mineAssistantPrefab, spawn.position, Quaternion.identity, mineRoot.transform);
        go.name = $"MineAssistant_{mineIdx}_{assistant.Key}";
        var fsm = go.GetComponent<MineAssistantFSM>();
        if (fsm != null)
            fsm.Init(assistant, mineIdx, mineRoot, this);
        slotUI.gameObject.GetComponent<MineAssistantSlotUIObjectRef>()?.SetAssistant(go);
        spawnedAssistants[mineIdx].Add(go);
    }

    void ClearSlotAssistant(int mineIdx, MineAssistantSlotUI slotUI)
    {
        var objRef = slotUI.GetComponent<MineAssistantSlotUIObjectRef>();
        if (objRef != null && objRef.spawnedObject != null)
        {
            Destroy(objRef.spawnedObject);
            spawnedAssistants[mineIdx].Remove(objRef.spawnedObject);
            objRef.spawnedObject = null;
        }
    }
}
