using UnityEngine;
using UnityEngine.UI;

// HeldAssistantItem.cs
// 보류 제자 UI에서 각 제자 종이 항목을 구성하는 컴포넌트입니다.
// 아이콘을 표시하고, 클릭 시 해당 제자를 RecruitPreviewManager로 전달합니다.

public class HeldAssistantItem : MonoBehaviour
{
    private AssistantInstance assistantData;
    private HeldListButtonHandler listButtonHandler;

    // 보류 제자 데이터 및 버튼 핸들러 초기화
    public void Init(AssistantInstance data, HeldListButtonHandler handler)
    {
        assistantData = data;
        listButtonHandler = handler;

        SetIcon(data.IconPath);
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

    // 클릭 시 호출 -> HeldListButtonHandler로 전달
    private void OnClick()
    {
        listButtonHandler.OnClickHeldAssistant(assistantData);
    }
}
