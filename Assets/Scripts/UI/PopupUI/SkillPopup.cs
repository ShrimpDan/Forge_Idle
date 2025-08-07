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
    [SerializeField] private Button equipBtn;
    [SerializeField] private Button unEquipBtn;
    [SerializeField] private Button[] slotBtns;
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
        unEquipBtn.onClick.AddListener(ClickUnEquipBtn);
        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.SkillPopup));

        for (int i = 0; i < slotBtns.Length; i++)
        {
            int idx = i;

            Button slotBtn = slotBtns[i];
            slotBtn.onClick.AddListener(() => ClickSlotBtn(idx));
            slotBtn.onClick.AddListener(() => slotBtn.transform.parent.gameObject.SetActive(false));

            SetSlotBtns(i);
        }
    }

    public override void Close()
    {
        base.Close();
        upgradeBtn.onClick.RemoveAllListeners();
        unEquipBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();

        for (int i = 0; i < slotBtns.Length; i++)
        {
            slotBtns[i].onClick.RemoveAllListeners();
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

        equipBtn.gameObject.SetActive(!skill.IsEquipped);
        unEquipBtn.gameObject.SetActive(skill.IsEquipped);
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

        bool isSuccess = skillSystem.SetSkill(idx, skill);

        if (!isSuccess)
        {
            AlertPopup();
        }

        SetPopupUI(skill, slot);
        slot.SetSlotUI(skill);

        uIManager.CloseUI(UIName.SkillPopup);
    }

    private void ClickUnEquipBtn()
    {
        if (skill == null) return;

        bool isSuccess = skillSystem.UnSetSkill(skill);

        if (!isSuccess)
        {
            AlertPopup();
        }

        SetPopupUI(skill, slot);
        slot.SetSlotUI(skill);

        for (int i = 0; i < slotBtns.Length; i++)
        {
            SetSlotBtns(i);
        }
    }

    private void AlertPopup()
    {
        var ui = uIManager.OpenUI<LackPopup>(UIName.LackPopup);
        ui.ShowCustom("쿨타임 중에는 교체할 수 없습니다.");
    }

    private void SetSlotBtns(int idx)
    {
        Button slotBtn = slotBtns[idx];
        SkillInstance skill = skillSystem.ActiveSkills[idx];

        if (skill != null)
        {
            Image icon = slotBtn.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = IconLoader.GetIconByPath(skill.SkillData.IconPath);
            icon.gameObject.SetActive(true);

            slotBtn.transform.Find("Text").gameObject.SetActive(false);
        }
        else
        {
            slotBtn.transform.Find("Icon").gameObject.SetActive(false);
            slotBtn.transform.Find("Text").gameObject.SetActive(true);
        }
    }
}
