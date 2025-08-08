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
    public Transform blockRoot;
    public MineBlock blockObj;
    public void EnsureSlots()
    {
        while (slots.Count < slotUIs.Count)
            slots.Add(new MineAssistantSlot());
        if (slots.Count > slotUIs.Count)
            slots.RemoveRange(slotUIs.Count, slots.Count - slotUIs.Count);
    }
    public void ShowBlock(bool show, int cost = 0, int currentGold = 0, MineSceneManager manager = null, int idx = 0)
    {
        if (blockRoot == null) return;
        if (blockObj == null)
            blockObj = blockRoot.GetComponentInChildren<MineBlock>(true);
        if (blockObj != null)
        {
            blockObj.gameObject.SetActive(show);
            if (show && manager != null)
                blockObj.Setup(idx, cost, currentGold, manager);
        }
        foreach (var slotUI in slotUIs)
            slotUI.SetBlocked(show);
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
    public Camera mineCamera;
    public Transform popupRoot;

    [Header("Mine Unlock")]
    [SerializeField] private List<int> mineUnlockGoldCosts;
    [SerializeField] private Transform IronSilverRoot;
    [SerializeField] private Transform GoldMithrillRoot;
    [SerializeField] private Transform GemRoot;
    [SerializeField] private GameObject mineBlockPrefab;
    [SerializeField] private LackPopup lackPopupPrefab;
    [SerializeField] private Transform lackPopupRoot;

    [Header("Popup References")]
    [SerializeField] private GameObject mineInfoPopupPrefab;
    [SerializeField] private Button playerStatButton;
    
    [Header("Mine Info Root")]
    [SerializeField] private Transform mineInfoPopupRoot;
    private MineInfoPopup spawnedMineInfoPopup;

    private List<bool> unlockedMines = new List<bool>();
    private List<List<GameObject>> spawnedAssistants;
    private AssistantInventory assistantInventory;
    private MineLoader mineLoader;
    private int currentMineIndex = 0;
    private MineSaveHandler mineSaveHandler;
    private SoundManager soundManager;

    [Header("UI Roots & Prefabs")]
    public Transform amountRoot;
    public GameObject itemAmountSlotPrefab;

    private Dictionary<MineAssistantSlot, MineAssistantFSM> slotFsmDict = new();
    private void Awake()
    {
        soundManager = SoundManager.Instance;
        mineSaveHandler = new MineSaveHandler(this);
        SceneCameraState.IsMineSceneActive = true;
        SetMainUIClickable(false);
        SetMainCameraActive(false);
        SetMineCameraActive(true);
        SetUICameraActive(false);

        if (unlockedMines.Count != 4)
        {
            unlockedMines.Clear();
            unlockedMines.Add(true);
            unlockedMines.Add(false);
            unlockedMines.Add(false);
            unlockedMines.Add(false);
        }

        if (playerStatButton != null)
        {
            playerStatButton.onClick.RemoveAllListeners();
            playerStatButton.onClick.AddListener(ShowMineInfoPopup);
        }
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

        // blockRoot 연결
        if (mineGroups.Count > 1) mineGroups[1].blockRoot = IronSilverRoot;
        if (mineGroups.Count > 2) mineGroups[2].blockRoot = GoldMithrillRoot;
        if (mineGroups.Count > 3) mineGroups[3].blockRoot = GemRoot;

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

        SetupMineBlockPrefabs();

        mineSaveHandler.Load();
        MineResourceCollectManager.Instance.SetMineGroups(mineGroups);
        MineResourceCollectManager.Instance.SetCurrentMineGroupIndex(currentMineIndex);

        if (collectButton != null)
            collectButton.onClick.AddListener(OnCollectAllButton);
        if (miningUIPanel != null)
            miningUIPanel.SetActive(true);

        ShowMineDetailMap();
        RemoveFiredAssistantsFromMine();
        RemoveEquippedAssistantsFromMineOnly();
        RemoveInactiveAssistantsFromMine();

        RefreshAllMineBlocks();
    }
    // 차단 프리팹 동적 세팅
    private void SetupMineBlockPrefabs()
    {
        if (IronSilverRoot != null && IronSilverRoot.childCount == 0)
            Instantiate(mineBlockPrefab, IronSilverRoot);
        if (GoldMithrillRoot != null && GoldMithrillRoot.childCount == 0)
            Instantiate(mineBlockPrefab, GoldMithrillRoot);
        if (GemRoot != null && GemRoot.childCount == 0)
            Instantiate(mineBlockPrefab, GemRoot);
    }

    // 마인 차단 UI 갱신
    public void RefreshAllMineBlocks()
    {
        int playerGold = GameManager.Instance?.ForgeManager?.Gold ?? 0;
        for (int i = 0; i < mineGroups.Count; i++)
        {
            if (i == 0) continue; 
            bool isUnlocked = unlockedMines[i];
            int cost = mineUnlockGoldCosts[i - 1];
            mineGroups[i].ShowBlock(!isUnlocked, cost, playerGold, this, i);
        }
    }

    // 해금 시도
    public void TryUnlockMine(int idx)
    {
        SoundManager.Instance?.Play("Click");

        if (idx < 1 || idx >= unlockedMines.Count) return;
        if (unlockedMines[idx]) return;
        if (!unlockedMines[idx - 1]) return;

        int cost = mineUnlockGoldCosts[idx - 1];
        int playerGold = GameManager.Instance.ForgeManager.Gold;

        // 골드 부족시 LackPopup 호출
        if (playerGold < cost)
        {
            ShowLackPopup(LackType.Gold);
            return;
        }

        // 골드 충분하면 해금
        if (GameManager.Instance.ForgeManager.UseGold(cost))
        {
            unlockedMines[idx] = true;
            RefreshAllMineBlocks();
            mineSaveHandler.Save();
        }
    }

    private void ShowLackPopup(LackType type)
    {
        if (lackPopupPrefab && lackPopupRoot)
        {
            var lackPopup = Instantiate(lackPopupPrefab, lackPopupRoot);
            lackPopup.Init(GameManager.Instance, null); 
            lackPopup.Show(type);
        }
    }


    // 세이브
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
                // FSM 버프/쿨타임 상태 저장
                if (slot.AssignedAssistant != null && slotFsmDict.TryGetValue(slot, out var fsm) && fsm != null)
                {
                    slotSave.IsBuffActive = fsm.IsBuffActive();
                    slotSave.BuffRemain = fsm.GetBuffRemain();
                    slotSave.IsCooldown = fsm.IsCooldown();
                    slotSave.CooldownRemain = fsm.GetCooldownRemain();
                }
                else
                {
                    slotSave.IsBuffActive = false;
                    slotSave.BuffRemain = 0f;
                    slotSave.IsCooldown = false;
                    slotSave.CooldownRemain = 0f;
                }
                groupSave.Slots.Add(slotSave);
            }
            saveData.Groups.Add(groupSave);
        }
        saveData.UnlockedMines = new List<bool>(unlockedMines);
        return saveData;
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
        var group = mineGroups[mineIdx];
        int slotIndex = group.slotUIs.IndexOf(slotUI);
        if (slotIndex < 0 || slotIndex >= group.slots.Count) return;
        var slot = group.slots[slotIndex];

        // 이미 어시스턴트가 배치된 경우 → 해제 처리
        if (slot.IsAssigned && slot.AssignedAssistant != null)
        {
            // 버프/쿨타임 FSM 정보 -> Slot에 저장
            if (slotFsmDict.TryGetValue(slot, out var fsm) && fsm != null)
            {
                slot.LastIsBuffActive = fsm.IsBuffActive();
                slot.LastBuffRemain = fsm.GetBuffRemain();
                slot.LastIsCooldown = fsm.IsCooldown();
                slot.LastCooldownRemain = fsm.GetCooldownRemain();
            }
            else
            {
                slot.LastIsBuffActive = false;
                slot.LastBuffRemain = 0f;
                slot.LastIsCooldown = false;
                slot.LastCooldownRemain = 0f;
            }

            slot.Unassign();
            slotUI.AssignAssistant(null);
            ClearSlotAssistant(mineIdx, slotUI);
            MineResourceCollectManager.Instance.UpdateExpectedMiningAmount();
            return;
        }

        // 배치된 어시스턴트가 없는 경우 → 기존 선택 팝업 로직 사용
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
                if (slotIndex >= 0 && slotIndex < group.slots.Count)
                {
                    slot.Assign(selected, DateTime.Now);
                    slotUI.AssignAssistant(selected);
                    ClearSlotAssistant(mineIdx, slotUI);

                    // **다시 등록 시, Slot에 저장된 버프/쿨타임 전달!**
                    bool isBuff = slot.LastIsBuffActive;
                    float buffRemain = slot.LastBuffRemain;
                    bool isCool = slot.LastIsCooldown;
                    float coolRemain = slot.LastCooldownRemain;

                    SpawnAssistantInMine(mineIdx, slotUI, selected, isBuff, buffRemain, isCool, coolRemain);

                    MineResourceCollectManager.Instance.UpdateExpectedMiningAmount();

                    // ***중복 사용 방지 위해 재등록시 버프상태 Slot->초기화!***
                    slot.LastIsBuffActive = false;
                    slot.LastBuffRemain = 0f;
                    slot.LastIsCooldown = false;
                    slot.LastCooldownRemain = 0f;
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

    void SpawnAssistantInMine(int mineIdx, MineAssistantSlotUI slotUI, AssistantInstance assistant,
    bool isBuffActive, float buffRemain, bool isCooldown, float cooldownRemain)
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
            {
                fsm.Init(assistant, mineIdx, assistantsRoot.gameObject, this);

                // FSM 상태 동기화!
                fsm.LoadBuffState(isBuffActive, buffRemain, isCooldown, cooldownRemain);

                // --- FSM 등록 ---
                var slotObjRef = slotUI.gameObject.GetComponent<MineAssistantSlotUIObjectRef>();
                if (slotObjRef != null && slotObjRef.spawnedObject != null)
                    slotFsmDict.Remove(group.slots[group.slotUIs.IndexOf(slotUI)]);
                slotFsmDict[group.slots[group.slotUIs.IndexOf(slotUI)]] = fsm;
            }

            slotUI.gameObject.GetComponent<MineAssistantSlotUIObjectRef>()?.SetAssistant(go);
            spawnedAssistants[mineIdx].Add(go);
        };
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
            {
                fsm.Init(assistant, mineIdx, assistantsRoot.gameObject, this);

                // --- FSM 등록 ---
                var slotObjRef = slotUI.gameObject.GetComponent<MineAssistantSlotUIObjectRef>();
                if (slotObjRef != null && slotObjRef.spawnedObject != null)
                    slotFsmDict.Remove(group.slots[group.slotUIs.IndexOf(slotUI)]);
                slotFsmDict[group.slots[group.slotUIs.IndexOf(slotUI)]] = fsm;
            }

            slotUI.gameObject.GetComponent<MineAssistantSlotUIObjectRef>()?.SetAssistant(go);
            spawnedAssistants[mineIdx].Add(go);
        };
    }


    public void ClearSlotAssistant(int mineIdx, MineAssistantSlotUI slotUI)
    {
        var objRef = slotUI.GetComponent<MineAssistantSlotUIObjectRef>();
        if (objRef != null && objRef.spawnedObject != null)
        {
            // --- FSM 해제 ---
            var group = mineGroups[mineIdx];
            int slotIndex = group.slotUIs.IndexOf(slotUI);
            if (slotIndex >= 0 && slotIndex < group.slots.Count)
                slotFsmDict.Remove(group.slots[slotIndex]);

            Destroy(objRef.spawnedObject);
            spawnedAssistants[mineIdx].Remove(objRef.spawnedObject);
            objRef.spawnedObject = null;
        }
    }

    public float GetBuffMultiplierForSlot(MineAssistantSlot slot)
    {
        if (slot == null) return 1.0f;
        if (slotFsmDict.TryGetValue(slot, out var fsm))
            return fsm != null ? fsm.ResourceBuffMultiplier() : 1.0f;
        return 1.0f;
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

    private void SetUICameraActive(bool active)
    {
        var uiCamObj = GameObject.FindWithTag("UICamera");
        if (uiCamObj != null)
        {
            if (uiCamObj.activeSelf != active)
                uiCamObj.SetActive(active);

            var cam = uiCamObj.GetComponent<Camera>();
            if (cam != null && cam.enabled != active)
                cam.enabled = active;
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
        SetUICameraActive(true);

        LoadSceneManager.Instance.UnLoadScene(SceneType.MineScene, () =>
        {
            SceneCameraState.IsMineSceneActive = false;
        });
    }


    public void LoadFromSaveData(MineSaveData saveData)
    {
        if (saveData == null)
        {
            ResetUnlockedMines();
            return;
        }

        if (saveData.UnlockedMines != null && saveData.UnlockedMines.Count == mineGroups.Count)
            unlockedMines = new List<bool>(saveData.UnlockedMines);
        else
            ResetUnlockedMines();

        var slotBuffStateDict = new Dictionary<MineAssistantSlot, (bool, float, bool, float)>();

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

                slotBuffStateDict[group.slots[slotIdx]] =
                    (slotSave.IsBuffActive, slotSave.BuffRemain, slotSave.IsCooldown, slotSave.CooldownRemain);

                if (assistant != null)
                {
                    var slotUI = group.slotUIs[slotIdx];
                    ClearSlotAssistant(groupIdx, slotUI);

                    var buffState = slotBuffStateDict[group.slots[slotIdx]];
                    SpawnAssistantInMine(groupIdx, slotUI, assistant, buffState.Item1, buffState.Item2, buffState.Item3, buffState.Item4);
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
        RefreshAllMineBlocks();
    }



    public void ResetUnlockedMines()
    {
        unlockedMines = new List<bool> { true, false, false, false };
        RefreshAllMineBlocks();
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
                }
            }
        }
    }

    private void RemoveInactiveAssistantsFromMine()
    {
        foreach (var group in mineGroups)
        {
            for (int i = 0; i < group.slots.Count; i++)
            {
                var slot = group.slots[i];
                var assi = slot.AssignedAssistant;

                if (slot.IsAssigned && assi != null && !assi.IsInUse)
                {
                    slot.Unassign();
                    group.slotUIs[i].AssignAssistant(null);
                    ClearSlotAssistant(mineGroups.IndexOf(group), group.slotUIs[i]);
                }
            }
        }
    }

    public void ShowMineInfoPopup()
    {
        if (spawnedMineInfoPopup != null)
        {
            return;
        }

        if (mineInfoPopupPrefab == null || mineInfoPopupRoot == null)
        {
            return;
        }

        GameObject go = Instantiate(mineInfoPopupPrefab, mineInfoPopupRoot);
        spawnedMineInfoPopup = go.GetComponent<MineInfoPopup>();
        if (spawnedMineInfoPopup == null)
        {
            Destroy(go);
            return;
        }

        spawnedMineInfoPopup.onPopupClosed = () =>
        {
            spawnedMineInfoPopup = null;
        };

        spawnedMineInfoPopup.Show();
    }


}