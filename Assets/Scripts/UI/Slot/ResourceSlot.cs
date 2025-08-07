using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;

    public void Set(Sprite sprite, int owned, int needed)
    {
        icon.sprite = sprite;
        icon.enabled = (sprite != null);
        amountText.text = $"{owned}/{needed}";
        amountText.color = owned < needed ? Color.red : Color.white;
    }

    public void SetDungeonResource(Sprite sprite, int min, int max)
    {
        icon.sprite = sprite;
        amountText.text = $"{min}~{max}";
    }
}
