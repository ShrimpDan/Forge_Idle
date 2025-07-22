using UnityEngine;
using UnityEngine.UI;

public class HeldAssistantItem : MonoBehaviour
{
    private AssistantInstance assistantData;
    private HeldListButtonHandler listButtonHandler;

    public void Init(AssistantInstance data, HeldListButtonHandler handler)
    {
        assistantData = data;
        listButtonHandler = handler;

        var icon = transform.Find("Held_Icon")?.GetComponent<Image>();
        if (icon != null && !string.IsNullOrEmpty(data.IconPath))
        {
            var sprite = Resources.Load<Sprite>(data.IconPath);
            if (sprite != null)
            {
                icon.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"[HeldAssistantItem] 아이콘 경로 오류: '{data.IconPath}' 스프라이트를 찾을 수 없습니다.");
            }
        }

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        listButtonHandler.OnClickHeldAssistant(assistantData);
    }
}
