using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;

    public void Set(Sprite sprite, int owned, int needed)
    {
        if (icon != null)
        {
            icon.sprite = sprite;
            icon.enabled = (sprite != null);
        }
        if (amountText != null)
        {
            amountText.text = $"{owned}/{needed}";
            amountText.color = owned < needed ? Color.red : Color.white;
        }
    }
}
