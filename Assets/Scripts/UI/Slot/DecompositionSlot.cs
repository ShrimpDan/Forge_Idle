using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecompositionSlot : MonoBehaviour
{
    private DecompositionWindow decompositionWindow;
    private ItemInstance slotItem;

    [Header("UI Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject selectIndicator;

    private Button slotBtn;

    private bool isSelected = false;

    public void Init(DecompositionWindow window, ItemInstance item)
    {
        decompositionWindow = window;
        slotItem = item;

        nameText.text = item.Data.Name;
        icon.sprite = IconLoader.GetIconByKey(slotItem.ItemKey);
        countText.gameObject.SetActive(false);

        slotBtn = GetComponent<Button>();
        slotBtn.onClick.RemoveAllListeners();
        slotBtn.onClick.AddListener(ClickSlotBtn);
    }

    public void Init(ItemData itemData, int minCount, int maxCount)
    {
        nameText.text = itemData.Name;
        icon.sprite = IconLoader.GetIconByKey(itemData.ItemKey);
        countText.text = $"{minCount}~{maxCount}";

        slotBtn = GetComponent<Button>();
        slotBtn.enabled = false;
    }

    private void ClickSlotBtn()
    {
        if (slotItem == null) return;

        isSelected = !isSelected;

        selectIndicator.SetActive(isSelected);
        decompositionWindow.ClickWeaponSlot(slotItem, isSelected);
    }
}
