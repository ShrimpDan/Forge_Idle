using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPopup : BaseUI
{
    public override UIType UIType => UIType.Popup;

    private ForgeSkillSystem skillSystem;

    [Header("UI Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image countFill;

    [Header("Buttons")]
    [SerializeField] private Button upgradeBtn;
    [SerializeField] private Button[] equipBtn;
    [SerializeField] private Button exitBtn;

    private SkillInstance skill;
    private SkillSlot slot;
    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);

        skillSystem = gameManager.ForgeManager.SkillSystem;
    }

    public override void Open()
    {
        base.Open();
        upgradeBtn.onClick.AddListener(ClickUpgradBtn);
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.SkillPopup));

        for (int i = 0; i < equipBtn.Length; i++)
        {
            int idx = i;
            equipBtn[i].onClick.AddListener(() => ClickSlotBtn(idx));
            equipBtn[i].onClick.AddListener(() => equipBtn[idx].transform.parent.gameObject.SetActive(false));
        }
    }

    public override void Close()
    {
        base.Close();
        upgradeBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();

        for (int i = 0; i < equipBtn.Length; i++)
        {
            equipBtn[i].onClick.RemoveAllListeners();
        }
    }

    public void SetPopupUI(SkillInstance skill, SkillSlot slot)
    {
        if (skill == null) return;

        this.skill = skill;
        this.slot = slot;

        icon.sprite = IconLoader.GetIconByPath(skill.SkillData.IconPath);
        levelText.text = $"Lv.{skill.Level}";
        nameText.text = skill.SkillData.Name;
        descriptionText.text = skill.GetDescription();
        countFill.fillAmount = (float)skill.CurCount / skill.NeedCount;
        countText.text = $"{skill.CurCount} / {skill.NeedCount}";
    }

    private void ClickUpgradBtn()
    {
        if (skill == null) return;
        skill.UpgradeSkill();

        SetPopupUI(skill, slot);
        slot.SetSlotUI(skill);
    }

    private void ClickSlotBtn(int idx)
    {
        if (skill == null) return;
        skillSystem.SetSkill(idx, skill);
    }  
}
