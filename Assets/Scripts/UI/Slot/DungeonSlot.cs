using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class TestDungeonData
{
    public string Key;
    public string DungeonName;
    public float MonsterHp;
    public float BossHp;
    public List<string> RewardItemKeys;
}

public class DungeonSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dungeonName;
    [SerializeField] Button startBtn;

    private TestDungeonData dungeonData;

    public void Init(TestDungeonData data)
    {
        dungeonData = data;
        startBtn.onClick.AddListener(StartDungeon);
    }

    private void StartDungeon()
    {
        SceneManager.LoadScene("DungeonScene", LoadSceneMode.Additive);
    }
}
