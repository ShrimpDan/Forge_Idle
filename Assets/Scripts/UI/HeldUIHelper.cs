using System.Collections.Generic;
using UnityEngine;

public class HeldUIHelper : MonoBehaviour
{
    public static HeldUIHelper Instance { get; private set; }

    [Header("활성/비활성화할 Check 아이콘 리스트")]
    [SerializeField] private List<GameObject> checkIcons;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        UpdateCheckIcons();
    }

    /// <summary>
    /// 보류 제자가 있으면 아이콘들을 전부 활성화, 없으면 비활성화
    /// </summary>
    public void UpdateCheckIcons()
    {
        bool hasHeld = GameManager.Instance != null &&
                       GameManager.Instance.HeldCandidates != null &&
                       GameManager.Instance.HeldCandidates.Count > 0;

        foreach (var icon in checkIcons)
        {
            if (icon != null)
                icon.SetActive(hasHeld);
        }
    }
}
