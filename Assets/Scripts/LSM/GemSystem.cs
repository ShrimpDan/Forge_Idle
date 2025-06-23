using UnityEngine;

public class GemSystem : MonoBehaviour
{
    public int removeGemCost = 200;

    public string AttachGem(Weapon weapon, Gem gem)
    {
        if (weapon.equippedGems.Count >= weapon.maxGemSlots)
            return "장착 가능한 소켓이 없습니다.";

        if (InventoryManager.Instance.HasEnoughItems(gem, 1))
        {
            weapon.equippedGems.Add(gem);
            InventoryManager.Instance.RemoveItem(gem);
            return $"{gem.itemName} 보석 장착 성공!";
        }
        else
        {
            return "보석이 인벤토리에 없습니다.";
        }
    }

    public string DetachGem(Weapon weapon, Gem gem)
    {
        if (!weapon.equippedGems.Contains(gem))
            return "이 보석은 장착되어 있지 않습니다.";

        if (InventoryManager.Instance.SpendGold(removeGemCost))
        {
            weapon.equippedGems.Remove(gem);
            InventoryManager.Instance.AddItem(gem);
            return $"{gem.itemName} 보석 제거 성공! ({removeGemCost}골드 소모)";
        }
        else
        {
            return "골드가 부족하여 보석을 제거할 수 없습니다.";
        }
    }
}
