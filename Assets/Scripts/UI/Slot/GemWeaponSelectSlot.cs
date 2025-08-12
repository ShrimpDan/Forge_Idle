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
    [SerializeField] private TMP_Text enhanceText;

    private ItemInstance weaponData;
    private Action<ItemInstance> onSelect;

    public void Init(ItemInstance weapon, Action<ItemInstance> onSelectCallback)
    {
        weaponData = weapon;
        onSelect = onSelectCallback;

        if (weapon.Data != null)
        {
            if (icon != null)
            {
                icon.sprite = IconLoader.GetIconByKey(weapon.Data.ItemKey);
                icon.enabled = true;
            }
            if (nameText != null)
            {
                nameText.text = weapon.Data.Name;
            }
        }

        UpdateEnhanceText(weapon?.CurrentEnhanceLevel ?? 0);

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
                        img.sprite = IconLoader.GetIconByKey(gem.Data.ItemKey);
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

        var btn = GetComponent<Button>();
        if (btn == null) btn = gameObject.AddComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onSelect?.Invoke(weaponData));
    }

    private void UpdateEnhanceText(int enhanceLevel)
    {
        if (enhanceText == null)
            return;

        if (enhanceLevel > 0)
        {
            enhanceText.gameObject.SetActive(true);
            enhanceText.text = $"+{enhanceLevel}";

            if (enhanceLevel <= 5)
                enhanceText.color = Color.green;
            else if (enhanceLevel <= 8)
                enhanceText.color = new Color(0.28f, 0.53f, 1f);
            else if (enhanceLevel <= 10)
                enhanceText.color = new Color(0.8f, 0.35f, 1f);
            else if (enhanceLevel <= 13)
                enhanceText.color = new Color(1f, 0.5f, 0f); 
            else
                enhanceText.color = Color.red;
        }
        else
        {
            enhanceText.text = "";
            enhanceText.gameObject.SetActive(false);
        }
    }
}
