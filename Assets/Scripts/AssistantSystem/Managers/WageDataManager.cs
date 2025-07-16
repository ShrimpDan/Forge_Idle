using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 급여 데이터(wage_data.json)를 로드하고, 키값으로 접근하거나 랜덤 급여/비용을 반환하는 매니저 클래스.
/// </summary>
public class WageDataManager : MonoBehaviour
{
    public static WageDataManager Instance { get; private set; }

    private Dictionary<string, WageData> wageDataDict;

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
    }

    /// <summary>
    /// wage_data.json 파일을 Resources 폴더에서 불러와 Dictionary로 변환
    /// </summary>
    private void LoadData()
    {
        wageDataDict = new Dictionary<string, WageData>();

        TextAsset json = Resources.Load<TextAsset>("wage_data"); // Assets/Resources/wage_data.json

        if (json == null)
        {
            Debug.LogError("wage_data.json 파일이 Resources 폴더에 없습니다.");
            return;
        }

        WageDataList dataList = JsonUtility.FromJson<WageDataList>(json.text);

        foreach (var wage in dataList.Items)
        {
            if (!wageDataDict.ContainsKey(wage.Key))
            {
                wageDataDict.Add(wage.Key, wage);
            }
        }

        Debug.Log($"[WageDataManager] 급여 데이터 {wageDataDict.Count}개 로드 완료");
    }

    /// <summary>
    /// 키를 기반으로 WageData 반환
    /// </summary>
    public WageData GetByKey(string key)
    {
        if (wageDataDict != null && wageDataDict.TryGetValue(key, out WageData data))
        {
            return data;
        }

        Debug.LogWarning($"[WageDataManager] 해당 키의 급여 데이터를 찾을 수 없습니다: {key}");
        return null;
    }

    /// <summary>
    /// 시급을 min~max 사이에서 랜덤 반환
    /// </summary>
    public int GetRandomWage(string key)
    {
        var data = GetByKey(key);
        if (data == null) return 0;

        return Random.Range(data.minWage, data.maxWage + 1);
    }

    /// <summary>
    /// 영입 비용을 min~max 사이에서 랜덤 반환
    /// </summary>
    public int GetRandomRecruitCost(string key)
    {
        var data = GetByKey(key);
        if (data == null) return 0;

        return Random.Range(data.minRecruitCost, data.maxRecruitCost + 1);
    }

    /// <summary>
    /// 재고용 비용을 min~max 사이에서 랜덤 반환
    /// </summary>
    public int GetRandomRehireCost(string key)
    {
        var data = GetByKey(key);
        if (data == null) return 0;

        return Random.Range(data.minRehireCost, data.maxRehireCost + 1);
    }
}
