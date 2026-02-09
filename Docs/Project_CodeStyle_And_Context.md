# Squad_System_Framework — 프로젝트 컨텍스트 & 코드 스타일

> **목적**: 취업용 포트폴리오 게임 제작. AI(에이전트)가 이 프로젝트를 수정·추가할 때 참고할 스타일·규칙을 정리한 문서입니다.

---

## 1. 프로젝트 개요

- **엔진**: Unity (C#)
- **프로젝트명**: Squad_System_Framework
- **역할**: 대화·퀘스트·인벤토리·NPC 상호작용이 있는 프레임워크형 게임
- **주요 시스템**
  - **Player**: 이동(PlayerMover, CharacterController), 애니메이션(PlayerAnimator), 상호작용 감지(PlayerInteractor), 입력(InputHandler)
  - **Dialogue**: DialogueSystem(데이터 로드·선택·재생), DialogueModel·Presenter·View, DialogueData(한 SO에 npcId·타입·조건·lines)
  - **Quest**: QuestSystem, QuestModel(QuestData 필드·생성자), QuestPresenter·View, QuestData(수집·처치·방문)
  - **Interaction**: IInteractable 구현체(Npc, ItemObject 등), PlayerInteractor(SphereCast 감지)
  - **Inventory**: Inventory(Model), InventoryView(View), InventoryPresenter, Slot
  - **Flag**: FlagManager + GameStateKeys(첫 대화, 퀘스트 수락/완료 등 상태 키)
  - **Event**: GameEvents 정적 클래스(OnQuestGoalProcessed, OnQuestUpdated 등)

---

## 2. 네이밍 규칙 (변수·함수·타입)

### 2.1 필드

| 구분 | 규칙 | 예시 |
|------|------|------|
| **private 필드** | `_camelCase` (언더스코어 접두사) | `_detectPoint`, `_moveSpeed`, `_dialogueByNpcId`, `_lastTarget` |
| **Serialized private** | 동일하게 `_camelCase` + `[SerializeField]` | `_npcId`, `_itemData`, `_targetLayerMask` |
| **public 필드 (데이터 클래스)** | `PascalCase` (ScriptableObject/Serializable에서만) | `QuestId`, `Title`, `NpcId`, `Tasks`, `TargetItemId` |

### 2.2 메서드·프로퍼티

| 구분 | 규칙 | 예시 |
|------|------|------|
| **public / private 메서드** | `PascalCase` | `Initialize()`, `Detect()`, `GetBestDialogue()`, `TryInteract()` |
| **public 프로퍼티** | `PascalCase` | `CurrentTarget`, `CanMove`, `IsLoaded`, `Instance` |
| **이벤트** | `On` + `PascalCase` | `OnTargetChanged`, `OnQuestUpdated`, `OnDialogueEnd`, `OnQuestAcceptRequested` |

### 2.3 타입·인터페이스·상수

| 구분 | 규칙 | 예시 |
|------|------|------|
| **클래스·구조체** | `PascalCase` | `Player`, `DialogueManager`, `QuestData`, `GameStateKeys` |
| **인터페이스** | `I` + `PascalCase` | `IInteractable` |
| **enum** | `PascalCase` (값도 PascalCase) | `DialogueType.FirstTalk`, `QuestState.InProgress` |
| **private const** | `PascalCase` | `FirstTalkPrefix`, `QuestPrefix` |
| **static 클래스(유틸/키/이벤트)** | `PascalCase` | `GameStateKeys`, `GameEvents` |

### 2.4 지역 변수·매개변수

- **camelCase** 사용.  
- 예: `Vector3 moveDir`, `string questButtonText`, `var flagManager`, `int numHit`

---

## 3. 코드 스타일 요약

- **한글 주석**: 스크립트 주석은 한글. (`.cursor/rules/script-comments-encoding.mdc` 참고)
- **인코딩**: UTF-8 (`.editorconfig`에 `charset = utf-8` 설정됨)
- **public API 문서화**: 외부에서 쓰는 메서드/프로퍼티에는 `/// <summary>` 사용.
- **Unity 어트리뷰트**
  - Inspector 노출: `[SerializeField] private ...`
  - 구역 구분: `[Header("----- Detection Settings -----")]`
  - 데이터 메뉴: `[CreateAssetMenu(...)]`, `[SerializeReference]` (다형성 리스트 등)
- **이벤트**
  - 인스턴스 이벤트: `public event Action<T> OnXxx;` 후 `OnXxx?.Invoke(...);`
  - 전역 이벤트: `GameEvents` 정적 클래스에 `public static Action<...> OnXxx;`
- **매니저/진입점**
  - 필요한 곳만 `Singleton<T>` 사용. **DialogueSystem**은 싱글톤 없이 씬에 한 개 두거나 조율층에서 주입.
  - 필요 시 `Initialize()` 패턴·`[SerializeField]` 로 의존성 주입(예: Npc에 DialogueSystem 주입).
- **플레이어·컴포넌트·애니메이터 규칙** (Awake 지양, Initialize 주입, State 패턴, AnimatorParams/래퍼)은 **`Docs/Conventions_Components_State_Animator.md`** 에 정리됨.

---

## 4. Model 역할 (상태 + 전환 로직)

- **원칙**: MVP에서 **Model은 자기 상태를 바꾸는 규칙까지 갖는다.** System·Presenter는 "시작해/다음/끝내"처럼 Model의 메서드만 호출하고, 필드를 직접 수정하지 않는다.
- **예시**
  - **DialogueModel**: 상태만 보관(현재 문장 반환). 제어(시작/다음/종료)는 **DialogueSystem**에서.
  - **QuestModel**(퀘스트 런타임): QuestData 필드 보유, 진행도 등 상태는 System이 설정.
- **반대 패턴(피함)**: Model은 데이터만 두고, System이 `model.CurrentAmount = ...`처럼 직접 넣는 방식은 프로젝트 일관성을 위해 사용하지 않는다.

---

## 5. 폴더 구조 (Assets/02_Scripts)

- `0_ScriptableObjects/` — 공용 SO
- `Dialogue/` — 대화 데이터·시스템·Model·Presenter·View
- `Quest/` — 퀘스트 데이터·시스템·QuestModel·Presenter·View
- `Player/` — Player, PlayerMover, PlayerAnimator, PlayerInteractor
- `Interaction/` — 상호작용(Items, Npc 등)
- `Interfaces/` — IInteractable 등
- `Inventory/` — 인벤토리 매니저·UI·슬롯
- `Flag/` — FlagManager, GameStateKeys
- `PlayScene/` — 씬별 진입점(입력·플레이어·대화 연동)
- `UI/` — 공용 UI 스크립트

---

## 6. AI 작업 시 적용 원칙

- **새 C# 스크립트**: 위 네이밍·스타일 준수 (private 필드는 `_camelCase`, 메서드/프로퍼티/이벤트는 표와 동일).
- **주석·summary**: 한글 + public API에는 `/// <summary>` 유지.
- **의존성**: 기존 매니저는 `Singleton<T>.Instance`, `FindFirstObjectByType<T>()`, `Resources.Load<T>()` 등 기존 방식 유지.
- **코드 수정 범위**: 사용자가 "코드 수정해줘", "스크립트 작성해줘" 등으로 **명시적으로 요청한 경우**에만 코드 변경. 의견만 물을 때는 `Docs/`에 정리만 할 수 있음. (`Docs/Conversation_Preference.md` 참고)

---

*이 문서는 프로젝트 이해 및 일관된 코드 스타일 적용을 위해 작성되었습니다. 스타일 추가·변경 시 이 파일을 갱신하면 됩니다.*
