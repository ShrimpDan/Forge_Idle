using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;

    [SerializeField] private Transform inputWeaponSlots;   // ���Ե��� �θ� ������Ʈ
    [SerializeField] private GameObject weaponSlotBtnPrefab; // ���� ��ư ������ (Button+Image ����)

    private List<Button> slotButtons = new List<Button>();
    private List<Image> slotIcons = new List<Image>();
    private int slotCount = 6;


    private ItemData selectedWeapon;

    private void Awake()
    {
        exitBtn.onClick.AddListener(Close);

        // ���� �ڵ� ����
        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = Instantiate(weaponSlotBtnPrefab, inputWeaponSlots);
            Button btn = go.GetComponent<Button>();
            Image icon = go.GetComponent<Image>();
            int idx = i;
            btn.onClick.AddListener(() => OnClickInputWeaponSlot(idx));
            slotButtons.Add(btn);
            slotIcons.Add(icon);

        }
    }

    private void OnClickInputWeaponSlot(int index)
    {

        UIManager.Instance.OpenUI<Forge_AssistantPopup>(UIName.Forge_AssistantPopup);
    }

    // ���� ���� �� ������ ����
    public void OnWeaponSelected(ItemData weapon)
    {
        selectedWeapon = weapon;
        if (slotIcons.Count > 0)
            slotIcons[0].sprite = LoadIcon(weapon.IconPath); // ù ��° ���� ����

    }

    private Sprite LoadIcon(string path)
    {
        Sprite icon = Resources.Load<Sprite>(path);
        return icon ? icon : null;
    }

    public override void Open()
    {
        base.Open();
        foreach (var icon in slotIcons)
            icon.sprite = null;
        selectedWeapon = null;
    }

    public override void Close()
    {
        base.Close();
    }
}
