using UnityEngine;
using UnityEngine.UI;
public class CustomerSlotUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite silhouetteSprite; //실루엣


    private RegularCustomerData data;

    public void Initialize(RegularCustomerData data)
    {
        this.data = data;
        bool isDiscovered = CollectionBookManager.Instance.IsDiscovered(data);

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
          
            image.sprite = IconLoader.GetIcon(data.iconPath);
            image.color = Color.white;
        }
        else
        {
            image.sprite = silhouetteSprite;
            image.color = new Color(1, 1, 1, 0.3f);
        }
    }

}
