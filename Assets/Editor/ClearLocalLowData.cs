using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ClearLocalLowData : EditorWindow
{
    // 삭제할 JSON 파일의 이름 패턴.
    // "*" 또는 "*.json"은 해당 경로의 모든 파일을 의미합니다.
    // 특정 파일 이름("saveData.json")도 가능합니다.
    private static string[] jsonFileNamesToDelete = {
        "*.json" // 이제 와일드카드를 올바르게 처리합니다.
    };

    private string appCompanyName;
    private string appProductName;

    private void OnEnable()
    {
        appCompanyName = Application.companyName;
        appProductName = Application.productName;
    }

    [MenuItem("Tools/Clear Game Data/Clear LocalLow JSON Files")]
    public static void ShowWindow()
    {
        ClearLocalLowData window = GetWindow<ClearLocalLowData>("Clear LocalLow Data");
        window.minSize = new Vector2(400, 300);
        // window.Show(); // GetWindow<T>는 기본적으로 Show()를 호출합니다.
    }

    private void OnGUI()
    {
        GUILayout.Label("LocalLow JSON File Deletion", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "이 도구는 Unity 프로젝트의 LocalLow 폴더 내 특정 JSON 파일들을 삭제합니다.\n\n" +
            "삭제할 패턴 목록:\n" + string.Join("\n", jsonFileNamesToDelete) +
            "\n\n주의: 이 작업은 되돌릴 수 없습니다!",
            MessageType.Warning);

        EditorGUILayout.Space();

        appCompanyName = EditorGUILayout.TextField("Company Name:", appCompanyName);
        appProductName = EditorGUILayout.TextField("Product Name:", appProductName);

        EditorGUILayout.Space();

        if (GUILayout.Button("Delete Selected JSON Files"))
        {
            DeleteJsonFiles();
        }
    }

    private void DeleteJsonFiles()
    {
        string localLowPath = Path.Combine(Application.persistentDataPath, "..", "..");
        string gameDataPath = Path.Combine(localLowPath, appCompanyName, appProductName);

        if (!Directory.Exists(gameDataPath))
        {
            Debug.LogWarning($"LocalLow 경로에 게임 데이터 폴더를 찾을 수 없습니다: {gameDataPath}");
            EditorUtility.DisplayDialog("삭제 실패", "게임 데이터 폴더를 찾을 수 없습니다.", "확인");
            return;
        }

        bool deletedAny = false;
        List<string> deletedFiles = new List<string>();
        List<string> failedToDeleteFiles = new List<string>(); // 삭제 시도했지만 실패한 파일 목록

        // 각 패턴에 대해 파일들을 찾아서 삭제합니다.
        foreach (string filePattern in jsonFileNamesToDelete)
        {
            // Directory.GetFiles를 사용하여 패턴에 맞는 모든 파일 경로를 가져옵니다.
            string[] foundFiles = Directory.GetFiles(gameDataPath, filePattern);

            if (foundFiles.Length == 0)
            {
                Debug.Log($"LocalLow에서 '{filePattern}' 패턴에 해당하는 파일을 찾을 수 없습니다.");
                continue; // 다음 패턴으로 넘어갑니다.
            }

            foreach (string filePath in foundFiles)
            {
                string fileName = Path.GetFileName(filePath); // 파일명만 추출
                try
                {
                    File.Delete(filePath);
                    deletedAny = true;
                    deletedFiles.Add(fileName);
                    Debug.Log($"LocalLow에서 파일 삭제 완료: {filePath}");
                }
                catch (System.Exception e)
                {
                    failedToDeleteFiles.Add(fileName); // 삭제 실패한 파일 목록에 추가
                    Debug.LogError($"파일 삭제 중 오류 발생: {filePath}\n{e.Message}");
                }
            }
        }

        // --- 결과 메시지 개선 ---
        if (deletedAny)
        {
            string message = "다음 JSON 파일들이 LocalLow 폴더에서 성공적으로 삭제되었습니다:\n" + string.Join("\n", deletedFiles);
            if (failedToDeleteFiles.Count > 0)
            {
                message += "\n\n다음 파일들은 삭제에 실패했습니다 (권한 문제 등이 원인일 수 있습니다):\n" + string.Join("\n", failedToDeleteFiles);
            }
            EditorUtility.DisplayDialog("삭제 완료", message, "확인");
        }
        else
        {
            string message = "삭제할 JSON 파일을 찾을 수 없었습니다.";
            if (failedToDeleteFiles.Count > 0)
            {
                message += "\n\n다음 파일들은 삭제에 실패했습니다 (권한 문제 등이 원인일 수 있습니다):\n" + string.Join("\n", failedToDeleteFiles);
            }
            EditorUtility.DisplayDialog("삭제 정보", message, "확인");
        }
    }
}