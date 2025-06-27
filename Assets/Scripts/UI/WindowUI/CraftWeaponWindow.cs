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

    // ���� ���� ���� ����ü
    private class SlotCraftProgress
    {
        public bool isCrafting = false;
        public float totalTime = 0f;
        public float timeLeft = 0f;
        public Image progressBar;
        public TMP_Text timeText;
        public CraftingData data;
        public ItemData itemData;
    }
    private List<SlotCraftProgress> slotProgressList = new List<SlotCraftProgress>();

    private int slotCount = 6;
    private int selectedSlotIndex = -1;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        if (exitBtn == null)
        {
            Debug.LogError("exitBtn�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.CraftWeaponWindow));

        if (inputWeaponSlots == null)
        {
            Debug.LogError("inputWeaponSlots�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }
        if (weaponSlotBtnPrefab == null)
        {
            Debug.LogError("weaponSlotBtnPrefab�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        foreach (Transform child in inputWeaponSlots)
            Destroy(child.gameObject);

        slotButtons.Clear();
        slotIcons.Clear();
        slotProgressList.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = Instantiate(weaponSlotBtnPrefab, inputWeaponSlots);
            if (go == null)
            {
                Debug.LogError($"���� ������ �ν��Ͻ�ȭ ����! (i={i})");
                continue;
            }

            Button btn = go.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogError($"Prefab�� Button ������Ʈ�� �����ϴ�! (i={i})");
                continue;
            }

            Image icon = go.GetComponent<Image>();
            if (icon == null)
            {
                Debug.LogError($"Prefab�� Image ������Ʈ�� �����ϴ�! (i={i})");
                continue;
            }

            int idx = i;
            btn.onClick.AddListener(() => OnClickInputWeaponSlot(idx));
            slotButtons.Add(btn);
            slotIcons.Add(icon);

            // ProgressBar, ProgressText ���� Ȯ��
            var progressBar = go.transform.Find("CraftProgressBarBG/CraftProgressBar")?.GetComponent<Image>();
            if (progressBar == null) Debug.LogError($"CraftProgressBar�� ã�� �� ����! (i={i})");

            var progressText = go.transform.Find("CraftProgressBarBG/CraftProgressText")?.GetComponent<TMP_Text>();
            if (progressText == null) Debug.LogError($"CraftProgressText�� ã�� �� ����! (i={i})");

            if (progressBar) progressBar.fillAmount = 0;
            if (progressBar) progressBar.gameObject.SetActive(false);
            if (progressText) progressText.gameObject.SetActive(false);

            slotProgressList.Add(new SlotCraftProgress
            {
                isCrafting = false,
                totalTime = 0f,
                timeLeft = 0f,
                progressBar = progressBar,
                timeText = progressText,
                data = null,
                itemData = null
            });
        }
    }

    private void OnClickInputWeaponSlot(int index)
    {
        // ���� ���̸� Ŭ�� ����
        if (index < 0 || index >= slotProgressList.Count)
        {
            Debug.LogError($"�߸��� ���� �ε���: {index}");
            return;
        }

        if (slotProgressList[index].isCrafting)
        {
            Debug.Log("�ش� ������ ���� ���� ���Դϴ�.");
            return;
        }

        selectedSlotIndex = index;
        var popup = uIManager.OpenUI<Forge_Recipe_Popup>(UIName.Forge_Recipe_Popup);
        if (popup == null)
        {
            Debug.LogError("Forge_Recipe_Popup�� �� �� �����ϴ�!");
            return;
        }

        popup.Init(gameManager.DataManager, uIManager);
        popup.SetRecipeSelectCallback(OnRecipeSelected);
        popup.SetForgeAndInventory(gameManager.Forge, gameManager.Inventory);
    }

    // ���� ������ ���ý� ���� ����
    private void OnRecipeSelected(ItemData itemData, CraftingData craftingData)
    {
        if (itemData == null || craftingData == null)
        {
            Debug.LogWarning("OnRecipeSelected: itemData �Ǵ� craftingData�� null�Դϴ�.");
            return;
        }
        if (selectedSlotIndex < 0 || selectedSlotIndex >= slotIcons.Count)
        {
            Debug.LogWarning("OnRecipeSelected: �߸��� selectedSlotIndex");
            return;
        }

        // ���� ��� ���� ����
        var inventory = gameManager.Inventory;
        if (inventory == null)
        {
            Debug.LogError("gameManager.Inventory�� null�Դϴ�.");
            return;
        }

        var required = craftingData.RequiredResources
            .Select(r => (r.ResourceKey, r.Amount)).ToList();

        if (!inventory.UseCraftingMaterials(required))
        {
            Debug.LogWarning("[���۽���] ��ᰡ �����ϰų� ���� �Ұ�!");
            return;
        }

        // ������ ����
        var iconSprite = Resources.Load<Sprite>(itemData.IconPath);
        if (iconSprite == null)
        {
            Debug.LogWarning($"������ ���({itemData.IconPath})�� Sprite�� ã�� �� �����ϴ�.");
        }
        slotIcons[selectedSlotIndex].sprite = iconSprite;

        // ���� ���� ���� ����
        var prog = slotProgressList[selectedSlotIndex];
        prog.isCrafting = true;
        prog.totalTime = craftingData.craftTime;
        prog.timeLeft = craftingData.craftTime;
        prog.data = craftingData;
        prog.itemData = itemData;

        // UI Ȱ��ȭ �� �ʱ�ȭ
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

    private void Update()
    {
        // ���Ժ� ���� ����
        for (int i = 0; i < slotProgressList.Count; i++)
        {
            var prog = slotProgressList[i];
            if (!prog.isCrafting) continue;

            prog.timeLeft -= Time.deltaTime;
            if (prog.timeLeft < 0f) prog.timeLeft = 0f;

            // UI ������Ʈ
            if (prog.progressBar && prog.totalTime > 0f)
                prog.progressBar.fillAmount = prog.timeLeft / prog.totalTime;
            if (prog.timeText)
                prog.timeText.text = $"{prog.timeLeft:0.0}s";

            // ���� �Ϸ� ó��
            if (prog.timeLeft <= 0f && prog.isCrafting)
            {
                prog.isCrafting = false;
                // ��/�ؽ�Ʈ ��Ȱ��ȭ
                if (prog.progressBar) prog.progressBar.gameObject.SetActive(false);
                if (prog.timeText) prog.timeText.gameObject.SetActive(false);
                if (i < slotButtons.Count) slotButtons[i].interactable = true;

                Debug.Log($"[���ۿϷ�] {prog.itemData?.Name ?? ""} ������ �������ϴ�!");
            }
        }
    }

    public override void Open()
    {
        base.Open();
        foreach (var icon in slotIcons)
            icon.sprite = null;
            
        selectedSlotIndex = -1;
        // ��� ���� UI/���� �ʱ�ȭ
        foreach (var prog in slotProgressList)
        {
            prog.isCrafting = false;
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
