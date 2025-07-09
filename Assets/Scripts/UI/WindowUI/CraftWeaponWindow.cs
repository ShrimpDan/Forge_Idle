using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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

    private List<InputSlotContext> inputSlots = new();
    private List<Button> tabButtons = new();

    private int slotCount = 6;
    private int selectedSlotIndex = -1;

    private GameManager gameManager;
    private UIManager uIManager;
    private ItemDataLoader itemLoader;
    private CraftingDataLoader craftingLoader;

    private class InputSlotContext
    {
        public Button Btn;
        public Image Icon;
        public Image ProgressBar;
        public TMP_Text TimeText;
        public Button ReceiveBtn;
        public int TaskIndex;
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

        // 탭 버튼 세팅
        tabButtons.Clear();
        foreach (Transform t in tabsRoot)
        {
            var btn = t.GetComponent<Button>();
            int jobIdx = tabButtons.Count;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => ShowRecipeListByJob(jobIdx));
            tabButtons.Add(btn);
        }

        // 인풋 슬롯 세팅
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
            ctx.Btn.onClick.AddListener(() => OnClickInputWeaponSlot(idx));
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

        // 첫 탭 선택(기본)
        ShowRecipeListByJob(0);
    }

    void ShowRecipeListByJob(int jobTypeIdx)
    {
        foreach (Transform child in recipeListRoot)
            Destroy(child.gameObject);

        var recipes = craftingLoader.CraftingList
            .Where(x => (int)x.jobType == jobTypeIdx)
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
                () => OnRecipeSelected(data));
        }
    }

    void OnClickInputWeaponSlot(int index)
    {
        if (gameManager.CraftingManager.GetCraftTask(index)?.isCrafting == true) return;
        selectedSlotIndex = index;
        // 레시피 팝업 띄우는 부분 (선택 콜백은 OnRecipeSelected)
        var popup = uIManager.OpenUI<Forge_Recipe_Popup>(UIName.Forge_Recipe_Popup);
        popup.Init(gameManager.DataManager, uIManager);
        popup.SetRecipeSelectCallback((itemData, craftingData) => OnRecipeSelected(craftingData));
        popup.SetForgeAndInventory(gameManager.Forge, gameManager.Inventory);
    }

    void OnRecipeSelected(CraftingData craftingData)
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= slotCount)
            return;

        var inventory = gameManager.Inventory;
        var forge = gameManager.Forge;
        if (inventory == null || forge == null)
            return;

        // 필요 재료
        var required = craftingData.RequiredResources
            .Select(r => (r.ResourceKey, r.Amount)).ToList();
        int goldNeed = (int)craftingData.craftCost;

        // 금액/재료 동시 소모 시도 (둘 다 성공해야 제작)
        if (!forge.UseGold(goldNeed))
            return;
        if (!inventory.UseCraftingMaterials(required))
        {
            // 골드 롤백
            forge.AddGold(goldNeed);
            return;
        }

        var itemData = gameManager.DataManager.ItemLoader.GetItemByKey(craftingData.ItemKey);
        Sprite iconSprite = IconLoader.GetIcon(itemData.IconPath);

        var slot = inputSlots[selectedSlotIndex];
        slot.Icon.sprite = iconSprite;
        slot.Icon.enabled = (iconSprite != null);

        gameManager.CraftingManager.StartCrafting(selectedSlotIndex, craftingData, itemData);
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
                slot.ReceiveBtn.gameObject.SetActive(false); // 진행 중일 때 비활성
            }
            else
            {
                slot.ProgressBar?.gameObject.SetActive(false);
                slot.TimeText?.gameObject.SetActive(false);

                // 제작 완료 & 보상 미수령이면 ReceiveBtn 활성화
                if (prog.rewardGiven && prog.itemData != null)
                {
                    slot.ReceiveBtn.gameObject.SetActive(true);
                    slot.Btn.interactable = false;
                }
                else
                {
                    slot.ReceiveBtn.gameObject.SetActive(false);
                    slot.Btn.interactable = true;
                    slot.Icon.sprite = null;
                    slot.Icon.enabled = false;
                }
            }
        }
    }

    // 수령 버튼 클릭 처리 (보상 지급은 CraftingManager에서 이미 처리됨, UI만 리셋)
    void OnClickReceive(int index)
    {
        var prog = gameManager.CraftingManager.GetCraftTask(index);
        if (!prog.rewardGiven || prog.itemData == null) return;

        prog.Reset(); // CraftingManager.CraftTask.Reset() 호출로 상태 초기화
        var slot = inputSlots[index];
        slot.Icon.sprite = null;
        slot.Icon.enabled = false;
        slot.Btn.interactable = true;
        slot.ReceiveBtn.gameObject.SetActive(false);
    }


    public override void Open()
    {
        base.Open();
        selectedSlotIndex = -1;
    }
}
