using UnityEngine;

public class GemSystem : MonoBehaviour
{
    public int removeGemCost = 200;

    public void AttachGem(Weapon weapon, Gem gem)
    {
        if (weapon.equippedGems.Count >= weapon.maxGemSlots)
        {
            Debug.LogWarning("장착 가능한 소켓이 없습니다.");
            return;
        }

        if (InventoryManager.Instance.HasEnoughItems(gem, 1))
        {
            weapon.equippedGems.Add(gem);
            InventoryManager.Instance.RemoveItem(gem);
            Debug.Log($"{gem.itemName} 보석 장착 성공!");
        }
        else
        {
            Debug.LogWarning("보석이 인벤토리에 없습니다.");
        }
    }

    public void DetachGem(Weapon weapon, Gem gem)
    {
        if (!weapon.equippedGems.Contains(gem))
        {
            Debug.LogWarning("이 보석은 장착되어 있지 않습니다.");
            return;
        }

        if (InventoryManager.Instance.SpendGold(removeGemCost))
        {
            weapon.equippedGems.Remove(gem);
            InventoryManager.Instance.AddItem(gem);
            Debug.Log($"{gem.itemName} 보석 제거 성공! ({removeGemCost}골드 소모)");
        }
        else
        {
            Debug.LogWarning("골드가 부족하여 보석을 제거할 수 없습니다.");
        }
    }
}
