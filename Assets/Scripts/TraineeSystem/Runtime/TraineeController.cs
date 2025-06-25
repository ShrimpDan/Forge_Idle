using DG.Tweening;
using UnityEngine;
using TMPro;

/// <summary>
/// TraineeData를 받아 UI에 표시하고, 상호작용(삭제/정보 출력)을 처리하는 컨트롤러 클래스입니다.
/// </summary>
public class TraineeController : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text specializationText;
    [SerializeField] private TMP_Text personalityText;
    [SerializeField] private TMP_Text abilitiesText;

    [Header("카드 앞 뒷면")]
    [SerializeField] private GameObject frontSide;
    [SerializeField] private GameObject backSide;

    private TraineeData data;
    private TraineeManager manager;

    private bool isFlipped = false;

    /// <summary>
    /// 제자 데이터를 받아 UI를 초기화합니다.
    /// </summary>
    public void Setup(TraineeData traineeData, TraineeManager traineeManager)
    {
        data = traineeData;
        manager = traineeManager;
        isFlipped = false;

        if (frontSide != null) frontSide.SetActive(false);
        if (backSide != null) backSide.SetActive(true);

        if (frontSide != null)
            frontSide.transform.localEulerAngles = new Vector3(0, 270, 0);

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (nameText != null)
            nameText.text = data.Name;

        if (specializationText != null)
            specializationText.text = GetSpecializationKorean(data.Specialization);

        if (personalityText != null)
            personalityText.text = $"{data.Personality.PersonalityName} (티어 {data.Personality.Tier})";

        if (abilitiesText != null)
            abilitiesText.text = GetFormattedAbilitiesText();
    }

    /// <summary>
    /// 제자의 상세 정보를 텍스트로 구성하여 반환합니다.
    /// </summary>
    private string GetFormattedAbilitiesText()
    {
        string text = "";
        foreach (var mul in data.Multipliers)
        {
            text += $"- {mul.AbilityName} x{mul.Multiplier:F2}\n";
        }
        return text.TrimEnd('\n');
    }

    /// <summary>
    /// 제자 삭제 버튼을 눌렀을 때 호출되는 함수입니다.
    /// 매니저에 삭제 요청을 전달합니다.
    /// 이후에 제자 방출 시스템에 사용할 기능입니다.
    /// </summary>
    public void OnClick_DeleteSelf()
    {
        if (manager != null)
            manager.RemoveTrainee(gameObject, data);
    }

    private string GetSpecializationKorean(SpecializationType spec)
    {
        return spec switch
        {
            SpecializationType.Crafting => "제작 특화",
            SpecializationType.Enhancing => "강화 특화",
            SpecializationType.Selling => "판매 특화",
            _ => "알 수 없음"
        };
    }

    public void OnClick_FlipCard()
    {
        if (isFlipped) return;

        TraineeData trainee = manager?.GenerateOneTimeTrainee();
        if (trainee == null)
        {
            Debug.LogError("랜덤 제자 생성 실패!");
            return;
        }

        Setup(trainee, manager); 

        frontSide.transform.localEulerAngles = new Vector3(0, 270, 0);

        backSide.transform.DORotate(new Vector3(0, 90, 0), 0.3f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                frontSide.SetActive(true);

                RefreshUI();

                frontSide.transform.DORotate(new Vector3(0, 360, 0), 0.3f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        backSide.SetActive(false);
                    });
            });

        isFlipped = true;
    }

    public void OnClick_FrontCard()
    {
        Destroy(gameObject);
    }
}
