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
            tabButtons[i].onClick.AddListener(() =>
            {
                SoundManager.Instance.Play("SFX_SystemClick");
                SwitchTab(index);
            });
        }

        SwitchTab(0);

        assistantManager = gameManager.AssistantManager;

        // 장착된 Assistant 슬롯 초기화
        craftAssi.Init(uIManager);
        enhanceAssi.Init(uIManager);
        sellingAssi.Init(uIManager);
    }

    public override void OpenTab()
    {
        base.OpenTab();

        if (gameManager.Forge != null)
            gameManager.Forge.Events.OnAssistantChanged += SetAssistant;

        RefreshSlots();
    }

    public override void CloseTab()
    {
        base.CloseTab();

        if (gameManager.Forge != null)
            gameManager.Forge.Events.OnAssistantChanged -= SetAssistant;
    }

    private void RefreshSlots()
    {
        if (assistantManager == null)
        {
            assistantManager = GameManager.Instance.AssistantManager;
        }

        foreach (var slot in activeSlots)
        {
            slot.SetActive(false);
            pooledSlots.Enqueue(slot);
        }
        activeSlots.Clear();

        List<AssistantInstance> craftingList = assistantManager.GetAssistantByType(SpecializationType.Crafting);
        List<AssistantInstance> enhancingList = assistantManager.GetAssistantByType(SpecializationType.Enhancing);
        List<AssistantInstance> sellingList = assistantManager.GetAssistantByType(SpecializationType.Selling);

        CreateSlots(craftingList, craftSlotRoot);
        CreateSlots(enhancingList, upgradeSlotRoot);
        CreateSlots(sellingList, gemSlotRoot);

        if (!isInit)
        {
            foreach (var assi in assistantManager.AssistantInventory.GetEquippedTrainees())
            {
                SetAssistant(assi, true);
            }
            isInit = true;
        }
    }

    private void CreateSlots(List<AssistantInstance> assiList, Transform parent)
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

    private void SetAssistant(AssistantInstance assi, bool isActive)
    {
        switch (assi.Specialization)
        {
            case SpecializationType.Crafting:
                craftAssi.SetAssistant(assi, isActive);
                break;

            case SpecializationType.Enhancing:
                enhanceAssi.SetAssistant(assi, isActive);
                break;

            case SpecializationType.Selling:
                sellingAssi.SetAssistant(assi, isActive);
                break;
        }

        ShowAssistantStat(assi, isActive);

        if (isActive)
            SoundManager.Instance.Play("SFX_UIEquip");
        else
            SoundManager.Instance.Play("SFX_UIUnequip");
    }

    private void ShowAssistantStat(AssistantInstance assi, bool isAcitve)
    {
        if (!isAcitve)
        {
            ClearStat(assi.Specialization);
            return;
        }

        Transform parent = null;

        switch (assi.Specialization)
        {
            case SpecializationType.Crafting:
                parent = craftStatRoot;
                break;

            case SpecializationType.Enhancing:
                parent = enhanceStatRoot;
                break;

            case SpecializationType.Selling:
                parent = sellingStatRoot;
                break;
        }

        ClearStat(assi.Specialization);

        foreach (var stat in assi.Multipliers)
        {
            GameObject obj = Instantiate(bonusStatPrefab, parent);
            if (obj.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.text = stat.AbilityName;
                tmp.text += $"\nx{stat.Multiplier:F2}";
            }
        }
    }

    private void ClearStat(SpecializationType type)
    {
        Transform parent = null;

        switch (type)
        {
            case SpecializationType.Crafting:
                parent = craftStatRoot;
                break;

            case SpecializationType.Enhancing:
                parent = enhanceStatRoot;
                break;

            case SpecializationType.Selling:
                parent = sellingStatRoot;
                break;
        }

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
