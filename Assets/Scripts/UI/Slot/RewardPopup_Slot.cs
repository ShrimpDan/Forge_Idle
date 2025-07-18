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
        icon.sprite = IconLoader.GetIconByPath(itemData.IconPath);
        quantity = count;
        quantityText.text = quantity.ToString();
    }

    // ���� ���� �� Add�ϴ� ���̸� ���
    public void Add(int count)
    {
        quantity += count;
        quantityText.text = quantity.ToString();
    }
}
