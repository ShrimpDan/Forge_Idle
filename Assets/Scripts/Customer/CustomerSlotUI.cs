using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CustomerSlotUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite silhouetteSprite; //실루엣

    private RegualrCustomerData data;


    public void Initialize(RegualrCustomerData data)
    {
        this.data = data;

    }


    public void UpdateState(bool discovered)
    {
        if (discovered)
        {
            image.sprite = data.Icon;
            image.color = Color.white;
        }
        else
        {
            image.sprite = silhouetteSprite;
            image.color = new Color(1, 1, 1, 0.3f);
        }
    }

}
