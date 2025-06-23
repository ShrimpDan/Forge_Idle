//📘 [성격 데이터 관리 가이드]

//이 문서는 제자 시스템의 성격 관련 ScriptableObject를 관리하고 확장하는 방법에 대한 가이드입니다.

//──────────────────────────────────────────────

//📌 1. 성격(Personality) 추가 방법

//① Assets/ScriptableObjects/TraineeSystem/Personalities/Tiers(숫자)/ 폴더로 이동
//② 새로운 ScriptableObject 생성
//   ▶ 우클릭 → Create → Trainee → Personality
//③ 성격 데이터 입력
//   - ScriptableObject의 이름 : 성격의 영문명 (예: Adventurous ) ( 도전적인 )
//   - Personality Name: 한글 이름(예: 도전적인)
//   - Tier: 숫자(예: 2)
//   - Crafting / Enhancing / Selling Multiplier: 해당 배율 입력

//④ 알맞은 티어 폴더에 저장 (예: Tiers2 폴더)

//──────────────────────────────────────────────

//📌 2. 티어(Tier) 추가 시 필요한 작업

//① Assets/ScriptableObjects/TraineeSystem/Tiers/ 폴더에 새`PersonalityTier 생성  
//   ▶ 이름 예시: `Tier6`

//② `TierLevel` 필드 값 지정 (예: 6)

//③ `Personalities` 리스트에 새로운 성격 SO 파일 추가

//④ Assets/ScriptableObjects/TraineeSystem/PersonalityTierDatabase.asset 열기  
//   ▶ Tiers 리스트에 새로 만든 Tier6.asset 드래그 & 드롭

//✅ **주의:** TierLevel 값은 반드시 중복 없이 입력. (정렬 및 검색 기준됨)

//──────────────────────────────────────────────

//📌 3. PersonalityTierDatabase 설정 요령

//- 필드: `tiers`  
//- 설명: `PersonalityTier`들을 1~5 티어 순으로 보관하는 리스트입니다.  
//- 용도: 게임 시작 시 또는 제자 생성 시 랜덤으로 티어 → 성격을 뽑는 핵심 데이터

//✔️ `tiers` 리스트는 티어 레벨 순서대로 정렬하는 것을 권장합니다.

//──────────────────────────────────────────────

//📌 4. 예시 구조

//- PersonalityData
//  - 이름: Diligent
//  - 티어: 1
//  - 배율: 제작 1.5 / 강화 1.3 / 판매 1.3

//- PersonalityTier
//  - TierLevel: 1
//  - 포함 성격: Diligent, Perfectionist, SocialLeader

//- PersonalityTierDatabase
//  - tiers: [Tier1, Tier2, Tier3, Tier4, Tier5]

//──────────────────────────────────────────────

//🧩 참고

//- ScriptableObject는 에디터에서 직접 드래그 & 드롭으로 연결해야 적용됩니다.
//- 새로운 성격이 추가되면 반드시 해당하는 `PersonalityTier.asset`에도 등록해야 게임에서 인식됩니다.

//──────────────────────────────────────────────

// 최근 작성일: 2025.06.23 오후 5:22
// 작성자: 이희민