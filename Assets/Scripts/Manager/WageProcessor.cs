using System;
using System.Collections.Generic;
using UnityEngine;

public class WageProcessor
{
    private readonly GameManager gm;

    public WageProcessor(GameManager gameManager)
    {
        gm = gameManager;
    }

    public void ProcessHourlyWage()
    {
        var inventory = gm.AssistantInventory;
        if (inventory == null)
        {
            Debug.LogWarning("[시급] AssistantInventory가 null입니다.");
            return;
        }

        var allTrainees = inventory.GetAll();
        int totalPaid = 0;
        int activeCount = 0;
        var runawayList = new List<AssistantInstance>();

        foreach (var assi in allTrainees)
        {
            if (assi.IsFired) continue;

            activeCount++;

            if (gm.ForgeManager.UseGold(assi.Wage))
            {
                totalPaid += assi.Wage;
                Debug.Log($"[시급] {assi.Name}에게 {assi.Wage}G 지급 완료");
            }
            else
            {
                assi.IsFired = true;
                runawayList.Add(assi);

                if (assi.IsEquipped && gm.Forge != null && gm.Forge.AssistantHandler != null)
                {
                    gm.Forge.AssistantHandler.DeActiveAssistant(assi);
                    assi.IsEquipped = false;
                }

                var mineSceneManager = GameObject.FindObjectOfType<MineSceneManager>();
                if (mineSceneManager != null)
                {
                    foreach (var group in mineSceneManager.mineGroups)
                    {
                        for (int slotIdx = 0; slotIdx < group.slots.Count; ++slotIdx)
                        {
                            var slot = group.slots[slotIdx];
                            if (slot.IsAssigned && slot.AssignedAssistant == assi)
                            {
                                slot.Unassign();
                                group.slotUIs[slotIdx].AssignAssistant(null);
                                mineSceneManager.ClearSlotAssistant(slotIdx, group.slotUIs[slotIdx]);
                                break;
                            }
                        }
                    }
                }

                var assistantTab = GameObject.FindObjectOfType<AssistantTab>();
                assistantTab?.RefreshSlots();
            }
        }

        if (runawayList.Count > 0)
        {
            var popup = GameObject.FindObjectOfType<AssistantRunawayPopup>();
            popup?.ShowPopup(runawayList);
        }

        if (totalPaid > 0)
        {
            Debug.Log($"<color=yellow>[시급 처리 완료] 총 {totalPaid}G 지출</color>");
            GoldChangeEffectManager.Instance?.ShowGoldChange(-totalPaid);
        }

        gm.SaveManager.SaveAll();
    }
}
