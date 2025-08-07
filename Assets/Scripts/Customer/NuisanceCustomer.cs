using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class NuisanceCustomer : Customer
{
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private int penaltyGold;
    [SerializeField] private GameObject InteractIcon;
    [SerializeField] private float offsetY = 0.5f;
    [SerializeField] private GameObject catchEffectPrefab;

    private FlapperEffect catchEffect;


    [Header("ExitPoint")]
    [SerializeField] private Transform exitPoint;



    private static bool blockClickCheck = false; // 마인씬 체크용 상태변수 추가
    private bool isClicked = false;




    protected override void Start()
    {

        if (buyPoint == null)
        {
            buyPoint = GetRandomBuyPoint();
        }
        if (InteractIcon != null)
        {
            InteractIcon.SetActive(true);
        }

        if (catchEffectPrefab != null)
        { 
            catchEffect = catchEffectPrefab.GetComponent<FlapperEffect>();
            if (catchEffect == null)
            {
                Debug.LogError("CatchEffectPrefab에 FlapperEffect 컴포넌트가 없습니다.", this);
            }
        }
        else
        {
            Debug.LogWarning("CatchEffectPrefab이 설정되지 않았습니다.", this);

        }
        //  StartCoroutine(NuisanceFlow());

    }
    protected override void OnEnable()
    {

        isClicked = false;
        if (InteractIcon != null)
        {
            InteractIcon.SetActive(true);
        }
        base.OnEnable();
    }

    protected override IEnumerator CustomerFlow()
    {
        yield return MoveRandomPlace();

        float timer = 0f;
        while (timer < waitTime && !isClicked)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return ExitFlow();
    }



    protected override void Update()
    {
        base.Update();


#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            if (blockClickCheck) return;
            else
            {
                Debug.Log("클릭됨");
                CheckClick(Input.mousePosition);
            }

        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
        CheckClick(Input.GetTouch(0).position);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (blockClickCheck) return; 
            else
            {
                CheckClick(Input.mousePosition);
            }
                
        }
#endif 


    }

    private void CheckClick(Vector2 screenPos)
    {
        if (Camera.main == null) return; // 메인카메라 없으면 클릭 무시

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && hit.transform == transform)
        {
            Interact();

        }
    }
    public override void Interact()
    {
        if (isClicked)
        {
            return;
        }
        isClicked = true;

        SoundManager.Instance.Play("SFX_GuestKickout01");

        if (InteractIcon != null)
        {
            Debug.Log("InteractIcon 비활성화 시도");
            InteractIcon.SetActive(false);
        }

        GameManager.Instance.DailyQuestManager.ProgressQuest("CatchMission", 1);
        ShowEffect(); //여기 이펙트 보여줄예정
        //여기에 매서드 추가
        StopAllCoroutines();
        StartCoroutine(ExitFlow());

    }

    private IEnumerator ExitFlow()
    {
        state = CustomerState.Exiting;

        if (moveWayPoint != null && moveWayPoint.Length > 0 && moveWayPoint[0] != null)
        {
            yield return MoveingWayPoint(moveWayPoint[0].position);
        }
        else
        {
            Debug.LogError($"[NuisanceCustomer] 퇴장 경로(moveWayPoint)가 설정되지 않았습니다!", this.gameObject);
        }

        if (!isClicked)
        {
            PenaltyGold();
        }

        CustomerExit();
    }

    private void Disappear()
    {
        //점점 사라지는 fade효과 연출
        

    }


    //private 

    private BuyPoint GetRandomBuyPoint() //수정해야될듯
    {
        var points = customerManager.allBuyPoints; //넣어줄 예정
        if (points == null || points.Count == 0)
        {
            return null;
        }
        return points[Random.Range(0, points.Count)];

    }

    private void PenaltyGold()
    {
        GameManager.Instance.ForgeManager.AddGold(-1000);
        Debug.Log("골드 차감");
    }

    // 마인씬용 매서드
    public static void SetBlockClick(bool block)
    {
        blockClickCheck = block;
    }

    private void ShowEffect()
    {
        Vector3 spawnPos = transform.position;
        GameObject effectObj = Instantiate(catchEffectPrefab, spawnPos, Quaternion.identity);

        effectObj.transform.position = spawnPos;
        
        Destroy(effectObj, 1f); // 2초 후에 이펙트 오브젝트 제거

    }
}
