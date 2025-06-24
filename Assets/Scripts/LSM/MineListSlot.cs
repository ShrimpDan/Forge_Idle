using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class MineListSlot : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] Image[] dropItemIcons;
    [SerializeField] Button selectBtn;

    private MineData data;
    private Action<MineData> onSelect;

    public void Setup(MineData data, Action<MineData> onSelect)
    {
        this.data = data;
        this.onSelect = onSelect;
        nameText.text = data.Name;

        // 아이콘 2개 세팅
        for (int i = 0; i < dropItemIcons.Length; i++)
        {
            dropItemIcons[i].sprite = (i < data.DropSprites.Count) ? data.DropSprites[i] : null;
            dropItemIcons[i].gameObject.SetActive(data.DropSprites[i] != null);
        }

        selectBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.AddListener(() => onSelect?.Invoke(data));
    }
}
