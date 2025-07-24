using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillEquipSlot : MonoBehaviour
{
    UIManager uIManager;
    ForgeEventHandler forgeEvent;
    ForgeSkillSystem skillSystem;

    [Header("UI Elements")]
    [SerializeField] Image icon;
    [SerializeField] Image cooldownFill;
    [SerializeField] TextMeshProUGUI cooldownText;
    private Button slotBtn;
    private SkillInstance skill;
    private int idx;

    public void Init(UIManager uIManager, ForgeManager forgeManager, int idx)
    {
        this.uIManager = uIManager;
        skillSystem = forgeManager.SkillSystem;
        forgeEvent = forgeManager.Events;
        this.idx = idx;

        if (slotBtn == null)
        {
            slotBtn = GetComponent<Button>();
            slotBtn.onClick.AddListener(() => ClickSlotBtn(idx));
        }

        forgeEvent.OnSkillChanged += SetSlotUI;
        forgeEvent.OnSkillCooldownStarted += StartCooldown;
        forgeEvent.OnSkillCooldownUpdate += UpdateCooldown;
        forgeEvent.OnSkillCooldownFinished += FinishCooldown;
    }

    private void SetSlotUI(int idx, SkillInstance skill)
    {
        if (this.idx != idx) return;

        this.skill = skill;
        
        if (skill == null)
            icon.sprite = IconLoader.GetIconByPath(null);
        else
            icon.sprite = IconLoader.GetIconByPath(skill.SkillData.IconPath);

        cooldownFill.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
    }

    private void ClickSlotBtn(int idx)
    {
        if (skill == null)
            uIManager.OpenUI<SkillWindow>(UIName.SkillWindow);
        else
            skillSystem.UseSkill(idx);
    }

    private void StartCooldown(int idx, float curCooldown)
    {
        if (this.idx != idx) return;

        slotBtn.interactable = false;

        cooldownFill.fillAmount = 1f;
        cooldownText.text = curCooldown.ToString("F1");

        cooldownFill.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true);
    }

    private void UpdateCooldown(int idx, float curCooldown, float maxCooldown)
    {
        if (this.idx != idx) return;

        cooldownFill.fillAmount = curCooldown / maxCooldown;
        cooldownText.text = curCooldown.ToString("F1");
    }

    private void FinishCooldown(int idx)
    {
        if (this.idx != idx) return;

        slotBtn.interactable = true;

        cooldownFill.fillAmount = 0f;
        cooldownText.text = "0.0";

        cooldownFill.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
    }
}
