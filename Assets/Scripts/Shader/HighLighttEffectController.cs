using UnityEngine;
using UnityEngine.UI;

public class HighLightEffectController : MonoBehaviour
{
    [SerializeField] private RawImage highlightImage;
    private Material highLightMaterial;

    void Awake()
    {
        if (highlightImage != null)
        {
            highLightMaterial = Instantiate(highlightImage.material);
            highlightImage.material = highLightMaterial;
        }
    }

    private void Start()
    {
        if (highLightMaterial != null)
        {
            highLightMaterial.SetFloat("_Radius", 0.15f);
            highLightMaterial.SetFloat("_Softness", 0.1f);
            highLightMaterial.SetColor("_HighlightColor", Color.clear);
            highLightMaterial.SetColor("_DimColor", new Color(0f, 0f, 0f, 0.7f));
        }
        HideHighlight();
    }


    public void ShowHighlight(Transform target, Camera camera)
    {
        if (highLightMaterial == null || highlightImage == null || camera == null)
        {
            return;
        }


        Vector3 screenPos = camera.WorldToScreenPoint(target.position);

        Vector2 normalizedPos = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        highLightMaterial.SetVector("_Center", normalizedPos);

        highlightImage.enabled = true;
    }

    public void ShowHighlight(Vector2 pos) //위치로만 하이라이트를 주는것이 가능하게 
    {
        if (highLightMaterial == null || highlightImage == null)
        {
            return;
        }
        Vector2 normalizedPos = new Vector2(pos.x / Screen.width, pos.y / Screen.height);

        highLightMaterial.SetVector("_Center", normalizedPos);

    }
    public void ShowHighlight(Vector2 pos, float highlightWidth) //넓이 조절가능
    {
        if (highLightMaterial == null || highlightImage == null)
        {
            return;
        }
        Vector2 normalizedPos = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
        highLightMaterial.SetVector("_Center", normalizedPos);
        highLightMaterial.SetFloat("_HighlightWidth", highlightWidth);
    
    }

    
     public void ResetHighlightSize()
    {
        if (highLightMaterial != null)
        {
            // 크기 값만 기본인 (1, 1)로 되돌립니다.
            highLightMaterial.SetVector("_HighlightSize", Vector2.one); // Vector2.one은 new Vector2(1, 1)과 같습니다.
        }
    }

    public void HideHighlight()
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = false;
        }
    }
    

}