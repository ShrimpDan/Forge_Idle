using System;
using System.Collections.Generic;
using UnityEngine;

// HeldCandidateSaveData.cs
// 보류 중인 제자(AssistantInstance)의 리스트를 저장하기 위한 데이터 구조입니다.
// 세이브 시스템에 의해 직렬화되어 저장됩니다.

[Serializable]
public class HeldCandidateSaveData
{
    public List<AssistantInstance> heldList = new();
}
