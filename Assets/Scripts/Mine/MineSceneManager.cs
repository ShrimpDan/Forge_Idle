using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

[Serializable]
public class MineGroup
{
    public string mineKey;
    public List<MineAssistantSlotUI> slotUIs;
    public List<MineAssistantSlot> slots = new();
    public Transform spawnPoint;
    public Transform assistantsRoot;
    public List<Tilemap> obstacleTilemaps;
    [NonSerialized] public MineAssistantManager mineManager;
    [NonSerialized] public DateTime lastCollectTime;

    public void EnsureSlots()
    {
        while (slots.Count < slotUIs.Count)
            slots.Add(new MineAssistantSlot());
        if (slots.Count > slotUIs.Count)
            slots.RemoveRange(slotUIs.Count, slots.Count - slotUIs.Count);
    }
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
    private MineSaveHandler mineSaveHandler;

    private void Awake()
    {
        mineSaveHandler = new MineSaveHandler(this);
        SceneCameraState.IsMineSceneActive = true;
        SetMainUIClickable(false);
        SetMainCameraActive(false);
        SetMineCameraActive(true);
    }

    private void OnDestroy()
    {
        SceneCameraState.IsMineSceneActive = false;
        SetMineCameraActive(false);
    }

    private void Start()
    {
        if (GameManager.Instance?.AssistantManager?.AssistantInventory == null) return;

        assistantInventory = GameManager.Instance.AssistantManager.AssistantInventory;

        mineLoader = new MineLoader();
        spawnedAssistants = new List<List<GameObject>>();

        for (int idx = 0; idx < mineGroups.Count; ++idx)
        {
            spawnedAssistants.Add(new List<GameObject>());
            var group = mineGroups[idx];
            var mineData = mineLoader.GetByKey(group.mineKey);
            if (mineData == null) continue;
            group.mineManager = new MineAssistantManager(mineData);
            group.EnsureSlots();
            group.mineManager.Slots = group.slots;
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

        mineSaveHandler.Load();

        MineResourceCollectManager.Instance.SetMineGroups(mineGroups);
        MineResourceCollectManager.Instance.SetMinedAmountText(minedAmountText);
        MineResourceCollectManager.Instance.SetCurrentMineGroupIndex(currentMineIndex);

        if (collectButton != null)
            collectButton.onClick.AddListener(OnCollectAllButton);

        if (miningUIPanel != null)
            miningUIPanel.SetActive(true);

        ShowMineDetailMap();
        RemoveFiredAssistantsFromMine();
        RemoveEquippedAssistantsFromMineOnly();
    }

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
            cameraTouchDrag.enableZoom = false;
            cameraTouchDrag.SetCameraSize(5f);
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
            cameraTouchDrag.enableZoom = true;
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
        MineResourceCollectManager.Instance.SetCurrentMineGroupIndex(currentMineIndex);
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

        var group = mineGroups[mineIdx];

        popup.OpenForSelection(selected =>
        {
            if (slotUI != null && slotUI.IsSceneSlot())
            {
                int slotIndex = group.slotUIs.IndexOf(slotUI);
                if (slotIndex >= 0 && slotIndex < group.slots.Count)
                {
                    group.slots[slotIndex].Assign(selected, DateTime.Now);
                    slotUI.AssignAssistant(selected);
                    ClearSlotAssistant(mineIdx, slotUI);
                    SpawnAssistantInMine(mineIdx, slotUI, selected);
                    MineResourceCollectManager.Instance.UpdateExpectedMiningAmount();
                }
            }
        }, true);
    }

    // 전체 자원 수령 버튼 → 한 번에 모든 마인 자원 수령
    public void OnCollectAllButton()
    {
        MineResourceCollectManager.Instance.CollectAllResources();
        MineResourceCollectManager.Instance.UpdateExpectedMiningAmount();
    }

