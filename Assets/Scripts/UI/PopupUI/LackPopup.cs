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
    [Header("UI References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text messageText;

    [Header("Animation Settings")]
    [SerializeField] private float popupDuration = 1.2f;   // 팝업 표시 시간(초)
    [SerializeField] private float animDuration = 0.25f;   // 팝업 오픈/클로즈 애니메이션 시간(초)

    private bool isShowing = false;

    public override UIType UIType => UIType.Popup;

    /// 부족 유형에 따른 메시지 팝업
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
        Invoke(nameof(DestroySelf), animDuration);
        // BaseUI.Init을 통해 uIManager가 할당되어 있다면 CloseUI 호출
        if (uIManager != null)
            uIManager.CloseUI(UIName.LackPopup);
    }

    private void DestroySelf()
    {
        isShowing = false;
        Destroy(gameObject);
    }

}
