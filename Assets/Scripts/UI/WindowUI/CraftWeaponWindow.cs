using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class CraftWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("Input Weapon Slot Btns")]
    [SerializeField] private List<InputWeaponSlotBtn> inputWeaponSlotBtns; // Inspector에서 6개 슬롯 모두 연결

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private Transform inputWeaponSlots;
    [SerializeField] private GameObject weaponSlotBtnPrefab;
    [SerializeField] private Transform recipeListRoot;
    [SerializeField] private GameObject recipeSlotPrefab;
    [SerializeField] private Transform tabsRoot;
    [SerializeField] private GameObject tabButtonPrefab;
    [SerializeField] private ScrollRect tabsScrollRect;

    [Header("LcakPopup")]
    [SerializeField] private LackPopup lackPopupPrefab;
    [SerializeField] private Transform popupParent;

    private List<Button> tabButtons = new();
    private List<WeaponType> forgeWeaponTypes = new();
    private int selectedTabIndex = -1;
    private int slotCount = 6;

    private bool isDraggingTabs = false;
    private float lastTabDragEndTime = -100f;
    private float tabClickCooldown = 0.25f;

    private ItemDataLoader itemLoader;
    private CraftingDataLoader craftingLoader;

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

        // 탭 세팅
        tabButtons.Clear();
        forgeWeaponTypes.Clear();

        foreach (Transform t in tabsRoot)
            Destroy(t.gameObject);

        ForgeType forgeType = gm.Forge.ForgeType;
        if (ForgeWeaponTypeMapping.ForgeWeaponTypeDict.TryGetValue(forgeType, out var weaponTypes))
        {
            forgeWeaponTypes = weaponTypes.ToList();
            for (int i = 0; i < forgeWeaponTypes.Count; i++)
            {
                var tabGo = Instantiate(tabButtonPrefab, tabsRoot);
                var btn = tabGo.GetComponent<Button>();
                tabButtons.Add(btn);

                // 탭 텍스트 설정
                var txt = tabGo.GetComponentInChildren<TMP_Text>();
                if (txt != null)
                    txt.text = forgeWeaponTypes[i].ToString();
            }
        }

        for (int i = 0; i < tabButtons.Count; i++)
        {
            int idx = i;
            tabButtons[i].onClick.RemoveAllListeners();
            tabButtons[i].onClick.AddListener(() => OnTabClicked(idx));
        }

        // 슬롯 초기화
        if (inputWeaponSlotBtns.Count < slotCount)
        {
            // Editor에서 미연결된 경우 자동으로 instantiate (but 권장: Inspector에서 할당!)
            inputWeaponSlotBtns.Clear();
            foreach (Transform child in inputWeaponSlots)
                Destroy(child.gameObject);

            for (int i = 0; i < slotCount; i++)
            {
                GameObject go = Instantiate(weaponSlotBtnPrefab, inputWeaponSlots);
                var slotBtn = go.GetComponent<InputWeaponSlotBtn>();
                if (slotBtn == null)
                    slotBtn = go.AddComponent<InputWeaponSlotBtn>();
                inputWeaponSlotBtns.Add(slotBtn);
            }
        }

        // 슬롯 UI 리셋 및 버튼 등록
        for (int i = 0; i < inputWeaponSlotBtns.Count; i++)
        {
            var slotBtn = inputWeaponSlotBtns[i];
            slotBtn.ResetSlot();
            int idx = i;

            if (slotBtn.slotButton)
            {
                slotBtn.slotButton.onClick.RemoveAllListeners();
                slotBtn.slotButton.interactable = false;
            }
            if (slotBtn.receiveBtn)
            {
                slotBtn.receiveBtn.onClick.RemoveAllListeners();
                slotBtn.receiveBtn.onClick.AddListener(() => OnClickReceive(idx));
                slotBtn.receiveBtn.gameObject.SetActive(false);
            }
        }

        OnTabClicked(0);
    }

    private void OnTabClicked(int tabIdx)
    {
        if (Time.unscaledTime - lastTabDragEndTime < tabClickCooldown)
            return;

        selectedTabIndex = tabIdx;
        for (int i = 0; i < tabButtons.Count; i++)
            tabButtons[i].interactable = (i != tabIdx);

        ShowRecipeListByJob(tabIdx);
        CenterTab(tabIdx);
    }

    private void ShowRecipeListByJob(int tabIdx)
    {
        foreach (Transform child in recipeListRoot)
            Destroy(child.gameObject);

        if (tabIdx < 0 || tabIdx >= forgeWeaponTypes.Count)
            return;

        WeaponType weaponType = forgeWeaponTypes[tabIdx];

        var unlockedKeys = gameManager.Forge.RecipeSystem.GetKeysByType(weaponType);
        if (unlockedKeys == null || unlockedKeys.Count == 0) return;

        var recipes = craftingLoader.CraftingList
            .Where(x => unlockedKeys.Contains(x.ItemKey))
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
        var inventory = gameManager.Inventory;
        var forge = gameManager.Forge;
        if (inventory == null || forge == null)
            return;

        int goldNeed = (int)craftingData.craftCost;
        if (forge.ForgeManager.Gold < goldNeed)
        {
            var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : transform);
            popup.Show(LackType.Gold);
            return;
        }

        var required = craftingData.RequiredResources.Select(r => (r.ResourceKey, r.Amount)).ToList();
        bool allHave = true;
        foreach (var (resourceKey, amount) in required)
        {
            int owned = inventory.ResourceList?.Find(x => x.ItemKey == resourceKey)?.Quantity ?? 0;
            if (owned < amount)
            {
                allHave = false;
                break;
            }
        }
        if (!allHave)
        {
            var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : transform);
            popup.Show(LackType.Resource);
            return;
        }

        // 3. 골드/재료 모두 충분할 때만 제작
        int idx = -1;
        for (int i = 0; i < inputWeaponSlotBtns.Count; i++)
        {
            var prog = gameManager.CraftingManager.GetCraftTask(i);
            if (prog == null || !prog.isCrafting)
            {
                idx = i;
                break;
            }
        }
        if (idx < 0) return;

        if (!forge.ForgeManager.UseGold(goldNeed))
            return;
        if (!inventory.UseCraftingMaterials(required))
        {
            forge.ForgeManager.AddGold(goldNeed);
            return;
        }

        SoundManager.Instance.Play("SFX_ForgeCraft");

        var itemData = gameManager.DataManager.ItemLoader.GetItemByKey(craftingData.ItemKey);
        Sprite iconSprite = IconLoader.GetIconByKey(itemData.ItemKey);

        var slotBtn = inputWeaponSlotBtns[idx];
        slotBtn.SetIcon(iconSprite);

        gameManager.CraftingManager.StartCrafting(idx, craftingData, itemData);
        if (slotBtn.slotButton) slotBtn.slotButton.interactable = false;
    }

    void Update()
    {
        for (int i = 0; i < inputWeaponSlotBtns.Count; i++)
        {
            var prog = gameManager.CraftingManager.GetCraftTask(i);
            var slotBtn = inputWeaponSlotBtns[i];

            if (prog != null && prog.isCrafting && prog.data != null)
            {
                Sprite sp = IconLoader.GetIconByKey(prog.itemData.ItemKey);
                slotBtn.SetIcon(sp);
                slotBtn.SetProgress(prog.timeLeft / prog.totalTime);
                slotBtn.SetTimeText($"{prog.timeLeft:0.0}s");

                if (slotBtn.slotButton) slotBtn.slotButton.interactable = false;
                slotBtn.SetReceiveBtnActive(false);
            }
            else
            {
                if (slotBtn.progressBar) slotBtn.progressBar.gameObject.SetActive(false);
                if (slotBtn.timeText) slotBtn.timeText.gameObject.SetActive(false);

                if (prog != null && prog.rewardGiven && prog.itemData != null)
                {
                    slotBtn.SetReceiveBtnActive(true);
                    if (slotBtn.slotButton) slotBtn.slotButton.interactable = false;
                }
                else
                {
                    slotBtn.SetReceiveBtnActive(false);
                    if (slotBtn.slotButton) slotBtn.slotButton.interactable = false;
                    slotBtn.SetIcon(null);
                }
            }
        }
    }

    void OnClickReceive(int index)
    {
        var prog = gameManager.CraftingManager.GetCraftTask(index);
        if (prog == null || !prog.rewardGiven || prog.itemData == null) return;

        SoundManager.Instance.Play("SFX_SystemReward");
        ShowCraftReward(prog.itemData);

        prog.Reset();
        var slotBtn = inputWeaponSlotBtns[index];
        slotBtn.ResetSlot();
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

        var rewardList = new List<(string itemKey, int count)>()
        {
            (itemData.ItemKey, 1)
        };

        var rewardPopup = uIManager.OpenUI<RewardPopup>(UIName.RewardPopup);
        rewardPopup.Show(rewardList, itemLoader, "");
    }
}
