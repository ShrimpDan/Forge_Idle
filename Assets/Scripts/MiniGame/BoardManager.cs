using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int DigCount => digCount;
    public int MaxDigCount => maxDigCount;
    public event Action<int, int> OnDigCountChange;//횟수 변경 UI반영

    [SerializeField] private int maxDigCount;


    [Header("BoradSize")]
    [SerializeField] private int width;
    [SerializeField] private int height;


    [Header("Treasure")]
    [SerializeField] private TreasureData[] treasures ;

    [Header("BlockPrefabs")]
    [SerializeField] private Block blockPrefab;
    [SerializeField] private Transform boardRoot;


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
                var block = Instantiate(blockPrefab, boardRoot);
                block.transform.localPosition = new Vector3(j, -i, 0);
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

            for (int i = 0; i < treasure.Shape.Length; i++) //보물 모양넣어주기
            {
                Vector2Int offset = treasure.Shape[i];
                Vector2Int pos = anchor + offset;
                board[pos.x, pos.y].SetTreasure(treasure.id, i, treasure.pratSprite[i]);

            
            }

            treasureCoordinate[treasure.id] = new HashSet<Vector2Int>();




        }
    }


    public void TryDig(Block block)
    {
        if (block.isDig || digCount < 0)
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
    void OnTreasureCompleted(TreasureData data)
    {
        Debug.Log("발굴 완료 ");
    }

    private Block GetBlock(Vector2Int pos)
    {
        int index = pos.y * width + pos.y;
        return boardRoot.GetChild(index).GetComponent<Block>();
    }

    private bool InsideCheck(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }


}



/*
  보드 생성
  블록 배치 ->안겹치게
  클릭하면 발굴
  보물 완성 채크
  클릭 횟수
   */