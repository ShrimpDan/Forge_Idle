using UnityEngine;
using TMPro;

/// 부족 자원 팝업 예시 사용법
/// [SerializeField] private LackPopup lackPopupPrefab;
/// [SerializeField] private Transform popupParent;
/// var popup = Instantiate(lackPopupPrefab, popupParent ? popupParent : null);
/// popup.Init(gameManager, uIManager);
/// popup.Show(LackType.Gold);       // 골드 부족
/// popup.Show(LackType.Resource);   // 재료 부족
/// popup.Show(LackType.Point);      // 포인트 부족
/// popup.Show(LackType.Dia);        // 다이아 부족
/// 
/// 커스텀 메시지는 popup.ShowCustom("직접 입력한 메시지");
public enum LackType
{
    Gold,
    Dia,
    Resource,
    Point
}

public class LackPopup : BaseUI
{
    private static LackPopup currentPopup;

    [Header("UI References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text messageText;

    [Header("Animation Settings")]
    [SerializeField] private float popupDuration = 1.2f;
    [SerializeField] private float animDuration = 0.25f;

    private bool isShowing = false;
    public override UIType UIType => UIType.Popup;

    private void Awake()
    {
        // 이미 떠 있는 LackPopup 있으면 파괴 (DestroySelf 안전하게 호출)
        if (currentPopup != null && currentPopup != this)
        {
            currentPopup.ForceClose();
        }
        currentPopup = this;
    }

    // 강제 닫기 함수
    private void ForceClose()
    {
        isShowing = false;
        Destroy(gameObject);
        currentPopup = null;
    }

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
        Invoke(nameof(DestroySelf), animDuration);

        if (uIManager != null)
            uIManager.CloseUI(UIName.LackPopup);
    }

    private void DestroySelf()
    {
        isShowing = false;
        if (currentPopup == this) currentPopup = null;
        Destroy(gameObject);
    }
}
