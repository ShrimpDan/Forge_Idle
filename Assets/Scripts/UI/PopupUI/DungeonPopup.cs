using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonPopup : MonoBehaviour
{
    private DungeonManager dungeonManager;

    [SerializeField] private TextMeshProUGUI clearText;
    [SerializeField] private Transform rewardRoot;
    [SerializeField] private Button confirmButton;


    public void Init(DungeonManager dungeonManager, bool isClear)
    {
        this.dungeonManager = dungeonManager;

        if (isClear)
        {
            clearText.text = "던전 클리어 !!";
        }
        else
        {
            clearText.text = "클리어 실패...";
        }

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnClickButton);

        gameObject.SetActive(true);
    }

    private void OnClickButton()
    {
        dungeonManager.ExitDungeon();
    }
}
