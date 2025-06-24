using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GemsSystemWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI ����")]
    [SerializeField] Button exitBtn;
    [SerializeField] Image weaponInputSlot;
    [SerializeField] Button weaponInputBtn;
    [SerializeField] Transform gemSocketParent;
    [SerializeField] GameObject gemSocketSlotPrefab;
    [SerializeField] Button executeBtn;
    [SerializeField] TMP_Text gemResultText;
    // InventoryPopup ���� �ʵ� ����!

    private Weapon selectedWeapon;
    private List<Gem> selectedGems = new List<Gem>();

    private void Awake()
    {
        exitBtn.onClick.AddListener(Close);
        weaponInputBtn.onClick.AddListener(OpenWeaponSelectPopup);
        executeBtn.onClick.AddListener(ExecuteAttachGems);
    }

    public override void Init(UIManager uIManager)
    {
        base.Init(uIManager);
        ResetUI();
    }

    void ResetUI()
    {
        weaponInputSlot.sprite = null;
        selectedWeapon = null;
        gemResultText.text = "";
        foreach (Transform child in gemSocketParent)
            Destroy(child.gameObject);
        selectedGems.Clear();
    }

    void OpenWeaponSelectPopup()
    {
        var weapons = InventoryManager.Instance.GetAllWeapons();
        InventoryPopup popup = UIManager.Instance.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        popup.Show(weapons, OnWeaponSelected);
    }

    void OnWeaponSelected(Item item)
    {
        selectedWeapon = item as Weapon;
        weaponInputSlot.sprite = selectedWeapon.icon;
        SetupGemSlots(selectedWeapon.maxGemSlots);
        gemResultText.text = "";
    }

    void SetupGemSlots(int count)
    {
        foreach (Transform child in gemSocketParent)
            Destroy(child.gameObject);

        selectedGems = new List<Gem>(new Gem[count]);
        for (int i = 0; i < count; i++)
        {
            var go = Object.Instantiate(gemSocketSlotPrefab, gemSocketParent);
            var slot = go.GetComponent<GemSocketSlot>();
            slot.SetIcon(null);
            slot.Init(i, OnGemSocketClick);
        }
    }

    void OnGemSocketClick(GemSocketSlot slot)
    {
        var gems = InventoryManager.Instance.GetAllGems();
        InventoryPopup popup = UIManager.Instance.OpenUI<InventoryPopup>(UIName.InventoryPopup);
        popup.Show(gems, (item) => OnGemSelected(slot.socketIndex, item as Gem));
    }

    void OnGemSelected(int index, Gem gem)
    {
        selectedGems[index] = gem;
        var slot = gemSocketParent.GetChild(index).GetComponent<GemSocketSlot>();
        slot.SetIcon(gem.icon);
    }

    void ExecuteAttachGems()
    {
        if (selectedWeapon == null)
        {
            gemResultText.text = "���⸦ �����ϼ���!";
            return;
        }
        for (int i = 0; i < selectedGems.Count; i++)
        {
            if (selectedGems[i] != null)
                GemSystem.Instance.AttachGem(selectedWeapon, selectedGems[i], i);
        }
        gemResultText.text = "�� ���� ����!";
    }

    public override void Open()
    {
        base.Open();
        ResetUI();
    }

    public override void Close()
    {
        base.Close();
    }
}
