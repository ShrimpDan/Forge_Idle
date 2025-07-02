using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private Transform fixedRoot;
    [SerializeField] private Transform windowRoot;
    [SerializeField] private Transform popupRoot;

    private Image popupBlockRay;
    private Image windowBlockRay;

    private Dictionary<string, BaseUI> activeUIs = new();
    private Dictionary<string, GameObject> loadedPrefabs = new();

    // 숫자 천단위 , 작업 ------
    public static string FormatNumber(long number)
    {
        return number.ToString("N0", CultureInfo.InvariantCulture);
    }
    public static string FormatNumber(int number)
    {
        return number.ToString("N0", CultureInfo.InvariantCulture);
    }
    public static string FormatNumber(float number, int decimalPoint = 0)
    {
        string fmt = decimalPoint > 0 ? $"N{decimalPoint}" : "N0";
        return number.ToString(fmt, CultureInfo.InvariantCulture);
    }
    public static string FormatNumber(double number, int decimalPoint = 0)
    {
        string fmt = decimalPoint > 0 ? $"N{decimalPoint}" : "N0";
        return number.ToString(fmt, CultureInfo.InvariantCulture);
    }
    // 숫자 천단위 , 작업 ------



    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;

        foreach (var ui in fixedRoot.GetComponentsInChildren<BaseUI>(true))
        {
            ui.Init(gameManager, this);
            ui.gameObject.SetActive(false);
        }

        var mainUI = fixedRoot.GetComponentInChildren<MainUI>(true);
        if (mainUI != null)
        {
            mainUI.gameObject.SetActive(true);
            mainUI.Open();
        }

        popupBlockRay = popupRoot.GetComponent<Image>();
        windowBlockRay = windowRoot.GetComponent<Image>();
    }

    public T OpenUI<T>(string uiName) where T : BaseUI
    {
        if (activeUIs.TryGetValue(uiName, out var cachedUi) && cachedUi != null)
        {
            cachedUi.Open();
            return cachedUi as T;
        }

        GameObject prefab = LoadPrefab(uiName);
        if (prefab == null)
            return null;

        var baseUi = prefab.GetComponent<BaseUI>();
        if (baseUi == null)
            return null;

        Transform parent = GetParentByType(baseUi.UIType);
        GameObject go = Instantiate(prefab, parent);

        T ui = go.GetComponent<T>();
        if (ui == null)
            return null;

        ui.Open();
        ui.Init(gameManager, this);
        activeUIs[uiName] = ui;

        if (ui.UIType == UIType.Popup && popupBlockRay != null)
            popupBlockRay.enabled = true;
        else if (ui.UIType == UIType.Window && windowBlockRay != null)
            windowBlockRay.enabled = true;

        return ui;
    }

    public void CloseUI(string uiName)
    {
        if (!activeUIs.TryGetValue(uiName, out var ui) || ui == null)
            return;

        ui.Close();

        if (ui.UIType != UIType.Fixed)
        {
            Destroy(ui.gameObject);
            activeUIs.Remove(uiName);
        }

        if (ui.UIType == UIType.Popup && popupBlockRay != null)
        {
            bool anyPopup = activeUIs.Values.Any(x => x != null && x.UIType == UIType.Popup);
            popupBlockRay.enabled = anyPopup;
        }
        else if (ui.UIType == UIType.Window && windowBlockRay != null)
        {
            bool anyWindow = activeUIs.Values.Any(x => x != null && x.UIType == UIType.Window);
            windowBlockRay.enabled = anyWindow;
        }
    }

    private GameObject LoadPrefab(string uiName)
    {
        if (!loadedPrefabs.TryGetValue(uiName, out var prefab) || prefab == null)
        {
            if (uiName.Contains("Window"))
            {
                prefab = Resources.Load<GameObject>($"UI/Window/{uiName}");
            }

            if (uiName.Contains("Popup"))
            {
                prefab = Resources.Load<GameObject>($"UI/Popup/{uiName}");
            }

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

    public void CloseAllWindowUI()
    {
        var closeList = activeUIs
            .Where(pair => pair.Value != null && pair.Value.UIType == UIType.Window)
            .Select(pair => pair.Key)
            .ToList();

        foreach (var uiName in closeList)
        {
            CloseUI(uiName);
        }
    }
}

