using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public bool IsTurtorialMode => isTurtorialMode;


//모든 이벤트 모음
    public Action OnBlockClick { get; private set; }



    private GameManager gameManager;
    private int tutorialStep = 0; //
    private bool isTurtorialMode = false;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject arrowIcon;
    [SerializeField] private List<GameObject> interactObjects = new List<GameObject>();
    //스킵은 나중에

    [Header("카메라")]
    [SerializeField] Camera uiCam;

    private bool isWaitingForClick = false;
    private bool isEventDone = true; // 해당 이벤트가 끝났는지 확인 이벤트가 있을때만 false로 변경해서 기다리자

    [Header("조명 이펙트")]
    [SerializeField] private HighLightEffectController effect;
    [SerializeField] List<Transform> hightLightTargets = new List<Transform>();


    [Header("클릭방지용")]
    [SerializeField] private GameObject clickBlocker;
    private Coroutine waitClickRoutine;

    private GameObject curInteractObject;

    public void Init(GameManager gm)
    {
        gameManager = gm;
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 1)
        {
            isTurtorialMode = false;
            tutorialPanel.SetActive(false);
            return;
        }


        PlayerPrefs.SetInt("TutorialDone", 0); // Test

        isTurtorialMode = true;
        tutorialPanel.SetActive(true);
        AllObjectInteractOff();
        StartTutorial();

    }



    public void StartTutorial()
    {
        tutorialStep = 0;
        HandleStep();
    }

    private void Start()
    {
        PlayerPrefs.SetInt("TutorialDone", 0); // Test       
        ClickBlocker.OnBlockClick += OnStepClear;
        InteractionObjectHandler.OnPointerClicked += OnSettingObject; // 클릭 이벤트 등록
        GameManager.Instance.CraftingManager.isCrafingDone += OnStepClear; // 제작 완료 이벤트 등록
    }

    private void OnDestroy()
    {
        ClickBlocker.OnBlockClick -= OnStepClear;
        if (waitClickRoutine != null)
        {
            StopCoroutine(waitClickRoutine);
            waitClickRoutine = null;
        }
        ClickBlockerOff();
    }


    private void HandleStep() //나중에 엑셀로 따로 만들어서 진행하는게 좋을듯
    {
        if (waitClickRoutine != null)
        {
            StopCoroutine(waitClickRoutine);
            waitClickRoutine = null;
        }


        switch (tutorialStep)
        {
            case 0:
                tutorialPanel.SetActive(true);
                tutorialText.text = "어서오세요!! 대장간은 처음 방문하시는군요!!\n 만나서 반갑습니다 간단한 운영법을 알려드릴께요!!";
                isWaitingForClick = true;
                isEventDone = true; //이벤트가 없으니
                // ClickBlockerOn();
                break;
            case 1:
                arrowIcon.SetActive(true);
                MoveArrowToTarget(hightLightTargets[0]);
                interactObjects[0].GetComponent<Collider2D>().enabled = true; //클릭 방지
                waitClickRoutine = StartCoroutine(WaitForClick());
                tutorialText.text = "제작대를 클릭해서 무기를 만들어 볼까요??";
                break;
            case 2:
                HideArrow();
                effect.HideHighlight();
                ClickBlockerOn(); //대화를 해야하니까
                tutorialText.text = "화면에 보시면 가장먼저 도끼를 생산할꺼에요!! 도끼를 만들기위해 제작에 필요한 재료를 드릴께요!! ";
                Vector2 centerScreen = new Vector2(Screen.width / 2 +40, Screen.height / 2);
                StartCoroutine(WaitForClick());
                effect.ShowHighlight(centerScreen); //중앙에 하이라이트 
                GameManager.Instance.Inventory.AddItem(GameManager.Instance.DataManager.ItemLoader.GetItemByKey("resource_copper"), 2);
                GameManager.Instance.Inventory.AddItem(GameManager.Instance.DataManager.ItemLoader.GetItemByKey("resource_bronze"), 1);

                break;
            case 3:
                effect.ResetHighlightSize();
                effect.HideHighlight();
                tutorialPanel.SetActive(true);
                tutorialText.text = "이제 도끼를 클릭해서 제작해볼까요??";
                //여기 다음 스텝을 해당 도끼가 제작이 끝나면 진행하는걸로 변경 근데 지금 다른곳을 클릭하면 다음단계로 넘어간다.
                break;
            case 4:
                tutorialText.text = "도끼가 제작되었어요!! 축하해요!! 자동으로 판매대에 등록될꺼에요!! \n 이제 손님이 방문할꺼에요!!";
                break;
            case 5:
                tutorialText.text = "자 이제 세공을 해볼꺼에요!! 세공";
                MoveArrowToTarget(hightLightTargets[1]);
                break;
            case 6:
                HideArrow();
                effect.HideHighlight();
                tutorialText.text = "이제 손님들이 방문할꺼에요!! 대장간을 한번 잘 운영해봐요!!";
                break;
            case 7:
                EndTutorial();
                break;

        }

        waitClickRoutine = StartCoroutine(WaitForClick());


    }


    private IEnumerator WaitForClick()
    {
        yield return WaitForSecondsCache.Wait(0.3f);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject());
        
        isWaitingForClick = false;
        waitClickRoutine = null;
        OnStepClear();
    }

    private void EndTutorial()
    {
        isTurtorialMode = false;
        PlayerPrefs.SetInt("TutorialDone", 1);
        tutorialPanel.SetActive(false);
        HideArrow();
        effect.HideHighlight();
        ClickBlockerOff();
        AllObjectInteractOn();

    }

    public void OnStepClear() //스텝 클리어 후 다음 셋팅시작하는 순서로 가야할듯하다.
    {
        if (!isTurtorialMode || !isEventDone)
        {
            //이벤트가 종료가 안된거 같으면 다시 리턴해준다.
            return;
        }


        tutorialStep++;
        HandleStep();
    }

    
    private bool CompareObject(GameObject obj)
    {
        if (curInteractObject == null || obj == null)
        {
            return false;
        }

        if (curInteractObject == obj)
        {
            return true;

        }

        return false;

    }

    private void SkipTutorial()
    {//스킵   
        isTurtorialMode = false;
        PlayerPrefs.SetInt("TutorialDone", 1);
        tutorialPanel.SetActive(false);
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
            screenPos.y += 200f;
            arrowIcon.transform.position = screenPos;
            HighlightTarget(target); //강조
        }
    }

    public void HighlightTarget(Transform target)
    {
        effect.ShowHighlight(target, uiCam);
    }

    public void ClickBlockerOn()
    {
        if (clickBlocker != null && clickBlocker.activeSelf == false)
        {
            clickBlocker.SetActive(true);
        }
    }
    public void ClickBlockerOff()
    {
        if (clickBlocker != null && clickBlocker.activeSelf == true)
        {
            clickBlocker.SetActive(false);
        }
    }
    public void AllObjectInteractOn()
    {
        foreach (var item in interactObjects)
        {
            item.GetComponent<Collider2D>().enabled = true;
        }

    }

    public void AllObjectInteractOff()
    {
        foreach (var item in interactObjects)
        {
            item.GetComponent<Collider2D>().enabled = false;
        }
    }
    
    public void OnSettingObject(GameObject gameObject)
{
    curInteractObject = gameObject;

    if (!isTurtorialMode) return;

    // 현재 스텝에서 정해진 오브젝트와 일치하면 튜토리얼 진행
    if (tutorialStep == 1 && CompareObject(interactObjects[0]))
    {
        OnStepClear();
    }
    else if (tutorialStep == 4 && CompareObject(interactObjects[1]))
    {
        OnStepClear();
    }
}

}
