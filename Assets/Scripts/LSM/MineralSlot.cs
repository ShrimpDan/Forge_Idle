using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MineralSlot : MonoBehaviour
{
    [SerializeField] TMP_Text mineralNameText;
    [SerializeField] Image mineralIcon;
    [SerializeField] Button assistantSlotBtn; // Á¶¼ö ½½·Ô

    public void Setup(string mineralName, Sprite icon, Action<MineralSlot> onAssign)
    {
        mineralNameText.text = mineralName;
        mineralIcon.sprite = icon;
        assistantSlotBtn.onClick.RemoveAllListeners();
        assistantSlotBtn.onClick.AddListener(() => onAssign?.Invoke(this));
    }
}
