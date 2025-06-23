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
            weapon.baseAttack += 10; // 예시: 레벨업마다 공격력 +10
            Debug.Log($"무기 강화 성공! 현재 레벨: {weapon.level}, 공격력: {weapon.baseAttack}");
        }
        else
        {
            Debug.LogWarning("골드 부족으로 무기 강화 실패");
        }
    }
}
