using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLighttEffectController : MonoBehaviour
{
    [SerializeField] private Material highLightMaterial;
    
    [SerializeField] private Camera uiCam;



    private void Update()
    {
    }


    public void SetHighLightTarget(Transform target)
    {
        Vector3 screenPos = uiCam.WorldToScreenPoint(target.position);

        Vector2 normalize = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);

        highLightMaterial.SetVector("_Center", normalize);
    }
    

    private void Start()
    {
        highLightMaterial.SetFloat("_Radius", 0.2f);
        highLightMaterial.SetFloat("_Softness", 0.05f);
        highLightMaterial.SetColor("_HighlightColor", Color.clear); // 밝은 부분 투명
        highLightMaterial.SetColor("_DimColor", new Color(0f, 0f, 0f, 0.7f)); // 어두운 부분 반투명
    }
}
