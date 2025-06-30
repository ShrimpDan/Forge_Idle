using UnityEngine;

/// <summary>
/// 제자 모집 관련 버튼을 처리하는 UI 핸들러입니다.
/// 버튼 클릭 시 TraineeManager의 기능을 호출합니다.
/// </summary>
public class TraineeButtonHandler : MonoBehaviour
{
    [SerializeField] private TraineeManager traineeManager;

    // 단일 모집
    public void OnClickRandom() => traineeManager.RecruitSingle();
    public void OnClickCrafting() => traineeManager.RecruitSingle(SpecializationType.Crafting);
    public void OnClickEnhancing() => traineeManager.RecruitSingle(SpecializationType.Enhancing);
    public void OnClickSelling() => traineeManager.RecruitSingle(SpecializationType.Selling);

    // 10연차 모집
    public void OnClickTenRandom() => traineeManager.RecruitMultiple(10);
    public void OnClickTenCrafting() => traineeManager.RecruitMultiple(10, SpecializationType.Crafting);
    public void OnClickTenEnhancing() => traineeManager.RecruitMultiple(10, SpecializationType.Enhancing);
    public void OnClickTenSelling() => traineeManager.RecruitMultiple(10, SpecializationType.Selling);
}
