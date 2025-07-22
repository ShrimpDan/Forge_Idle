using UnityEngine;

public class GoldChangeEffectManager : MonoBehaviour
{
    public static GoldChangeEffectManager Instance { get; private set; }

    [SerializeField] private GoldChangeEffect effectPrefab;
    [SerializeField] private RectTransform spawnAnchor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 골드 변화 이펙트를 지정된 UI 위치에서 재생
    /// </summary>
    public void ShowGoldChange(int amount)
    {
        if (amount == 0) return;

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, spawnAnchor.position);

        var effect = Instantiate(effectPrefab);
        effect.Play(amount, screenPos);
    }
}
