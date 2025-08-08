using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DailySlotController : MonoBehaviour
{
    [SerializeField] private List<DailyQuestUI> dailyQuestUIs;

    private DailyQuestManager dailyQuestManager;
    private DailyQuestLoader currentLoader;
    public void Init(DailyQuestManager manager)
    {
        dailyQuestManager = manager;
        SettingSlot();
    }


    private void SettingSlot()
    {
        var questList = dailyQuestManager.GetActiveQuestDic().Values.ToList();

        for (int i = 0; i < dailyQuestUIs.Count; i++)
        {
            if (dailyQuestUIs[i] == null)
            {
                Debug.Log("프리팹에서 할당되지 않았습니다.");
                continue;
            }

            if (i < questList.Count)
            {
                dailyQuestUIs[i].gameObject.SetActive(true);
                dailyQuestUIs[i].InitButton(questList[i], dailyQuestManager);
            }
            else
            {
                dailyQuestUIs[i].gameObject.SetActive(false);
            }
        }

    }

    public void Refresh()
    {
      
        SettingSlot();
    }
    public void Refresh(DailyQuestLoader loader)
    {
        foreach (var ui in dailyQuestUIs)
        {
            if (ui.IsSameQuest(loader)) // questId 같은지 체크
            {
                ui.Refresh(loader);
                break;
            }
        }
    }


}
