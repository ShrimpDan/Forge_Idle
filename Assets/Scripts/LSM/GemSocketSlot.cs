using UnityEngine;
using UnityEngine.UI;

public class GemSocketSlot : MonoBehaviour
{
    public Image iconImage;
    public Button selectButton;

    private System.Action<GemSocketSlot> onClick;
    public int socketIndex;

    public void SetIcon(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }

    public void Init(int index, System.Action<GemSocketSlot> onClick)
    {
        this.socketIndex = index;
        this.onClick = onClick;
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onClick(this));
    }
}