    void SpawnAssistantInMine(int mineIdx, MineAssistantSlotUI slotUI, AssistantInstance assistant)
    {
        if (mineIdx < 0 || mineIdx >= mineGroups.Count) return;
        var group = mineGroups[mineIdx];

        Transform spawnPoint = group.spawnPoint;
        Transform assistantsRoot = group.assistantsRoot;
        if (spawnPoint == null || assistantsRoot == null)
            return;

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
                sr.sortingOrder = 104;

            var fsm = go.GetComponent<MineAssistantFSM>();
            if (fsm != null)
                fsm.Init(assistant, mineIdx, assistantsRoot.gameObject, this);

            slotUI.gameObject.GetComponent<MineAssistantSlotUIObjectRef>()?.SetAssistant(go);
            spawnedAssistants[mineIdx].Add(go);
        };
    }

    public void ClearSlotAssistant(int mineIdx, MineAssistantSlotUI slotUI)
    {
        var objRef = slotUI.GetComponent<MineAssistantSlotUIObjectRef>();
        if (objRef != null && objRef.spawnedObject != null)
        {
            Destroy(objRef.spawnedObject);
            spawnedAssistants[mineIdx].Remove(objRef.spawnedObject);
            objRef.spawnedObject = null;
        }
    }

    public void SetMainUIClickable(bool clickable)
    {
        var mainUIObj = GameObject.Find("Main_UI");
        if (mainUIObj == null) return;
        var cg = mainUIObj.GetComponent<CanvasGroup>();
        if (cg == null) cg = mainUIObj.AddComponent<CanvasGroup>();
        cg.interactable = clickable;
        cg.blocksRaycasts = clickable;
    }

    private void SetMainCameraActive(bool active)
    {
        var mainCamObj = GameObject.FindWithTag("MainCamera");
        if (mainCamObj != null)
        {
            mainCamObj.SetActive(active);
            var cam = mainCamObj.GetComponent<Camera>();
            if (cam != null) cam.enabled = active;
            SetAudioListenerEnabled(mainCamObj, active);
        }
    }

    private void SetMineCameraActive(bool active)
    {
        if (mineCamera != null)
        {
            mineCamera.gameObject.SetActive(active);
            mineCamera.enabled = active;
            SetAudioListenerEnabled(mineCamera.gameObject, active);
        }
    }

    private void SetAudioListenerEnabled(GameObject camObj, bool enabled)
    {
        var allListeners = GameObject.FindObjectsOfType<AudioListener>();
        foreach (var listener in allListeners)
        {
            if (listener.gameObject == camObj)
                listener.enabled = enabled;
            else if (enabled)
                listener.enabled = false;
        }
    }

    public void OnReturnToForgeMain()
    {
        mineSaveHandler.Save();
        SetMainUIClickable(true);
        SetMineCameraActive(false);

        LoadSceneManager.Instance.UnLoadScene(SceneType.MineScene, () =>
        {
            SceneCameraState.IsMineSceneActive = false;
        });
    }

    public MineSaveData ToSaveData()
    {
        var saveData = new MineSaveData();
        foreach (var group in mineGroups)
        {
            var groupSave = new MineGroupSaveData
            {
                MineKey = group.mineKey,
                LastCollectTime = group.lastCollectTime.ToString("o")
            };
            foreach (var slot in group.slots)
            {
                var slotSave = new MineSlotSaveData
                {
                    AssistantKey = slot.AssignedAssistant != null ? slot.AssignedAssistant.Key : null,
                    AssignedTime = slot.AssignedTime.ToString("o")
                };
                groupSave.Slots.Add(slotSave);
            }
            saveData.Groups.Add(groupSave);
        }
        return saveData;
    }

    public void LoadFromSaveData(MineSaveData saveData)
    {
        if (saveData == null) return;
        for (int groupIdx = 0; groupIdx < saveData.Groups.Count; groupIdx++)
        {
            var groupSave = saveData.Groups[groupIdx];
            var group = mineGroups.Find(g => g.mineKey == groupSave.MineKey);
            if (group == null) continue;

            if (!string.IsNullOrEmpty(groupSave.LastCollectTime))
                group.lastCollectTime = DateTime.Parse(groupSave.LastCollectTime);

            for (int slotIdx = 0; slotIdx < group.slots.Count && slotIdx < groupSave.Slots.Count; slotIdx++)
            {
                var slotSave = groupSave.Slots[slotIdx];
                AssistantInstance assistant = null;
                DateTime assignedTime = DateTime.Now;

                if (!string.IsNullOrEmpty(slotSave.AssignedTime))
                    assignedTime = DateTime.Parse(slotSave.AssignedTime);

                if (!string.IsNullOrEmpty(slotSave.AssistantKey))
                {
                    assistant = assistantInventory.GetAssistantInstance(slotSave.AssistantKey);
                    group.slots[slotIdx].Assign(assistant, assignedTime);
                }
                else
                {
                    group.slots[slotIdx].Assign(null, assignedTime);
                }

                if (assistant != null)
                {
                    var slotUI = group.slotUIs[slotIdx];
                    ClearSlotAssistant(groupIdx, slotUI);
                    SpawnAssistantInMine(groupIdx, slotUI, assistant);
                }
            }
        }
        for (int groupIdx = 0; groupIdx < mineGroups.Count; groupIdx++)
        {
            var group = mineGroups[groupIdx];
            for (int slotIdx = 0; slotIdx < group.slotUIs.Count; slotIdx++)
            {
                group.slotUIs[slotIdx].SetSlot(group.slots[slotIdx]);
            }
        }

        MineResourceCollectManager.Instance.UpdateExpectedMiningAmount();
    }

    public void ClearAllSlots()
    {
        foreach (var group in mineGroups)
            foreach (var slot in group.slots)
                slot.Unassign();
    }

    private void RemoveFiredAssistantsFromMine()
    {
        foreach (var group in mineGroups)
        {
            for (int i = 0; i < group.slots.Count; i++)
            {
                var slot = group.slots[i];
                var assi = slot.AssignedAssistant;

                if (slot.IsAssigned && assi != null && assi.IsFired)
                {
                    slot.Unassign();
                    group.slotUIs[i].AssignAssistant(null);
                    ClearSlotAssistant(mineGroups.IndexOf(group), group.slotUIs[i]);

                    Debug.Log($"[광산] 탈주한 제자 {assi.Name} 자동 해제됨");
                }
            }
        }
    }

    private void RemoveEquippedAssistantsFromMineOnly()
    {
        for (int groupIdx = 0; groupIdx < mineGroups.Count; ++groupIdx)
        {
            var group = mineGroups[groupIdx];
            for (int slotIdx = 0; slotIdx < group.slots.Count; ++slotIdx)
            {
                var slot = group.slots[slotIdx];
                var assi = slot.AssignedAssistant;

                if (slot.IsAssigned && assi != null && assi.IsEquipped)
                {
                    slot.Unassign();
                    group.slotUIs[slotIdx].AssignAssistant(null);
                    ClearSlotAssistant(groupIdx, group.slotUIs[slotIdx]);

                    Debug.Log($"[광산] 착용 중인 제자 {assi.Name} → 광산에서 해제됨");
                }
            }
        }
    }

}