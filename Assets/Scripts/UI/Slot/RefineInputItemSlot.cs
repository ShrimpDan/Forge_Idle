using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RefineInputItemSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text amountText;

    private static readonly Color EnoughColor = new Color32(50, 130, 255, 255); // �Ķ�
    private static readonly Color NotEnoughColor = new Color32(230, 40, 40, 255); // ����

    public void Set(ItemData item, int owned, int required)
    {
        icon.sprite = item != null ? IconLoader.GetIconByKey(item.ItemKey) : null;
        icon.enabled = item != null;
        itemName.text = item != null ? item.Name : "";
        amountText.text = $"{owned}/{required}";

        if (owned >= required)
            amountText.color = EnoughColor;
        else
            amountText.color = NotEnoughColor;
    }
}
