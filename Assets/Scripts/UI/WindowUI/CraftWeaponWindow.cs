using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftWeaponWindow : BaseUI
{
    public override UIType UIType => UIType.Window;

    [Header("UI Elements")]
    [SerializeField] private Button exitBtn;

    [SerializeField] private Transform inputWeaponSlots;   // 슬롯들의 부모 오브젝트
    [SerializeField] private GameObject weaponSlotBtnPrefab; // 슬롯 버튼 프리팹 (Button+Image 포함)

    private List<Button> slotButtons = new List<Button>();
    private List<Image> slotIcons = new List<Image>();
    private int slotCount = 6;

    private ItemData selectedWeapon;

    private void Awake()
    {
        exitBtn.onClick.AddListener(Close);

        // 기존 수동 생성 슬롯 오브젝트 제거
        foreach (Transform child in inputWeaponSlots)
            Destroy(child.gameObject);

        slotButtons.Clear();
        slotIcons.Clear();

        // 슬롯 자동 생성
        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = Instantiate(weaponSlotBtnPrefab, inputWeaponSlots);
            Button btn = go.GetComponent<Button>();
            Image icon = go.GetComponent<Image>();
            int idx = i;
            btn.onClick.AddListener(() => OnClickInputWeaponSlot(idx));
            slotButtons.Add(btn);
            slotIcons.Add(icon);

            // 초기화: 아이콘 비움 등
            icon.sprite = null;
        }
    }

    private void OnClickInputWeaponSlot(int index)
    {
        UIManager.Instance.OpenUI<Forge_AssistantPopup>(UIName.Forge_AssistantPopup);
    }

    // 무기 선택 시 아이콘 적용
    public void OnWeaponSelected(ItemData weapon)
    {
        selectedWeapon = weapon;
        if (slotIcons.Count > 0)
            slotIcons[0].sprite = LoadIcon(weapon.IconPath); // 첫 번째 슬롯 예시
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
