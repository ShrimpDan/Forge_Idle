using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForgeInventorySlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Button slotBtn;

    private ItemInstance item;

    public void Init(ItemInstance item, System.Action onClick)
    {
        this.item = item;

        icon.sprite = IconLoader.GetIcon(item.Data.IconPath);
        nameText.text = item.Data.Name;
        countText.text = item.Quantity.ToString();

        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(() => onClick?.Invoke());
    }
}
