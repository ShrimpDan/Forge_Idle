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
    [SerializeField] private GameObject assistantSlotPrefab; // ⭐ 반드시 MineAssistantSlotUI 프리팹이어야 함

    private AssistantInventory assistantInventory;
    private Action<AssistantInstance> selectCallback;

    private enum TabType { Craft, Mine, Sell }
    private TabType curTab = TabType.Craft;

    private List<GameObject> craftPool = new();
    private List<GameObject> miningPool = new();
    private List<GameObject> sellPool = new();

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

    public void OpenForSelection(Action<AssistantInstance> callback, bool preventPopup = false)
    {
        selectCallback = callback;
        RefreshAllTabs();
        SwitchTab(curTab);
    }

    private void SwitchTab(TabType tab)
    {
        curTab = tab;
        craftRoot.gameObject.SetActive(tab == TabType.Craft);
        miningRoot.gameObject.SetActive(tab == TabType.Mine);
        sellRoot.gameObject.SetActive(tab == TabType.Sell);
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

        var assistants = assistantInventory?.GetBySpecialization(type) ?? new List<AssistantInstance>();
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

            // ⭐⭐ 여기 반드시 MineAssistantSlotUI!
            var slot = slotObj.GetComponent<MineAssistantSlotUI>();
            if (slot == null)
            {
                Debug.LogError($"MineAssistantSlotUI 컴포넌트가 {slotObj.name} 프리팹에 없습니다.");
                continue;
            }
            slot.Init(assistantInventory); // 인벤토리 전달(중요)
            slot.SetTempAssistant(assistant, OnSelectAssistant); // 슬롯을 임시 미리보기 용으로 세팅
            idx++;
        }
        for (int i = idx; i < pool.Count; i++)
            pool[i].SetActive(false);
    }

    // 호출시, 선택된 assistant가 전달됨
    private void OnSelectAssistant(AssistantInstance assistant)
    {
        selectCallback?.Invoke(assistant);
    }
}
