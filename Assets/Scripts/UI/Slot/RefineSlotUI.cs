using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RefineSlotUI : MonoBehaviour
{
    public Button inputButton;
    public Image inputIcon;
    public Image outputIcon;
    public Button minusBtn;
    public Button plusBtn;
    public TMP_Text amountText;
    public TMP_Text costText;
    public Button executeBtn;

    public int Amount { get; private set; } = 1;

    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 1, 999);
        if (amountText != null) amountText.text = Amount.ToString();
    }
}
