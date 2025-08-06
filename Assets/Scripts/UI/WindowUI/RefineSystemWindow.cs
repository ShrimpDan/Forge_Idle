using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RefineSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI")]
    [SerializeField] private Button exitButton;
    [SerializeField] private Button gemTabButton;
    [SerializeField] private Button ingotTabButton;
    [SerializeField] private RefineOutputSlot outputSlot;
    [SerializeField] private Transform inputListRoot;
    [SerializeField] private GameObject inputItemSlotPrefab;
    [SerializeField] private Transform refineListRoot;
    [SerializeField] private GameObject refineListSlotPrefab;

    [Header("Amount UI")]
    [SerializeField] private Button minusBtn;
    [SerializeField] private Button plusBtn;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Button craftBtn;

    [Header("Popup Prefabs & Root")]
    [SerializeField] private LackPopup lackPopupPrefab;
    [SerializeField] private Transform popupParent;
    [SerializeField] private RewardPopup rewardPopupPrefab; 
    private DataManager dataManager;
    private List<ItemData> gemList = new();
    private List<ItemData> ingotList = new();
    private List<RefineInputItemSlot> inputSlots = new();
    private List<RefineListSlot> refineListSlots = new();

    private ItemData selectedOutput;
    private List<ItemData> currentList;

    private int baseRequiredAmount = 5;
    private int craftAmount = 1;
    private int maxCraftAmount = 99;

    public override void Init(GameManager gameManager, UIManager uiManager)
    {
        base.Init(gameManager, uiManager);
        dataManager = gameManager.DataManager;

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => uiManager.CloseUI(UIName.RefineSystemWindow));

        gemTabButton.onClick.RemoveAllListeners();
        gemTabButton.onClick.AddListener(ShowGemTab);

        ingotTabButton.onClick.RemoveAllListeners();
        ingotTabButton.onClick.AddListener(ShowIngotTab);

        minusBtn.onClick.RemoveAllListeners();
        minusBtn.onClick.AddListener(() => { SetCraftAmount(craftAmount - 1); });

        plusBtn.onClick.RemoveAllListeners();
        plusBtn.onClick.AddListener(() => { SetCraftAmount(craftAmount + 1); });

        craftBtn.onClick.RemoveAllListeners();
        craftBtn.onClick.AddListener(OnClickCraft);

        var all = dataManager.ItemLoader.ItemList;
        gemList = all.Where(x => x.ItemKey.StartsWith("gem_")).ToList();
        ingotList = all.Where(x => x.ItemKey.StartsWith("ingot_")).ToList();

        ShowGemTab();
    }

    private void ShowGemTab()
    {
        currentList = gemList;
        UpdateRefineList();
        SelectOutput(null);
    }

    private void ShowIngotTab()
    {
        currentList = ingotList;
        UpdateRefineList();
        SelectOutput(null);
    }

    private void UpdateRefineList()
    {
        foreach (Transform child in refineListRoot) Destroy(child.gameObject);
        refineListSlots.Clear();

        foreach (var data in currentList)
        {
            var go = Instantiate(refineListSlotPrefab, refineListRoot);
            var slot = go.GetComponent<RefineListSlot>();
            slot.Set(data, () => SelectOutput(data));
            refineListSlots.Add(slot);
        }
        var scroll = refineListRoot.GetComponentInParent<ScrollRect>();
        if (scroll != null) scroll.verticalNormalizedPosition = 1f;
    }

    private void SelectOutput(ItemData data)
    {
        selectedOutput = data;
        craftAmount = 1;
        UpdateAmountUI();

        outputSlot.Set(data);

        foreach (Transform child in inputListRoot) Destroy(child.gameObject);
        inputSlots.Clear();

        if (data == null) return;

        string resourceKey = GetResourceKeyForOutput(data);
        if (string.IsNullOrEmpty(resourceKey)) return;

        int owned = 0;
        var invList = GameManager.Instance.Inventory.ResourceList;
        if (invList != null)
            owned = invList.Where(inst => inst.ItemKey == resourceKey).Sum(inst => inst.Quantity);

        int required = baseRequiredAmount * craftAmount;
        var slotGo = Instantiate(inputItemSlotPrefab, inputListRoot);
        var slot = slotGo.GetComponent<RefineInputItemSlot>();
        slot.Set(
            dataManager.ItemLoader.GetItemByKey(resourceKey),
            owned,
            required
        );
        inputSlots.Add(slot);
    }

    private void SetCraftAmount(int value)
    {
        if (selectedOutput == null) return;
        craftAmount = Mathf.Clamp(value, 1, maxCraftAmount);

        UpdateAmountUI();

        string resourceKey = GetResourceKeyForOutput(selectedOutput);
        if (string.IsNullOrEmpty(resourceKey) || inputSlots.Count == 0) return;

        int owned = 0;
        var invList = GameManager.Instance.Inventory.ResourceList;
        if (invList != null)
            owned = invList.Where(inst => inst.ItemKey == resourceKey).Sum(inst => inst.Quantity);

        int required = baseRequiredAmount * craftAmount;
        inputSlots[0].Set(dataManager.ItemLoader.GetItemByKey(resourceKey), owned, required);
    }

    private void UpdateAmountUI()
    {
        if (amountText != null)
            amountText.text = craftAmount.ToString();
    }

    private void OnClickCraft()
    {
        if (selectedOutput == null || inputSlots.Count == 0) return;
        string resourceKey = GetResourceKeyForOutput(selectedOutput);
        int totalNeed = baseRequiredAmount * craftAmount;

        int owned = GameManager.Instance.Inventory.ResourceList
            .Where(x => x.ItemKey == resourceKey).Sum(x => x.Quantity);

        if (owned < totalNeed)
        {
            ShowLackPopup(LackType.Resource);
            return;
        }

        GameManager.Instance.Inventory.UseCraftingMaterials(new List<(string resourceKey, int amount)> { (resourceKey, totalNeed) });

        var outItem = dataManager.ItemLoader.GetItemByKey(selectedOutput.ItemKey);
        GameManager.Instance.Inventory.AddItem(outItem, craftAmount);

        ShowRewardPopup(outItem, craftAmount);

        SetCraftAmount(1);
        SelectOutput(selectedOutput);
    }

    private void ShowLackPopup(LackType type)
    {
        if (lackPopupPrefab != null)
        {
            var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
            popup.Init(GameManager.Instance, uIManager);
            popup.Show(type);
        }
        else
        {
            Debug.LogWarning("LackPopup 프리팹이 인스펙터에 할당되어 있지 않습니다.");
        }
    }

    private void ShowRewardPopup(ItemData outItem, int amount)
    {
        SoundManager.Instance?.Play("SFX_SystemReward");

        if (uIManager != null)
        {
            var rewardList = new List<(string itemKey, int count)> { (outItem.ItemKey, amount) };
            var popup = uIManager.OpenUI<RewardPopup>(UIName.RewardPopup);
            popup.Show(rewardList, dataManager.ItemLoader, "획득 보상");
        }
        else if (rewardPopupPrefab != null)
        {
            var popup = Instantiate(rewardPopupPrefab, popupParent ? popupParent : null);
            popup.Init(GameManager.Instance, uIManager);
            var rewardList = new List<(string itemKey, int count)> { (outItem.ItemKey, amount) };
            popup.Show(rewardList, dataManager.ItemLoader, "획득 보상");
        }
        else
        {
            Debug.LogWarning("RewardPopup 프리팹이 인스펙터에 할당되어 있지 않습니다.");
        }
    }

    private string GetResourceKeyForOutput(ItemData output)
    {
        if (output == null) return null;
        if (output.ItemKey.StartsWith("gem_")) return "resource_" + output.ItemKey.Substring(4);
        if (output.ItemKey.StartsWith("ingot_")) return "resource_" + output.ItemKey.Substring(6);
        return null;
    }
}
