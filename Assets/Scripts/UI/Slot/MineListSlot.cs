using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MineListSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text mineNameText;
    [SerializeField] private Image[] dropIcons; // 2°³
    [SerializeField] private Button selectBtn;

    public void Init(string mineName, Sprite[] drops, UnityEngine.Events.UnityAction onClick)
    {
        mineNameText.text = mineName;
        for (int i = 0; i < dropIcons.Length; i++)
            dropIcons[i].sprite = (drops != null && i < drops.Length) ? drops[i] : null;
        selectBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.AddListener(onClick);
    }
}
