using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputWeaponSlotBtn : MonoBehaviour
{
    [Header("UI References")]
    public Button slotButton;
    public Image icon;
    public Image progressBar;
    public TMP_Text timeText;
    public Button receiveBtn;

    public void SetIcon(Sprite sp)
    {
        icon.sprite = sp;
        icon.enabled = (sp != null);
    }
    public void SetProgress(float normalized)
    {
        if (progressBar)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.fillAmount = normalized;
        }
    }
    public void SetTimeText(string text)
    {
        if (timeText)
        {
            timeText.gameObject.SetActive(true);
            timeText.text = text;
        }
    }
    public void SetReceiveBtnActive(bool active)
    {
        if (receiveBtn)
            receiveBtn.gameObject.SetActive(active);
    }
    public void ResetSlot()
    {
        SetIcon(null);
        if (progressBar) progressBar.gameObject.SetActive(false);
        if (timeText) timeText.gameObject.SetActive(false);
        if (receiveBtn) receiveBtn.gameObject.SetActive(false);
        if (slotButton) slotButton.interactable = false;
    }
}
