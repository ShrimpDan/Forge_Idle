using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RefineOutputSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text description;

    public void Set(ItemData data)
    {
        if (icon != null)
            icon.sprite = data != null ? IconLoader.GetIcon(data.IconPath) : null;
        if (itemName != null)
            itemName.text = data != null ? data.Name : "";
        if (description != null)
            description.text = data != null ? data.Description : "";
        gameObject.SetActive(data != null);
    }
}
