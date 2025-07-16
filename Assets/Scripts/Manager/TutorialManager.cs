using System.Collections.Generic;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TutorialManager : MonoBehaviour
{
    public bool IsTurtorialMode => isTurtorialMode;

    private GameManager gameManager;
    private int tutorialStep = 0; //
    private bool isTurtorialMode = false;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject arrowIcon;
    //스킵은 나중에

    [Header("카메라")]
    [SerializeField] Camera uiCam;

    private bool isWaitingForClick = false;

    [Header("조명 이펙트")]
    [SerializeField] private HighLighttEffectController effect;



    [SerializeField] List<Transform> hightLightTargets = new List<Transform>();

    public void Init(GameManager gm)
    {
        gameManager = gm;
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 1)
        {
            isTurtorialMode = false; 
            return; //이미 깸 
        }
        isTurtorialMode = true;
        tutorialPanel.SetActive(true);


     

        StartTutorial();
       
    }


    public void StartTutorial()
    {
        tutorialStep = 0;
        HandleStep();
    }

    private void Update()
    {
        if (!isTurtorialMode || !isWaitingForClick)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            OnStepClear();
        }
    }

    private void HandleStep() //나중에 엑셀로 따로 만들어서 진행하는게 좋을듯
    {
        isWaitingForClick = true;

        switch (tutorialStep)
        {
            case 0:
                tutorialPanel.SetActive(true);
                tutorialText.text = "어서오세요!! 대장간은 처음 방문하시는군요!!\n 만나서 반갑습니다 간단한 운영법을 알려드릴께요!!";
                break;
            case 1:
                tutorialText.text = "제작대를 클릭해서 무기를 만들어 볼까요??";
                arrowIcon.SetActive(true);
                MoveArrowToTarget(hightLightTargets[0]);
                
                break;


            case 2:
                tutorialText.text = "화면에 보시면 가장먼저 도끼를 생산할꺼에요!! 도끼를 클릭하기전 제작에 필요한 재료를 드릴께요!! ";
                break;
            case 3:
                tutorialPanel.SetActive(true);
                break;
            case 4:
                tutorialText.text = "이제 판매대에 등록해 볼꺼에요!! 판매대를 클릭후 도끼를 등록해주세요!!";
                MoveArrowToTarget(hightLightTargets[1]);
                break;


            case 5:
                EndTutorial();
                break;

        }


       
    }


    private void EndTutorial()
    {
        isTurtorialMode = false;
        PlayerPrefs.SetInt("TutorialDone", 1);
        tutorialPanel.SetActive(false);
        HideArrow();
    }

    public void OnStepClear()
    {
        if (!isTurtorialMode)
        {
            return;
        }
        isWaitingForClick = false;

        tutorialStep++;
        HandleStep();
    }

    private void SkipTutorial()
    {//스킵   
        isTurtorialMode = false;
        PlayerPrefs.SetInt("TutorialDone", 1);
        tutorialPanel.SetActive(false);
    }

    private void HighLight(int index)
    { 
        
    }


    private void HideArrow()
    {
        arrowIcon.SetActive(false);
    }

    private void MoveArrowToTarget(Transform target)
    {
        if (target != null)
        {
            Vector3 screenPos = uiCam.WorldToScreenPoint(target.position);
            screenPos.y += 120f;
            arrowIcon.transform.position = screenPos;
            HighlightTarget(target);
        }
    }

    public void HighlightTarget(Transform target)
    {
        effect.SetHighLightTarget(target);
    }
    
}
