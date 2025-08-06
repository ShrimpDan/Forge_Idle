using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    private Coroutine drawCoroutine;
    private List<SkillDrawSlot> drawnSlots = new List<SkillDrawSlot>();
    private bool isDone = false;

    void Start()
    {
        skillManager = GameManager.Instance.SkillManager;
        skillDataLoader = GameManager.Instance.DataManager.SkillDataLoader;
        forgeManager = GameManager.Instance.ForgeManager;

        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(ClickConfirmBtn);
    }

    public void DrawSkill(int count)
    {
        if (count == 1 && !forgeManager.UseDia(oneDrawNeedDia))
            return;

        if (count == 10 && !forgeManager.UseDia(tenDrawNeedDia))
            return;

        ClearSlotRoot();
        gatchaBG.SetActive(true);

        drawCoroutine = StartCoroutine(DrawSkillsSequentially(count));
    }

    public void SkipDrawAnimation()
    {
        if (drawCoroutine == null) return;

        DOTween.timeScale = 10f;
    }

    private IEnumerator DrawSkillsSequentially(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SkillData skillData = skillDataLoader.GetRandomSkill();
            SkillDrawSlot slot = Instantiate(slotPrefab, slotRoot);
            slot.SetSlot(skillData);

            // 뽑힌 슬롯을 리스트에 추가
            drawnSlots.Add(slot);

            skillManager.AddSkill(skillData);

            bool animationDone = false;
            slot.OnAnimationComplete = () => animationDone = true;

            slot.PlayDrawAnimation(slotAnimationDuration);
            yield return new WaitUntil(() => animationDone);
        }

        DOTween.timeScale = 1f;
        drawCoroutine = null;
        isDone = true;
    }

    private void ClearSlotRoot()
    {
        if (slotRoot.childCount == 0) return;

        foreach (Transform child in slotRoot)
        {
            Destroy(child.gameObject);
        }

        drawnSlots.Clear();
    }

    private void ClickConfirmBtn()
    {
        if (!isDone)
        {
            SkipDrawAnimation();
            return;
        }

        isDone = false;
        gatchaBG.SetActive(false);
    }
}
