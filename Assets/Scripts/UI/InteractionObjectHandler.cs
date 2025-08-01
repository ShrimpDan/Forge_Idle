using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionObjectHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private UIManager uIManager;
    [SerializeField] ButtonType type;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;


    public ButtonType Type => type;

    void Start()
    {
        uIManager = GameManager.Instance.UIManager;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case ButtonType.Craft:
                uIManager.OpenUI<CraftWeaponWindow>(UIName.GetUINameByType(type));
                break;

            case ButtonType.Upgrade:
                uIManager.OpenUI<UpgradeWeaponWindow>(UIName.GetUINameByType(type));
                break;

            case ButtonType.Refine:
                uIManager.OpenUI<RefineSystemWindow>(UIName.GetUINameByType(type));
                break;
        }
       
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        spriteRenderer.color = Color.gray;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        spriteRenderer.color = originalColor;
    }
    
}
