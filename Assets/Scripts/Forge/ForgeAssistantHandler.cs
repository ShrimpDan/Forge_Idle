using System.Collections.Generic;

public class ForgeAssistantHandler
{
    private Forge forge;
    private ForgeManager forgeManager;

    private ForgeVisualHandler visualHandler;
    private ForgeStatHandler statHandler;

    public ForgeAssistantHandler(Forge forge)
    {
        this.forge = forge;
        forgeManager = forge.ForgeManager;
        visualHandler = forge.VisualHandler;
        statHandler = forge.StatHandler;

        InitAssistant(forgeManager.EquippedAssistant[forge.ForgeType]);
    }

    private void InitAssistant(Dictionary<SpecializationType, AssistantInstance> assiDict)
    {
        foreach (var assi in assiDict.Values)
        {
            if (assi == null) continue;
            statHandler.ApplyAssistantStat(assi, true);
            visualHandler.SpawnAssistantPrefab(assi);
        }
    }

    public void ActiveAssistant(AssistantInstance assi)
    {
        // 이전에 등록된 제자가 있다면 해제
        AssistantInstance preAssi = forgeManager.EquippedAssistant[forge.ForgeType][assi.Specialization];

        if (preAssi != null)
        {
            DeActiveAssistant(preAssi);
        }

        forgeManager.EquippedAssistant[forge.ForgeType][assi.Specialization] = assi;
        assi.EquipAssi(true, forge.ForgeType);

        statHandler.ApplyAssistantStat(assi, true);
        visualHandler.SpawnAssistantPrefab(assi);
    }

    public void DeActiveAssistant(AssistantInstance assi)
    {
        forgeManager.EquippedAssistant[assi.EquippedForge][assi.Specialization] = null;
        assi.EquipAssi(false);

        statHandler.ApplyAssistantStat(assi, false);
        visualHandler.ClearSpawnRoot(assi.Specialization);
    }
}
