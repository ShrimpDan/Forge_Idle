using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ForgeUIManager : MonoBehaviour
{
    [Header("�г�/���� ����")]
    public GameObject gemUIPanel, craftingUIPanel, upgradeUIPanel, refineUIPanel;
    public GameObject gemInputSlot, refineInputSlot, upgradeInputSlot, craftingInputSlot;
    public GameObject gemResultImage, refineResultImage, upgradeResultBox, craftingResultBox;
    public TMP_Text gemResultText, refineResultText, upgradeResultText, craftingResultText;

    public InventoryPopup inventoryPopup;

    [Header("���� �ý��� ����")]
    public InventoryManager inventoryManager;
    public WeaponCraftingSystem craftingSystem;
    public WeaponUpgradeSystem upgradeSystem;
    public RefineSystem refineSystem;
    public GemSystem gemSystem;


    // �� �гκ� ����
    Item selectedOre, selectedGemOre, selectedWeapon, selectedGem, selectedUpgradeWeapon, selectedUpgradeGem;

    // --------- Gem ���� Panel ---------
    public void OnClickGemInputSlot()
    {
        // GemOre(��������)�� �����ؼ� �˾��� ����
        var gemOres = inventoryManager.items.FindAll(i => (i is MaterialItem mat) && mat.materialType == MaterialType.GemOre);
        inventoryPopup.Show(gemOres, (item) => {
            selectedGemOre = item;
            gemInputSlot.GetComponent<Image>().sprite = item.icon;
            gemResultImage.SetActive(false);
        });
    }
    public void OnClickGemExecute()
    {
        if (selectedGemOre == null) { gemResultText.text = "���� ������ �־��ּ���"; return; }
        string result = refineSystem.Refine((MaterialItem)selectedGemOre);
        gemResultText.text = result;
        // ���� �� �����ʿ� ���� �̹��� �����ֱ�(�κ��丮���� ���� �߰��� ���� ã�Ƽ�)
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
        if (selectedOre == null) { refineResultText.text = "������ �־��ּ���"; return; }
        string result = refineSystem.Refine((MaterialItem)selectedOre);
        refineResultText.text = result;
        // ���(�ֱ�) �̹��� ǥ��
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
            upgradeResultText.text = "��� �Ҹ�: " + (upgradeSystem.baseUpgradeCost * ((Weapon)item).level);
        });
    }
    public void OnClickUpgradeExecute()
    {
        if (selectedUpgradeWeapon == null) { upgradeResultText.text = "���⸦ �־��ּ���"; return; }
        string result = upgradeSystem.UpgradeWeapon((Weapon)selectedUpgradeWeapon);
        upgradeResultText.text = result;
    }
    // --------- Gem ���� Panel ---------
    public void OnClickGemAttachInputSlot()
    {
        // ���⸸ ����
        var weapons = inventoryManager.items.FindAll(i => i is Weapon);
        inventoryPopup.Show(weapons, (item) => {
            selectedWeapon = item;
            gemInputSlot.GetComponent<Image>().sprite = item.icon;
            // ���� ���� ��ĭ(��������) ���� �� �߰� ���� �ʿ�
        });
    }
    public void OnClickGemSocketSlot()
    {
        // �κ��丮���� Gem�� ������
        var gems = inventoryManager.items.FindAll(i => i is Gem);
        inventoryPopup.Show(gems, (item) => {
            selectedGem = item;
            // Gem ������ ���Ͽ� ����
        });
    }
    public void OnClickGemSocketExecute()
    {
        if (selectedWeapon == null || selectedGem == null) { gemResultText.text = "����/������ �־��ּ���"; return; }
        string result = gemSystem.AttachGem((Weapon)selectedWeapon, (Gem)selectedGem);
        gemResultText.text = result;
    }
    // ���� ���� ����, Ȯ��â/��� ���� �� ���� ���� � ���� �Լ��� ����

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
