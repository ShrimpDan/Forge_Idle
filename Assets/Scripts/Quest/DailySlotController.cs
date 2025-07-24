using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DailySlotController : MonoBehaviour
{
    [SerializeField] private List<DailyQuestUI> dailyQuestUIs;

    private DailyQuestManager dailyQuestManager;

    public void Init(DailyQuestManager manager)
    {
        dailyQuestManager = manager;
        SettingSlot();
    }


    private void SettingSlot()
    {
        var activeQuests = dailyQuestManager.GetActiveQuests();
        for (int i = 0; i < dailyQuestUIs.Count; i++)
        {
            if (dailyQuestUIs[i] == null)
            {
                Debug.Log("프리팹에서 할당되지 않았습니다.");
                continue;
            }

            if (i < activeQuests.Count)
            {
                dailyQuestUIs[i].gameObject.SetActive(true);
                dailyQuestUIs[i].InitButton(activeQuests[i], dailyQuestManager);
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

}
