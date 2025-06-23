using UnityEngine;

public class RefineSystem : MonoBehaviour
{
    public MaterialItem ore;
    public MaterialItem ingot;
    public MaterialItem gemOre;
    public Gem gem;
    public MaterialItem strangeStone;

    public void Refine(MaterialItem material)
    {
        if (material.materialType == MaterialType.Ore)
        {
            if (InventoryManager.Instance.HasEnoughItems(material, 1))
            {
                InventoryManager.Instance.RemoveItem(material);
                InventoryManager.Instance.AddItem(ingot);
                Debug.Log("정련 성공! 원석 → 주괴");
            }
        }
        else if (material.materialType == MaterialType.GemOre)
        {
            if (InventoryManager.Instance.HasEnoughItems(material, 1))
            {
                InventoryManager.Instance.RemoveItem(material);
                InventoryManager.Instance.AddItem(gem);
                Debug.Log("세공 성공! 보석 원석 → 보석");
            }
        }
        else if (material.materialType == MaterialType.StrangeStone)
        {
            if (InventoryManager.Instance.HasEnoughItems(material, 1))
            {
                InventoryManager.Instance.RemoveItem(material);
                InventoryManager.Instance.AddItem(ingot);
                InventoryManager.Instance.AddItem(gem);
                Debug.Log("이상한 돌 → 주괴 & 보석 둘 다 획득!");
            }
        }
        else
        {
            Debug.LogWarning("정련/세공 불가능한 재료입니다.");
        }
    }
}
