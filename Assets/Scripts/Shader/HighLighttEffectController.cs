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

        // Canvas가 Screen Space - Overlay 모드일 경우, target의 월드 좌표가 아닌 RectTransform의 위치를 사용해야 합니다.
        // 만약 Overlay 모드를 사용하신다면 아래 주석처리된 코드를 대신 사용해보세요.
        // Vector3 screenPos = target.position;

        Vector2 normalizedPos = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        highLightMaterial.SetVector("_Center", normalizedPos);

        highlightImage.enabled = true;
    }

    public void HideHighlight()
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = false;
        }
    }
}