using UnityEngine;

public class WeaponUpgradeSystem : MonoBehaviour
{
    public int baseUpgradeCost = 100;

    public string UpgradeWeapon(Weapon weapon)
    {
        int cost = baseUpgradeCost * weapon.level;
        if (InventoryManager.Instance.SpendGold(cost))
        {
            weapon.level += 1;
            weapon.baseAttack += 10;
            return $"무기 강화 성공! 현재 레벨: {weapon.level}, 공격력: {weapon.baseAttack}";
        }
        else
        {
            return "골드 부족으로 무기 강화 실패";
        }
    }
}
