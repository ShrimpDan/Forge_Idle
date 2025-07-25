using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDrawSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private Image glowEffect;
    public System.Action OnAnimationComplete;
    private Material glowMaterialInstance; 
    
    [SerializeField] private Material defaultUIMaterial; 

    [Header("Glow Effect Settings")]
    [SerializeField] private Color startGlowColor = Color.white; 
    [SerializeField] private float startGlowIntensity = 5.0f;    
    [SerializeField] private float endGlowIntensity = 0.0f;       
    [SerializeField] private float fadeOutDuration = 0.3f;
    
    public void SetSlot(SkillData skillData)
    {
        icon.sprite = IconLoader.GetIconByPath(skillData.IconPath);
        skillName.text = skillData.Name;

        if (glowEffect.material != null)
        {
            glowMaterialInstance = Instantiate(glowEffect.material);
            glowEffect.material = glowMaterialInstance;
        }
        else
        {
            Debug.LogWarning("No material assigned to slotImage. Defaulting to a new material.", this);
            glowMaterialInstance = new Material(Shader.Find("UI/Default"));
            glowEffect.material = glowMaterialInstance;
        }

        if (glowMaterialInstance.HasProperty("_GlowIntensity"))
        {
            glowMaterialInstance.SetFloat("_GlowIntensity", endGlowIntensity);
        }
    }

    public void PlayDrawAnimation(float duration)
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 3f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(originalScale, duration * 0.5f).SetEase(Ease.OutBack));

        if (glowMaterialInstance != null && glowMaterialInstance.HasProperty("_GlowColor") && glowMaterialInstance.HasProperty("_GlowIntensity"))
        {
            glowMaterialInstance.SetColor("_GlowColor", startGlowColor);

            sequence.Join(DOTween.To(() => glowMaterialInstance.GetFloat("_GlowIntensity"), 
                                     x => glowMaterialInstance.SetFloat("_GlowIntensity", x), 
                                     startGlowIntensity, 
                                     duration * 0.5f) 
                              .SetEase(Ease.OutSine)); 

            sequence.Join(DOTween.To(() => glowMaterialInstance.GetFloat("_GlowIntensity"),
                                     x => glowMaterialInstance.SetFloat("_GlowIntensity", x),
                                     endGlowIntensity, 
                                     duration * 0.5f) 
                              .SetEase(Ease.InQuad) 
                              .SetDelay(duration * 0.5f));
        }

        sequence.Join(glowEffect.DOFade(0f, fadeOutDuration).SetEase(Ease.OutQuad)); 

        sequence.OnComplete(() =>
        {
            OnAnimationComplete?.Invoke();
            glowEffect.enabled = false;
        });
    }
}
