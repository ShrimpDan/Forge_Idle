using UnityEngine;

public class WeaponUpgradeSystem : MonoBehaviour
{
    public int baseUpgradeCost = 100;

    public void UpgradeWeapon(Weapon weapon)
    {
        int cost = baseUpgradeCost * weapon.level;
        if (InventoryManager.Instance.SpendGold(cost))
        {
            weapon.level += 1;
            weapon.baseAttack += 10; // ����: ���������� ���ݷ� +10
            Debug.Log($"���� ��ȭ ����! ���� ����: {weapon.level}, ���ݷ�: {weapon.baseAttack}");
        }
        else
        {
            Debug.LogWarning("��� �������� ���� ��ȭ ����");
        }
    }
}
