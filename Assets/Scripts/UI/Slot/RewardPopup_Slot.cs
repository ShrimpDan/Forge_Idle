using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPopup_Slot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;

    private string itemKey;
    private int quantity = 0;

    public void Init(ItemData itemData, int count)
    {
        itemKey = itemData.ItemKey;
        icon.sprite = IconLoader.GetIcon(itemData.IconPath);
        quantity = count;
        quantityText.text = quantity.ToString();
    }

    // 만약 여러 번 Add하는 식이면 사용
    public void Add(int count)
    {
        quantity += count;
        quantityText.text = quantity.ToString();
    }
}
