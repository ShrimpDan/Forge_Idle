using System.Collections.Generic;
using UnityEngine;

public static class IconLoader
{
    private static Dictionary<string, Sprite> iconDict = new();

    public static Sprite GetIcon(string path)
    {
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

    public static void ClearDict()
    {
        iconDict.Clear();
    }   
}
