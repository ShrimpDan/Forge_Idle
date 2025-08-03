using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EquippedAssistantSaveHandler : ISaveHandler
{
    private const string SaveKey = "EquippedAssistant";
    private ForgeManager forgeManager;

    public EquippedAssistantSaveHandler(ForgeManager forgeManager)
    {
        this.forgeManager = forgeManager;
    }

    public void Save()
    {
        var data = new Dictionary<string, string>();

        foreach (var forgePair in forgeManager.EquippedAssistant)
        {
            foreach (var specPair in forgePair.Value)
            {
                var assi = specPair.Value;
                if (assi != null)
                {
                    string key = $"{forgePair.Key}_{specPair.Key}";
                    data[key] = assi.Key;
                }
            }
        }

        string json = JsonUtility.ToJson(new Wrapper { equippedData = data });
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return;

        string json = PlayerPrefs.GetString(SaveKey);
        var wrapper = JsonUtility.FromJson<Wrapper>(json);
        var equipped = forgeManager.EquippedAssistant;

        foreach (var kvp in wrapper.equippedData)
        {
            var split = kvp.Key.Split('_');
            if (split.Length != 2) continue;

            if (System.Enum.TryParse(split[0], out ForgeType forgeType) &&
                System.Enum.TryParse(split[1], out SpecializationType specType))
            {
                var assi = GameManager.Instance.AssistantInventory.GetAssistantInstance(kvp.Value);
                if (assi != null)
                {
                    equipped[forgeType][specType] = assi;
                    assi.IsEquipped = true;
                }
            }
        }
    }

    public void Delete()
    {
        PlayerPrefs.DeleteKey(SaveKey);
    }

    [System.Serializable]
    private class Wrapper
    {
        public Dictionary<string, string> equippedData = new();
    }
}
