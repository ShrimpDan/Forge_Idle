using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    //대사 출력확인
    private bool isTyping = false;
    private Coroutine typingRoutine;
    private string fullText = "";

    private GameManager gameManager;
    [SerializeField] private int tutorialStep = 0; //숫자 몇인지 확인하면서 작업
    public static bool isTurtorialMode = true;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject arrowIcon;
    [SerializeField] private List<GameObject> interactObjects = new List<GameObject>();
    //스킵은 나중에

    [Header("카메라")]
    [SerializeField] Camera uiCam;

    private bool isWaitingForClick = false;
    private bool isEvent = false; // 해당 이벤트가 끝났는지 확인 이벤트가 있을때만 false로 변경해서 기다리자

    [Header("조명 이펙트")]
    [SerializeField] private HighLightEffectController effect;
    [SerializeField] List<Transform> hightLightTargets = new List<Transform>();


    [Header("클릭방지용")]
    [SerializeField] private GameObject clickBlocker;
    [SerializeField] private GameObject topHalfBlocker;
    private Coroutine waitClickRoutine;


    private GameObject curInteractObject;
    [Header("상점")]
    [SerializeField] private ShopTab shopTab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button drawButton;
    [SerializeField] private Button normalButton;


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

        GameManager.Instance.CraftingManager.isCrafingDone += OnEventDone; // 제작 완료 이벤트 등록
        GameManager.Instance.UIManager.CloseUIName += HandleUIClose;
        GameManager.Instance.UIManager.OpenUIName += HandleUIOpen;
        MainUI.onTabClick += HandleTapOpen;
        ForgeTab.onClickButton += HandleButtonClick;
        RecruitPreviewManager.isEndGecha += GetchEnd;

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        if (drawButton != null)
            drawButton.onClick.AddListener(OnDrawButtonClicked);
        if (normalButton != null)
            normalButton.onClick.AddListener(onClickNormalButton);


      
    }

    private void OnDestroy()
    {
        ClickBlocker.OnBlockClick -= HandleClickBlock;
        ForgeTab.onClickButton -= HandleButtonClick;

        GameManager.Instance.UIManager.CloseUIName -= HandleUIClose;
        GameManager.Instance.CraftingManager.isCrafingDone -= OnEventDone; //일단 막아둬
        GameManager.Instance.UIManager.OpenUIName -= HandleUIOpen;
        MainUI.onTabClick -= HandleTapOpen;
        RecruitPreviewManager.isEndGecha -= GetchEnd;

        if (waitClickRoutine != null)
        {
            StopCoroutine(waitClickRoutine);
            waitClickRoutine = null;
        }
        ClickBlockerOff();
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
        if (drawButton != null)
            drawButton.onClick.RemoveListener(OnDrawButtonClicked);

        if (normalButton != null)
            normalButton.onClick.RemoveListener(onClickNormalButton);
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
                ClickBlockerOn();

                ShowTextWithTyping("어서오세요!! 대장간은 처음 방문하시는군요!!\n 만나서 반갑습니다 간단한 운영법을 알려드릴께요!!");
                isWaitingForClick = true;
                isEvent = false; //박스랑 상호작용해도 대사 넘어가게
                break;
            case 1:
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(503, -764);
                ShowTextWithTyping("대장간을 운영하기 위해 기본적인 기능들을 알려드릴께요!! 우측 화살표를 눌러주세요!!");
                break;
            case 2:
                ShowTextWithTyping("이곳은 대장관 관련된 강화 , 레시피, 스킬 관련된 버튼이 숨겨져 있어요!!\n 첫번째로 강화에 대해서 설명해드릴께요 강화버튼을 눌러주세요!!");
                HighlightPos(-220, -710);
                break;
            case 3:
                AllEffectOff();
                ShowTextWithTyping("강화는 골드를 사용해서 가게운영에 도움이 되는 다양한 기술들을 배울수 있어요!!");
                ClickBlockerOn();
                break;
            case 4:
                ShowTextWithTyping("간단하게 둘러보고 창을 닫아볼까요??");
                ClickBlockerOn();
                break;
            case 5:
                tutorialPanel.SetActive(false);
                break;
            case 6:
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("다음은 제작에 대해서 설명해드릴께요!! 제작버튼을 눌러주세요!!");
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(-44, -710);
                break;
            case 7:
                HighlightPos(40, 300);
                ShowTextWithTyping("제작을 위해서는 제작포인트가 필요해요!! 처음 무기를 만드시는거니 이번에는 포인트 소모 없이 제작할수 있게 해드릴께요!!");
                ClickBlockerOn();
                break;
            case 8:
                AllEffectOff();
                ShowTextWithTyping("제작 포인트는 무기를 판매하거나 던전을 클리어 하면 얻을수 있어요!! \n자 그럼이제 한번 레시피 제작을 해볼까요?? 제작을 하고 창을 닫아주세요!!");
                ClickBlockerOn();
                break;
            case 9:
                tutorialPanel.SetActive(false);
                break;
            case 10:
                tutorialPanel.SetActive(true);
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(31, 81);
                ShowTextWithTyping("이제 제작한 레시피를 사용해서 무기를 만들어 볼꺼에요!! 가운데 제작대를 클릭해 주세요!!");
                GameManager.Instance.Inventory.AddItem(GameManager.Instance.DataManager.ItemLoader.GetItemByKey("resource_bronze"), 1);
                GameManager.Instance.Inventory.AddItem(GameManager.Instance.DataManager.ItemLoader.GetItemByKey("ingot_copper"), 2);
                GameManager.Instance.ForgeManager.AddGold(1000); //골드 제공
                break;
            case 11:
                topHalfBlocker.SetActive(true);
                AllEffectOff();
                ShowTextWithTyping("이제 무기를 클릭해서 제작해볼까요??\n재료가 부족하네요!! 부족한 재료는 제가 드릴께요!!");
                ClickBlockerOn();
                break;
            case 12:
                topHalfBlocker.SetActive(false);
                ShowTextWithTyping("제작을 해주세요!!");
                ClickBlockerOn();

                break;
            case 13:
                AllEffectOff();
                tutorialPanel.SetActive(false);
                break;
            case 14:
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("제작이 끝나면 자동으로 인벤토리에 무기가 들어가요!! 그리고 손님이 이제 올꺼에요!!");
                ClickBlockerOn();
                break;
            case 15:
                ShowTextWithTyping("손님은 일반손님,단골손님, 진상손님이 있어요!! 진상손님은 클릭해서 쫒아내야해요!!\n단골손님은 방문시 클릭하면 컬렉션에 등록되요!!");
                ClickBlockerOn();
                break;
            case 16:
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(-400, 834);
                ShowTextWithTyping("컬렉션은 레벨 버튼을 누르면 확인할수 있어요!!");
                break;
            case 17:
                AllEffectOff();
                ShowTextWithTyping("단골 손님이 여러번 방문하면 특별한 효과를 얻을수 있어요!! 창을 닫아주세요!!");
                break;
            case 18:
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(250, 400);
                ShowTextWithTyping("이제 세공에 대해서 알아볼꺼에요!!");
                break;
            case 19:
                AllEffectOff();
                ClickBlockerOn();
                ShowTextWithTyping("세공은 제작에 필요한 광석이나 장착에 필요한 보석을 만들수 있어요!!\n각 보석들을 클릭하면 필요한 재료가 보일꺼에요!!");
                break;
            case 20:
                ShowTextWithTyping("둘러보고 창을 닫아볼까요??");
                ClickBlockerOn();
                break;
            case 21:
                tutorialPanel.SetActive(false);
                break;
            case 22:
                arrowIcon.SetActive(true);
                effect.HighLightOn();
                HighlightPos(-200, 630);
                ShowTextWithTyping("다음은 강화에 대해서 알려드릴께요!!");
                break;
            case 23:
                ClickBlockerOn();
                HighlightPos(-160, 667);
                ShowTextWithTyping("강화는 일반강화, 고급강화로 나눠져 있어요!! 가운데 창을 클릭해서 무기를 올려볼까요?");
                break;
            case 24:
                HighlightPos(0, 300);
                ClickBlockerOn();
                ShowTextWithTyping("강화는 일반강화, 고급강화로 나눠져 있어요!! 가운데 창을 클릭해서 강화하고 싶은 무기를 올릴 수 있어요!!");
                break;
            case 25:
                ClickBlockerOn();
                HighlightPos(-120, -627);
                ShowTextWithTyping("일반강화는 골드를 사용해서 확률을 통해서 강화 할수있어요!! ");
                break;
            case 26:
                ClickBlockerOn();
                HighlightPos(216, -627);
                ShowTextWithTyping("고급강화는 재화를 사용해서 좀더 확률이 높게 강화가 가능해요!! ");
                break;

            case 27:
                HighlightPos(250, 667);
                ShowTextWithTyping("이번에는 보석강화에 대해서 알려드릴께요!! \n 보석강화 버튼을 눌러주세요!!");
                break;
            case 28:
                HighlightPos(44, 170);
                ClickBlockerOn();
                ShowTextWithTyping("해당 슬롯을 클릭해서 보석을 넣으면 해당 보석의 능력치를 부여할수 있어요!!");
                break;
            case 29:
                HighlightPos(55, -853);
                ShowTextWithTyping("이제 창을 닫아볼께요!!");
                break;
            case 30:
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("이번에는 스킬에 대해서 설명해 드릴꺼에요!! 가게를 운영하면서 뽑기를 통해 스킬을 얻을수 있어요!! \n하단 탭에서 상점으로 가볼까요??");
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(400, -900);
                break;
            case 31:
                HighlightPos(20, -700);
                effect.HideHighlight();
                ShowTextWithTyping("상점에서는 제자나 대장간 스킬을 구매할수 있어요!! 먼저 스킬을 구매해 볼꺼에요!!\n먼저 하단에 스킬 탭 버튼을 눌러주세요!!");
                shopTab.AllButtonOff();
                shopTab.SetButtonInteractableOnly(1);
                ClickBlockerOn();
                GameManager.Instance.ForgeManager.AddDia(100);
                break;
            case 32:
                ShowTextWithTyping("스킬을 1개 뽑아보세요!!");
                ClickBlockerOn();
                break;
            case 33:
                tutorialPanel.SetActive(false);
                ClickBlockerOn();
                break;
            case 34:
                tutorialPanel.SetActive(false);
                AllEffectOff();
                break;
            case 35:
                tutorialPanel.SetActive(true);
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0, -880);
                ShowTextWithTyping("스킬을 얻었어요!! 다시 대장간으로 돌아가보죠!!");
                break;
            case 36:
                HighlightPos(90, -730);
                ShowTextWithTyping("대장간 스킬을 클릭해서 획득한 스킬을 확인해봐요!!");
                break;
            case 37:
                AllEffectOff();
                arrowIcon.SetActive(false);
                ShowTextWithTyping("화면을 보시면 아까 획득한 스킬이 들어가있죠?? 스킬을 뽑으면 자동으로 들어가게 됩니다!!");
                ClickBlockerOn();
                break;
            case 38:
                ShowTextWithTyping("마지막으로 제자에 대해서 알려드릴께요!! 스킬 창을 닫아주세요!!");
                break;
            case 39:
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(400, -900);
                ShowTextWithTyping("먼저 상점으로 가서 제자를 뽑아볼꺼에요 상점으로 이동해볼까요??");
                break;
            case 40:
                shopTab.AllButtonON(); //모든 버튼 활성화
                ShowTextWithTyping("이번엔 제자뽑기를 해볼꺼에요!!\n 다이아를 지급해 드릴께요!!\n제자뽑기 창으로 가볼께요!!");
                GameManager.Instance.ForgeManager.AddDia(500);
                GameManager.Instance.ForgeManager.AddGold(2000);
                HighlightPos(-204, -706);
                effect.HideHighlight();
                break;
            case 41:
                tutorialPanel.SetActive(false);
                HighlightPos(66, -281);
                effect.HideHighlight();
                break;
            case 42:
                AllEffectOff();
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("제자는 뽑기는 3가지로 나눠져 있어요 \n 합격, 불합격, 보류로 나눠져 있어요!!");
                ClickBlockerOn();
                break;
            case 43:
                ShowTextWithTyping("맘에드는 제자는 합격 , 맘에 들지 않으면 불합격, 맘에들지만 골드가 부족할때는 보류를 누르시면되요!!");
                ClickBlockerOn();
                break;
            case 44:
                tutorialPanel.SetActive(false);
                break;
            case 45:
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(-140, -900);
                ShowTextWithTyping("이제 제자탭으로 가서 뽑은 제자를 확인해 볼까요??");
                break;
            case 46:
                AllEffectOff();
                ShowTextWithTyping("각 특성마다 제자를 작창시킬 수 있어요!! 장착하고 창을 닫으면 제자들이 일을 하고 있을꺼에요!!");
                ClickBlockerOn();
                break;
            case 47:
                ShowTextWithTyping("이제 다시 대장간으로 돌아가 볼까요??");
                break;
            case 48:
                ShowTextWithTyping("자! 기본적인 운영 방법은 전부 설명드렸어요!! 대장간을 잘 운영하셔서 부자되세요!!");
                EndTutorial();
                break;



        }






    }


    private void HandleButtonClick(string buttonName)
    {
        if (tutorialStep == 1 && buttonName == "SlideTab_Btn")
        {
            OnStepClear();
        }
        else if (tutorialStep == 6 && buttonName == "Forge_Recipe")
        {
            OnStepClear();
        }
    }


    private void EndTutorial()
    {
        isTurtorialMode = false;
        PlayerPrefs.SetInt("TutorialDone", 1);
        tutorialPanel.SetActive(false);
        AllEffectOff();
        effect.HideHighlight();
        ClickBlockerOff();
        GameManager.Instance.Forge.CustomerManager.EndTutorial();

    }

    public void OnStepClear() //스텝 클리어 후 다음 셋팅시작하는 순서로 가야할듯하다.
    {
        if (!isTurtorialMode || isEvent)
        {
            //이벤트가 종료가 안된거 같으면 다시 리턴해준다.
            return;
        }

        ClickBlockerOff();
        tutorialStep++;
        HandleStep();
    }

    public void OnEventDone()
    {

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
            screenPos.x += 20f;
            arrowIcon.transform.position = screenPos;

        }
    }

    private void MoveArrowToPos(Vector2 pos)
    {
        Canvas canvas = arrowIcon.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pos, uiCam, out Vector2 localPos);


        arrowIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(localPos.x + 20f, localPos.y);


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





    private void HandleUIOpen(string uiName)
    {
        if (uiName == UIName.ForgeUpgradeWindow && tutorialStep == 2)
        {
            OnStepClear();

        }
        else if (uiName == UIName.CraftWeaponWindow && tutorialStep == 10)
        {
            OnStepClear();
        }
        else if (uiName == UIName.CollectionWindow && tutorialStep == 16)
        {
            OnStepClear();
        }
        else if (uiName == UIName.RefineSystemWindow && tutorialStep == 18)
        {
            OnStepClear();
        }

        else if (uiName == UIName.UpgradeWeaponWindow && tutorialStep == 22)
        {
            OnStepClear();
        }

        else if (uiName == UIName.SkillWindow && tutorialStep == 36)
        {
            OnStepClear();
        }
    }
    private void HandleTapOpen(string tabName)
    {
        if (tabName == "Shop_Tab" && tutorialStep == 30)
        {
            OnStepClear();
        }
        else if (tabName == "Forge_Tab" && (tutorialStep == 35 || tutorialStep == 47))
        {
            OnStepClear();
        }
        else if (tabName == "Shop_Tab" && tutorialStep == 39)
        {
            OnStepClear();
        }
        else if (tabName == "Assistant_Tab" && tutorialStep == 45)
        {
            OnStepClear();
        }
    }

    private void HandleUIClose(string uiName)
    {
        if (uiName == UIName.ForgeUpgradeWindow && tutorialStep == 5)
        {
            OnStepClear();
        }
        else if (uiName == UIName.WeaponRecipeWindow && tutorialStep == 9)
        {
            OnStepClear();
        }
        else if (uiName == UIName.CraftWeaponWindow && tutorialStep == 13)
        {
            OnStepClear();
        }
        else if (uiName == UIName.CollectionWindow && tutorialStep == 17)
        {
            OnStepClear();
        }
        else if (uiName == UIName.RefineSystemWindow && tutorialStep == 21)
        {
            OnStepClear();
        }
        else if (uiName == UIName.UpgradeWeaponWindow && tutorialStep == 29)
        {
            OnStepClear();
        }
        else if (uiName == UIName.SkillWindow && tutorialStep == 38)
        {
            OnStepClear();
        }
    }

    private void AllEffectOff()
    {
        arrowIcon.SetActive(false);
        effect.HideHighlight();
    }


    public void ForceStepClear() //외부에서 튜토리얼 진행  -> 이거 흠... 이걸쓸껄그랬나..
    {
        if (!isTurtorialMode || isEvent)
        {
            return;
        }
        OnStepClear();
    }




    private void OnConfirmButtonClicked()
    {
        // 튜토리얼 단계 체크 후 처리
        if (tutorialStep == 34) //해당 단계가 맞는지 일단 비교후 넘김 
        {
            Debug.Log("확인 버튼 클릭됨 -> 다음 단계 진행");
            OnStepClear();
        }
        else if (tutorialStep == 41)
        {
            Debug.Log("제자 뽑기 확인 버튼 클릭됨 -> 다음 단계 진행");
            OnStepClear();
        }
       
    }

    private void OnDrawButtonClicked()
    {
        if (tutorialStep == 41)
        {
            OnStepClear();

        }

    }

    private void GetchEnd()
    {
        if (tutorialStep ==44)
        {
            OnStepClear();
        }
    }

    private void onClickNormalButton()
    {
        if (tutorialStep ==40)
        {
            OnStepClear();
        }
    }
 

}
