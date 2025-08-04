using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssistantTab : BaseTab
{
    private AssistantManager assistantManager;
    private ForgeManager forgeManager;
    private ForgeAssistantHandler assistantHandler;

    [Header("Tab Buttons")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color defaultColor;

    [Header("Dismiss UI")]
    [SerializeField] private Button dismissButton;

    [Header("Wage UI")]
    [SerializeField] private GameObject wagePopup;
    [SerializeField] private Button wageButton;
    [SerializeField] private Button wageCloseButton;

    [Header("Tab Panels")]
    [SerializeField] private GameObject[] tabPanels;

    [Header("To Create Assistant")]
    [SerializeField] private GameObject assiSlotPrefab;
    [SerializeField] private Transform craftSlotRoot;
    [SerializeField] private Transform gemSlotRoot;
    [SerializeField] private Transform upgradeSlotRoot;

    [Header("Selected Assistant")]
    [SerializeField] private AssiEquippedSlot craftAssi;
    [SerializeField] private AssiEquippedSlot sellingAssi;

    [Header("To Create Bonus Stat")]
    [SerializeField] private GameObject bonusStatPrefab;
    [SerializeField] private Transform craftStatRoot;
    [SerializeField] private Transform sellingStatRoot;

    private Queue<GameObject> pooledSlots = new();
    private List<GameObject> activeSlots = new();

    private bool isDismissMode = false;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() =>
            {
                SoundManager.Instance.Play("ClickSound");
                SwitchTab(index);
            });
        }

        wageButton.onClick.AddListener(OnClickWageButton);
        wageCloseButton.onClick.AddListener(OnClickCloseWagePopup);
        dismissButton.onClick.AddListener(OnClickDismissButton);

        SwitchTab(0);

        assistantManager = gameManager.AssistantManager;
        forgeManager = gameManager.ForgeManager;

        if (assistantManager == null || forgeManager == null)
        {
            Debug.LogWarning("[AssistantTab] Manager가 아직 초기화되지 않았습니다.");
            return;
        }

        craftAssi.Init(uIManager);
        sellingAssi.Init(uIManager);

        wagePopup.SetActive(false);
        DismissManager.Instance?.SetDismissMode(false);
    }

    public override void OpenTab()
    {
        base.OpenTab();

        if (assistantManager == null || forgeManager == null)
        {
            assistantManager = GameManager.Instance.AssistantManager;
            forgeManager = GameManager.Instance.ForgeManager;

            if (assistantManager == null || forgeManager == null)
                return;
        }

        SetAssistant();
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        if (assistantManager == null)
            assistantManager = GameManager.Instance.AssistantManager;

        foreach (var slot in activeSlots)
        {
            slot.SetActive(false);
            pooledSlots.Enqueue(slot);
        }
        activeSlots.Clear();

        List<AssistantInstance> craftingList = assistantManager.GetAssistantByType(SpecializationType.Crafting);
        SortByEquippedFiredTier(craftingList);

        List<AssistantInstance> miningList = assistantManager.GetAssistantByType(SpecializationType.Mining);
        SortByEquippedFiredTier(miningList);

        List<AssistantInstance> sellingList = assistantManager.GetAssistantByType(SpecializationType.Selling);
        SortByEquippedFiredTier(sellingList);

        CreateSlots(craftingList, craftSlotRoot);
        CreateSlots(miningList, upgradeSlotRoot);
        CreateSlots(sellingList, gemSlotRoot);

        SetAssistant();
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

    public void SetAssistant()
    {
        foreach (var key in forgeManager.EquippedAssistant[forgeManager.CurrentForge.ForgeType].Keys)
        {
            AssistantInstance assi = forgeManager.EquippedAssistant[forgeManager.CurrentForge.ForgeType][key];

            switch (key)
            {
                case SpecializationType.Crafting:
                    craftAssi.SetAssistant(assi);
                    break;
                case SpecializationType.Selling:
                    sellingAssi.SetAssistant(assi);
                    break;
            }

            ShowAssistantStat();
            RefreshEquippedIndicators();
        }
    }

    private void ShowAssistantStat()
    {
        foreach (var type in forgeManager.EquippedAssistant[forgeManager.CurrentForge.ForgeType].Keys)
        {
            Transform parent = type switch
            {
                SpecializationType.Crafting => craftStatRoot,
                SpecializationType.Selling => sellingStatRoot,
                _ => null
            };

            ClearStat(parent);

            AssistantInstance assi = forgeManager.EquippedAssistant[forgeManager.CurrentForge.ForgeType][type];
            if (assi != null && !assi.IsFired)
            {
                foreach (var stat in assi.Multipliers)
                {
                    if (stat.Multiplier == 0) continue;

                    GameObject obj = Instantiate(bonusStatPrefab, parent);
                    if (obj.TryGetComponent(out TextMeshProUGUI tmp))
                    {
                        tmp.text = stat.AbilityName;

                        float percent = (stat.Multiplier - 1f) * 100f;
                        string sign = percent > 0 ? "+" : "";
                        string display = percent != 0 ? $"{sign}{percent:F0}%" : "0%";

                        tmp.text += $"\n{display}";
                    }
                }
            }
        }
    }

    private void ClearStat(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }

    public void RefreshEquippedIndicators()
    {
        foreach (var slotObj in activeSlots)
        {
            var slot = slotObj.GetComponent<AssistantSlot>();
            slot?.RefreshEquippedState();
        }
    }

    private void SortByEquippedFiredTier(List<AssistantInstance> list)
    {
        list.Sort((a, b) =>
        {
            int equippedCompare = b.IsEquipped.CompareTo(a.IsEquipped);
            if (equippedCompare != 0)
                return equippedCompare;

            int firedCompare = b.IsFired.CompareTo(a.IsFired);
            if (firedCompare != 0)
                return firedCompare;

            int aTier = a.Personality?.tier ?? 999;
            int bTier = b.Personality?.tier ?? 999;

            return aTier.CompareTo(bTier);
        });
    }

    private void OnClickWageButton()
    {
        SoundManager.Instance?.Play("ClickSound");
        wagePopup.SetActive(true);
        wagePopup.GetComponent<WagePopup>().Show();
    }

    private void OnClickCloseWagePopup()
    {
        SoundManager.Instance?.Play("ClickSound");
        wagePopup.SetActive(false);
    }

    private void OnClickDismissButton()
    {
        SoundManager.Instance?.Play("ClickSound");
        isDismissMode = !isDismissMode;
        DismissManager.Instance?.SetDismissMode(isDismissMode);
    }
}
