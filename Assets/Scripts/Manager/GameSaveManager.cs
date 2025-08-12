using System.Collections.Generic;

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
    }

    public void LoadAll()
    {
        foreach (var handler in saveHandlers)
        {
            handler.Load();
        }
    }

    public void DeleteAll()
    {
        foreach (var handler in saveHandlers)
        {
            handler.Delete();
        }
    }
}
