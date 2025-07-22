using UnityEngine;

public class RecruitPopup : MonoBehaviour
{
    [Header("UI 오브젝트")]
    [SerializeField] private GameObject recruitUI;
    [SerializeField] private AssistantInfoView infoView;

    /// <summary>
    /// 영입 후보 제자 정보를 UI에 보여주기
    /// </summary>
    public void ShowPopup(AssistantInstance instance)
    {
        Debug.Log("ShowPopup 호출됨");
        recruitUI.SetActive(true);
        infoView.SetData(instance);
    }

    /// <summary>
    /// UI를 숨기기
    /// </summary>
    public void HidePopup()
    {
        recruitUI.SetActive(false);
    }
}
