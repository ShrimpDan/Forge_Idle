using System.Collections.Generic;
using UnityEngine;

public static class IconLoader
{
    private static Dictionary<string, Sprite> iconDict = new();

    public static Sprite GetIconByPath(string path)
    {
        if (iconDict.Count == 0)
            LoadAllWeaponIconSprites();

        if (string.IsNullOrEmpty(path))
        {
            if (iconDict.TryGetValue("Icons/Empty", out Sprite emptyIcon))
            {
                return emptyIcon;
            }

            emptyIcon = Resources.Load<Sprite>("Icons/Empty");
            iconDict["Icons/Empty"] = emptyIcon;
            
            return emptyIcon;
        }
        
        if (iconDict.TryGetValue(path, out Sprite icon))
            return icon;

        icon = Resources.Load<Sprite>(path);

        if (icon != null)
        {
            iconDict[path] = icon;
        }
        else
        {
            Debug.LogWarning($"아이콘이 존재하지않습니다. 경로: {path}");
        }

        return icon;
    }

    public static Sprite GetIconByKey(string key)
    {
        if (iconDict.Count == 0)
            LoadAllWeaponIconSprites();

        if (iconDict.TryGetValue(key, out Sprite icon))
            return icon;

        return null;
    }

    private static void LoadAllWeaponIconSprites()
    {
        string[] spriteSheetPaths = { "Icons/weapon", "Icons/armor" };

        foreach (string path in spriteSheetPaths)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);

            if (sprites.Length > 0)
            {
                foreach (Sprite sprite in sprites)
                {
                    if (iconDict.ContainsKey(sprite.name))
                    {
                        Debug.LogWarning($"'{sprite.name}' 이름의 스프라이트가 이미 존재하여 덮어씁니다. 시트 간 이름이 중복되지 않도록 확인해주세요.");
                    }
                    iconDict[sprite.name] = sprite;
                }
            }
        }
    }

    public static void ClearDict()
    {
        iconDict.Clear();
    }
}
