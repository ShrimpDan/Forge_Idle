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

        // ������/�̸�
        if (weapon.Data != null)
        {
            if (icon != null)
            {
                icon.sprite = IconLoader.GetIconByKey(weapon.ItemKey);
                icon.enabled = true;
            }
            if (nameText != null)
            {
                nameText.text = weapon.Data.Name;
            }
        }

        // �� ���� ������
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
                        img.sprite = IconLoader.GetIconByPath(gem.Data.IconPath);
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

        // ��ư ���
        var btn = GetComponent<Button>();
        if (btn == null) btn = gameObject.AddComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            onSelect?.Invoke(weaponData);
        });
    }
}
