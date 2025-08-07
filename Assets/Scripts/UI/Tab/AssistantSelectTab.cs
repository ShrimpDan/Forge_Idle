using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class AssistantSelectTab : MonoBehaviour
{
    [SerializeField] private Button craftTabBtn;
    [SerializeField] private Button miningTabBtn;
    [SerializeField] private Button sellTabBtn;
    [SerializeField] private Transform craftRoot;
    [SerializeField] private Transform miningRoot;
    [SerializeField] private Transform sellRoot;
    [SerializeField] private GameObject assistantSlotPrefab; // 팝업에 임시 슬롯용 프리팹

    private AssistantInventory assistantInventory;
    private Action<AssistantInstance> selectCallback;

    private enum TabType { Craft, Mine, Sell }
    private TabType curTab = TabType.Craft;

    private List<GameObject> craftPool = new();
    private List<GameObject> miningPool = new();
    private List<GameObject> sellPool = new();

    private bool mineSceneAssign = false; // 마인씬에서 어시스턴트 할당할 때만 true

    public void Init(AssistantInventory inventory)
    {
        assistantInventory = inventory;
        craftTabBtn.onClick.RemoveAllListeners();
        craftTabBtn.onClick.AddListener(() => SwitchTab(TabType.Craft));
        miningTabBtn.onClick.RemoveAllListeners();
        miningTabBtn.onClick.AddListener(() => SwitchTab(TabType.Mine));
        sellTabBtn.onClick.RemoveAllListeners();
        sellTabBtn.onClick.AddListener(() => SwitchTab(TabType.Sell));
        SwitchTab(TabType.Craft); // 기본탭
    }

    /// <summary>
    /// isMineOrQuestAssign: 마인씬 팝업인지 여부 (true: 마인씬 등에서 사용중/장착/임무 중인 어시스턴트는 노출 안함)
    /// </summary>
    public void OpenForSelection(Action<AssistantInstance> callback, bool isMineOrQuestAssign = false)
    {
        selectCallback = callback;
        mineSceneAssign = isMineOrQuestAssign;
        RefreshAllTabs();
        SwitchTab(curTab);
    }

    private void SwitchTab(TabType tab)
    {
        curTab = tab;
        craftRoot.gameObject.SetActive(tab == TabType.Craft);
        miningRoot.gameObject.SetActive(tab == TabType.Mine);
        sellRoot.gameObject.SetActive(tab == TabType.Sell);

        craftTabBtn.image.color = tab == TabType.Craft ? Color.white : Color.gray;
        miningTabBtn.image.color = tab == TabType.Mine ? Color.white : Color.gray;
        sellTabBtn.image.color = tab == TabType.Sell ? Color.white : Color.gray;
    }

    private void RefreshAllTabs()
    {
        RefreshTab(SpecializationType.Crafting, craftRoot, craftPool);
        RefreshTab(SpecializationType.Mining, miningRoot, miningPool);
        RefreshTab(SpecializationType.Selling, sellRoot, sellPool);
    }

    private void RefreshTab(SpecializationType type, Transform root, List<GameObject> pool)
    {
        foreach (var go in pool) go.SetActive(false);

        List<AssistantInstance> assistants;

        // 마인씬에서 할당할 때는 GetAvailableForMine()로 필터 후 해당 타입만
        if (mineSceneAssign)
            assistants = assistantInventory?.GetAvailableForMine().FindAll(a => a.Specialization == type) ?? new List<AssistantInstance>();
        else
            assistants = assistantInventory?.GetBySpecialization(type) ?? new List<AssistantInstance>();

        int idx = 0;
        foreach (var assistant in assistants)
        {
            GameObject slotObj;
            if (idx < pool.Count)
            {
                slotObj = pool[idx];
                slotObj.SetActive(true);
            }
            else
            {
                slotObj = Instantiate(assistantSlotPrefab, root);
                pool.Add(slotObj);
            }
            var slot = slotObj.GetComponent<MineAssistantSlotUI>();
            if (slot == null)
            {
                Debug.LogError($"MineAssistantSlotUI 컴포넌트가 {slotObj.name} 프리팹에 없습니다.");
                continue;
            }
            slot.Init(assistantInventory);
            slot.SetTempAssistant(assistant, assistantInstance =>
            {
                selectCallback?.Invoke(assistantInstance);
            });
            idx++;
        }
        for (int i = idx; i < pool.Count; i++)
            pool[i].SetActive(false);
    }
}
