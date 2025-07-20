using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialManager : MonoBehaviour
{

    public bool IsEventDone => isEvent;

    //대사 출력확인
    private bool isTyping = false;
    private Coroutine typingRoutine;
    private string fullText = "";

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
    private bool isEvent = true; // 해당 이벤트가 끝났는지 확인 이벤트가 있을때만 false로 변경해서 기다리자

    [Header("조명 이펙트")]
    [SerializeField] private HighLightEffectController effect;
    [SerializeField] List<Transform> hightLightTargets = new List<Transform>();


    [Header("클릭방지용")]
    [SerializeField] private GameObject clickBlocker;
    [SerializeField] private GameObject topHalfBlocker;
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

        ClickBlocker.OnBlockClick += HandleClickBlock;
        InteractionObjectHandler.OnPointerClicked += OnSettingObject; // 클릭 이벤트 등록
        GameManager.Instance.CraftingManager.isCrafingDone += OnEventDone; // 제작 완료 이벤트 등록
        GameManager.Instance.UIManager.CloseUIName += HandleUIClose;

        PlayerPrefs.SetInt("TutorialDone", 0);
    }

    private void OnDestroy()
    {
        ClickBlocker.OnBlockClick -= HandleClickBlock;
        InteractionObjectHandler.OnPointerClicked -= OnSettingObject;
        //GameManager.Instance.UIManager.CloseUIName -= HandleUIClose;
        //GameManager.Instance.CraftingManager.isCrafingDone -= OnEventDone; 일단 막아둬

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

        if (!tutorialPanel.gameObject.activeInHierarchy && tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        switch (tutorialStep)
        {
            case 0:
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("어서오세요!! 대장간은 처음 방문하시는군요!!\n 만나서 반갑습니다 간단한 운영법을 알려드릴께요!!");
                isWaitingForClick = true;
                isEvent = false; //박스랑 상호작용해도 대사 넘어가게
                tutorialStep = 3; //일단 제작 넘기고 Test
                break;
            case 1:
                arrowIcon.SetActive(true);
                MoveArrowToTarget(hightLightTargets[0]);
                interactObjects[0].GetComponent<Collider2D>().enabled = true; //클릭 방지
                waitClickRoutine = StartCoroutine(WaitForClick());
                ShowTextWithTyping("제작대를 클릭해서 무기를 만들어 볼까요??");
                GameManager.Instance.Inventory.AddItem(GameManager.Instance.DataManager.ItemLoader.GetItemByKey("ingot_copper"), 2);
                GameManager.Instance.Inventory.AddItem(GameManager.Instance.DataManager.ItemLoader.GetItemByKey("resource_bronze"), 1);
                break;
            case 2:
                AllEffectOff();
                ClickBlockerOn();//대화를 해야하니까
                ShowTextWithTyping("화면에 보시면 가장먼저 도끼를 생산할꺼에요!! 도끼를 만들기위해 제작에 필요한 재료를 드릴께요!! ");
                StartCoroutine(WaitForClick());
                break;
            case 3:
                topHalfBlocker.SetActive(true);
                tutorialPanel.SetActive(true);
                topHalfBlocker.SetActive(true);
                ShowTextWithTyping("이제 도끼를 클릭해서 제작해볼까요??");
                isEvent = false;
                //여기 다음 스텝을 해당 도끼가 제작이 끝나면 진행하는걸로 변경 근데 지금 다른곳을 클릭하면 다음단계로 넘어간다.
                break;
            case 4:
                topHalfBlocker.SetActive(false);
                ShowTextWithTyping("도끼가 제작되었어요!! 축하해요!! 자동으로 판매대에 등록될꺼에요!! \n 이제 손님이 방문할꺼에요!!");
                break;
            case 5:
                AllObjectInteractOff(); //일단 모든 오브젝트의 상호작용을 막는다.
                arrowIcon.SetActive(true);
                MoveArrowToTarget(interactObjects[1].transform);
                HighlightTarget(hightLightTargets[1]);
                interactObjects[1].GetComponent<Collider2D>().enabled = true;
                ShowTextWithTyping("자 이제 세공을 해볼꺼에요!!");
                isEvent = true;
                break;
            case 6:
                AllEffectOff();
                ClickBlockerOn();
                ShowTextWithTyping("세공은 제작에 필요한 광석이나 장착에 필요한 보석을 만들수 있어요!!\n 각 보석들을 클릭하면 필요한 재료가 보일꺼에요!!");
                break;
            case 7:
                ShowTextWithTyping("둘러보고 창을 닫아볼까요??");
                ClickBlockerOn();
                break;
            case 8:
                tutorialPanel.SetActive(false);
                break;
            case 9:
                AllObjectInteractOff();
                arrowIcon.SetActive(true);
                MoveArrowToTarget(interactObjects[2].transform);
                HighlightTarget(hightLightTargets[2]);
                interactObjects[2].GetComponent<Collider2D>().enabled = true;
                ShowTextWithTyping("다음은 강화에 대해서 알려드릴께요!!");
                isEvent = true; //이걸 해야 다른걸 눌렀을때 상호작용이 막힘
                break;
            case 10:
                ClickBlockerOn();
                HighlightPos(-190, 500);
                ShowTextWithTyping("강화는 일반강화, 고급강화로 나눠져 있어요!! 가운데 창을 클릭해서 무기를 올려볼까요?");
                break;

            case 11:
                HighlightPos(0, 300);
                ClickBlockerOn();
                ShowTextWithTyping("강화는 일반강화, 고급강화로 나눠져 있어요!! 가운데 창을 클릭해서 강화하고 싶은 무기를 올릴수 있어요!!");
                break;
            case 12:
                ClickBlockerOn();
                HighlightPos(-160, -900);
                ShowTextWithTyping("일반강화는 골드를 사용해서 확률을 통해서 강화 할수있어요!! ");
                break;
            case 13:
                ClickBlockerOn();
                HighlightPos(160, -900);
                ShowTextWithTyping("고급강화는 재화를 사용해서 좀더 확률이 높게 강화가 가능해요!! ");
                break;
            case 15:
                AllEffectOff();
                ShowTextWithTyping("이제 손님들이 방문할꺼에요!! 대장간을 한번 잘 운영해봐요!!");
                break;
            case 20:
                EndTutorial();
                break;

        }


        waitClickRoutine = StartCoroutine(WaitForClick());




    }




    private void EndTutorial()
    {
        isTurtorialMode = false;
        PlayerPrefs.SetInt("TutorialDone", 1);
        tutorialPanel.SetActive(false);
        AllEffectOff();
        effect.HideHighlight();
        ClickBlockerOff();
        AllObjectInteractOn();

    }

    public void OnStepClear() //스텝 클리어 후 다음 셋팅시작하는 순서로 가야할듯하다.
    {
        if (!isTurtorialMode || isEvent)
        {
            //이벤트가 종료가 안된거 같으면 다시 리턴해준다.
            return;
        }


        tutorialStep++;
        HandleStep();
    }

    public void OnEventDone()
    {
        isEvent = true;
        OnStepClear();

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



    private void MoveArrowToTarget(Transform target)
    {
        if (!arrowIcon.gameObject.activeInHierarchy)
        {
            arrowIcon.SetActive(true);
        }

        if (target != null)
        {
            Vector3 screenPos = uiCam.WorldToScreenPoint(target.position);
            screenPos.y += 200f;
            arrowIcon.transform.position = screenPos;

        }
    }

    private void MoveArrowToPos(Vector2 pos)
    {

        arrowIcon.transform.position = new Vector2(pos.x, pos.y + 150);
    }

    public void HighlightTarget(Transform target) //조명 비추기
    {
        if (!effect.gameObject.activeInHierarchy)
        {
            effect.gameObject.SetActive(true);
        }

        effect.ShowHighlight(target, uiCam);
    }

    public void HighlightPos(float x, float y)
    {
        Vector2 centerScreen = new Vector2((uiCam.pixelWidth / 2f + x), (uiCam.pixelHeight / 2f + y));
        effect.ShowHighlight(centerScreen);
        MoveArrowToPos(centerScreen);
    }

    public void ClickBlockerOn() //클릭버튼 블록
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
    public void AllObjectInteractOn() //모든 오브젝트 상호작용 켜기
    {
        foreach (var item in interactObjects)
        {
            item.GetComponent<Collider2D>().enabled = true;
        }

    }


    private void HandleClickBlock() //클릭버튼 이벤트 및 조건 처리
    {
        if (!isTurtorialMode)
        {
            return;
        }

        if (isTyping)
        {
            if (typingRoutine != null)
            {
                StopCoroutine(typingRoutine);
                tutorialText.text = fullText;
                typingRoutine = null;
                isTyping = false;
            }
        }
        else if (!isTyping)
        {
            // 클릭 대기 중이면 다음 스텝
            if (!isEvent)
            {
                ClickBlockerOff();
                OnStepClear();
            }
        }
    }
    private void ShowTextWithTyping(string text)
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        fullText = text;
        typingRoutine = StartCoroutine(TypeText(text));
    }
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        tutorialText.text = "";
        foreach (char c in text)
        {
            tutorialText.text += c;
            yield return WaitForSecondsCache.Wait(0.03f); // 속도 조절 가능
        }
        isTyping = false;
    }
    private IEnumerator WaitForClick()
    {
        yield return WaitForSecondsCache.Wait(0.3f);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject());

        isWaitingForClick = false;
        waitClickRoutine = null;
        OnStepClear();
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
        isEvent = false;

        if (!isTurtorialMode) return;

        // 현재 스텝에서 정해진 오브젝트와 일치하면 튜토리얼 진행
        if (tutorialStep == 1 && CompareObject(interactObjects[0]))
        {
            OnStepClear();
        }
        else if (CompareObject(interactObjects[1]))
        {
            OnStepClear();
        }
        else if (CompareObject(interactObjects[2]))
        {
            OnStepClear();
        }

    }

    private void HandleUIClose(string uiName)
    {
        if (uiName == UIName.RefineSystemWindow || tutorialStep == 6)
        {
            Debug.Log(uiName);
            isEvent = false;
            OnStepClear();
        }

    }

    private void AllEffectOff()
    {
        arrowIcon.SetActive(false);
        effect.HideHighlight();
    }


    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, effect.transform.position);
        screenPos.z = 10f; // 카메라에서의 거리
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Gizmos.DrawSphere(worldPos, 0.5f);
    }



}
