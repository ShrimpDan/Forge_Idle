using UnityEngine;
using TMPro;


/// 사용법
/// [SerializeField] private LackPopup lackPopupPrefab; 
/// [SerializeField] private Transform popupParent;  
/// // 골드 부족
/// var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
/// popup.Show(LackType.Gold);
/// 
/// // 재료 부족
/// var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
/// popup.Show(LackType.Resource);
///
/// // 포인트 부족
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
    [SerializeField] private float popupDuration = 1.2f;   // 팝업 유지 시간 (초)
    [SerializeField] private float animDuration = 0.25f;   // 팝업 열고 닫는 애니메이션 시간 (초)

    private bool isShowing = false;

    public void Show(LackType type)
    {
        if (isShowing) return;
        isShowing = true;

        messageText.text = type switch
        {
            LackType.Gold => "골드가 부족합니다.",
            LackType.Resource => "재료가 부족합니다.",
            LackType.Point => "포인트가 부족합니다.",
            _ => "필요한 자원이 부족합니다."
        };

        UIEffect.PopupOpenEffect(panel, animDuration);

        Invoke(nameof(Hide), popupDuration + animDuration);
    }

    /// 커스텀 메시지 팝업
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
        Invoke(nameof(DestroySelf), animDuration); // 애니메이션 끝나고 오브젝트 삭제
    }

    private void DestroySelf()
    {
        isShowing = false;
        Destroy(gameObject);
    }
}
