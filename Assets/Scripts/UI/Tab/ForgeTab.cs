using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ForgeTab : BaseTab
{
    private ForgeManager forgeManager;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private Image progressFill;
    [SerializeField] private Button forgeRecipeBtn;
    [SerializeField] private Button slideTabBtn;
    [SerializeField] private Transform slideTab;
    private Transform arrowTr;
    private bool isOpen = false;

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        forgeManager = gameManager.ForgeManager;

        if (forgeManager != null)
            forgeManager.Events.OnCraftStarted += SetWeaponIcon;

        if (arrowTr == null)
            arrowTr = slideTabBtn.transform.GetChild(0);

        slideTabBtn.onClick.RemoveAllListeners();
        slideTabBtn.onClick.AddListener(ClickSlideButton);
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

    private void ClickSlideButton()
    {
        isOpen = !isOpen;
        OpenSlideTab(isOpen);
    }

    private void OpenSlideTab(bool isOpen)
    {
        if (isOpen)
        {
            slideTab.DOMoveY(200f, 0.5f).SetEase(Ease.Linear).OnComplete(() => arrowTr.localScale = new Vector3(1f, 1f, 1f));
        }
        else
        {
            slideTab.DOMoveY(16, 0.5f).SetEase(Ease.Linear).OnComplete(() => arrowTr.localScale = new Vector3(1f, -1f, 1f));
        }
    }
}
