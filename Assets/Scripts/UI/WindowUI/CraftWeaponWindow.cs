using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class CraftWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform inputWeaponSlots;
    [SerializeField] private GameObject weaponSlotBtnPrefab;
    [SerializeField] private Transform recipeListRoot;
    [SerializeField] private GameObject recipeSlotPrefab;
    [SerializeField] private Transform tabsRoot;
    [SerializeField] private ScrollRect tabsScrollRect;

    private List<InputSlotContext> inputSlots = new();
    private List<Button> tabButtons = new();
    private List<CustomerJob> tabJobTypes = new();
    private int selectedTabIndex = -1;
    private int slotCount = 6;

    private bool isDraggingTabs = false;
    private float lastTabDragEndTime = -100f;
    private float tabClickCooldown = 0.25f;

    private GameManager gameManager;
    private UIManager uIManager;
    private ItemDataLoader itemLoader;
    private CraftingDataLoader craftingLoader;

    private Dictionary<CustomerJob, List<string>> jobToWeaponKeyList = new()
    {
        { CustomerJob.Woodcutter, new List<string> { "weapon_axe_crude", "weapon_axe_normal", "weapon_axe_fine", "weapon_axe_excellent", "weapon_axe_perfect" } },
        { CustomerJob.Farmer,     new List<string> { "weapon_hoe_crude", "weapon_hoe_normal", "weapon_hoe_fine", "weapon_hoe_excellent", "weapon_hoe_perfect" } },
        { CustomerJob.Miner,      new List<string> { "weapon_pickaxe_crude", "weapon_pickaxe_normal", "weapon_pickaxe_fine", "weapon_pickaxe_excellent", "weapon_pickaxe_perfect" } },
        { CustomerJob.Warrior,    new List<string> { "weapon_sword_crude", "weapon_sword_normal", "weapon_sword_fine", "weapon_sword_excellent", "weapon_sword_perfect" } },
        { CustomerJob.Archer,     new List<string> { "weapon_bow_crude", "weapon_bow_normal", "weapon_bow_fine", "weapon_bow_excellent", "weapon_bow_perfect" } },
        { CustomerJob.Tanker,     new List<string> { "weapon_shield_crude", "weapon_shield_normal", "weapon_shield_fine", "weapon_shield_excellent", "weapon_shield_perfect" } },
        { CustomerJob.Assassin,   new List<string> { "weapon_dagger_crude", "weapon_dagger_normal", "weapon_dagger_fine", "weapon_dagger_excellent", "weapon_dagger_perfect" } },
    };

    private class InputSlotContext
    {
        public Button Btn;
        public Image Icon;
        public Image ProgressBar;
        public TMP_Text TimeText;
        public Button ReceiveBtn;
        public int TaskIndex;
    }

    private void Awake()
    {
        if (tabsScrollRect != null)
        {
            tabsScrollRect.onValueChanged.AddListener(OnTabsScroll);
        }
    }

    private void OnEnable()
    {
        if (tabsScrollRect != null)
        {
            EventTrigger trigger = tabsScrollRect.GetComponent<EventTrigger>();
            if (trigger == null) trigger = tabsScrollRect.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            // Begin Drag
            EventTrigger.Entry begin = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
            begin.callback.AddListener((data) => { isDraggingTabs = true; });
            trigger.triggers.Add(begin);

            // End Drag
            EventTrigger.Entry end = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
            end.callback.AddListener((data) => {
                isDraggingTabs = false;
                lastTabDragEndTime = Time.unscaledTime;
            });
            trigger.triggers.Add(end);
        }
    }

    private void OnTabsScroll(Vector2 pos)
    {
        // 드래그 중이면 아무것도 하지 않는다.
    }

    public override void Init(GameManager gm, UIManager ui)
    {
        base.Init(gm, ui);
        gameManager = gm;
        uIManager = ui;
        itemLoader = gm.DataManager.ItemLoader;
        craftingLoader = gm.DataManager.CraftingLoader;

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.CraftWeaponWindow));

        tabButtons.Clear();
        tabJobTypes.Clear();
        foreach (Transform t in tabsRoot)
        {
            var btn = t.GetComponent<Button>();
            tabButtons.Add(btn);
        }
        tabJobTypes.Add(CustomerJob.Woodcutter);
        tabJobTypes.Add(CustomerJob.Farmer);
        tabJobTypes.Add(CustomerJob.Miner);
        tabJobTypes.Add(CustomerJob.Warrior);
        tabJobTypes.Add(CustomerJob.Archer);
        tabJobTypes.Add(CustomerJob.Tanker);
        tabJobTypes.Add(CustomerJob.Assassin);

        for (int i = 0; i < tabButtons.Count; i++)
        {
            int idx = i;
            tabButtons[i].onClick.RemoveAllListeners();
            tabButtons[i].onClick.AddListener(() => OnTabClicked(idx));
        }

        foreach (Transform child in inputWeaponSlots)
            Destroy(child.gameObject);
        inputSlots.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = Instantiate(weaponSlotBtnPrefab, inputWeaponSlots);
            var ctx = new InputSlotContext();
            ctx.Btn = go.GetComponent<Button>();
            ctx.Icon = go.transform.Find("Icon")?.GetComponent<Image>() ?? go.GetComponent<Image>();
            ctx.ProgressBar = go.transform.Find("CraftProgressBarBG/CraftProgressBar")?.GetComponent<Image>();
            ctx.TimeText = go.transform.Find("CraftProgressBarBG/CraftProgressText")?.GetComponent<TMP_Text>();
            ctx.ReceiveBtn = go.transform.Find("ReceiveButton")?.GetComponent<Button>();
            ctx.TaskIndex = i;

            int idx = i;
            ctx.Btn.onClick.RemoveAllListeners();
            ctx.Btn.interactable = false;
            if (ctx.ReceiveBtn)
            {
                ctx.ReceiveBtn.onClick.RemoveAllListeners();
                ctx.ReceiveBtn.onClick.AddListener(() => OnClickReceive(idx));
                ctx.ReceiveBtn.gameObject.SetActive(false);
            }
            ctx.Icon.sprite = null;
            ctx.Icon.enabled = false;
            ctx.ProgressBar?.gameObject.SetActive(false);
            ctx.TimeText?.gameObject.SetActive(false);

            inputSlots.Add(ctx);
        }

        OnTabClicked(0);
    }

    private void OnTabClicked(int tabIdx)
    {
        // 드래그 끝난지 얼마 안됐으면 CenterTab 호출하지 않는다.
        if (Time.unscaledTime - lastTabDragEndTime < tabClickCooldown)
            return;

        selectedTabIndex = tabIdx;
        for (int i = 0; i < tabButtons.Count; i++)
            tabButtons[i].interactable = (i != tabIdx);

        ShowRecipeListByJob(tabJobTypes[tabIdx]);
        CenterTab(tabIdx);
    }

    private void ShowRecipeListByJob(CustomerJob jobType)
    {
        foreach (Transform child in recipeListRoot)
            Destroy(child.gameObject);

        List<string> validKeys = jobToWeaponKeyList.TryGetValue(jobType, out var keys) ? keys : null;
        if (validKeys == null) return;

        var recipes = craftingLoader.CraftingList
            .Where(x => validKeys.Contains(x.ItemKey))
            .ToList();

        foreach (var data in recipes)
        {
            GameObject go = Instantiate(recipeSlotPrefab, recipeListRoot);
            var slot = go.GetComponent<RecipeSlot>();
            slot.Setup(
                data,
                itemLoader,
                gameManager.Forge,
                gameManager.Inventory,
                () => OnRecipeSelected(data)
            );
        }
    }

    private void CenterTab(int tabIdx)
    {
        if (tabsScrollRect == null || tabIdx < 0 || tabIdx >= tabButtons.Count) return;
        var tabRect = tabButtons[tabIdx].GetComponent<RectTransform>();
        var contentRect = tabsRoot.GetComponent<RectTransform>();
        var scrollRect = tabsScrollRect.GetComponent<RectTransform>();

        float contentWidth = contentRect.rect.width;
        float scrollWidth = scrollRect.rect.width;

        float tabPos = tabRect.anchoredPosition.x + (tabRect.rect.width / 2f);
        float normalized = Mathf.Clamp01((tabPos - scrollWidth / 2f) / (contentWidth - scrollWidth));
        tabsScrollRect.horizontalNormalizedPosition = normalized;
    }

    void OnClickInputWeaponSlot(int index) { }

    void OnRecipeSelected(CraftingData craftingData)
    {
        int idx = inputSlots.FindIndex(slot =>
            gameManager.CraftingManager.GetCraftTask(slot.TaskIndex)?.isCrafting == false);
        if (idx < 0) return;

        var inventory = gameManager.Inventory;
        var forge = gameManager.Forge;
        if (inventory == null || forge == null)
            return;

        var required = craftingData.RequiredResources
            .Select(r => (r.ResourceKey, r.Amount)).ToList();
        int goldNeed = (int)craftingData.craftCost;

        if (!forge.UseGold(goldNeed))
            return;
        if (!inventory.UseCraftingMaterials(required))
        {
            forge.AddGold(goldNeed);
            return;
        }

        var itemData = gameManager.DataManager.ItemLoader.GetItemByKey(craftingData.ItemKey);
        Sprite iconSprite = IconLoader.GetIcon(itemData.IconPath);

        var slot = inputSlots[idx];
        slot.Icon.sprite = iconSprite;
        slot.Icon.enabled = (iconSprite != null);

        gameManager.CraftingManager.StartCrafting(idx, craftingData, itemData);
        slot.Btn.interactable = false;
    }

    void Update()
    {
        for (int i = 0; i < inputSlots.Count; i++)
        {
            var prog = gameManager.CraftingManager.GetCraftTask(i);
            var slot = inputSlots[i];

            if (prog.isCrafting && prog.data != null)
            {
                Sprite sp = IconLoader.GetIcon(prog.itemData.IconPath);
                slot.Icon.sprite = sp;
                slot.Icon.enabled = (sp != null);

                slot.ProgressBar?.gameObject.SetActive(true);
                slot.ProgressBar.fillAmount = prog.timeLeft / prog.totalTime;
                slot.TimeText?.gameObject.SetActive(true);
                slot.TimeText.text = $"{prog.timeLeft:0.0}s";

                slot.Btn.interactable = false;
                slot.ReceiveBtn.gameObject.SetActive(false);
            }
            else
            {
                slot.ProgressBar?.gameObject.SetActive(false);
                slot.TimeText?.gameObject.SetActive(false);

                if (prog.rewardGiven && prog.itemData != null)
                {
                    slot.ReceiveBtn.gameObject.SetActive(true);
                    slot.Btn.interactable = false;
                }
                else
                {
                    slot.ReceiveBtn.gameObject.SetActive(false);
                    slot.Btn.interactable = false;
                    slot.Icon.sprite = null;
                    slot.Icon.enabled = false;
                }
            }
        }
    }

    void OnClickReceive(int index)
    {
        var prog = gameManager.CraftingManager.GetCraftTask(index);
        if (!prog.rewardGiven || prog.itemData == null) return;

        // RewardPopup 연동
        ShowCraftReward(prog.itemData);

        prog.Reset();
        var slot = inputSlots[index];
        slot.Icon.sprite = null;
        slot.Icon.enabled = false;
        slot.Btn.interactable = false;
        slot.ReceiveBtn.gameObject.SetActive(false);
    }

    public override void Open()
    {
        base.Open();
        selectedTabIndex = -1;
    }

    private void ShowCraftReward(ItemData itemData)
    {
        if (itemData == null || uIManager == null || itemLoader == null)
            return;

        // 리워드 정보 생성 (여러개면 리스트)
        var rewardList = new List<(string itemKey, int count)>()
    {
        (itemData.ItemKey, 1)
    };

        var rewardPopup = uIManager.OpenUI<RewardPopup>(UIName.RewardPopup);
        rewardPopup.Show(rewardList, itemLoader, "장비 제작 성공!");
    }
}
