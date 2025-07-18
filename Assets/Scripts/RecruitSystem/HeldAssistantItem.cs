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
        if (icon != null)
        {
            icon.sprite = Resources.Load<Sprite>(data.IconPath);
        }

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        listButtonHandler.OnClickHeldAssistant(assistantData);
    }
}
