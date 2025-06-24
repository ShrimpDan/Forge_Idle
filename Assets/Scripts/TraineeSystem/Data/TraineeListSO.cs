using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 생성된 제자(TraineeData)들을 리스트 형태로 저장하는 ScriptableObject입니다.
/// 게임 중 생성된 제자들을 임시 저장하거나, 퇴출 등 관리 기능에 활용됩니다.
/// </summary>
[CreateAssetMenu(fileName = "TraineeList", menuName = "Trainee/Trainee List")]
public class TraineeListSO : ScriptableObject
{
    /// <summary>
    /// 현재 생성되어 있는 모든 제자 데이터 목록입니다.
    /// Recruit 시 Add, 삭제 시 Remove 하여 실시간 관리할 수 있습니다.
    /// </summary>
    public List<TraineeData> trainees = new List<TraineeData>();
}