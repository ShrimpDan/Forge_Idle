using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionObjectHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private UIManager uIManager;
    [SerializeField] ButtonType type;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        uIManager = GameManager.Instance.UIManager;

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
