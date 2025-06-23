using UnityEngine;

public class GemSystem : MonoBehaviour
{
    public int removeGemCost = 200;

    public void AttachGem(Weapon weapon, Gem gem)
    {
        if (weapon.equippedGems.Count >= weapon.maxGemSlots)
        {
            Debug.LogWarning("���� ������ ������ �����ϴ�.");
            return;
        }

        if (InventoryManager.Instance.HasEnoughItems(gem, 1))
        {
            weapon.equippedGems.Add(gem);
            InventoryManager.Instance.RemoveItem(gem);
            Debug.Log($"{gem.itemName} ���� ���� ����!");
        }
        else
        {
            Debug.LogWarning("������ �κ��丮�� �����ϴ�.");
        }
    }

    public void DetachGem(Weapon weapon, Gem gem)
    {
        if (!weapon.equippedGems.Contains(gem))
        {
            Debug.LogWarning("�� ������ �����Ǿ� ���� �ʽ��ϴ�.");
            return;
        }

        if (InventoryManager.Instance.SpendGold(removeGemCost))
        {
            weapon.equippedGems.Remove(gem);
            InventoryManager.Instance.AddItem(gem);
            Debug.Log($"{gem.itemName} ���� ���� ����! ({removeGemCost}��� �Ҹ�)");
        }
        else
        {
            Debug.LogWarning("��尡 �����Ͽ� ������ ������ �� �����ϴ�.");
        }
    }
}
