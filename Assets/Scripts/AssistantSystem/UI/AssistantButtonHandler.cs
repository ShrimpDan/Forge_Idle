using UnityEngine;

/// <summary>
/// 제자 모집 및 합성 관련 버튼을 처리하는 UI 핸들러입니다.
/// 버튼 클릭 시 assistantManager와 FusionUIController의 기능을 호출합니다.
/// </summary>
public class TraineeButtonHandler : MonoBehaviour
{
    [Header("제자 모집")]
    [SerializeField] private AssistantManager assistantManager;

    [Header("합성 UI")]
    [SerializeField] private FusionUIController fusionUIController;

    [Header("합성 연출")]
    [SerializeField] private FusionAnimator fusionAnimator;

    // 단일 모집
    public void OnClickRandom() => assistantManager.RecruitSingle();
    public void OnClickCrafting() => assistantManager.RecruitSingle(SpecializationType.Crafting);
    public void OnClickEnhancing() => assistantManager.RecruitSingle(SpecializationType.Enhancing);
    public void OnClickSelling() => assistantManager.RecruitSingle(SpecializationType.Selling);

    // 10연차 모집
    public void OnClickTenRandom() => assistantManager.RecruitMultiple(10);
    public void OnClickTenCrafting() => assistantManager.RecruitMultiple(10, SpecializationType.Crafting);
    public void OnClickTenEnhancing() => assistantManager.RecruitMultiple(10, SpecializationType.Enhancing);
    public void OnClickTenSelling() => assistantManager.RecruitMultiple(10, SpecializationType.Selling);

    // 합성 UI 열기
    public void OnClickOpenFusionUI()
    {
        SoundManager.Instance.Play("SFX_SystemClick");
        var assistantList = assistantManager.AssistantInventory.GetAll();
        fusionUIController.OpenUI(assistantList);
    }

    // 합성 UI 닫기
    public void OnClickCloseFusionUI()
    {
        SoundManager.Instance.Play("SFX_SystemClick");
        fusionUIController.CloseUI();
    }

    public void OnClickStartFusion()
    {
        var slots = fusionUIController.GetCurrentSlots();
        if (slots == null || slots.Count == 0) return;

        fusionUIController.AddSlotButtonsToDisableList();
        fusionUIController.SetButtonsInteractable(false);

        if (slots.TrueForAll(s => s.Data != null))
        {
            fusionAnimator.PlayMergeAnimation(slots, () =>
            {
                fusionUIController.OnClick_FusionButton();
            });
        }
        else
        {
            SoundManager.Instance.Play("SFX_SystemFail");

            fusionAnimator.PlayEmphasizeEmptySlots(slots);
            fusionUIController.SetButtonsInteractable(true);
        }
    }

    public void OnClickAutoFusionAll()
    {
        fusionUIController.SetButtonsInteractable(false);
        fusionUIController.PerformAutoFusionAll();
        fusionUIController.SetButtonsInteractable(true);
    }

}
