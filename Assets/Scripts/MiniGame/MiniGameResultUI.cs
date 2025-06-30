
using UnityEngine;
using UnityEngine.UI;
public class MiniGameResultUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefabs;

    [SerializeField] private Image[] slots; // 미리 할당된 이미지 슬롯들 (보물 개수와 같음)
   
    private int currentIndex = 0;

    public void InitSlotCount(int count)
    {
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);  
        }
        slots = new Image[count];
        currentIndex = 0;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(slotPrefabs, slotParent);
            Image image = obj.GetComponent<Image>();

            image.sprite = null;
            image.color = new Color(1, 1, 1, 0);
            slots[i] = image;
        }


    }   
    public void AddIcon(Sprite icon)
    {
        if (currentIndex >= slots.Length) return;

        slots[currentIndex].sprite = icon;
        slots[currentIndex].color = Color.white;
        currentIndex++;
    }

  
    public void ResetUI()
    {
        currentIndex = 0;
        foreach (var slot in slots)
        {
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0); // 투명하게 초기화
        }
    }
}
