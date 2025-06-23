using UnityEngine;

public class GemSystem : MonoBehaviour
{
    public int removeGemCost = 200;

    public string AttachGem(Weapon weapon, Gem gem)
    {
        if (weapon.equippedGems.Count >= weapon.maxGemSlots)
            return "���� ������ ������ �����ϴ�.";

        if (InventoryManager.Instance.HasEnoughItems(gem, 1))
        {
            weapon.equippedGems.Add(gem);
            InventoryManager.Instance.RemoveItem(gem);
            return $"{gem.itemName} ���� ���� ����!";
        }
        else
        {
            return "������ �κ��丮�� �����ϴ�.";
        }
    }

    public string DetachGem(Weapon weapon, Gem gem)
    {
        if (!weapon.equippedGems.Contains(gem))
            return "�� ������ �����Ǿ� ���� �ʽ��ϴ�.";

        if (InventoryManager.Instance.SpendGold(removeGemCost))
        {
            weapon.equippedGems.Remove(gem);
            InventoryManager.Instance.AddItem(gem);
            return $"{gem.itemName} ���� ���� ����! ({removeGemCost}��� �Ҹ�)";
        }
        else
        {
            return "��尡 �����Ͽ� ������ ������ �� �����ϴ�.";
        }
    }
}
