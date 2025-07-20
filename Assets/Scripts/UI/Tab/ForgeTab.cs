using UnityEngine;
using UnityEngine.UI;

public class ForgeTab : BaseTab
{
    private ForgeManager forgeManager;
    [SerializeField] Image weaponIcon;
    [SerializeField] Image progressFill;
    [SerializeField] Button forgeRecipeBtn;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        forgeManager = gameManager.ForgeManager;

        if (forgeManager != null)
            forgeManager.Events.OnCraftStarted += SetWeaponIcon;
    }

    public override void OpenTab()
    {
        base.OpenTab();

        if (forgeManager.CurrentForge != null)
        {
            forgeManager.CurrentForge.SetForgeMap(true);
            forgeManager.Events.OnCraftProgress += UpdateProgress;

            forgeRecipeBtn.onClick.RemoveAllListeners();
            forgeRecipeBtn.onClick.AddListener(OpenRecipeWindow);
        }
    }

    public override void CloseTab()
    {
        base.CloseTab();

        if (forgeManager.CurrentForge != null)
        {
            forgeManager.CurrentForge.SetForgeMap(false);
            forgeManager.Events.OnCraftProgress -= UpdateProgress;
        }
    }

    public void SetWeaponIcon(Sprite icon)
    {
        weaponIcon.sprite = icon;
    }

    public void UpdateProgress(float curTime, float totalTime)
    {
        progressFill.fillAmount = curTime / totalTime;
    }

    private void OpenRecipeWindow()
    {
        if (forgeManager.CurrentForge == null) return;

        uIManager.OpenUI<WeaponRecipeWindow>(UIName.GetRecipeWindowByType(forgeManager.CurrentForge.ForgeType));
    }
}
