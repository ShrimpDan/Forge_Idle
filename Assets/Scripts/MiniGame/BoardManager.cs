using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public event Action<TreasureData> OnTreasureComplete; //보물 완성되면 이걸로 어떤 보물이 완성된건지 전달
    public int DigCount => digCount;
    public int MaxDigCount => maxDigCount;
    public event Action<int, int> OnDigCountChange;//횟수 변경 UI반영

    [SerializeField] private int maxDigCount;
    [SerializeField] private Button ExitButton;

    [Header("BoradSize")]
    [SerializeField] private int width;
    [SerializeField] private int height;


    [Header("Treasure")]
    [SerializeField] private TreasureData[] treasures ;

    [Header("BlockPrefabs")]
    [SerializeField] private Block blockPrefab;
    [SerializeField] private Transform boardRoot; //GridLayoutGroup 붙여야함


    private Block[,] board;
    private int digCount;
    private Dictionary<int, HashSet<Vector2Int>> treasureCoordinate = new Dictionary<int, HashSet<Vector2Int>>();//중복 방지 HashSet



    private void Start()
    {
       
        SettingBorad();
        SettingTreasuresRandom();

        digCount = maxDigCount;
        OnDigCountChange?.Invoke(digCount, maxDigCount);
    }

    private void SettingBorad()
    {
        board = new Block[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Block block = Instantiate(blockPrefab, boardRoot);

                int reversedY = height - 1 - i;
                Vector2Int pos = new Vector2Int(j, reversedY);

                block.Init(new Vector2Int(j, i), this);
                board[j, i] = block;
            }
        }
    }

    private void SettingTreasuresRandom()
    {
        foreach (var treasure in treasures)
        {
            bool placed = false;
            Vector2Int anchor = Vector2Int.zero;

            while (!placed)
            {
                anchor = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
                placed = treasure.Shape.All(offset =>
                {
                    Vector2Int pos = anchor + offset;
                    return InsideCheck(pos) && board[pos.x, pos.y].TreasureId == -1;
                    //요거 구조 변경할지도.

                }
                );

            }
            treasureCoordinate[treasure.id] = new HashSet<Vector2Int>();

            for (int i = 0; i < treasure.Shape.Length; i++) //보물 모양넣어주기
            {
                Vector2Int offset = treasure.Shape[i];
                Vector2Int pos = anchor + offset;
                board[pos.x, pos.y].SetTreasure(treasure.id, i, treasure.pratSprite[i]);

            
            }
        }
    }


    public void TryDig(Block block)
    {
        if (block.isDig || digCount <= 0)
        {
            return;        
        }

        digCount--;
        OnDigCountChange?.Invoke(digCount, maxDigCount);
        block.Reveal();
        if (block.TreasureId != -1)
        {
            var set = treasureCoordinate[block.TreasureId];
            set.Add(block.Pos);

            TreasureData data = treasures.First(t => t.id == block.TreasureId);
            if (set.Count == data.Shape.Length)
            {
                OnTreasureCompleted(data);
            }
        }

        if (digCount == 0)
        {
            FairDig();    
        }


    }


    private void FairDig()
    {
        Debug.Log("발굴 실패");
    }
    void OnTreasureCompleted(TreasureData data) //완성된 보물 전달 메서드
    {
        Debug.Log($"보물{data.id}");
        Debug.Log("발굴 완료 ");
        OnTreasureComplete?.Invoke(data); //데이터 전달
    }

    private Block GetBlock(Vector2Int pos)
    {
        int index = pos.y * width + pos.x; // GetChild 인덱스 계산 수정 (x -> pos.x)
        return boardRoot.GetChild(index).GetComponent<Block>();
        //나중에 효과 연결할때 사용할까? 
    }

    private bool InsideCheck(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public void OnClickExit()
    {
        LoadSceneManager.Instance.UnLoadScene(SceneType.MiniGame);
    }

}



/*
  보드 생성
  블록 배치 ->안겹치게
  클릭하면 발굴
  보물 완성 채크
  클릭 횟수
   */