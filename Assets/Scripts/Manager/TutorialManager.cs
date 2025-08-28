using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    [SerializeField] private Button skillDrawButton;
    [SerializeField] private Button skillGetachButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button drawButton;
    [SerializeField] private Button normalButton;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            float xRatio = mousePos.x / Screen.width;
            float yRatio = mousePos.y / Screen.height;
            Debug.Log($"비율 좌표: ({xRatio:F2}, {yRatio:F2})");
        }
    }
    public void Init(GameManager gm)
    {
        gameManager = gm;
        if (PlayerPrefs.GetInt("TutorialDone", 0) == 1)
        {
            isTurtorialMode = false;
            Destroy(gameObject);
            return;
        }

        PlayerPrefs.SetInt("TutorialDone", 0); // Test
        isTurtorialMode = true;
        tutorialPanel.SetActive(true);

        StartCoroutine(StartTutorial());
    }

    IEnumerator StartTutorial()
    {
        yield return new WaitUntil(() => gameManager.ForgeManager.Name != null);
        
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

        if (skillGetachButton != null)
            skillGetachButton.onClick.AddListener(onClickSkillButton);
        if (skillDrawButton != null)
            skillDrawButton.onClick.AddListener(onClickSkillDraw);
        if (confirmButton != null)
        { 
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            Debug.Log($"{confirmButton.onClick.GetPersistentEventCount()}");
            Debug.Log("confirmButton 리스너 연결됨");
        }
        if (drawButton != null)
            drawButton.onClick.AddListener(OnDrawButtonClicked);
        if (normalButton != null)
            normalButton.onClick.AddListener(onClickNormalButton);



    }

    private void OnDisable()
    {
        if (!isTurtorialMode)
        {
            return;
        }

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
        if (skillDrawButton != null)
            skillDrawButton.onClick.RemoveListener(onClickSkillDraw);

        if (normalButton != null)
            normalButton.onClick.RemoveListener(onClickNormalButton);

        if (skillGetachButton != null)
            skillGetachButton.onClick.RemoveListener(onClickSkillButton);
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
                ShowTextWithTyping("어서오세요!! 대장간은 처음 방문하시는군요!!\n 만나서 반갑습니다 간단한 운영법을 알려드릴게요!!");
                isWaitingForClick = true;
                isEvent = false; //박스랑 상호작용해도 대사 넘어가게
                break;
            case 1:
                ShowTextWithTyping("대장간을 운영하기 위해 기본적인 기능들을 알려드릴게요!! 우측 화살표를 눌러주세요!!");
                break;
            case 2:
                ClickBlockerOff();
                tutorialPanel.SetActive(false);
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.94f, 0.12f);
                break;
            case 3:
                ClickBlockerOn();
                ShowTextWithTyping("이곳은 대장관 관련된 강화 , 레시피, 스킬 관련된 버튼이 숨겨져 있어요!!\n 첫번째로 강화에 대해서 설명해드릴게요 강화버튼을 눌러주세요!!");
                AllEffectOff();
                break;
            case 4:
                tutorialPanel.SetActive(false);
                ClickBlockerOff();
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.26f, 0.15f);
                break;
            case 5:
                tutorialPanel.SetActive(true);
                ClickBlockerOn();
                AllEffectOff();
                ShowTextWithTyping("강화는 골드를 사용해서 가게운영에 도움이 되는 다양한 기술들을 배울수 있어요!!\n간단하게 둘러보고 창을 닫아 볼까요??");
                break;
            case 6:
                ClickBlockerOff();
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.5f, 0.15f);
                tutorialPanel.SetActive(false);
                break;
            case 7:
                tutorialPanel.SetActive(true);
                ClickBlockerOn();
                ShowTextWithTyping("다음은 레시피 해금에 대해 알려드릴게요!! 무기 레시피 버튼을 눌러 주세요!!");
                AllEffectOff();
                break;
            case 8:
                ClickBlockerOff();
                tutorialPanel.SetActive(false);
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.43f, 0.15f);
                break;
            case 9:
                AllEffectOff();
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("레시피 해금을 위해서는 레시피 포인트가 필요해요!! 처음 무기를 만드시는거니 이번에는 포인트 소모 없이 제작할수 있게 해드릴께요!!");
                ClickBlockerOn();
                break;
            case 10:
                tutorialPanel.SetActive(false);
                HighlightPos(0.50f, 0.67f);
                arrowIcon.SetActive(true);
                effect.HideHighlight();
                break;
            case 11:
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("레시피 포인트는 무기를 판매하거나 던전을 클리어 해서 레벨업하면 \n자 그럼 이제 한번 레시피 해금을 해볼까요?? 해금을 하고 창을 닫아주세요!!");
                HighlightPos(0.74f, 0.28f);
                effect.HideHighlight();
                arrowIcon.SetActive(true);
                break;
            case 12:
                ClickBlockerOff();
                tutorialPanel.SetActive(false);
                break;  
            case 13:
                tutorialPanel.SetActive(true);
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.50f, 0.57f); //0.50,0.57
                ShowTextWithTyping("이제 해금한 레시피를 사용해서 무기를 만들어 볼꺼에요!! 가운데 제작대를 클릭해 주세요!!");
                GameManager.Instance.Inventory.AddItem(GameManager.Instance.DataManager.ItemLoader.GetItemByKey("resource_bronze"), 1);
                GameManager.Instance.Inventory.AddItem(GameManager.Instance.DataManager.ItemLoader.GetItemByKey("ingot_copper"), 2);
                GameManager.Instance.ForgeManager.AddGold(1000); //골드 제공
                break;
            case 14:
                topHalfBlocker.SetActive(true);
                AllEffectOff();
                ShowTextWithTyping("이제 무기를 클릭해서 제작해볼까요??\n재료가 부족하네요!! 부족한 재료는 제가 드릴께요!!");
                ClickBlockerOn();
                break;
            case 15:
                topHalfBlocker.SetActive(false);
                ShowTextWithTyping("제작을 해주세요!!");
                break;
            case 16:
                ClickBlockerOff();
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.25f, 0.47f);
                effect.HideHighlight();
                tutorialPanel.SetActive(false);
                break;
            case 17:
                tutorialPanel.SetActive(true);
                ClickBlockerOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.22f, 0.64f);
                effect.HideHighlight();
                ShowTextWithTyping("제작이 끝나면 수령 버튼을 누르거나 탭을 닫으면 자동으로 인벤토리에 무기가 들어가요!! 그리고 손님이 이제 올꺼에요!! 창을 닫아 볼까요?");
                break;
            case 18:
                AllEffectOff();
                tutorialPanel.SetActive(false);
                ClickBlockerOff();
                break;
            case 19:
                AllEffectOff();
                ShowTextWithTyping("손님은 일반손님,단골손님, 진상손님이 있어요!! 진상손님은 클릭해서 쫒아내야 해요!!\n단골손님은 방문시 클릭하면 컬렉션에 등록되요!!");
                ClickBlockerOn();
                break;
            case 20:
                arrowIcon.SetActive(true);
                HighlightPos(0.92f, 0.79f); //0.92,0.81
                ShowTextWithTyping("컬렉션 북을 클릭하시면 컬렉션북을 열수 있어요!!");
                ClickBlockerOff();
                break;
            case 21:
                ClickBlockerOn();
                AllEffectOff();
                ShowTextWithTyping("단골 손님이 여러번 방문하면 특별한 효과를 얻을수 있어요!! 창을 닫아 주세요!!");
                break;
            case 22:
                ClickBlockerOff();
                arrowIcon.SetActive(true);
                HighlightPos(0.52f, 0.14f); 
                break;
            case 23:
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.74f, 0.73f); //0.74,0.73
                ShowTextWithTyping("이제 세공에 대해서 알아볼 거예요!!");
                break;
            case 24:
                AllEffectOff();
                ClickBlockerOn();
                ShowTextWithTyping("세공은 제작에 필요한 광석이나 장착에 필요한 보석을 만들수 있어요!!\n각 보석들을 클릭하면 필요한 재료가 보일꺼에요!!");
                break;
            case 25:
                ShowTextWithTyping("둘러보고 창을 닫아 볼까요??");
                break;
            case 26:
                ClickBlockerOff();
                tutorialPanel.SetActive(false);
                break;
            case 27:
                arrowIcon.SetActive(true);
                effect.HighLightOn();
                HighlightPos(0.29f, 0.81f); //0.29,0.81
                ShowTextWithTyping("다음은 강화에 대해서 알려드릴게요!!");
                break;
            case 28:
                ClickBlockerOn();
                HighlightPos(0.32f, 0.80f); //0.32,0.81
                ShowTextWithTyping("강화는 일반강화, 고급강화로 나눠져 있어요!! 가운데 창을 클릭해서 무기를 올려 볼까요?");
                break;
            case 29:
                HighlightPos(0.51f, 0.70f); //0.51,0.70
                ClickBlockerOn();
                ShowTextWithTyping("강화는 일반강화, 고급강화로 나눠져 있어요!! 가운데 창을 클릭해서 강화하고 싶은 무기를 올릴 수 있어요!!");
                break;
            case 30:
                ClickBlockerOn();
                HighlightPos(0.36f, 0.11f); 
                ShowTextWithTyping("일반강화는 골드를 사용해서 확률을 통해서 강화 할 수 있어요!!");
                break;
            case 31:
                ClickBlockerOn();
                HighlightPos(0.68f, 0.11f); 
                ShowTextWithTyping("고급강화는 재화를 사용해서 좀더 확률이 높게 강화할 수 있어요!! ");
                break;

            case 32:
                HighlightPos(0.70f, 0.80f); 
                ShowTextWithTyping("이번에는 보석강화에 대해서 알려드릴게요!! \n 보석강화 버튼을 눌러주세요!!");
                ClickBlockerOff();
                break;
            case 33:
                HighlightPos(0.50f, 0.54f); //0.50f,0.62f
                ClickBlockerOn();
                ShowTextWithTyping("해당 슬롯을 클릭해서 보석을 넣으면 해당 보석의 능력치를 부여할수 있어요!!");
                break;
            case 34:
                HighlightPos(0.51f, 0.02f); //0.51f,0.08f
                ShowTextWithTyping("이제 창을 닫아볼께요!!");
                ClickBlockerOff();
                break;
            case 35:
                AllEffectOff();
                ClickBlockerOn();
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("이번에는 스킬에 대해서 설명해 드릴 거예요!! 가게를 운영하면서 뽑기를 통해 스킬을 얻을수 있어요!! \n하단 탭에서 상점으로 가볼까요??");
                break;
            case 36:
                tutorialPanel.SetActive(false);
                ClickBlockerOff();
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.85f, 0.05f); //0.85f,0.06f
                break;
            case 37:
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("상점에서는 제자나 대장간 스킬을 구매할수 있어요!! 먼저 스킬을 구매해 볼꺼에요!!\n먼저 하단에 스킬 탭 버튼을 눌러주세요!!");
                shopTab.AllButtonOff();
                shopTab.SetButtonInteractableOnly(1);
                ClickBlockerOn();
                GameManager.Instance.ForgeManager.AddDia(100);
                break;
            case 38:
                tutorialPanel.SetActive(false);
                ClickBlockerOff();
                HighlightPos(0.51f, 0.15f);//0.51f,0.15f
                effect.HideHighlight();
                break;
            case 39:
                ClickBlockerOn();
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("스킬을 1개 뽑아 보세요!!");
                break;
            case 40:
                HighlightPos(0.35f, 0.27f);
                ClickBlockerOff();
                effect.HideHighlight();
                tutorialPanel.SetActive(false);
                break;
            case 41:
                HighlightPos(0.51f, 0.20f);
                effect.HideHighlight();
                tutorialPanel.SetActive(false);
                break;
            case 42:
                ClickBlockerOn();
                AllEffectOff();
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("스킬을 얻었어요!! 다시 대장간으로 돌아가 보죠!!");
                break;
            case 43:
                ClickBlockerOff();
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.52f, 0.05f);
                tutorialPanel.SetActive(false);
                break;
            case 44:
                AllEffectOff();
                ClickBlockerOn();
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("대장간 스킬을 클릭해서 획득한 스킬을 확인해봐요!!");
                break;
            case 45:
                ClickBlockerOff();
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.59f, 0.16f);
                tutorialPanel.SetActive(false);
                break;
            case 46:
                AllEffectOff();
                tutorialPanel.SetActive(true);
                ClickBlockerOn();
                ShowTextWithTyping("화면을 보시면 아까 획득한 스킬이 들어가있죠?? 스킬들은 좌측 메뉴에 최대 3개까지 등록할수 있어요!!");
                break;
            case 47:
                ShowTextWithTyping("마지막으로 제자에 대해서 알려드릴께요!! 스킬 창을 닫아주세요!!");
                break;
            case 48:
                tutorialPanel.SetActive(false);
                ClickBlockerOff();
                HighlightPos(0.51f,0.22f);
                arrowIcon.SetActive(true);
                effect.HideHighlight();
                break;
            case 49:
                ClickBlockerOn();
                arrowIcon.SetActive(false);
                ShowTextWithTyping("먼저 상점으로 가서 제자를 뽑아볼꺼에요 상점으로 이동해볼까요??");
                break;
            case 50:
                ClickBlockerOff();
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.87f, 0.06f); //0.87f,0.06f
                break;
            case 51:
                ClickBlockerOn();
                shopTab.AllButtonON(); //모든 버튼 활성화
                ShowTextWithTyping("이번엔 제자뽑기를 해볼꺼에요!!\n 다이아를 지급해 드릴께요!!\n제자뽑기 창으로 가볼께요!!");
                GameManager.Instance.ForgeManager.AddDia(500);
                GameManager.Instance.ForgeManager.AddGold(10000);
                break;
            case 52:
                ClickBlockerOff();
                tutorialPanel.SetActive(false);
                HighlightPos(0.28f, 0.16f); //0.28,0.16
                effect.HideHighlight();
                break;
            case 53:
                tutorialPanel.SetActive(false);
                HighlightPos(0.38f, 0.31f);//0.38f,0.31f
                effect.HideHighlight();
                break;
            case 54:
                AllEffectOff();
                tutorialPanel.SetActive(true);
                ShowTextWithTyping("제자는 뽑기는 3가지로 나눠져 있어요 \n 합격, 불합격, 보류로 나눠져 있어요!!");
                ClickBlockerOn();
                break;
            case 55:
                ShowTextWithTyping("마음에 드는 제자는 합격 , 마음에 들지 않으면 불합격, 맘에들지만 골드가 부족할때는 보류를 누르시면되요!!");
                ClickBlockerOn();
                break;
            case 56:
                ClickBlockerOff();
                tutorialPanel.SetActive(false);
                break;
            case 57:
                effect.HighLightOn();
                arrowIcon.SetActive(true);
                HighlightPos(0.33f, 0.06f); //0.33f,0.06f
                ShowTextWithTyping("이제 제자탭으로 가서 뽑은 제자를 확인해 볼까요??");
                break;
            case 58:
                AllEffectOff();
                ShowTextWithTyping("각 특성마다 제자를 작창시킬 수 있어요!! 장착하고 창을 닫으면 제자들이 일을 하고 있을꺼에요!!");
                ClickBlockerOn();
                break;
            case 59:
                ShowTextWithTyping("이제 다시 대장간으로 돌아가 볼까요??");
                break;
            case 60:
                ClickBlockerOff();
                HighlightPos(0.5f,0.05f);
                arrowIcon.SetActive(true);
                effect.HideHighlight();
                break;
                    
            case 61:
                
                ShowTextWithTyping("자! 기본적인 운영 방법은 전부 설명드렸어요!! 대장간을 잘 운영하셔서 부자되세요!!\n튜토리얼 보상으로 다이아 5000개랑 10000골드를 드릴게요!!");
                GameManager.Instance.ForgeManager.AddDia(5000);
                GameManager.Instance.ForgeManager.AddGold(10000);
                ClickBlockerOn();
                break;
            case 62:
                EndTutorial();
                break;
        }

    }


    private void HandleButtonClick(string buttonName)
    {
        if (tutorialStep == 2 && buttonName == "SlideTab_Btn")
        {
            OnStepClear();
        }
        else if (tutorialStep == 8 && buttonName == "Forge_Recipe")
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



  

    public void HighlightTarget(Transform target) //조명 비추기
    {
        if (!effect.gameObject.activeInHierarchy)
        {
            effect.gameObject.SetActive(true);
        }

        effect.ShowHighlight(target, uiCam);
    }

    public void HighlightPos(float xRatio, float yRatio)
    {
        Vector2 normalized = new Vector2(xRatio, yRatio);


        effect.ShowHighlight(normalized);
        Vector2 screenPos = new Vector2(xRatio * Screen.width, yRatio * Screen.height);

        Canvas canvas = arrowIcon.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCam, out Vector2 localPos);

        arrowIcon.GetComponent<RectTransform>().anchoredPosition = localPos + new Vector2(20f, 0f);
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


    public void HandleClickBlock() //클릭버튼 이벤트 및 조건 처리
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
                //ClickBlockerOff();
                OnStepClear();
            }
        }
    }
    public void ShowTextWithTyping(string text)
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





    public void HandleUIOpen(string uiName)
    {


        if (uiName == UIName.ForgeUpgradeWindow && tutorialStep == 4)
        {
            OnStepClear();

        }
        else if (uiName == UIName.CraftWeaponWindow && tutorialStep == 13)
        {
            OnStepClear();
        }
        else if (uiName == UIName.CollectionWindow && tutorialStep == 20)
        {
            OnStepClear();
        }
        else if (uiName == UIName.RefineSystemWindow && tutorialStep == 23)
        {
            OnStepClear();
        }

        else if (uiName == UIName.UpgradeWeaponWindow && tutorialStep == 27)
        {
            OnStepClear();
        }

        else if (uiName == UIName.SkillWindow && tutorialStep == 45)
        {
            OnStepClear();
        }
    }
    private void HandleTapOpen(string tabName)
    {
        if (tabName == "Shop_Tab" && (tutorialStep == 36|| tutorialStep ==50))
        {
            OnStepClear();
        }
        else if (tabName == "Forge_Tab" && (tutorialStep == 43 || tutorialStep == 60))
        {
            OnStepClear();
        }
        else if (tabName == "Assistant_Tab" && tutorialStep == 57)
        {
            OnStepClear();
        }
    }

    private void HandleUIClose(string uiName)
    {
        if (uiName == UIName.ForgeUpgradeWindow && tutorialStep == 6)
        {
            OnStepClear();
        }
        else if (uiName == UIName.WeaponRecipeWindow && tutorialStep == 12)
        {
            OnStepClear();
        }

        else if (uiName == UIName.CraftWeaponWindow && tutorialStep == 18)
        {
            OnStepClear();
        }

        else if (uiName == UIName.CollectionWindow && tutorialStep == 22)
        {
            OnStepClear();
        }
        else if (uiName == UIName.RefineSystemWindow && tutorialStep == 26)
        {
            OnStepClear();
        }
        else if (uiName == UIName.UpgradeWeaponWindow && tutorialStep == 34)
        {
            OnStepClear();
        }
        else if (uiName == UIName.SkillWindow && tutorialStep == 48)
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
        Debug.Log("함수 걸림");
        // 튜토리얼 단계 체크 후 처리
        if (tutorialStep == 41) //해당 단계가 맞는지 일단 비교후 넘김 
        {
            OnStepClear();
        }
        else if (tutorialStep == 51)
        {
            Debug.Log("제자 뽑기 확인 버튼 클릭됨 -> 다음 단계 진행");
            OnStepClear();
        }

    }

    private void OnDrawButtonClicked()
    {
        if (tutorialStep == 53)
        {
            OnStepClear();

        }

    }

    private void GetchEnd()
    {
        if (tutorialStep == 56)
        {
            OnStepClear();
        }
    }

    private void onClickNormalButton()
    {
        if (tutorialStep == 52)
        {
            OnStepClear();
        }
    }
    private void onClickSkillButton()
    {
        if (tutorialStep == 38)
        {
            OnStepClear();
        }
    }
    private void onClickSkillDraw()
    {
        if (tutorialStep == 40)
        {
            AllEffectOff();
            OnStepClear();
        }
    }

}
