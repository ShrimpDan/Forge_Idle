using System.Collections.Generic;
using UnityEngine;

public static class IconLoader
{
    private static Dictionary<string, Sprite> iconDict = new();

    public static Sprite GetIcon(ItemType itemType, string key)
    {
        string path = GetIconPath(itemType, key);
        return GetIconByPath(path);
    }

    public static string GetIconPath(ItemType itemType, string key)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                return $"Icons/weapon/{key}";
            case ItemType.Armor:
                return $"Icons/armor/{key}";
            case ItemType.Gem:
            case ItemType.Ingot:
            case ItemType.Resource:
                return $"Icons/{key}";
            default:
                return $"Icons/{key}";
        }
    }

    public static Sprite GetIconByPath(string path)
    {
        if (iconDict.Count == 0)
            LoadAllWeaponArmorSprites();

        if (string.IsNullOrEmpty(path))
        {
            if (iconDict.TryGetValue("Icons/Empty", out Sprite emptyIcon))
                return emptyIcon;

            emptyIcon = Resources.Load<Sprite>("Icons/Empty");
            iconDict["Icons/Empty"] = emptyIcon;
            return emptyIcon;
        }

        if (iconDict.TryGetValue(path, out Sprite icon))
            return icon;

        icon = Resources.Load<Sprite>(path);

        if (icon != null)
            iconDict[path] = icon;
        else
            Debug.LogWarning($"아이콘이 존재하지 않습니다. 경로: {path}");

        return icon;
    }

    public static Sprite GetIconByKey(string key)
    {
        if (iconDict.Count == 0)
            LoadAllWeaponArmorSprites();

        if (iconDict.TryGetValue(key, out Sprite icon))
            return icon;

        return null;
    }

    /// 무기/방어구만 미리 로드
    private static void LoadAllWeaponArmorSprites()
    {
        string[] spriteSheetPaths = { "Icons/weapon", "Icons/armor" };

        foreach (string path in spriteSheetPaths)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);
            foreach (Sprite sprite in sprites)
            {
                iconDict[$"{path}/{sprite.name}"] = sprite;
            }
        }
    }

    public static void ClearDict()
    {
        iconDict.Clear();
    }
}
