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

    [Header("To Show Reward Resources")]
    [SerializeField] ResourceSlot[] resourceSlots;

    public void Init(GameManager gameManager, DungeonData data)
    {
        this.gameManager = gameManager;
        dungeonData = data;

        dungeonName.text = data.DungeonName;
        startBtn.onClick.AddListener(StartDungeon);
        SetRewardSlot(data);
        SetUnlock();
    }

    public void SetUnlock()
    {
        unlockIndicator.SetActive(!gameManager.DungeonSystem.CheckUnlock(dungeonData.Key));
    }

    private void StartDungeon()
    {
        if (gameManager.Inventory.IsEquippedWeapon())
        {
            gameManager.UIManager.CloseUI(UIName.DungeonWindow);
            gameManager.DungeonSystem.EnterDungeon(dungeonData);
            return;
        }

        var ui = gameManager.UIManager.OpenUI<LackPopup>(UIName.LackPopup);
        ui.ShowCustom("무기를 장착해주세요.");
    }

    private void SetRewardSlot(DungeonData data)
    {
        int idx = 0;

        foreach (var resourceKey in data.RewardItemKeys)
        {
            if (resourceKey == null) continue;
            
            ResourceSlot slot = resourceSlots[idx];
            ItemData item = gameManager.DataManager.ItemLoader.GetItemByKey(resourceKey);
            slot.SetDungeonResource(item.Name, IconLoader.GetIconByKey(resourceKey), data.MinCount, data.MaxCount);
            slot.gameObject.SetActive(true);

            idx++;
        }
    }
}
