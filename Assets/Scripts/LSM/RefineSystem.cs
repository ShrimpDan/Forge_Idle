using UnityEngine;

public class RefineSystem : MonoBehaviour
{
    public MaterialItem ore;
    public MaterialItem ingot;
    public MaterialItem gemOre;
    public Gem gem;
    public MaterialItem strangeStone;

    public string Refine(MaterialItem material)
    {
        if (material.materialType == MaterialType.Ore)
        {
            if (InventoryManager.Instance.HasEnoughItems(material, 1))
            {
                InventoryManager.Instance.RemoveItem(material);
                InventoryManager.Instance.AddItem(ingot);
                return "정련 성공! 원석 → 주괴";
            }
            else return "원석이 부족합니다!";
        }
        else if (material.materialType == MaterialType.GemOre)
        {
            if (InventoryManager.Instance.HasEnoughItems(material, 1))
            {
                InventoryManager.Instance.RemoveItem(material);
                InventoryManager.Instance.AddItem(gem);
                return "세공 성공! 보석 원석 → 보석";
            }
            else return "보석 원석이 부족합니다!";
        }
        else if (material.materialType == MaterialType.StrangeStone)
        {
            if (InventoryManager.Instance.HasEnoughItems(material, 1))
            {
                InventoryManager.Instance.RemoveItem(material);
                InventoryManager.Instance.AddItem(ingot);
                InventoryManager.Instance.AddItem(gem);
                return "이상한 돌 → 주괴 & 보석 둘 다 획득!";
            }
            else return "이상한 돌이 부족합니다!";
        }
        else
        {
            return "정련/세공 불가능한 재료입니다.";
        }
    }
}
