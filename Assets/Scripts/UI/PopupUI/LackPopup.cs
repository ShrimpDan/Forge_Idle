using UnityEngine;
using TMPro;


/// ����
/// [SerializeField] private LackPopup lackPopupPrefab; 
/// [SerializeField] private Transform popupParent;  
/// // ��� ����
/// var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
/// popup.Show(LackType.Gold);
/// 
/// // ��� ����
/// var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
/// popup.Show(LackType.Resource);
///
/// // ����Ʈ ����
/// var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
/// popup.Show(LackType.Point);
/// 
public enum LackType
{
    Gold,
    Resource,
    Point
}

public class LackPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text messageText;

    [Header("Animation Settings")]
    [SerializeField] private float popupDuration = 1.2f;   // �˾� ���� �ð� (��)
    [SerializeField] private float animDuration = 0.25f;   // �˾� ���� �ݴ� �ִϸ��̼� �ð� (��)

    private bool isShowing = false;

    public void Show(LackType type)
    {
        if (isShowing) return;
        isShowing = true;

        messageText.text = type switch
        {
            LackType.Gold => "��尡 �����մϴ�.",
            LackType.Resource => "��ᰡ �����մϴ�.",
            LackType.Point => "����Ʈ�� �����մϴ�.",
            _ => "�ʿ��� �ڿ��� �����մϴ�."
        };

        UIEffect.PopupOpenEffect(panel, animDuration);

        Invoke(nameof(Hide), popupDuration + animDuration);
    }

    /// Ŀ���� �޽��� �˾�
    public void ShowCustom(string customMsg)
    {
        if (isShowing) return;
        isShowing = true;

        messageText.text = customMsg;

        UIEffect.PopupOpenEffect(panel, animDuration);
        Invoke(nameof(Hide), popupDuration + animDuration);
    }

    private void Hide()
    {
        UIEffect.PopupCloseEffect(panel, animDuration);
        Invoke(nameof(DestroySelf), animDuration); // �ִϸ��̼� ������ ������Ʈ ����
    }

    private void DestroySelf()
    {
        isShowing = false;
        Destroy(gameObject);
    }
}
