using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        // �̹� ���������� ��Ȱ��
        if (activeUIs.ContainsKey(uiName))
        {
            var cached = activeUIs[uiName] as T;
            if (cached == null)
            {
                Debug.LogError($"[UIManager.OpenUI] activeUIs�� {uiName} Ÿ�� ����ġ or null!");
            }
            else
            {
                cached.Open();
                return cached;
            }
        }

        GameObject prefab = LoadPrefab(uiName);
        if (prefab == null)
        {
            Debug.LogError($"[UIManager.OpenUI] Resources�� UI ������ ����: UI/{uiName}");
            return null;
        }

        var baseUi = prefab.GetComponent<BaseUI>();
        if (baseUi == null)
        {
            Debug.LogError($"[UIManager.OpenUI] �����տ� BaseUI �Ļ� ������Ʈ�� ����: {uiName}");
            return null;
        }

        Transform parent = GetParentByType(baseUi.UIType);
        GameObject go = Instantiate(prefab, parent);

        T ui = go.GetComponent<T>();
        if (ui == null)
        {
            Debug.LogError($"[UIManager.OpenUI] �ν��Ͻ��� ���ϴ� ������Ʈ ����: {typeof(T).Name}");
            return null;
        }

        ui.Open();
        ui.Init(gameManager, this);
        activeUIs[uiName] = ui;

        if (ui.UIType == UIType.Popup)
        {
            if (popupBlockRay != null) popupBlockRay.enabled = true;
        }
        else if (ui.UIType == UIType.Window)
        {
            if (windowBlockRay != null) windowBlockRay.enabled = true;
        }

        return ui;
    }

    public void CloseUI(string uiName)
    {
        if (!activeUIs.TryGetValue(uiName, out var ui) || ui == null)
        {
            Debug.LogWarning($"[UIManager.CloseUI] {uiName} is not active or already null");
            return;
        }

        ui.Close();
        if (ui.UIType != UIType.Fixed)
        {
            Destroy(ui.gameObject);
            activeUIs.Remove(uiName);
        }

        if (ui.UIType == UIType.Popup)
        {
            if (popupBlockRay != null) popupBlockRay.enabled = false;
        }
        else if (ui.UIType == UIType.Window)
        {
            if (windowBlockRay != null) windowBlockRay.enabled = false;
        }
    }

    private GameObject LoadPrefab(string uiName)
    {
        if (!loadedPrefabs.TryGetValue(uiName, out var prefab) || prefab == null)
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

    public void CloseAllWindowUI()
    {
        // Dictionary ���� �� �ݺ� ���� ����
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


//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UI;

//public class UIManager : MonoBehaviour
//{
//    private GameManager gameManager;

//    [SerializeField] private Transform fixedRoot;
//    [SerializeField] private Transform windowRoot;
//    [SerializeField] private Transform popupRoot;

//    private Image popupBlockRay;
//    private Image windowBlockRay;

//    private Dictionary<string, BaseUI> activeUIs = new();
//    private Dictionary<string, GameObject> loadedPrefabs = new();

//    public void Init(GameManager gameManager)
//    {
//        this.gameManager = gameManager;

//        foreach (var ui in fixedRoot.GetComponentsInChildren<BaseUI>(true))
//        {
//            ui.Init(gameManager, this);
//            ui.gameObject.SetActive(false);
//        }

//        var mainUI = fixedRoot.GetComponentInChildren<MainUI>(true);
//        if (mainUI != null)
//        {
//            mainUI.gameObject.SetActive(true);
//            mainUI.Open();
//        }

//        popupBlockRay = popupRoot.GetComponent<Image>();
//        windowBlockRay = windowRoot.GetComponent<Image>();
//    }

//    public T OpenUI<T>(string uiName) where T : BaseUI
//    {
//        if (activeUIs.ContainsKey(uiName))
//        {
//            activeUIs[uiName].Open();
//            return activeUIs[uiName] as T;
//        }

//        GameObject prefab = LoadPrefab(uiName);
//        Transform parent = GetParentByType(prefab.GetComponent<BaseUI>().UIType);
//        GameObject go = Instantiate(prefab, parent);

//        T ui = go.GetComponent<T>();
//        ui.Open();
//        ui.Init(gameManager, this);
//        activeUIs.Add(uiName, ui);

//        if (ui.UIType == UIType.Popup)
//        {
//            popupBlockRay.enabled = true;
//        }
//        else if (ui.UIType == UIType.Window)
//        {
//            windowBlockRay.enabled = true;
//        }

//        return ui;
//    }

//    public void CloseUI(string uiName)
//    {
//        if (activeUIs.TryGetValue(uiName, out var ui))
//        {
//            ui.Close();
//            if (ui.UIType != UIType.Fixed)
//            {
//                Destroy(ui.gameObject);
//                activeUIs.Remove(uiName);
//            }

//            if (ui.UIType == UIType.Popup)
//            {
//                popupBlockRay.enabled = false;
//            }
//            else if (ui.UIType == UIType.Window)
//            {
//                windowBlockRay.enabled = false;
//            }
//        }
//    }

//    private GameObject LoadPrefab(string uiName)
//    {
//        if (!loadedPrefabs.TryGetValue(uiName, out var prefab))
//        {
//            prefab = Resources.Load<GameObject>($"UI/{uiName}");
//            loadedPrefabs[uiName] = prefab;
//        }

//        return prefab;
//    }

//    private Transform GetParentByType(UIType type)
//    {
//        return type switch
//        {
//            UIType.Fixed => fixedRoot,
//            UIType.Window => windowRoot,
//            UIType.Popup => popupRoot,
//            _ => windowRoot,
//        };
//    }

//    public void CloseAllWindowUI()
//    {
//        foreach (var uiName in activeUIs.Keys.ToList())
//        {
//            UIType type = activeUIs[uiName].UIType;

//            if (type == UIType.Window)
//            {
//                CloseUI(uiName);
//            }
//        }
//    }
//}
