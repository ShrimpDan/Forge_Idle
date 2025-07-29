using UnityEngine;
using TMPro;


/// ����
/// [SerializeField] private LackPopup lackPopupPrefab; 
/// [SerializeField] private Transform popupParent;  
/// // ��� 
/// var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
/// popup.Show(LackType.Gold);
/// 
/// // ��� 
/// var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
/// popup.Show(LackType.Resource);
///
/// // ����Ʈ
/// var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
/// popup.Show(LackType.Point);
/// 
public enum LackType
{
    Gold,
    Dia,
    Resource,
    Point
}

public class LackPopup : BaseUI
{
    [Header("UI References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text messageText;

    [Header("Animation Settings")]
    [SerializeField] private float popupDuration = 1.2f;   // �˾� ���� �ð� (��)
    [SerializeField] private float animDuration = 0.25f;   // �˾� ���� �ݴ� �ִϸ��̼� �ð� (��)

    private bool isShowing = false;

    public override UIType UIType => UIType.Popup;

    public void Show(LackType type)
    {
        if (isShowing) return;
        isShowing = true;

        messageText.text = type switch
        {
            LackType.Gold => "골드가 부족합니다.",
            LackType.Dia => "다이아가 부족합니다.",
            LackType.Resource => "재료가 부족합니다.",
            LackType.Point => "포인트가 부족합니다.",
            _ => "필요한 자원이 부족합니다."
        };

        SoundManager.Instance?.Play("LackSound");

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
        uIManager.CloseUI(UIName.LackPopup);
    }

    private void DestroySelf()
    {
        isShowing = false;
        Destroy(gameObject);
    }
}
