using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dungeonName;
    [SerializeField] Button startBtn;

    private GameManager gameManager;
    private DungeonData dungeonData;

    public void Init(GameManager gameManager, DungeonData data)
    {
        this.gameManager = gameManager;
        dungeonData = data;

        dungeonName.text = data.DungeonName;
        startBtn.onClick.AddListener(StartDungeon);
    }

    private void StartDungeon()
    {
        gameManager.StartDungeon(dungeonData);
    }
}
