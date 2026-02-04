# 계획: DialogueUI·대화 시스템 SRP 대규모 리팩터링

단일 책임 원칙(SRP)을 기준으로 DialogueUI부터 대화 관련 구조 전체를 나누는 공사 계획서.

---

## 1. 현재 구조에서의 문제 (SRP 위반)

| 구분 | 현재 | 문제 |
|------|------|------|
| **DialogueUI** | `Awake`에서 `DialogueSystem.Instance.EndDialogue()`, `OnQuestPanelButtonClicked()` 직접 호출 | "클릭은 알리기만" 위반. UI가 **누가 처리할지** 알고 있음. |
| **DialogueUI** | `Open(..., showQuestButton, questButtonText, showSubmitButton, submitButtonText)` | 버튼이 2개(끝내기/퀘스트)로 고정, 파라미터 과다. 새 액션 추가 시 시그니처 계속 늘어남. |
| **DialogueSystem** | `_questButtonIsSubmit` 등 버튼 의미 보유 | "세션 생명주기" 외에 **버튼 의미 해석**까지 담당. |
| **Npc** | 퀘스트/제출 표시 여부·텍스트 직접 판단 후 `StartDialogue`에 넘김 | 버튼 목록 생성 규칙이 Npc에 박혀 있음. 한 곳으로 모으면 유지보수·확장이 쉬움. |

목표: **표시·입력 알림 / 세션 생명주기 / 버튼 목록 생성 / 액션 처리**를 각각 한 역할만 갖게 분리.

---

## 2. 목표 아키텍처 (역할 분리)

```
[버튼 목록 생성]  →  [대화 세션]  →  [UI 표시]  →  [입력 알림]
     한 곳              DialogueSystem    DialogueUI      이벤트
  (DialogueManager     StartDialogue     Open(infos)    OnEndClicked
   또는 전용 빌더)      EndDialogue       ShowNext       OnActionClicked(type)
```

### 2.1 역할 정의

| 컴포넌트 | 단일 책임 | 하지 않는 것 |
|----------|-----------|--------------|
| **DialogueUI** | 받은 정보로 **대화창·버튼만 그림**. 다음 문장 넘기기. 클릭 시 **이벤트만 발행**. | DialogueSystem/Quest 호출, 버튼 의미 해석, 버튼 목록 결정 |
| **DialogueSystem** | 대화 **세션 생명주기**(시작/다음/종료), **현재 npcId·타입** 보유. UI 이벤트 구독 후 **타입별 이벤트만 재발행**(예: `OnQuestAcceptRequested(npcId)`). | 퀘스트/플래그 등 실제 비즈니스 로직, 버튼 목록 생성 |
| **버튼 목록 생성** | NPC·대화 데이터·상태를 보고 **이번 대화에 쓸 버튼 정보 목록**만 만듦. | UI 그리기, 클릭 처리 |
| **PlayScene / QuestManager** | `OnQuestAcceptRequested` 등 구독 후 **실제 수락/제출/플래그** 처리. | (기존과 동일) |

### 2.2 데이터 흐름

1. **대화 시작**: Npc 등 → `DialogueManager.GetBestDialogue` + `GetDialogueButtonInfos(npcId, data)` → `DialogueSystem.StartDialogue(name, sentences, npcId, dialogueType, buttonInfos, onComplete)`.
2. **DialogueSystem**: `_ui.Open(name, sentences, buttonInfos)` 호출. 세션 상태(`_currentNpcId` 등) 보관.
3. **DialogueUI**: `Open` 시 버튼은 **버튼 정보 목록**만 받아서 그리기(고정 슬롯 또는 풀링된 `UI_Button`). 클릭 시 `OnEndClicked` / `OnActionClicked(DialogueActionType)` 이벤트만 발행.
4. **DialogueSystem**: UI 이벤트 구독. `OnEndClicked` → `EndDialogue()`. `OnActionClicked(QuestAccept)` → `OnQuestAcceptRequested(_currentNpcId)` 등으로 재발행.
5. **QuestManager 등**: 기존처럼 `OnQuestAcceptRequested` / `OnQuestSubmitRequested` 구독해 처리.

---

