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
    public static event Action<GameObject> OnPointerClicked;


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
            case ButtonType.Sell:
                uIManager.OpenUI<SellWeaponWindow>(UIName.GetUINameByType(type));

                break;

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
        OnPointerClicked?.Invoke(this.gameObject); //전달
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
