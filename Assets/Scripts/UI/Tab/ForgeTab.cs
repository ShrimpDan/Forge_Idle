using UnityEngine;
using UnityEngine.UI;

public class ForgeTab : BaseTab
{
    private Forge forge;

    [SerializeField] Image weaponIcon;
    [SerializeField] Image progressFill;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        forge = gameManager.Forge;

        forge.Events.OnCraftStarted += SetWeaponIcon;
    }

    public override void OpenTab()
    {
        base.OpenTab();

        forge.OpenForgeTab();
        forge.Events.OnCraftProgress += UpdateProgress;
    }

    public override void CloseTab()
    {
        base.CloseTab();

        forge.CloseForgeTab();
        forge.Events.OnCraftProgress -= UpdateProgress;
    }

    public void SetWeaponIcon(Sprite icon)
    {
        weaponIcon.sprite = icon;
    }

    public void UpdateProgress(float curTime, float totalTime)
    {
        progressFill.fillAmount = curTime / totalTime;
    }
}
