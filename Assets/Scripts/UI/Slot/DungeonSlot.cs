using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonSlot : MonoBehaviour
{
    private GameManager gameManager;
    private DungeonData dungeonData;

    [SerializeField] TextMeshProUGUI dungeonName;
    [SerializeField] Button startBtn;
    [SerializeField] GameObject unlockIndicator;

    public void Init(GameManager gameManager, DungeonData data)
    {
        this.gameManager = gameManager;
        dungeonData = data;

        dungeonName.text = data.DungeonName;
        startBtn.onClick.AddListener(StartDungeon);

        SetUnlock();
    }

    public void SetUnlock()
    {
        unlockIndicator.SetActive(!gameManager.DungeonSystem.CheckUnlock(dungeonData.Key));
    }

    private void StartDungeon()
    {
        gameManager.DungeonSystem.EnterDungeon(dungeonData);
    }
}
