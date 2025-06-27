using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dungeonName;
    [SerializeField] Button startBtn;

    private DungeonData dungeonData;

    public void Init(DungeonData data)
    {
        dungeonData = data;

        dungeonName.text = data.DungeonName;
        startBtn.onClick.AddListener(StartDungeon);
    }

    private void StartDungeon()
    {
        LoadSceneManager.Instance.LoadSceneAsync(SceneType.Dungeon, true);
    }
}
