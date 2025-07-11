using UnityEngine;
using System.Collections.Generic;
using System;

public class GameSaveManager
{
    private List<ISaveHandler> saveHandlers = new();

    public void RegisterSaveHandler(ISaveHandler handler)
    {
        if (!saveHandlers.Contains(handler))
            saveHandlers.Add(handler);
    }

    public void SaveAll()
    {
        foreach (var handler in saveHandlers)
        {
            handler.Save();
        }

        Debug.Log("[저장 시스템] 게임 저장 완료.");
    }

    public void LoadAll()
    {
        foreach (var handler in saveHandlers)
        {
            handler.Load();
        }

        Debug.Log("[저장 시스템] 게임 불러오기 완료.");
    }

    public void DeleteAll()
    {
        foreach (var handler in saveHandlers)
        {
            handler.Delete();
        }
    }
}
