using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ForgeUIManager : MonoBehaviour
{
    [Header("패널/슬롯 연결")]
    public GameObject gemUIPanel, craftingUIPanel, upgradeUIPanel, refineUIPanel;
    public GameObject gemInputSlot, refineInputSlot, upgradeInputSlot, craftingInputSlot;
    public GameObject gemResultImage, refineResultImage, upgradeResultBox, craftingResultBox;
    public TMP_Text gemResultText, refineResultText, upgradeResultText, craftingResultText;

    public InventoryPopup inventoryPopup;

    [Header("게임 시스템 연결")]
    public InventoryManager inventoryManager;
    public WeaponCraftingSystem craftingSystem;
    public WeaponUpgradeSystem upgradeSystem;
    public RefineSystem refineSystem;
    public GemSystem gemSystem;


    // 각 패널별 상태
    Item selectedOre, selectedGemOre, selectedWeapon, selectedGem, selectedUpgradeWeapon, selectedUpgradeGem;

    // --------- Gem 세공 Panel ---------
    public void OnClickGemInputSlot()
    {
        // GemOre(보석원석)만 추출해서 팝업에 전달
        var gemOres = inventoryManager.items.FindAll(i => (i is MaterialItem mat) && mat.materialType == MaterialType.GemOre);
        inventoryPopup.Show(gemOres, (item) => {
            selectedGemOre = item;
            gemInputSlot.GetComponent<Image>().sprite = item.icon;
            gemResultImage.SetActive(false);
        });
    }
    public void OnClickGemExecute()
    {
        if (selectedGemOre == null) { gemResultText.text = "보석 원석을 넣어주세요"; return; }
        string result = refineSystem.Refine((MaterialItem)selectedGemOre);
        gemResultText.text = result;
        // 성공 시 오른쪽에 보석 이미지 보여주기(인벤토리에서 새로 추가된 보석 찾아서)
        var gem = inventoryManager.items.Find(i => i is Gem);
        if (gem != null)
        {
            gemResultImage.SetActive(true);
            gemResultImage.GetComponent<Image>().sprite = gem.icon;
        }
    }
    // --------- Refine Panel ---------
    public void OnClickRefineInputSlot()
    {
        var ores = inventoryManager.items.FindAll(i => (i is MaterialItem mat) && mat.materialType == MaterialType.Ore);
        inventoryPopup.Show(ores, (item) => {
            selectedOre = item;
            refineInputSlot.GetComponent<Image>().sprite = item.icon;
            refineResultImage.SetActive(false);
        });
    }
    public void OnClickRefineExecute()
    {
        if (selectedOre == null) { refineResultText.text = "원석을 넣어주세요"; return; }
        string result = refineSystem.Refine((MaterialItem)selectedOre);
        refineResultText.text = result;
        // 결과(주괴) 이미지 표시
        var ingot = inventoryManager.items.Find(i => (i is MaterialItem mat) && mat.materialType == MaterialType.Ingot);
        if (ingot != null)
        {
            refineResultImage.SetActive(true);
            refineResultImage.GetComponent<Image>().sprite = ingot.icon;
        }
    }
    // --------- Upgrade Panel ---------
    public void OnClickUpgradeInputSlot()
    {
        var weapons = inventoryManager.items.FindAll(i => i is Weapon);
        inventoryPopup.Show(weapons, (item) => {
            selectedUpgradeWeapon = item;
            upgradeInputSlot.GetComponent<Image>().sprite = item.icon;
            upgradeResultText.text = "골드 소모: " + (upgradeSystem.baseUpgradeCost * ((Weapon)item).level);
        });
    }
    public void OnClickUpgradeExecute()
    {
        if (selectedUpgradeWeapon == null) { upgradeResultText.text = "무기를 넣어주세요"; return; }
        string result = upgradeSystem.UpgradeWeapon((Weapon)selectedUpgradeWeapon);
        upgradeResultText.text = result;
    }
    // --------- Gem 장착 Panel ---------
    public void OnClickGemAttachInputSlot()
    {
        // 무기만 선택
        var weapons = inventoryManager.items.FindAll(i => i is Weapon);
        inventoryPopup.Show(weapons, (item) => {
            selectedWeapon = item;
            gemInputSlot.GetComponent<Image>().sprite = item.icon;
            // 무기 위에 빈칸(보석소켓) 생성 등 추가 구현 필요
        });
    }
    public void OnClickGemSocketSlot()
    {
        // 인벤토리에서 Gem만 보여줌
        var gems = inventoryManager.items.FindAll(i => i is Gem);
        inventoryPopup.Show(gems, (item) => {
            selectedGem = item;
            // Gem 아이콘 소켓에 세팅
        });
    }
    public void OnClickGemSocketExecute()
    {
        if (selectedWeapon == null || selectedGem == null) { gemResultText.text = "무기/보석을 넣어주세요"; return; }
        string result = gemSystem.AttachGem((Weapon)selectedWeapon, (Gem)selectedGem);
        gemResultText.text = result;
    }
    // 보석 해제 로직, 확인창/골드 차감 후 슬롯 비우기 등도 별도 함수로 구현

    public void ShowGemPanel()
    {
        gemUIPanel.SetActive(true);
        craftingUIPanel.SetActive(false);
        upgradeUIPanel.SetActive(false);
        refineUIPanel.SetActive(false);
    }
    public void ShowCraftingPanel()
    {
        gemUIPanel.SetActive(false);
        craftingUIPanel.SetActive(true);
        upgradeUIPanel.SetActive(false);
        refineUIPanel.SetActive(false);
    }
    public void ShowUpgradePanel()
    {
        gemUIPanel.SetActive(false);
        craftingUIPanel.SetActive(false);
        upgradeUIPanel.SetActive(true);
        refineUIPanel.SetActive(false);
    }
    public void ShowRefinePanel()
    {
        gemUIPanel.SetActive(false);
        craftingUIPanel.SetActive(false);
        upgradeUIPanel.SetActive(false);
        refineUIPanel.SetActive(true);
    }
}
