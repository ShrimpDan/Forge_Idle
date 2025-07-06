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
        UpdateState(false);
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

    private Sprite LoadIconSprite(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;  //없는거
        }

        var sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.Log("사진 없음");
        }
        return sprite;

    }

}
