using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI 버튼 등에 연결해서 테스트용으로 활용

    public WeaponCraftingSystem craftingSystem;
    public WeaponRecipe testRecipe;
    public RefineSystem refineSystem;
    public MaterialItem testMaterial;
    public WeaponUpgradeSystem upgradeSystem;
    public Weapon testWeapon;
    public GemSystem gemSystem;
    public Gem testGem;

    public void OnClick_CraftWeapon()
    {
        craftingSystem.CraftWeapon(testRecipe);
    }

    public void OnClick_Refine()
    {
        refineSystem.Refine(testMaterial);
    }

    public void OnClick_UpgradeWeapon()
    {
        upgradeSystem.UpgradeWeapon(testWeapon);
    }

    public void OnClick_AttachGem()
    {
        gemSystem.AttachGem(testWeapon, testGem);
    }

    public void OnClick_DetachGem()
    {
        gemSystem.DetachGem(testWeapon, testGem);
    }
}
