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
    private List<Image> progressBars = new List<Image>();
    private List<TMP_Text> timeTexts = new List<TMP_Text>();

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
        progressBars.Clear();
        timeTexts.Clear();

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
            if (progressBar) progressBar.gameObject.SetActive(false);
            if (progressText) progressText.gameObject.SetActive(false);
            progressBars.Add(progressBar);
            timeTexts.Add(progressText);

            icon.sprite = null;
            icon.enabled = false;
        }
    }

    private void OnClickInputWeaponSlot(int index)
    {
        if (index < 0 || index >= slotCount) return;
        var prog = gameManager.CraftingManager.GetCraftTask(index);
        if (prog != null && prog.isCrafting)
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

        var required = craftingData.RequiredResources
            .Select(r => (MapItemKey(r.ResourceKey), r.Amount)).ToList();

        int goldNeed = (int)craftingData.craftCost;
        if (forge.Gold < goldNeed)
            return;

        bool hasAll = required.All(req =>
            inventory.ResourceList.Where(x => x.ItemKey == req.Item1).Sum(x => x.Quantity) >= req.Item2);

        if (!hasAll)
            return;

        if (!inventory.UseCraftingMaterials(required))
            return;
        forge.AddGold(-goldNeed);

        Sprite iconSprite = IconLoader.GetIcon(itemData.IconPath);
        if (iconSprite == null)
            iconSprite = Resources.Load<Sprite>(itemData.IconPath);

        slotIcons[selectedSlotIndex].sprite = iconSprite;
        slotIcons[selectedSlotIndex].enabled = (iconSprite != null);

        // CraftingManager로 제작 시작 요청
        gameManager.CraftingManager.StartCrafting(selectedSlotIndex, craftingData, itemData);

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
        for (int i = 0; i < slotCount; i++)
        {
            var prog = gameManager.CraftingManager.GetCraftTask(i);
            var icon = slotIcons[i];
            var btn = slotButtons[i];
            var progressBar = progressBars[i];
            var timeText = timeTexts[i];

            if (prog.isCrafting && prog.data != null)
            {
                if (icon && prog.itemData != null)
                {
                    Sprite sp = IconLoader.GetIcon(prog.itemData.IconPath);
                    if (sp == null)
                        sp = Resources.Load<Sprite>(prog.itemData.IconPath);
                    icon.sprite = sp;
                    icon.enabled = (sp != null);
                }
                if (progressBar && prog.totalTime > 0f)
                {
                    progressBar.gameObject.SetActive(true);
                    progressBar.fillAmount = prog.timeLeft / prog.totalTime;
                }
                if (timeText)
                {
                    timeText.gameObject.SetActive(true);
                    timeText.text = $"{prog.timeLeft:0.0}s";
                }
                btn.interactable = false;
            }
            else
            {
                if (progressBar) progressBar.gameObject.SetActive(false);
                if (timeText) timeText.gameObject.SetActive(false);
                btn.interactable = true;

                if (prog.itemData == null)
                {
                    icon.sprite = null;
                    icon.enabled = false;
                }
            }
        }
    }

    public override void Open()
    {
        base.Open();
        selectedSlotIndex = -1;
    }

    public override void Close()
    {
        base.Close();
    }
}
