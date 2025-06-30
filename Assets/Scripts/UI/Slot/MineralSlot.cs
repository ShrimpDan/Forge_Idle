using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MineralSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text mineralNameText;
    [SerializeField] private Image mineralIcon;
    [SerializeField] private Button assignAssistantBtn;

    public void Init(string mineralName, Sprite mineralSprite, UnityEngine.Events.UnityAction onClick)
    {
        mineralNameText.text = mineralName;
        mineralIcon.sprite = mineralSprite;
        assignAssistantBtn.onClick.RemoveAllListeners();
        assignAssistantBtn.onClick.AddListener(onClick);
    }
}
