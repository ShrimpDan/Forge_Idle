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

    private List<Button> slotButtons = new List<Button>();
    private List<Image> slotIcons = new List<Image>();

    private class SlotCraftProgress
    {
        public bool isCrafting = false;
        public float totalTime = 0f;
        public float timeLeft = 0f;
        public Image progressBar;
        public TMP_Text timeText;
        public CraftingData data;
        public ItemData itemData;
        public bool rewardGiven = false;
    }
    private List<SlotCraftProgress> slotProgressList = new List<SlotCraftProgress>();

    private int slotCount = 6;
    private int selectedSlotIndex = -1;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.CraftWeaponWindow));

        foreach (Transform child in inputWeaponSlots)
            Destroy(child.gameObject);

        slotButtons.Clear();
        slotIcons.Clear();
        slotProgressList.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = Instantiate(weaponSlotBtnPrefab, inputWeaponSlots);

            Button btn = go.GetComponent<Button>();
            Image icon = go.transform.Find("Icon")?.GetComponent<Image>() ?? go.GetComponent<Image>();
            int idx = i;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnClickInputWeaponSlot(idx));
            slotButtons.Add(btn);
            slotIcons.Add(icon);

            var progressBar = go.transform.Find("CraftProgressBarBG/CraftProgressBar")?.GetComponent<Image>();
            var progressText = go.transform.Find("CraftProgressBarBG/CraftProgressText")?.GetComponent<TMP_Text>();

            if (progressBar) { progressBar.fillAmount = 0; progressBar.gameObject.SetActive(false); }
            if (progressText) { progressText.gameObject.SetActive(false); }

            slotProgressList.Add(new SlotCraftProgress
            {
                isCrafting = false,
                totalTime = 0f,
                timeLeft = 0f,
                progressBar = progressBar,
                timeText = progressText,
                data = null,
                itemData = null,
                rewardGiven = false
            });

            icon.sprite = null;
            icon.enabled = false;
        }
    }

    private void OnClickInputWeaponSlot(int index)
    {
        if (index < 0 || index >= slotProgressList.Count)
            return;
        if (slotProgressList[index].isCrafting)
            return;

        selectedSlotIndex = index;
        var popup = uIManager.OpenUI<Forge_Recipe_Popup>(UIName.Forge_Recipe_Popup);
        popup.Init(gameManager.DataManager, uIManager);
        popup.SetRecipeSelectCallback(OnRecipeSelected);
        popup.SetForgeAndInventory(gameManager.Forge, gameManager.Inventory);
    }

    private void OnRecipeSelected(ItemData itemData, CraftingData craftingData)
    {
        if (itemData == null || craftingData == null)
            return;
        if (selectedSlotIndex < 0 || selectedSlotIndex >= slotIcons.Count)
            return;

        var inventory = gameManager.Inventory;
        var forge = gameManager.Forge;

        if (inventory == null || forge == null)
            return;

        // --- 재료, 골드 체크 ---
        var required = craftingData.RequiredResources
            .Select(r => (MapItemKey(r.ResourceKey), r.Amount)).ToList();

        int goldNeed = (int)craftingData.craftCost;
        if (forge.Gold < goldNeed)
            return;

        bool hasAll = required.All(req =>
            inventory.ResourceList.Where(x => x.ItemKey == req.Item1).Sum(x => x.Quantity) >= req.Item2);

        if (!hasAll)
            return;

        // -- 차감 처리 --
        if (!inventory.UseCraftingMaterials(required))
            return;
        forge.AddGold(-goldNeed);

        Sprite iconSprite = IconLoader.GetIcon(itemData.IconPath);
        if (iconSprite == null)
            iconSprite = Resources.Load<Sprite>(itemData.IconPath);

        slotIcons[selectedSlotIndex].sprite = iconSprite;
        slotIcons[selectedSlotIndex].enabled = (iconSprite != null);

        var prog = slotProgressList[selectedSlotIndex];
        prog.isCrafting = true;
        prog.totalTime = craftingData.craftTime;
        prog.timeLeft = craftingData.craftTime;
        prog.data = craftingData;
        prog.itemData = itemData;
        prog.rewardGiven = false;

        if (prog.progressBar)
        {
            prog.progressBar.fillAmount = 1f;
            prog.progressBar.gameObject.SetActive(true);
        }
        if (prog.timeText)
        {
            prog.timeText.gameObject.SetActive(true);
            prog.timeText.text = $"{prog.timeLeft:0.0}s";
        }
        slotButtons[selectedSlotIndex].interactable = false;
    }

    private string MapItemKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return key;
        if (key.StartsWith("weapon_") || key.StartsWith("resource_") || key.StartsWith("gem_") || key.StartsWith("ingot_"))
            return key;
        string[] types = { "axe", "pickaxe", "sword", "dagger", "bow", "shield", "hoe" };
        var parts = key.Split('_');
        if (parts.Length == 2)
        {
            string p0 = parts[0];
            string p1 = parts[1];
            string type = types.Contains(p0) ? p0 : (types.Contains(p1) ? p1 : null);
            string quality = types.Contains(p0) ? p1 : (types.Contains(p1) ? p0 : null);
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(quality))
                return $"weapon_{type}_{quality}";
        }
        return key;
    }

    private void Update()
    {
        for (int i = 0; i < slotProgressList.Count; i++)
        {
            var prog = slotProgressList[i];
            if (!prog.isCrafting) continue;

            prog.timeLeft -= Time.deltaTime;
            if (prog.timeLeft < 0f) prog.timeLeft = 0f;

            if (prog.progressBar && prog.totalTime > 0f)
                prog.progressBar.fillAmount = prog.timeLeft / prog.totalTime;
            if (prog.timeText)
                prog.timeText.text = $"{prog.timeLeft:0.0}s";

            // 제작 완료 + 인벤토리 지급
            if (prog.timeLeft <= 0f && prog.isCrafting && !prog.rewardGiven)
            {
                prog.isCrafting = false;
                prog.rewardGiven = true;

                if (prog.itemData != null)
                    gameManager.Inventory.AddItem(prog.itemData, 1);

                if (prog.progressBar) prog.progressBar.gameObject.SetActive(false);
                if (prog.timeText) prog.timeText.gameObject.SetActive(false);
                if (i < slotButtons.Count) slotButtons[i].interactable = true;
            }
        }
    }

    public override void Open()
    {
        base.Open();
        foreach (var icon in slotIcons)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
        selectedSlotIndex = -1;
        foreach (var prog in slotProgressList)
        {
            prog.isCrafting = false;
            prog.rewardGiven = false;
            prog.totalTime = 0f;
            prog.timeLeft = 0f;
            if (prog.progressBar) prog.progressBar.gameObject.SetActive(false);
            if (prog.timeText) prog.timeText.gameObject.SetActive(false);
        }
    }

    public override void Close()
    {
        base.Close();
    }
}
