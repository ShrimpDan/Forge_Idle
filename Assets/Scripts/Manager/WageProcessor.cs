using System;
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

                if (assi.IsEquipped && gm.Forge != null && gm.Forge.AssistantHandler != null)
                {
                    gm.Forge.AssistantHandler.DeActiveAssistant(assi);
                    assi.IsEquipped = false;
                    Debug.Log($"[시급] {assi.Name} 착용 해제됨 (탈주 처리)");
                }

                Debug.LogWarning($"[시급] {assi.Name} 시급 {assi.Wage}G 지급 실패 → 제자가 탈주 처리됨");

                var assistantTab = GameObject.FindObjectOfType<AssistantTab>();
                assistantTab?.RefreshSlots();
            }
        }

        if (activeCount == 0)
        {
            Debug.Log("<color=gray>[시급] 지급 대상 제자가 없습니다.</color>");
        }
        else if (totalPaid > 0)
        {
            Debug.Log($"<color=yellow>[시급 처리 완료] 총 {totalPaid}G 지출</color>");
            GoldChangeEffectManager.Instance?.ShowGoldChange(-totalPaid);
        }

        gm.SaveManager.SaveAll();
    }
}
