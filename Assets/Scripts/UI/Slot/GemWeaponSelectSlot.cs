using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GemWeaponSelectSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image[] gemSlotIcons; 

    private ItemInstance weaponData;
    private Action<ItemInstance> onSelect;

    public void Init(ItemInstance weapon, Action<ItemInstance> onSelectCallback)
    {
        weaponData = weapon;
        onSelect = onSelectCallback;

        // 아이콘/이름
        if (weapon.Data != null)
        {
            if (icon != null)
            {
                icon.sprite = IconLoader.GetIcon(weapon.Data.IconPath);
                icon.enabled = true;
            }
            if (nameText != null)
            {
                nameText.text = weapon.Data.Name;
            }
        }

        // 젬 슬롯 아이콘
        if (gemSlotIcons != null && weapon.GemSockets != null)
        {
            for (int i = 0; i < gemSlotIcons.Length; i++)
            {
                var img = gemSlotIcons[i];
                var gem = (weapon.GemSockets.Count > i) ? weapon.GemSockets[i] : null;
                if (img != null)
                {
                    if (gem != null && gem.Data != null)
                    {
                        img.sprite = IconLoader.GetIcon(gem.Data.IconPath);
                        img.enabled = true;
                    }
                    else
                    {
                        img.sprite = null;
                        img.enabled = false;
                    }
                }
            }
        }

        // 버튼 등록
        var btn = GetComponent<Button>();
        if (btn == null) btn = gameObject.AddComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            onSelect?.Invoke(weaponData);
        });
    }
}
