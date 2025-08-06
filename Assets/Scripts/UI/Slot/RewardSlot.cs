using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI nameText;

    private int quantity = 0;

    public ItemData SlotItem { get; private set; }

    public void Init(ItemData item, int amount)
    {
        SlotItem = item;
        quantity += amount;

        icon.sprite = IconLoader.GetIconByKey(item.ItemKey);
        quantityText.text = quantity.ToString();
        nameText.text = item.Name;
    }

    public void AddItem(int amount)
    {
        quantity += amount;
        quantityText.text = quantity.ToString();
    }
}