## 3. 새로 도입할 타입

### 3.1 액션 타입 (enum)

```csharp
public enum DialogueActionType
{
    End,           // 대화 끝내기
    QuestAccept,   // 퀘스트 수락
    QuestSubmit,   // 퀘스트 제출
    GiveGift,      // (추후) 선물
    // 확장 시 여기만 추가
}
```

### 3.2 버튼 정보 (구조체)

```csharp
public struct DialogueButtonInfo
{
    public string Label;              // 표시 텍스트
    public DialogueActionType Action; // 클릭 시 발행할 액션
    public Sprite Icon;               // 선택. 없으면 null
}
```

- 대화를 **열 때** "이번에 쓸 버튼 목록"을 `List<DialogueButtonInfo>`로 한 번만 넘김.
- UI는 이 목록 길이만큼 버튼을 그리고, 클릭 시 `Action`만 이벤트로 발행.

---

## 4. DialogueUI 구체 변경

### 4.1 제거

- `Awake`에서 `DialogueSystem.Instance` 호출 전부 제거.
- `Open(..., showQuestButton, questButtonText, showSubmitButton, submitButtonText)` 제거.

### 4.2 추가·변경

- **이벤트**
  - `event Action OnEndClicked;`
  - `event Action<DialogueActionType> OnActionClicked;`
- **Open**
  - `Open(string name, string[] sentences, IReadOnlyList<DialogueButtonInfo> buttonInfos)`
  - 규칙: "끝내기"는 목록에 `DialogueActionType.End` 하나로 포함하거나, 별도 고정 버튼 1개로 두고 나머지만 `buttonInfos`로 전달. (권장: 목록에 End 포함해 통일.)
- **버튼 렌더링**
  - 기존 `_endButton`, `_questButton` 대신 **재사용 가능한 버튼 슬롯** 사용:
    - 고정 슬롯 2~3개: `UI_Button[] _actionSlots`, `Open` 시 `buttonInfos` 순서대로 `UI_Button.Initialize(icon, label, () => OnActionClicked?.Invoke(info.Action))`, 미사용 슬롯은 `Initialize(null, null, null)` + `SetActive(false)` 또는 동일한 Reset 패턴.
  - 또는 풀링: 버튼 프리팹 N개 풀, `buttonInfos.Count`만큼 꺼내서 `Initialize` 후 배치, 나머지는 비활성화.
- **초기화/리셋**
  - 재사용 슬롯은 사용 전 `UI_Button` 쪽에서 `icon=null, text=null, RemoveAllListeners` 후 새로 `Initialize` (이미 의견서대로 적용).

---

## 5. DialogueSystem 구체 변경

### 5.1 유지

- `StartDialogue`, `DisplayNextSentence`, `EndDialogue`, `ReplaceContent`, `SetQuestButtonVisible`, `RegisterOnDialogueEndOnce`.
- `OnDialogueEnd`, `OnQuestAcceptRequested`, `OnQuestSubmitRequested` (구독처는 PlayScene/QuestManager 그대로).

### 5.2 변경

- **StartDialogue**
  - 시그니처: `StartDialogue(string speakerName, string[] sentences, string npcId, DialogueType dialogueType, IReadOnlyList<DialogueButtonInfo> buttonInfos, Action onComplete = null)`.
  - 내부: `_ui.Open(speakerName, sentences, buttonInfos)`만 호출. `_questButtonIsSubmit` 같은 플래그 제거.
- **UI 이벤트 구독** (Awake 또는 초기화 시)
  - `_ui.OnEndClicked += EndDialogue;`
  - `_ui.OnActionClicked += OnDialogueUIActionClicked;`
- **OnDialogueUIActionClicked(DialogueActionType type)**
  - `End` → `EndDialogue()`
  - `QuestAccept` → `OnQuestAcceptRequested?.Invoke(_currentNpcId)`
  - `QuestSubmit` → `OnQuestSubmitRequested?.Invoke(_currentNpcId)`
  - `GiveGift` 등 → (추후) 동일 패턴으로 이벤트만 발행.

이렇게 하면 DialogueSystem은 "세션 상태 + UI 이벤트 → 도메인 이벤트"만 담당한다.

