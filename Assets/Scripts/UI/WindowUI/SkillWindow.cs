using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillWindow : BaseUI
{
    public override UIType UIType => UIType.Window;
    private SkillManager skillManager;
    private ForgeSkillSystem skillSystem;

    [SerializeField] private SkillSlot[] skillSlots;
    [SerializeField] private Button exitBtn;
    public Dictionary<ForgeUpgradeType, SkillSlot> SlotDict { get; private set; }

    public override void Init(GameManager gameManager, UIManager uIManager)
    {
        base.Init(gameManager, uIManager);
        skillManager = gameManager.SkillManager;
        skillSystem = gameManager.ForgeManager.SkillSystem;

        SlotDict = new Dictionary<ForgeUpgradeType, SkillSlot>();
        foreach (var slot in skillSlots)
        {
            slot.Init(this);
            SlotDict[slot.Type] = slot;
        }

        exitBtn.onClick.AddListener(() => uIManager.CloseUI(UIName.SkillWindow));
    }

    public override void Open()
    {
        base.Open();

        foreach (var slot in skillSlots)
        {
            SkillInstance skill = skillManager.GetSkillByType(slot.Type);
            slot.SetSlotUI(skill);
        }
    }

    public override void Close()
    {
        base.Close();
    }

    public void OpenSkillPopup(SkillInstance skill, SkillSlot slot)
    {
        var ui = uIManager.OpenUI<SkillPopup>(UIName.SkillPopup);
        ui.SetPopupUI(skill, slot);
    }
}
