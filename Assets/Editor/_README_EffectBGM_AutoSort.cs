

//이펙트 & BGM 자동 정렬 기능 설명서


//1. 목적

//사운드(SoundData)와 이펙트(EffectData)를 ScriptableObject 형태로 관리하며,  
//지정된 폴더 구조에 따라 자동으로 정렬 및 데이터베이스에 등록합니다.  
//수동 등록의 번거로움을 줄이고 일관된 정렬을 유지합니다.


//2. 폴더 구조

//[사운드 데이터]
//Assets/ScriptableObjects/Sound/
//├── BGM/
//├── SFX/
//└── _DataBase/
//    └── SoundDataBase.asset

//[이펙트 데이터]
//Assets/ScriptableObjects/Effect/
//├── Ambient/
//├── Trainee/
//├── Combat/
//├── Economy/
//├── Production/
//├── UI/
//└── _DataBase/
//    └── EffectDatabase.asset


//3. 자동 등록 방법

//// Unity 메뉴 상단에서 아래 항목 실행

//Tools > Sound > Build Ordered SoundDatabase  
//Tools > Effect > Build Ordered EffectDatabase

//→ 클릭 시, 지정된 폴더 순서대로 ScriptableObject가 자동 탐색 및 정렬되어 Database.asset에 저장됩니다.


//4. 작동 원리

//- 각 폴더 내부의 `.asset` 파일 중 `t:SoundData`, `t:EffectData` 타입만 탐색
//- 폴더 순서를 기준으로 정렬 (수동 이름 정렬 불필요)
//- 중복 등록 방지
//- `AssetDatabase.SaveAssets()`로 즉시 반영


//5. 주의사항

//  → ScriptableObject 파일 이름이 내부 필드(`soundName`, `effectName`)에 자동 반영됨
//- 정렬 순서는 고정된 폴더명 기준
//  → 폴더명을 변경하면 탐색 대상에서 제외될 수 있음
//- 새로 추가한 데이터는 메뉴에서 "Build" 실행 시 반영됨


//6. 정렬 순서 (우선순위)

//[사운드]
//1. BGM
//2. SFX

//[이펙트]
//1. Ambient
//2. Trainee
//3. Combat
//4. Economy
//5. Production
//6. UI

//7. 스크립트 위치

//Assets/Editor/SoundDatabaseAutoLoader.cs  
//Assets/Editor/EffectDatabaseAutoLoader.cs