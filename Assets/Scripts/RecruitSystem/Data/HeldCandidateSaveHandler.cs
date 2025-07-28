using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ### 스크립트 설명 ###
// HeldCandidateSaveHandler.cs
// 보류 중인 제자(HeldCandidates)를 JSON 파일로 저장/불러오기/삭제하는 기능을 담당하는 세이브 핸들러입니다.
// GameManager와 AssistantSerializationUtil을 이용해 직렬화 및 역직렬화를 처리합니다.

public class HeldCandidateSaveHandler : ISaveHandler
{
    private readonly GameManager gameManager;
    private static string SavePath => $"{Application.persistentDataPath}/held_candidates.json";

    public HeldCandidateSaveHandler(GameManager gm)
    {
        gameManager = gm;
    }

    // 데이터 저장용 내부 구조체
    [System.Serializable]
    private class HeldCandidateSaveData
    {
        public List<SerializableAssistantInstance> heldList;
    }

    // 보류 제자 목록 저장
    public void Save()
    {
        var serializableList = gameManager.HeldCandidates
            .ConvertAll(AssistantSerializationUtil.ToSerializable);

        var data = new HeldCandidateSaveData { heldList = serializableList };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"[HeldCandidateSaveHandler] 저장 완료: {SavePath}");
    }

    // 저장된 보류 제자 목록 불러오기
    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[HeldCandidateSaveHandler] 저장 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<HeldCandidateSaveData>(json);

        gameManager.HeldCandidates.Clear();

        if (data?.heldList != null)
        {
            var runtimeList = data.heldList.ConvertAll(AssistantSerializationUtil.ToRuntime);
            gameManager.HeldCandidates.AddRange(runtimeList);
            Debug.Log($"[HeldCandidateSaveHandler] 불러오기 완료: {runtimeList.Count}개 로드됨");
        }
        else
        {
            Debug.LogWarning("[HeldCandidateSaveHandler] 데이터가 없거나 파싱 실패");
        }
    }

    // 저장 파일 삭제
    public void Delete()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[HeldCandidateSaveHandler] 저장 파일 삭제 완료");
        }
    }
}
