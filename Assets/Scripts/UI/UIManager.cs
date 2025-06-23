using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Transform fixedRoot;
    [SerializeField] private Transform windowRoot;
    [SerializeField] private Transform popupRoot;

    private Dictionary<string, BaseUI> activeUIs = new();
    private Dictionary<string, GameObject> loadedPrefabs = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var ui in fixedRoot.GetComponentsInChildren<BaseUI>(true))
        {
            ui.Init(this);
            ui.gameObject.SetActive(false);
        }

        var mainUI = fixedRoot.GetComponentInChildren<MainUI>(true);
        if (mainUI != null)
        {
            mainUI.gameObject.SetActive(true);
            mainUI.Open();
        }
    }

    public T OpenUI<T>(string uiName) where T : BaseUI
    {
        if (activeUIs.ContainsKey(uiName))
        {
            activeUIs[uiName].Open();
            return activeUIs[uiName] as T;
        }

        GameObject prefab = LoadPrefab(uiName);
        Transform parent = GetParentByType(prefab.GetComponent<BaseUI>().UIType);
        GameObject go = Instantiate(prefab, parent);

        T ui = go.GetComponent<T>();
        ui.Open();
        ui.Init(this);
        activeUIs.Add(uiName, ui);

        return ui;
    }

    public void CloseUI(string uiName)
    {
        if (activeUIs.TryGetValue(uiName, out var ui))
        {
            ui.Close();
            if (ui.UIType != UIType.Fixed)
            {
                Destroy(ui.gameObject);
                activeUIs.Remove(uiName);
            }
        }
    }

    private GameObject LoadPrefab(string uiName)
    {
        if (!loadedPrefabs.TryGetValue(uiName, out var prefab))
        {
            prefab = Resources.Load<GameObject>($"UI/{uiName}");
            loadedPrefabs[uiName] = prefab;
        }

        return prefab;
    }

    private Transform GetParentByType(UIType type)
    {
        return type switch
        {
            UIType.Fixed => fixedRoot,
            UIType.Window => windowRoot,
            UIType.Popup => popupRoot,
            _ => windowRoot,
        };
    }
}
