using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeldCandidateSaveHandler : ISaveHandler
{
    private readonly GameManager gameManager;
    private static string SavePath => $"{Application.persistentDataPath}/held_candidates.json";

    public HeldCandidateSaveHandler(GameManager gm)
    {
        gameManager = gm;
    }

    [System.Serializable]
    private class HeldCandidateSaveData
    {
        public List<SerializableAssistantInstance> heldList;
    }

    public void Save()
    {
        var serializableList = gameManager.HeldCandidates
            .ConvertAll(AssistantSerializationUtil.ToSerializable);

        var data = new HeldCandidateSaveData { heldList = serializableList };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"[HeldCandidateSaveHandler] 저장 완료: {SavePath}");
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[HeldCandidateSaveHandler] 저장 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<HeldCandidateSaveData>(json);

        if (data != null && data.heldList != null)
        {
            gameManager.HeldCandidates = data.heldList
                .ConvertAll(AssistantSerializationUtil.ToRuntime);

            Debug.Log($"[HeldCandidateSaveHandler] 불러오기 완료: {data.heldList.Count}개 로드됨");
        }
        else
        {
            gameManager.HeldCandidates = new List<AssistantInstance>();
            Debug.LogWarning("[HeldCandidateSaveHandler] 데이터가 없거나 파싱 실패");
        }
    }

    public void Delete()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[HeldCandidateSaveHandler] 저장 파일 삭제 완료");
        }
    }
}
