using System;
using System.Collections.Generic;

public class ForgeAssistantHandler
{
    private Forge forge;
    private ForgeVisualHandler visualHandler;
    private ForgeStatHandler statHandler;

    public Dictionary<SpecializationType, AssistantInstance> EquippedAssistant { get; private set; }

    public ForgeAssistantHandler(Forge forge, ForgeVisualHandler visualHandler, ForgeStatHandler statHandler)
    {
        this.forge = forge;
        this.visualHandler = visualHandler;
        this.statHandler = statHandler;

        InitAssistant();
    }

    private void InitAssistant()
    {
        EquippedAssistant = new Dictionary<SpecializationType, AssistantInstance>()
        {
            { SpecializationType.Crafting, null },
            { SpecializationType.Selling, null },
        };
    }

    public void ActiveAssistant(AssistantInstance assi)
    {
        // 이전에 등록된 제자가 있다면 해제
        AssistantInstance preAssi = EquippedAssistant[assi.Specialization];
        if (preAssi != null)
        {
            DeActiveAssistant(preAssi);
        }

        EquippedAssistant[assi.Specialization] = assi;
        forge.Events.RaiseAssistantChanged(assi, true);
        assi.IsEquipped = true;

        statHandler.ApplyAssistantStat(assi);
        visualHandler.SpawnAssistantPrefab(assi);
    }

    public void DeActiveAssistant(AssistantInstance assi)
    {
        assi.IsEquipped = false;
        EquippedAssistant[assi.Specialization] = null;
        forge.Events.RaiseAssistantChanged(assi, false);

        statHandler.DeApplyAssistantStat(assi);
        visualHandler.ClearSpawnRoot(assi.Specialization);
    }

    public List<string> GetSaveData()
    {
        List<string> keys = new List<string>();

        foreach (var assi in EquippedAssistant.Values)
        {
            if(assi != null)
                keys.Add(assi.Key);
        }

        return keys;
    }

    public void LoadFromData(List<string> equippedAssistantKeys)
    {
        //throw new NotImplementedException();
    }
}
