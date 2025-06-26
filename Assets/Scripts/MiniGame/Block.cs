using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    //프로퍼티
    public Vector2Int Pos { get; private set; } //Vector2는 float기반이라 좌표 나타낼때 오차가 생길수도 있음
    public int TreasureId { get; private set; } = -1;
    public bool isDig { get; private set; } = false;

    public int TresurePartIndex { get; private set; } = -1;


    [SerializeField] private Image image;
    [SerializeField] private Sprite coveredSprite;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private GameObject Effect;
    private Sprite treasureSprite;
    //보드 메니저 추가할 예정


    private BoardManager manager;
    public void Init(Vector2Int _pos, BoardManager _manager) //Manager에서 호출
    {
        Pos = _pos;
        manager = _manager;
        isDig = false;
        image.sprite = coveredSprite;
        GetComponent<Button>().onClick.AddListener(() => manager.TryDig(this)); //UI로 변경     
    }



    //보물

    public void SetTreasure(int id ,int partindex , Sprite sprite)
    {
        TreasureId = id;
        TresurePartIndex = partindex;
        treasureSprite = sprite;
    }
    


    #region 마우스
    private void OnMouseUpAsButton()
    {
        if (isDig)
        {
            return;
        }
        manager.TryDig(this);     //팔꺼임
    }

    private void OnMouseEnter()
    {
        if (!isDig && Effect != null)
        {
            Effect.SetActive(true);//효과
        }

    }
    private void OnMouseExit()
    {
        if (Effect != null)
        {
            Effect.SetActive(false);
        }
    }

    #endregion


    public void Reveal()
    {
        isDig = true;
        if (Effect != null)
        {
            Effect.SetActive(false);
        }
        bool hasTreasure = TreasureId != -1;

        image.sprite = hasTreasure ? treasureSprite : emptySprite; 
        //있으면 보물사진 없으면 빈거
    }

    //보물을 부분별로 알아야할때 블록이 알아야하나?

}
