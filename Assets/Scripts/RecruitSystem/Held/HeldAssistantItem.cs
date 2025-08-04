using UnityEngine;
using UnityEngine.UI;

// HeldAssistantItem.cs
// 보류 제자 UI에서 각 제자 종이 항목을 구성하는 컴포넌트입니다.
// 아이콘을 표시하고, 클릭 시 해당 제자를 RecruitPreviewManager로 전달합니다.

public class HeldAssistantItem : MonoBehaviour
{
    private AssistantInstance assistantData;
    private HeldListButtonHandler listButtonHandler;

    // ▼ Inspector에 드래그해서 할당
    [Header("Rank Icon")]
    [SerializeField] private Image rankIconImage;
    [SerializeField] private Sprite rankN;
    [SerializeField] private Sprite rankR;
    [SerializeField] private Sprite rankSR;
    [SerializeField] private Sprite rankSSR;
    [SerializeField] private Sprite rankUR;

    // 보류 제자 데이터 및 버튼 핸들러 초기화
    public void Init(AssistantInstance data, HeldListButtonHandler handler)
    {
        assistantData = data;
        listButtonHandler = handler;

        SetIcon(data.IconPath);
        SetRankIcon(data.grade);
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // 아이콘 이미지 로딩 및 설정
    private void SetIcon(string iconPath)
    {
        var icon = transform.Find("Held_Icon")?.GetComponent<Image>();
        if (icon == null || string.IsNullOrEmpty(iconPath)) return;

        var sprite = Resources.Load<Sprite>(iconPath);
        if (sprite != null)
        {
            icon.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"[HeldAssistantItem] 아이콘 경로 오류: '{iconPath}' 스프라이트를 찾을 수 없습니다.");
        }
    }

    // ▼ 등급 아이콘 로딩/표시
    private void SetRankIcon(string grade)
    {
        if (rankIconImage == null) return;
        switch (grade)
        {
            case "N": rankIconImage.sprite = rankN; break;
            case "R": rankIconImage.sprite = rankR; break;
            case "SR": rankIconImage.sprite = rankSR; break;
            case "SSR": rankIconImage.sprite = rankSSR; break;
            case "UR": rankIconImage.sprite = rankUR; break;
            default: rankIconImage.sprite = null; break;
        }
        rankIconImage.enabled = rankIconImage.sprite != null;
    }

    // 클릭 시 호출 -> HeldListButtonHandler로 전달,
    private void OnClick()
    {
        listButtonHandler.OnClickHeldAssistant(assistantData);
    }
}
