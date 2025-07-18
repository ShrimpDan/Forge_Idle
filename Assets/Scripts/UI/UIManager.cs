using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System;

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

    public event Action<String> CloseUIName;

    // 1000단위마다 k/m/b 접미사로 줄여서 숫자 포맷을 반환 (1.2k, 1.5m 등)
    public static string FormatNumber(int number)
    {
        if (number >= 1_000_000_000)
            return (number % 1_000_000_000 == 0)
                ? (number / 1_000_000_000) + "b"
                : (number / 1_000_000_000f).ToString("0.##") + "b";
        else if (number >= 1_000_000)
            return (number % 1_000_000 == 0)
                ? (number / 1_000_000) + "m"
                : (number / 1_000_000f).ToString("0.##") + "m";
        else if (number >= 1_000)
            return (number % 1_000 == 0)
                ? (number / 1_000) + "k"
                : (number / 1_000f).ToString("0.##") + "k";
        else
            return number.ToString();
    }

    public static string FormatNumber(long number)
    {
        if (number >= 1_000_000_000)
            return (number % 1_000_000_000 == 0)
                ? (number / 1_000_000_000) + "b"
                : (number / 1_000_000_000d).ToString("0.##") + "b";
        else if (number >= 1_000_000)
            return (number % 1_000_000 == 0)
                ? (number / 1_000_000) + "m"
                : (number / 1_000_000d).ToString("0.##") + "m";
        else if (number >= 1_000)
            return (number % 1_000 == 0)
                ? (number / 1_000) + "k"
                : (number / 1_000d).ToString("0.##") + "k";
        else
            return number.ToString();
    }

    public static string FormatNumber(float number, int decimalPoint = 0)
    {
        // 소수점 이하 두 자리까지 k/m/b 변환 (정밀 표기)
        if (number >= 1_000_000_000)
            return (number % 1_000_000_000 == 0)
                ? ((long)number / 1_000_000_000) + "b"
                : (number / 1_000_000_000f).ToString("0.##") + "b";
        else if (number >= 1_000_000)
            return (number % 1_000_000 == 0)
                ? ((long)number / 1_000_000) + "m"
                : (number / 1_000_000f).ToString("0.##") + "m";
        else if (number >= 1_000)
            return (number % 1_000 == 0)
                ? ((long)number / 1_000) + "k"
                : (number / 1_000f).ToString("0.##") + "k";
        else
            return number.ToString($"N{decimalPoint}");
    }

    public static string FormatNumber(double number, int decimalPoint = 0)
    {
        if (number >= 1_000_000_000)
            return (number % 1_000_000_000 == 0)
                ? ((long)number / 1_000_000_000) + "b"
                : (number / 1_000_000_000d).ToString("0.##") + "b";
        else if (number >= 1_000_000)
            return (number % 1_000_000 == 0)
                ? ((long)number / 1_000_000) + "m"
                : (number / 1_000_000d).ToString("0.##") + "m";
        else if (number >= 1_000)
            return (number % 1_000 == 0)
                ? ((long)number / 1_000) + "k"
                : (number / 1_000d).ToString("0.##") + "k";
        else
            return number.ToString($"N{decimalPoint}");
    }





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

        ui.Init(gameManager, this);
        // dotween 효과 넣기 
        if ((ui.UIType == UIType.Popup || ui.UIType == UIType.Window) && ui.RootPanel != null)
        {
            UIEffect.PopupOpenEffect(ui.RootPanel, 0.25f);
        }

        ui.Open();
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

        // Popup/Window: 애니메이션
        if ((ui.UIType == UIType.Popup || ui.UIType == UIType.Window) && ui.RootPanel != null)
        {
            UIEffect.PopupCloseEffect(ui.RootPanel, 0.18f);
            Destroy(ui.gameObject, 0.19f); 
        }
        else
        {
            ui.Close();
            Destroy(ui.gameObject);
        }

        activeUIs.Remove(uiName);
        CloseUIName?.Invoke(uiName);

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

    public void ReLoadUI(string uiName)
    {
        if (!activeUIs.TryGetValue(uiName, out var ui) || ui == null)
            return;

        ui.Init(gameManager, this);
    }
}