---

## 6. 버튼 목록 생성 (단일 책임)

### 6.1 한 곳에 모을 API

- **제안 위치**: `DialogueManager` (이미 NPC/퀘스트/플래그 접근 가능)
- **메서드**: `GetDialogueButtonInfos(string npcId, DialogueData currentDialogue) → List<DialogueButtonInfo>`

**규칙 예시** (순서 고정):

1. Common/Affection일 때만 퀘스트/제출 검사.
2. 제출 가능하면 → `QuestSubmit` + 라벨(제출하기 등) 1개 추가.
3. 그렇지 않고 수락 가능하면 → `QuestAccept` + 라벨 1개 추가.
4. **맨 마지막**에 항상 `End` ("대화 끝내기") 1개 추가.

(선물 등은 나중에 2~3번 사이에 조건부 추가.)

### 6.2 Npc.Interact 변경

- `DialogueData data = DialogueManager.Instance.GetBestDialogue(_npcId);`
- `var buttonInfos = DialogueManager.Instance.GetDialogueButtonInfos(_npcId, data);`
- `DialogueSystem.Instance.StartDialogue(_npcId, sentences, _npcId, data.DialogueType, buttonInfos, onDialogueEnd);`
- Npc는 더 이상 `showQuestButton`/`showSubmitButton`/텍스트를 직접 만들지 않음.

---

## 7. UI_Button 사용 규칙

- 재사용 시마다 **리셋 후 초기화**:
  - `Initialize(null, null, null)` 또는 전용 `Reset()`: `icon = null`, `text = null`(또는 `""`), `onClick.RemoveAllListeners()`.
  - 그 다음 `Initialize(icon, label, () => OnActionClicked?.Invoke(info.Action))`.
- DialogueUI는 "버튼 정보 목록"만 받고, 각 항목에 대해 위 순서로 슬롯/풀 버튼을 채운다.

---

## 8. 단계별 공사 순서 (권장)

| 단계 | 내용 | 비고 |
|------|------|------|
| **1** | `DialogueActionType` enum, `DialogueButtonInfo` 구조체 추가 | Enum.cs 또는 Dialogue 폴더 |
| **2** | `DialogueManager.GetDialogueButtonInfos(npcId, data)` 구현 | 기존 GetCompletableQuestForNpc, GetAvailableQuestForNpc 활용 |
| **3** | DialogueUI: 이벤트 추가, `Open(..., buttonInfos)` 로 받고, Awake에서 DialogueSystem 호출 제거. 버튼은 기존 _endButton/_questButton을 **임시로** 새 시그니처에 맞게만 연결 | 기존 동작 유지 |
| **4** | DialogueSystem: StartDialogue를 buttonInfos 받도록 변경, UI 이벤트 구독, OnDialogueUIActionClicked 분기 구현, _questButtonIsSubmit 제거 | |
| **5** | Npc: GetDialogueButtonInfos 호출해 StartDialogue에 buttonInfos 전달 | |
| **6** | DialogueUI: 버튼을 고정 슬롯(UI_Button 배열) 또는 풀링으로 전환, End도 buttonInfos에 포함해 통일 | UI 구조 정리 |
| **7** | (선택) QuestManager 퀘스트 대사 전환 시 ReplaceContent 등 호출할 때 버튼 목록 다시 넘길지 정책 결정. 필요 시 `SetDialogueButtons(buttonInfos)` 같은 API 추가 | |

---

## 9. 한 줄 요약

- **DialogueUI**: 그리기 + 다음 문장 + **클릭 시 이벤트만 발행** (DialogueSystem 직접 호출 제거).
- **DialogueSystem**: 세션 생명주기 + UI 이벤트 구독 후 **타입별 이벤트 재발행** (npcId 붙여서).
- **버튼 목록**: `DialogueManager.GetDialogueButtonInfos` **한 곳**에서 생성.
- **버튼 표시**: `DialogueButtonInfo` 리스트 + 재사용 `UI_Button` (null / null / RemoveAllListeners 후 Initialize).

이 순서로 적용하면 단일 책임 원칙을 지키는 거대 공사를 단계적으로 진행할 수 있다.
