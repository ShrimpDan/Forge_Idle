using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    private SkillWindow skillWindow;
    [SerializeField] private ForgeUpgradeType type;

    [Header("UI Elements")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image countFill;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject lockUI;
    [SerializeField] private GameObject equipIndicator;

    private Button slotBtn;
    public ForgeUpgradeType Type { get => type; }
    private SkillInstance slotSkill;

    public void Init(SkillWindow skillWindow)
    {
        this.skillWindow = skillWindow;

        if (slotBtn == null)
        {
            slotBtn = GetComponent<Button>();
            slotBtn.onClick.AddListener(ClickSlotBtn);
        }
    }

    public void SetSlotUI(SkillInstance skill)
    {
        if (skill == null) return;

        slotSkill = skill;

        icon.sprite = IconLoader.GetIconByPath(skill.SkillData.IconPath);
        nameText.text = skill.SkillData.Name;
        countFill.fillAmount = (float)skill.CurCount / skill.NeedCount;
        countText.text = $"{skill.CurCount} / {skill.NeedCount}";
        slotBtn.interactable = true;
        lockUI.SetActive(false);
        equipIndicator.SetActive(skill.IsEquipped);
    }

    private void ClickSlotBtn()
    {
        if (slotSkill == null) return;

        skillWindow.OpenSkillPopup(slotSkill, this);
    }
}
