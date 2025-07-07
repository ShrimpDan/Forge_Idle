using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class AssistantSelectTab : MonoBehaviour
{
    [Header("ÅÇ ¹öÆ°")]
    [SerializeField] private Button craftTabBtn;
    [SerializeField] private Button enhanceTabBtn;
    [SerializeField] private Button sellTabBtn;

    [Header("ÅÇ Root")]
    [SerializeField] private Transform craftRoot;
    [SerializeField] private Transform enhanceRoot;
    [SerializeField] private Transform sellRoot;

    [Header("½½·Ô ÇÁ¸®ÆÕ")]
    [SerializeField] private GameObject assistantSlotPrefab;

    private GameManager gameManager;
    private UIManager uiManager;

    private Action<TraineeData> selectCallback;

    private enum TabType { Craft, Enhance, Sell }
    private TabType curTab = TabType.Craft;

    private List<GameObject> craftPool = new();
    private List<GameObject> enhancePool = new();
    private List<GameObject> sellPool = new();

    public void Init(GameManager gameManager, UIManager uiManager)
    {
        this.gameManager = gameManager;
        this.uiManager = uiManager;

        craftTabBtn.onClick.RemoveAllListeners();
        craftTabBtn.onClick.AddListener(() => SwitchTab(TabType.Craft));
        enhanceTabBtn.onClick.RemoveAllListeners();
        enhanceTabBtn.onClick.AddListener(() => SwitchTab(TabType.Enhance));
        sellTabBtn.onClick.RemoveAllListeners();
        sellTabBtn.onClick.AddListener(() => SwitchTab(TabType.Sell));

        SwitchTab(TabType.Craft); // ±âº»°ª
    }

    public void OpenForSelection(Action<TraineeData> callback)
    {
        selectCallback = callback;
        RefreshAllTabs();
        SwitchTab(curTab);
    }

    private void SwitchTab(TabType tab)
    {
        curTab = tab;
        craftRoot.gameObject.SetActive(tab == TabType.Craft);
        enhanceRoot.gameObject.SetActive(tab == TabType.Enhance);
        sellRoot.gameObject.SetActive(tab == TabType.Sell);
    }

    private void RefreshAllTabs()
    {
        RefreshTab(SpecializationType.Crafting, craftRoot, craftPool);
        RefreshTab(SpecializationType.Enhancing, enhanceRoot, enhancePool);
        RefreshTab(SpecializationType.Selling, sellRoot, sellPool);
    }

    private void RefreshTab(SpecializationType type, Transform root, List<GameObject> pool)
    {
        foreach (var go in pool) go.SetActive(false);

        var trainees = gameManager?.AssistantManager?.TraineeInventory?.GetBySpecialization(type)
            ?? new List<TraineeData>();

        int idx = 0;
        foreach (var trainee in trainees)
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
            var slot = slotObj.GetComponent<AssistantSlot>();
            slot.Init(trainee, OnSelectAssistant);
            idx++;
        }
        for (int i = idx; i < pool.Count; i++)
            pool[i].SetActive(false);
    }

    private void OnSelectAssistant(TraineeData trainee)
    {
        selectCallback?.Invoke(trainee);
        if (uiManager != null)
            uiManager.CloseUI(UIName.AssistantSelectPopup);
    }
}
