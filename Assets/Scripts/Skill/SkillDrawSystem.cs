using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillDrawSystem : MonoBehaviour
{
    private SkillManager skillManager;
    private SkillDataLoader skillDataLoader;
    private ForgeManager forgeManager;

    [Header("Gatcha UI")]
    [SerializeField] GameObject gatchaBG;
    [SerializeField] Button confirmBtn;

    [Header("Slot Prefab")]
    [SerializeField] SkillDrawSlot slotPrefab;
    [SerializeField] Transform slotRoot;

    [SerializeField] float slotAnimationDuration = 0.2f;

    [Header("Needed Dia")]
    [SerializeField] int oneDrawNeedDia;
    [SerializeField] int tenDrawNeedDia;

    void Start()
    {
        skillManager = GameManager.Instance.SkillManager;
        skillDataLoader = GameManager.Instance.DataManager.SkillDataLoader;
        forgeManager = GameManager.Instance.ForgeManager;
    }

    public void DrawSkill(int count)
    {
        if (count == 1 && !forgeManager.UseDia(oneDrawNeedDia))
            return;

        if (count == 10 && !forgeManager.UseDia(tenDrawNeedDia))
            return;

        ClearSlotRoot();
        gatchaBG.SetActive(true);
        confirmBtn.interactable = false;

        StartCoroutine(DrawSkillsSequentially(count));
    }

    private IEnumerator DrawSkillsSequentially(int count)
    {
        

        for (int i = 0; i < count; i++)
        {
            SkillData skillData = skillDataLoader.GetRandomSkill();
            SkillDrawSlot slot = Instantiate(slotPrefab, slotRoot);
            slot.SetSlot(skillData);

            skillManager.AddSkill(skillData);

            bool animationDone = false;
            slot.OnAnimationComplete = () => animationDone = true;

            slot.PlayDrawAnimation(slotAnimationDuration);
            yield return new WaitUntil(() => animationDone);
        }

        confirmBtn.interactable = true;
    }

    private void ClearSlotRoot()
    {
        if (slotRoot.childCount == 0) return;

        foreach (Transform child in slotRoot)
        {
            Destroy(child.gameObject);
        }
    }
}
