using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI DigCountText;
    [SerializeField] private TextMeshProUGUI TreasureCountText;
    [SerializeField] private GameObject resultUI;





    [Header("BoradSize")]
    [SerializeField] private int width;
    [SerializeField] private int height;


    [Header("Treasure")]
    [SerializeField] private TreasureData[] allTreasures;
    private TreasureData[] treasures ;

    [Header("BlockPrefabs")]
    [SerializeField] private Block blockPrefab;
    [SerializeField] private Transform boardRoot; //GridLayoutGroup 붙여야함


  
    


    private Block[,] board;
    private int digCount;
    private Dictionary<int, HashSet<Vector2Int>> treasureCoordinate = new Dictionary<int, HashSet<Vector2Int>>();//중복 방지 HashSet
    private HashSet<int> foundTreasureId = new HashSet<int>();


    private void Start()
    {
        treasures = allTreasures.OrderBy(x => UnityEngine.Random.value).Take(5).ToArray(); //가지고 있는 보물 갯수중에 랜덤 5개만 
        resultUI.GetComponent<MiniGameResultUI>().InitSlotCount(treasures.Length); //여기서 결과창 갯수 초기화

        SettingBorad();
        SettingTreasuresRandom();
        foundTreasureId.Clear();//초기화
        UpdateTreasureCountUI();

        digCount = maxDigCount;
        OnDigCountChange += UpdateCountUI;
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
            SoundManager.Instance.Play("SFX_MineMiniGameSuccess");
            
            var set = treasureCoordinate[block.TreasureId];
            set.Add(block.Pos);
            
            TreasureData data = treasures.First(t => t.id == block.TreasureId);
            if (set.Count == data.Shape.Length)
            {
                OnTreasureCompleted(data);
            }
        }
        else
        {
            SoundManager.Instance.Play("SFX_MineMiniGameMiss");
        }

        if (digCount == 0)
        {
            FairDig();
        }
    }


    private void FairDig()
    {
        ShowResult();
        Debug.Log("발굴 실패");
    }
    void OnTreasureCompleted(TreasureData data) //완성된 보물 전달 메서드
    {
        Debug.Log($"보물{data.id}");
        Debug.Log("발굴 완료 ");
        if (foundTreasureId.Add(data.id))
        {
            UpdateTreasureCountUI(); //해당 보물이 발굴되면 UI업데이트  
            RewardGem(data);
            resultUI.GetComponent<MiniGameResultUI>().AddIcon(data.rewardImage);
        }

        if (foundTreasureId.Count == treasures.Length)
        {
            //다찾으면
            ShowResult();
        }

        OnTreasureComplete?.Invoke(data); //데이터 전달
    }

    private void RewardGem(TreasureData data)
    {
        Debug.Log("들어감");
        if (data.gemType == GemType.Gem)
        {
            GameManager.Instance.Inventory.AddItem(GameManager.Instance.DataManager.ItemLoader.GetItemByKey(data.Name));
        }
        else
        {
            GameManager.Instance.ForgeManager.AddDia(50); //나중에 다이아 수치 따로 빼둘예정
        }
    }

  

    private bool InsideCheck(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public void OnClickExit()
    {
        LoadSceneManager.Instance.UnLoadScene(SceneType.MiniGame);
    }


    private void UpdateCountUI(int current, int max)
    {
        DigCountText.text = $"{current} / {max}";
    }

    private void UpdateTreasureCountUI()
    {
        TreasureCountText.text = $"{foundTreasureId.Count}/ {treasures.Length}";
    }

    private void ShowResult()
    {
        resultUI.SetActive(true);
        SoundManager.Instance.Play("SFX_SystemReward");
    }
}



/*
  보드 생성
  블록 배치 ->안겹치게
  클릭하면 발굴
  보물 완성 채크
  클릭 횟수
  보석 갯수중에 5개만 
   */