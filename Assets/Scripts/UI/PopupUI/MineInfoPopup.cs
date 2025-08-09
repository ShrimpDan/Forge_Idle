using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MineInfoPopup : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI fameText;

    [Header("Stat Texts")]
    [SerializeField] private TextMeshProUGUI miningSpeedText;
    [SerializeField] private TextMeshProUGUI maxResourceText;

    [Header("Exit Button")]
    [SerializeField] private Button exitBtn;

    private CanvasGroup canvasGroup;

    [NonSerialized] public Action onPopupClosed;

    private static readonly Dictionary<string, string> MineNameMap = new()
    {
        ["mine_copperbronze"] = "구리/청동 광산",
        ["mine_ironsilver"] = "철/은 광산",
        ["mine_goldmithril"] = "금/미스릴 광산",
        ["mine_gem"] = "보석 광산",
    };

    private static readonly Dictionary<string, string> WordMap = new()
    {
        ["mine"] = "광산",
        ["copper"] = "구리",
        ["bronze"] = "청동",
        ["iron"] = "철",
        ["silver"] = "은",
        ["gold"] = "금",
        ["mithril"] = "미스릴",
        ["gem"] = "보석",
    };

    private void Awake()
    {
        if (exitBtn != null)
            exitBtn.onClick.AddListener(Close);

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Show()
    {
        SetMineInfo();

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        transform.localScale = Vector3.one * 0.8f;
        transform.DOScale(1.0f, 0.25f).SetEase(Ease.OutBack);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.2f);
        }
    }

    public void Close()
    {
        transform.DOScale(0.8f, 0.2f).SetEase(Ease.InBack);
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, 0.15f).OnComplete(() =>
            {
                onPopupClosed?.Invoke();
                Destroy(gameObject);
            });
        }
        else
        {
            onPopupClosed?.Invoke();
            Destroy(gameObject);
        }
    }

    private void SetMineInfo()
    {
        var forgeManager = GameManager.Instance?.ForgeManager;
        if (forgeManager == null)
        {
            nameText.text = "";
            levelText.text = "";
            fameText.text = "";
            miningSpeedText.text = "";
            maxResourceText.text = "";
            return;
        }

        nameText.text = forgeManager.Name;
        levelText.text = forgeManager.Level.ToString();
        fameText.text = $"(명성치: {forgeManager.TotalFame})";

        var sceneMgr = FindObjectOfType<MineSceneManager>();
        if (sceneMgr == null)
        {
            miningSpeedText.text = "";
            maxResourceText.text = "";
            return;
        }

        var groupsField = sceneMgr.GetType().GetField("mineGroups",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var mineGroups = groupsField?.GetValue(sceneMgr) as List<MineGroup>;
        if (mineGroups == null || mineGroups.Count == 0)
        {
            miningSpeedText.text = "";
            maxResourceText.text = "";
            return;
        }

        System.Text.StringBuilder miningSb = new();
        System.Text.StringBuilder resourceSb = new();

        for (int i = 0; i < mineGroups.Count; i++)
        {
            var group = mineGroups[i];
            float miningAmountSum = 0f;
            float maxResourceSum = 0f;

            foreach (var slot in group.slots)
            {
                var assistant = slot.AssignedAssistant;
                if (assistant == null || assistant.IsFired) continue;

                foreach (var m in assistant.Multipliers)
                {
                    if (m.AbilityName.Contains("자원 채굴량"))
                        miningAmountSum += m.Multiplier;
                    else if (m.AbilityName.Contains("최대 자원량"))
                        maxResourceSum += m.Multiplier;
                }
            }

            string mineName = LocalizeMineName(group.mineKey, i);
            miningSb.AppendLine($"{mineName}: +{(miningAmountSum * 100f):0.#}%");
            resourceSb.AppendLine($"{mineName}: +{(maxResourceSum * 100f):0.#}%");
        }

        miningSpeedText.text = miningSb.ToString();
        maxResourceText.text = resourceSb.ToString();
    }

    private string LocalizeMineName(string key, int index)
    {
        if (string.IsNullOrEmpty(key)) return $"광산 {index + 1}";
        if (MineNameMap.TryGetValue(key, out var ko)) return ko;

        var parts = key.Split('_');
        var tokens = new List<string>();
        foreach (var p in parts)
        {
            if (WordMap.TryGetValue(p, out var t)) tokens.Add(t);
            else tokens.Add(p);
        }

        var joined = string.Join("/", tokens.FindAll(t => t != "광산"));
        return string.IsNullOrEmpty(joined) ? $"광산 {index + 1}" : $"{joined} 광산";
    }
}
