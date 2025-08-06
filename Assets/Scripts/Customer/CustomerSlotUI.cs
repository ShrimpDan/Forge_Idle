using Assets.PixelFantasy.PixelHeroes.Common.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CustomerSlotUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite silhouetteSprite; //실루엣
    [SerializeField] private Image gaugeBar;
    [SerializeField] private TextMeshProUGUI progressText;

    private Button btn;
    


    private RegularCustomerData data;
    private CustomerCollectionData collectionData;
    public void Initialize(RegularCustomerData data , CustomerCollectionData collectionData)
    {
        this.data = data;
        this.collectionData = collectionData;
        bool isDiscovered = GameManager.Instance.CollectionManager.IsDiscovered(data);
        btn = GetComponent<Button>();
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
            btn.onClick.AddListener(() =>
            {
                GameManager.Instance.UIManager.OpenUI<CollectionPopup>(UIName.CollectionPopup).SetPopup(data);

            });
        }
        else
        {
            image.sprite = silhouetteSprite;
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
