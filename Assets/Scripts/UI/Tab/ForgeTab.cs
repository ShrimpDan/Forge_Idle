using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ForgeTab : BaseTab
{



    private ForgeManager forgeManager;
    [SerializeField] private Image progressFill;
    
    [SerializeField] private Button slideTabBtn;
    [SerializeField] private RectTransform slideTab;

    [Header("BottomSlider Button")]
    [SerializeField] private Button forgeUpgradeBtn;
    [SerializeField] private Button forgeRecipeBtn;
    [SerializeField] private Button forgeSkillBtn;
    [SerializeField] private Button forgeMoveBtn;

    [Header("Skill Slots")]
    [SerializeField] private SkillEquipSlot[] skillSlots;

    private Transform arrowTr;
    private bool isOpen = false;


    #region 튜토리얼 이벤트
    public static event Action<string> onClickButton;

    #endregion


    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        forgeManager = gameManager.ForgeManager;

        if (arrowTr == null)
            arrowTr = slideTabBtn.transform.GetChild(0);

        slideTabBtn.onClick.RemoveAllListeners();
        slideTabBtn.onClick.AddListener(ClickSlideButton);

        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].Init(gameManager.UIManager, forgeManager, i);
        }

    }

    public override void OpenTab()
    {
        base.OpenTab();

        if (forgeManager.CurrentForge != null)
        {
            forgeManager.CurrentForge.SetForgeMap(true);
            forgeManager.Events.OnCraftProgress += UpdateProgress;

            forgeUpgradeBtn.onClick.RemoveAllListeners();
            forgeUpgradeBtn.onClick.AddListener(OpenUpgradeWindow);

            forgeRecipeBtn.onClick.RemoveAllListeners();
            forgeRecipeBtn.onClick.AddListener(OpenRecipeWindow);


            forgeSkillBtn.onClick.RemoveAllListeners();
            forgeSkillBtn.onClick.AddListener(OpenSkillWindow);

            forgeMoveBtn.onClick.RemoveAllListeners();
            forgeMoveBtn.onClick.AddListener(OpenMoveWindow);

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

    public void UpdateProgress(float curTime, float totalTime)
    {
        progressFill.fillAmount = curTime / totalTime;
    }

    private void OpenUpgradeWindow()
    {
        uIManager.OpenUI<ForgeUpgradeWindow>(UIName.ForgeUpgradeWindow);
    }

    private void OpenRecipeWindow()
    {
        if (forgeManager.CurrentForge == null) return;

        uIManager.OpenUI<WeaponRecipeWindow>(UIName.GetRecipeWindowByType(forgeManager.CurrentForge.ForgeType));
        onClickButton?.Invoke(forgeRecipeBtn.name);
    }

    private void OpenSkillWindow()
    {
        uIManager.OpenUI<SkillWindow>(UIName.SkillWindow);
    }

    private void OpenMoveWindow()
    {
        uIManager.OpenUI<ForgeMoveWindow>(UIName.ForgeMoveWindow);
    }

    private void ClickSlideButton()
    {
        isOpen = !isOpen;

        OpenSlideTab(isOpen);
        onClickButton?.Invoke(slideTabBtn.name);
    }

    private void OpenSlideTab(bool isOpen)
    {
        if (isOpen)
        {
            slideTab.DOAnchorPosY(0f, 0.5f).SetEase(Ease.Linear).OnComplete(() => arrowTr.localScale = new Vector3(1f, 1f, 1f));
        }
        else
        {
            slideTab.DOAnchorPosY(-200f, 0.5f).SetEase(Ease.Linear).OnComplete(() => arrowTr.localScale = new Vector3(1f, -1f, 1f));
        }
    }
}
