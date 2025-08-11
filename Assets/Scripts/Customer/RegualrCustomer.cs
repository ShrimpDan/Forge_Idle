using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class RegualrCustomer : Customer
{
    public Action<CustomerJob> OnPriceBoosted; // 단골손님 가격올리기

    [SerializeField] private GameObject InteractObject; // 말풍선
    [SerializeField] private float WaitTime = 4.0f;
    [SerializeField] private RegularCustomerData CollectData;

    #region 사운드 및 효과

    [SerializeField] private HeartEffect HeartEffectPrefab;
    [SerializeField] private float effectLife = 1.0f;
    [SerializeField] private float effectOffsetY = 0.5f;
    [SerializeField] private string clickSfxName; //아직 사운드 없음 

    #endregion

    private bool isDiscovered = false;
    private bool isInteracting = false;

    protected override void Start()
    {
        base.Start();
        if (isDiscovered && InteractObject != null)
        {
            InteractObject.SetActive(false);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isDiscovered = false;
        isInteracting = false;

        if (InteractObject != null)
            InteractObject.SetActive(true);

    }

    protected override void Update()
    {
        //마인씬이면 동작 금지
        if (SceneCameraState.IsMineSceneActive)
            return;

        base.Update();
        if (isDiscovered)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("클릭됨");
            CheckClick(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckClick(Input.GetTouch(0).position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("클릭됨");
            CheckClick(Input.mousePosition);
        }
#endif    
    }

    private void CheckClick(Vector2 screenPos)
    {
        // 카메라 없으면 무시 (마인씬)
        if (Camera.main == null) return;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit == null || hit.transform != transform) return;

        ShowEffect(); //이펙트 보여주기
        
        isDiscovered = true;
        if (InteractObject != null)
        {
            InteractObject.SetActive(false);
            StartCoroutine(ShowInteractBubbleForSeconds(2f));
        }


        var gm = GameManager.Instance;

        if (gm != null)
        {
            if (gm.CollectionManager != null)
            {
                gm.CollectionManager.Discover(CollectData);
                gm.CollectionManager.AddVisited(CollectData.Key);
            }
            if (gm.DailyQuestManager != null)
            {
                gm.DailyQuestManager.ProgressQuest("InteractMissition", 1);
            }
        }

    }
    private IEnumerator ShowInteractBubbleForSeconds(float duration)
    {
        InteractObject.SetActive(true);
        yield return WaitForSecondsCache.Wait(duration);
        InteractObject.SetActive(false);
    }

    public override void Interact()
    {
        //아이에 사용 안됨
        OnPriceBoosted?.Invoke(Job); //가격 증가
        if (customerManager != null)
            customerManager.NotifyNormalCustomerPurchased(Job);
    }

    public void SettingCollectData(RegularCustomerData data)
    {
        CollectData = data;
    }

    protected override IEnumerator PerformPurChase()
    {
        state = CustomerState.Purchasing;


        orderBubble.SetActive(false);
        speech.Show("Love");

        Interact();
        yield return WaitForSecondsCache.Wait(1f);
        speech.Hide();

        if (buyPoint != null)
        {
            buyPoint.CustomerOut();
        }

        yield return MoveToExit();
    }

    private IEnumerator MoveToExit()
    {
        state = CustomerState.Exiting;
        if (moveWayPoint != null && moveWayPoint.Length > 1)
            yield return MoveingWayPoint(moveWayPoint[1].position);
        CustomerExit();
    }

    private void ShowEffect()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y += effectOffsetY;

        GameObject effectobj = customerManager.PoolManager.Get(
            HeartEffectPrefab.gameObject, // ← 원본 프리팹으로 키 유지
            spawnPos,
            Quaternion.identity
        );

        if (effectobj == null)
        {
            Debug.LogError("[ShowEffect] effectobj is NULL");
            return;
        }

        var effect = effectobj.GetComponent<HeartEffect>();
        if (effect == null)
        {
            Debug.LogError("[ShowEffect] HeartEffect component missing on pooled instance!");
            return;
        }

        
        effect.SourcePrefab = HeartEffectPrefab.gameObject;

        effect.Init(spawnPos, () =>
        {
            customerManager.PoolManager.ReturnComponent(effect);
        });


    }
}
