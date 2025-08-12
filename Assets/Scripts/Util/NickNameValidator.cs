using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class NicknameValidator : MonoBehaviour
{
    private HashSet<string> badWords = new HashSet<string>();

    void Awake()
    {
        LoadBadWordsFile();
    }

    private void LoadBadWordsFile()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "BadWords.txt");
        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(LoadFromAndroid(path));
        }
        else
        {
            LoadBadWords(path);
        }
    }

    private void LoadBadWords(string path)
    {
        if (File.Exists(path))
        {
            string[] words = File.ReadAllLines(path);
            foreach (string word in words)
            {
                badWords.Add(word.Trim());
            }
        }
        else
        {
            Debug.LogError("BadWords.txt 파일을 찾을 수 없습니다.");
        }
    }

    private IEnumerator LoadFromAndroid(string path)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);

        yield return request.SendWebRequest();

        try
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"BadWords.txt 파일을 로드하는 데 실패했습니다. 에러: {request.error}");
            }

            string[] words = request.downloadHandler.text.Split('\n');
            foreach (string word in words)
            {
                badWords.Add(word.Trim());
            }
            Debug.Log("BadWords.txt 파일을 성공적으로 로드했습니다.");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            request.Dispose();
        }
    }

    private bool ContainsBadWord(string input)
    {
        foreach (string badWord in badWords)
        {
            if (input.Contains(badWord))
                return true;
        }

        return false;
    }

    public bool IsValidNickName(string nickname, out string resultMessage)
    {
        if (nickname.Length < 2 || nickname.Length > 10)
        {
            resultMessage = "닉네임은 2~10글자여야 합니다.";
            return false;
        }

        if (!Regex.IsMatch(nickname, @"^[a-zA-Z가-힣]+$"))
        {
            resultMessage = "사용할 수 없는 기호가 포함되어있습니다.";
            return false;
        }

        if (ContainsBadWord(nickname))
        {
            resultMessage = "부적절한 언어가 포함되어있습니다.";
            return false;
        }

        resultMessage = "사용가능한 닉네임입니다.";
        return true;
    }
}
