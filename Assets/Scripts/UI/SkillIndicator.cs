using UnityEngine;
using UnityEngine.UI;

public class SkillIndicator : MonoBehaviour
{
    private ForgeManager forgeManager;
    private SkillInstance skill;

    [SerializeField] private Image skillIcon;
    [SerializeField] private Image coolDown;

    public void Init(SkillInstance skill, ForgeManager forgeManager)
    {
        this.forgeManager = forgeManager;
        this.skill = skill;
        skillIcon.sprite = IconLoader.GetIconByPath(skill.SkillData.IconPath);
        
        forgeManager.Events.OnSkillDurationUpdate += SetSkillCoolDown;
    }

    private void OnDisable()
    {
        forgeManager.Events.OnSkillDurationUpdate -= SetSkillCoolDown;
    }

    private void SetSkillCoolDown(SkillInstance skill, float fillAmount)
    {
        if (skill != this.skill) return;

        coolDown.fillAmount = fillAmount;

        if (fillAmount >= 1f)
            Destroy(gameObject);
    }
}
