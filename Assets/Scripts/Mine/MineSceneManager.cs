using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class MineGroup
{
    public string mineKey;
    public List<MineAssistantSlotUI> slotUIs;
    public List<MineAssistantSlot> slots;

    public Transform spawnPoint;     
    public Transform assistantsRoot;  

    [NonSerialized] public MineAssistantManager mineManager;
    [NonSerialized] public DateTime lastCollectTime;
}

public class MineSceneManager : MonoBehaviour
{
    public GameObject mineAssistantPrefab;
    public List<MineGroup> mineGroups;
    public GameObject mineDetailMap;
    public List<GameObject> minePrefabs;
    public CameraTouchDrag cameraTouchDrag;
    public List<CameraLimit> cameraLimits;
    public GameObject miningUIPanel;
    public Button collectButton;
    public TMP_Text minedAmountText;
    public Camera mineCamera;
    public Transform popupRoot;

    private List<List<GameObject>> spawnedAssistants;
    private AssistantInventory assistantInventory;
    private MineLoader mineLoader;
    private int currentMineIndex = 0;

    private void Awake()
    {
        var mainCamObj = GameObject.FindWithTag("MainCamera");
        if (mainCamObj != null) mainCamObj.SetActive(false);
        if (mineCamera != null) mineCamera.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
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
                    slotUI.SetSlot(group.slots[slotIdx]);

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
        if (mineIdx < 0 || mineIdx >= mineGroups.Count) return;
        var group = mineGroups[mineIdx];

        Transform spawnPoint = group.spawnPoint;
        Transform assistantsRoot = group.assistantsRoot;
        if (spawnPoint == null || assistantsRoot == null)
        {
            Debug.LogError($"[Mine] SpawnPoint 또는 AssistantsRoot가 NULL! (mineIdx={mineIdx})");
            return;
        }

        Debug.Log($"[SpawnTest] group: {group.mineKey}, spawnPoint: {spawnPoint.name}, parent: {assistantsRoot.name}, pos: {spawnPoint.position}");

        string key = $"Assets/Prefabs/Assistant/{assistant.Key}.prefab";
        Addressables.LoadAssetAsync<GameObject>(key).Completed += handle =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                return;

            Vector3 spawnPos = spawnPoint.position;
            spawnPos.z = -1; 
            GameObject go = Instantiate(handle.Result, spawnPos, Quaternion.identity, assistantsRoot);

            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 104; 
            }

            var fsm = go.GetComponent<MineAssistantFSM>();
            if (fsm != null)
            {
                fsm.Init(assistant, mineIdx, assistantsRoot.gameObject, this);
            }

            slotUI.gameObject.GetComponent<MineAssistantSlotUIObjectRef>()?.SetAssistant(go);
            spawnedAssistants[mineIdx].Add(go);
        };
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
