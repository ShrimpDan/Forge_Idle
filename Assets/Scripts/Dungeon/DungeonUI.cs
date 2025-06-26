using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUI : MonoBehaviour
{
    private DungeonManager dungeonManager;
    private UIManager uIManager;

    [Header("Battle Info UI")]
    [SerializeField] private Image timeFill;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI monsterText;
    [SerializeField] private DungeonPopup dungeonPopup;

    public void Init(DungeonManager dungeonManager, UIManager uIManager)
    {
        this.dungeonManager = dungeonManager;
        this.uIManager = uIManager;
    }

    public void UpdateTimerUI(float current, float max)
    {
        timeFill.fillAmount = current / max;
        timeText.text = current.ToString("F0");
    }

    public void UpdateMonsterUI(int killedMonster, int maxMonster)
    {
        if (killedMonster < maxMonster)
            monsterText.text = $"{killedMonster}/{maxMonster}";
        else
            monsterText.text = "Boss";
    }

    public void OpenClearPopup(bool isClear)
    {
        dungeonPopup.Init(dungeonManager, isClear);
    }
}
