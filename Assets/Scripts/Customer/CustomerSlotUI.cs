using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CustomerSlotUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite silhouetteSprite; //실루엣
    [SerializeField] private Image gaugeBar;
    [SerializeField] private TextMeshProUGUI progressText;
    


    private RegularCustomerData data;
    private CustomerCollectionData collectionData;
    public void Initialize(RegularCustomerData data , CustomerCollectionData collectionData)
    {
        this.data = data;
        this.collectionData = collectionData;
        bool isDiscovered = GameManager.Instance.CollectionManager.IsDiscovered(data);

        UpdateState(isDiscovered);
    }



    public void UpdateState(bool discovered)
    {
        if (image == null)
        {
            image = GetComponent<Image>();

        }

        if (discovered)
        {
          
            image.sprite = IconLoader.GetIconByPath(data.iconPath);
            image.color = Color.white;
        }
        else
        {
            image.sprite = silhouetteSprite;
            image.color = new Color(1, 1, 1, 0.3f);
        }


        if (gaugeBar != null)
        {
            gaugeBar.fillAmount = (float)collectionData.visitedCount / collectionData.maxVisitedCount;
        }
        if (progressText != null)
        {
            progressText.text = $"{collectionData.visitedCount}/ {collectionData.maxVisitedCount}";
        }
    }

}
