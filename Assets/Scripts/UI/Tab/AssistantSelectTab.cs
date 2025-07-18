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
            var slot = slotObj.GetComponent<MineAssistantSlotUI>();
            if (slot == null)
            {
                Debug.LogError($"MineAssistantSlotUI 컴포넌트가 {slotObj.name} 프리팹에 없습니다.");
                continue;
            }
            slot.Init(assistantInventory);
            // ⭐⭐⭐ 절대 SetSlot 호출 금지! (임시)
            slot.SetTempAssistant(assistant, assistantInstance =>
            {
                // 팝업 슬롯에서 selectCallback만 호출(씬 슬롯에 전달됨)
                selectCallback?.Invoke(assistantInstance);
            });
            idx++;
        }
        for (int i = idx; i < pool.Count; i++)
            pool[i].SetActive(false);
    }


}
