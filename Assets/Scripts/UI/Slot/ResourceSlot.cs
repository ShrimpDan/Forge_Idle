using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text nameText;

    public void Set(string name, Sprite sprite, int owned, int needed)
    {
        icon.sprite = sprite;
        icon.enabled = (sprite != null);
        amountText.text = $"{owned}/{needed}";
        amountText.color = owned < needed ? Color.red : Color.white;
        nameText.text = name;
    }

    public void SetDungeonResource(string name, Sprite sprite, int min, int max)
    {
        icon.sprite = sprite;
        amountText.text = $"{min}~{max}";
        nameText.text = name;
    }
}
