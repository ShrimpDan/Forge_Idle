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
    }
    private List<SlotCraftProgress> slotProgressList = new List<SlotCraftProgress>();

    private int slotCount = 6;
    private int selectedSlotIndex = -1;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        if (exitBtn == null)
        {
            Debug.LogError("exitBtn이 할당되지 않았습니다!");
            return;
        }
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.CraftWeaponWindow));

        if (inputWeaponSlots == null)
        {
            Debug.LogError("inputWeaponSlots이 할당되지 않았습니다!");
            return;
        }
        if (weaponSlotBtnPrefab == null)
        {
            Debug.LogError("weaponSlotBtnPrefab이 할당되지 않았습니다!");
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
                Debug.LogError($"슬롯 프리팹 인스턴스화 실패! (i={i})");
                continue;
            }

            Button btn = go.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogError($"Prefab에 Button 컴포넌트가 없습니다! (i={i})");
                continue;
            }

            Image icon = go.GetComponent<Image>();
            if (icon == null)
            {
                Debug.LogError($"Prefab에 Image 컴포넌트가 없습니다! (i={i})");
                continue;
            }

            int idx = i;
            btn.onClick.AddListener(() => OnClickInputWeaponSlot(idx));
            slotButtons.Add(btn);
            slotIcons.Add(icon);

            // 진행바 및 텍스트 확인
            var progressBar = go.transform.Find("CraftProgressBarBG/CraftProgressBar")?.GetComponent<Image>();
            if (progressBar == null) Debug.LogError($"CraftProgressBar를 찾을 수 없음! (i={i})");

            var progressText = go.transform.Find("CraftProgressBarBG/CraftProgressText")?.GetComponent<TMP_Text>();
            if (progressText == null) Debug.LogError($"CraftProgressText를 찾을 수 없음! (i={i})");

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

            // **슬롯을 초기화 시에는 아이콘을 비활성화**
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    private void OnClickInputWeaponSlot(int index)
    {
        // 잘못된 인덱스 체크
        if (index < 0 || index >= slotProgressList.Count)
        {
            Debug.LogError($"잘못된 슬롯 인덱스: {index}");
            return;
        }

        if (slotProgressList[index].isCrafting)
        {
            Debug.Log("해당 슬롯은 이미 제작 중입니다.");
            return;
        }

        selectedSlotIndex = index;
        var popup = uIManager.OpenUI<Forge_Recipe_Popup>(UIName.Forge_Recipe_Popup);
        if (popup == null)
        {
            Debug.LogError("Forge_Recipe_Popup을 열 수 없습니다!");
            return;
        }

        popup.Init(gameManager.DataManager, uIManager);
        popup.SetRecipeSelectCallback(OnRecipeSelected);
        popup.SetForgeAndInventory(gameManager.Forge, gameManager.Inventory);
    }

    // 레시피 선택 시
    private void OnRecipeSelected(ItemData itemData, CraftingData craftingData)
    {
        if (itemData == null || craftingData == null)
        {
            Debug.LogWarning("OnRecipeSelected: itemData 혹은 craftingData가 null입니다.");
            return;
        }
        if (selectedSlotIndex < 0 || selectedSlotIndex >= slotIcons.Count)
        {
            Debug.LogWarning("OnRecipeSelected: 잘못된 selectedSlotIndex");
            return;
        }

        // 인벤토리에서 재료 차감
        var inventory = gameManager.Inventory;
        if (inventory == null)
        {
            Debug.LogError("gameManager.Inventory가 null입니다.");
            return;
        }

        var required = craftingData.RequiredResources
            .Select(r => (r.ResourceKey, r.Amount)).ToList();

        if (!inventory.UseCraftingMaterials(required))
        {
            Debug.LogWarning("[제작실패] 재료가 부족하거나 사용 실패!");
            return;
        }

        // **아이콘 반영 (IconLoader 이용)**
        var iconSprite = IconLoader.GetIcon(itemData.IconPath);
        slotIcons[selectedSlotIndex].sprite = iconSprite;
        slotIcons[selectedSlotIndex].enabled = (iconSprite != null);

        // 제작 슬롯 진행상태 설정
        var prog = slotProgressList[selectedSlotIndex];
        prog.isCrafting = true;
        prog.totalTime = craftingData.craftTime;
        prog.timeLeft = craftingData.craftTime;
        prog.data = craftingData;
        prog.itemData = itemData;

        // 진행바, 텍스트 세팅
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
        // 제작 진행 갱신
        for (int i = 0; i < slotProgressList.Count; i++)
        {
            var prog = slotProgressList[i];
            if (!prog.isCrafting) continue;

            prog.timeLeft -= Time.deltaTime;
            if (prog.timeLeft < 0f) prog.timeLeft = 0f;

            // UI 갱신
            if (prog.progressBar && prog.totalTime > 0f)
                prog.progressBar.fillAmount = prog.timeLeft / prog.totalTime;
            if (prog.timeText)
                prog.timeText.text = $"{prog.timeLeft:0.0}s";

            // 제작 완료 처리
            if (prog.timeLeft <= 0f && prog.isCrafting)
            {
                prog.isCrafting = false;
                // 바/텍스트 비활성화
                if (prog.progressBar) prog.progressBar.gameObject.SetActive(false);
                if (prog.timeText) prog.timeText.gameObject.SetActive(false);
                if (i < slotButtons.Count) slotButtons[i].interactable = true;

                Debug.Log($"[제작완료] {prog.itemData?.Name ?? ""} 제작이 완료되었습니다!");
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
