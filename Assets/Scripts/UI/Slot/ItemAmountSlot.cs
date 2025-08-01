using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemAmountSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    public void Init(Sprite iconSprite, int amount)
    {
        icon.sprite = iconSprite;
        icon.color = Color.white;
        amountText.text = amount.ToString();
    }
}
