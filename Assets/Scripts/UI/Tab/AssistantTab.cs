using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssistantTab : BaseTab
{
    private AssistantManager assistantManager;

    [Header("Tab Buttons")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color defaultColor;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabPanels;

    [Header("To Create Assistant")]
    [SerializeField] GameObject assiSlotPrefab;
    [SerializeField] Transform craftSlotRoot;
    [SerializeField] Transform gemSlotRoot;
    [SerializeField] Transform upgradeSlotRoot;

    [Header("Selected Assistant")]
    [SerializeField] AssiEquippedSlot craftAssi;
    [SerializeField] AssiEquippedSlot enhanceAssi;
    [SerializeField] AssiEquippedSlot sellingAssi;

    [Header("To Create Bonus Stat")]
    [SerializeField] GameObject bonusStatPrefab;
    [SerializeField] Transform craftStatRoot;
    [SerializeField] Transform enhanceStatRoot;
    [SerializeField] Transform sellingStatRoot;

    private Queue<GameObject> pooledSlots = new Queue<GameObject>();
    private List<GameObject> activeSlots = new List<GameObject>();
    private bool isInit = false;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => SwitchTab(index));
        }

        SwitchTab(0);

        assistantManager = gameManager.AssistantManager;

        craftAssi.Init(uIManager);
        enhanceAssi.Init(uIManager);
        sellingAssi.Init(uIManager);
    }

    public override void OpenTab()
    {
        base.OpenTab();
        gameManager.Forge.Events.OnAssistantChanged += SetAssistant;
        RefreshSlots();
    }

    public override void CloseTab()
    {
        base.CloseTab();
        gameManager.Forge.Events.OnAssistantChanged -= SetAssistant;
    }

    private void RefreshSlots()
    {
        if (assistantManager == null)
            assistantManager = GameManager.Instance.AssistantManager;

        foreach (var slot in activeSlots)
        {
            slot.SetActive(false);
            pooledSlots.Enqueue(slot);
        }
        activeSlots.Clear();

        List<AssistantData> craftingList = assistantManager.GetAssistantsByType(SpecializationType.Crafting);
        List<AssistantData> enhancingList = assistantManager.GetAssistantsByType(SpecializationType.Enhancing);
        List<AssistantData> sellingList = assistantManager.GetAssistantsByType(SpecializationType.Selling);

        CreateSlots(craftingList, craftSlotRoot);
        CreateSlots(enhancingList, upgradeSlotRoot);
        CreateSlots(sellingList, gemSlotRoot);

        if (!isInit)
        {
            foreach (var assi in assistantManager.AssistantInventory.GetEquippedAssistants())
            {
                SetAssistant(assi, true);
            }
            isInit = true;
        }
    }

    private void CreateSlots(List<AssistantData> assiList, Transform parent)
    {
        foreach (var assi in assiList)
        {
            if (assi == null) continue;

            GameObject slotObj = GetSlotFromPool();
            slotObj.transform.SetParent(parent, false);
            slotObj.SetActive(true);

            var slot = slotObj.GetComponent<AssistantSlot>();
            slot.Init(assi, null);

            activeSlots.Add(slotObj);
        }
    }

    private void SwitchTab(int index)
    {
        for (int i = 0; i < tabPanels.Length; i++)
        {
            bool isSelected = i == index;
            tabPanels[i].SetActive(isSelected);
            tabButtons[i].image.color = isSelected ? selectedColor : defaultColor;
        }
    }

    private GameObject GetSlotFromPool()
    {
        if (pooledSlots.Count > 0)
            return pooledSlots.Dequeue();
        return Instantiate(assiSlotPrefab);
    }

    private void SetAssistant(AssistantData assi, bool isActive)
    {
        var type = AssistantInventory.SpecializationKeyToType(assi.specializationKey);
        switch (type)
        {
            case SpecializationType.Crafting:
                craftAssi.SetAssistant(assi);
                break;
            case SpecializationType.Enhancing:
                enhanceAssi.SetAssistant(assi);
                break;
            case SpecializationType.Selling:
                sellingAssi.SetAssistant(assi);
                break;
        }
        // 보너스 스탯 구현 필요시 아래 함수 사용
        // ShowAssistantStat(assi, isActive);
    }
}
