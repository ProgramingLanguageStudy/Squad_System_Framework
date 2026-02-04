# 설계 관련 정리: 런타임 데이터, SRP, Model–View 연결

> 런타임 데이터 관리·단일 책임 원칙·로직과 UI 연결에 대한 의견을 정리한 문서입니다.

---

## 1. 런타임 데이터 관리

### 1.1 정의 vs 런타임

- **정의(설계) 데이터**: 퀘스트 목표, 보상, 조건 등 “설계” → ScriptableObject 등 에셋으로 보관.
- **런타임 데이터**: “지금 어떤 퀘스트를 수락했는지”, “각 목표 진행도” 등 → 플레이 중에만 의미 있고 세션/저장마다 달라지는 값.

“진행 중인 퀘스트”는 **정의(QuestData) + 진행 상태(수락 여부, 진행도)** 를 합친 **런타임 상태**로 보는 것이 맞다.

### 1.2 관리 원칙

- **한 곳에서만 소유·접근 (Single source of truth)**  
  예: 진행 중 퀘스트 목록·진행도는 QuestManager가 전부 갖고, 다른 코드는 매니저 API·이벤트로만 접근.
- **정의와 런타임 상태 분리**  
  진행도·수락 여부는 에셋(SO)에 직접 쓰지 않고, 매니저 쪽 일반 C# 구조(List, 클래스 등)에 두는 편이 안전. (SO 수정 시 에디터 반영·저장 설계 이슈 방지)
- **저장/로드**  
  나중에 세이브를 넣을 때는 “저장할 런타임 데이터”만 DTO로 직렬화하고, 로드 시 매니저가 그 DTO를 읽어 상태를 복원하는 방식이 일반적.

---

## 2. 단일 책임 원칙(SRP)과 역할 분리

### 2.1 SRP가 말하는 “책임”

- **한 클래스는 “변경되는 이유”가 하나여야 한다** (한 축으로만 바뀐다).
- “기능이 여러 개”라서 무조건 쪼개는 것이 아니라, **그 기능들이 같은 이유로 바뀌는지**가 기준.

### 2.2 퀘스트에 적용

- **한 클래스에 두는 것이 자연스러운 것**  
  진행 중 퀘스트 목록 관리, 수락, 진행도 갱신, 완료 처리 → 모두 “퀘스트 규칙/상태”라는 한 축으로 바뀜 → **QuestManager 하나**에 두는 것이 합리적.
- **이미 분리하는 것이 좋은 것**  
  UI에 띄우기 → **QuestUI**가 “표시”만 담당. QuestManager는 “데이터·로직”만 담당.
- **나중에 분리할 만한 경우**  
  세이브 포맷이 복잡해지면 QuestSaveHandler, 조건/보상 계산이 매우 복잡해지면 별도 서비스 클래스로 분리 검토.

### 2.3 요약

- **“다른 역할 = 무조건 다른 클래스”가 아니라 “변경 이유(축)가 다르면 다른 클래스”**로 보는 것이 실무에 맞음.
- 퀘스트에서는 **상태 + 수락/완료 로직 = QuestManager**, **표시 = QuestUI**로만 나눠도 SRP를 잘 지키는 것.

---

## 3. Model–View 연결과 싱글톤

### 3.1 현재 구조 (요약)

- **Model 쪽**: QuestManager, DialogueManager, InventoryManager 등이 Singleton으로 존재. 상태·로직 보유.
- **View 쪽**: QuestUI, DialogueUI, InventoryUI 등이 **직접** `QuestManager.Instance`, `DialogueSystem.Instance`, `GameEvents.OnQuestUpdated` 등을 참조·구독.
- **연결 방식**: View가 전역 Instance와 정적 이벤트에 직접 의존. “연결을 한 곳에서 명시적으로 한다”는 레이어는 없음 (PlayScene은 입력·플레이어·UI 활성화는 묶지만, “QuestUI에게 QuestManager를 넘겨준다” 같은 주입은 하지 않음).

### 3.2 잘 된 점

- 로직(Model)과 표시(View)는 **클래스 단위로는 분리**되어 있음 (QuestManager vs QuestUI).
- 이벤트(OnQuestUpdated 등)로 “상태가 바뀌었을 때 UI 갱신”이라는 **방향**은 명확함.
- 게임은 동작하고, 프로토타입·포트폴리오 규모에서는 실용적.

### 3.3 아쉬운 점 (이상적인 관점)

- **연결이 “암묵적”**  
  QuestUI가 `QuestManager.Instance`를 직접 부르므로, “QuestUI는 QuestManager에 의존한다”가 코드만 봐도 드러나지만, “누가, 어디서 이 둘을 이어주기로 했는지”는 한 곳에 정리되어 있지 않음.
- **테스트·교체가 어려움**  
  UI 단위 테스트 시 Manager를 Mock으로 바꾸려면 QuestUI가 Instance를 쓰는 한 어렵고, “연결 지점”을 한 곳으로 모으지 않으면 리팩터 비용이 커짐.
- **의존성 방향**  
  View → Singleton Model로만 가므로 “역방향 의존”은 없지만, View가 **구체 타입(QuestManager)** 과 **전역 접근**에 묶여 있음.

즉, **“로직과 UI의 연결”이 “잘 되어 있다”기보다는 “동작은 하되, 전역 싱글톤에 의존하는 암묵적 연결”**에 가깝다.

### 3.4 더 이상적인/합리적인 방향

- **연결을 “한 곳”에서 명시적으로 하기**  
  - 예: PlayScene(또는 Bootstrap)이 `QuestManager`, `QuestUI` 참조를 갖고,  
    - **옵션 A**  
      QuestUI에 `void SetQuestModel(IQuestProvider provider)` 같은 메서드를 두고, PlayScene에서 `_questUI.SetQuestModel(QuestManager.Instance);` 로 한 번만 연결.  
    - **옵션 B**  
      QuestPresenter 같은 얇은 클래스를 두고, Presenter가 Model과 View 참조를 받아서 “Model에서 읽어 View에 넘긴다 / View 입력 시 Model 호출”만 담당.
- **View는 “인터페이스”에만 의존하게 하기**  
  QuestUI가 `QuestManager` 구체 타입이 아니라 `IQuestProvider`(GetActiveQuests, 이벤트만 노출)에 의존하면, 테스트 시 Mock 구현으로 교체하기 쉬움.
- **싱글톤 자체를 없애는 것이 목표는 아님**  
  Manager가 싱글톤으로 존재하는 것은 유지해도 됨. 중요한 것은 **“View가 전역 Instance를 직접 부르는가”** vs **“한 곳(씬/부트스트랩)에서 Manager를 View나 Presenter에 넘겨주는가”**.  
  후자면 “Model과 View의 연결”이 명시적이고, 나중에 테스트·교체 시 유리함.

### 3.5 정리

- **Model과 View의 “연결”**이란:  
  “누가, 어디서, 어떤 방식으로 Model과 View를 이어줄지”를 **한 곳에서 명시적으로 결정**하는 것이 이상적.
- **현재는**  
  전부 싱글톤으로 처리되어 **연결은 동작하지만 암묵적**이고, View가 전역에 직접 의존함.
- **합리적인 다음 단계**  
  - 퀘스트만이라도:  
    - QuestManager는 그대로 두고,  
    - QuestUI가 `Instance` 대신 “퀘스트 목록/이벤트를 주는 객체”를 주입받도록 바꾸거나,  
    - 또는 QuestPresenter를 도입해 “연결 책임”만 Presenter에 두기.  
  - 포트폴리오에서는 **“현재는 싱글톤으로 단순하게 했고, 더 나아가면 주입/Presenter로 연결을 명시하겠다”**라고 설계 의도를 문서로 남겨 두는 것만으로도 설계 이해도를 보여 줄 수 있음.

---

## 4. 퀘스트 시스템에서 M / V / P에 해당하는 기능

MVP 기준으로 퀘스트 쪽 역할을 나누면 아래와 같이 보는 것이 좋다.

### Model (M) — 상태 + 도메인 로직

- **상태 보유**
  - 진행 중인 퀘스트 목록 (`_activeQuests`)
  - 각 퀘스트·태스크의 진행도 (현재는 `QuestData`/`QuestTask` 필드에 있음; 이상적으로는 런타임 전용 구조로 분리 가능)
- **도메인 로직**
  - 퀘스트 수락 (`AcceptQuest`) — 목록 추가, 수집 퀘스트 소급 적용
  - 퀘스트 완료 (`CompleteQuest`) — 아이템 차감, 목록 제거, 플래그 설정
  - 진행도 갱신 (`ProcessQuestProgress`) — 목표 ID/수량 신호 받아서 `UpdateProgress`, 완료 여부 판단
- **읽기 API**
  - `GetActiveQuests()` — 진행 중인 퀘스트 목록 조회
- **알림**
  - 상태가 바뀔 때 인스턴스 이벤트 `OnQuestUpdated` 발행 (Presenter가 구독)

대화창에서 “퀘스트 수락/완료” 버튼을 눌렀을 때의 **처리 로직**(수락 시 플래그 설정·퀘스트 추가, 제출 시 완료·완료 대사 전환)은 “퀘스트 도메인”에 가깝이 두면 Model 쪽에 둘 수 있고, “대화 UI 이벤트를 퀘스트에 연결한다”는 행위만 Presenter나 씬 코디네이터에 둘 수 있다.

---

### View (V) — 표시 + 최소한의 입력

- **표시**
  - 퀘스트 트래커 패널/텍스트 표시·숨김 (`_panel`, `_logText`)
  - “보여줄 문자열” 또는 “보여줄 목록”을 **받아서** 그대로 그리기만 함 (예: `SetQuestDisplayText(string)` 또는 `DisplayQuests(IReadOnlyList<QuestDisplayItem>)`)
- **입력 (현재는 거의 없음)**
  - 나중에 “전체 목록 패널” 토글(`Toggle`), “포기” 버튼 등이 생기면 → 버튼 클릭 시 Presenter에만 알림 (예: `OnAbandonClicked(questId)`), View는 어떤 Manager도 모름

View가 **하지 말아야 할 것**: `QuestModel`/Manager 접근, 이벤트 구독, `QuestData`를 직접 순회하며 문자열 조합. 즉 “무엇을 보여줄지”를 결정하지 않고, “받은 내용을 화면에 반영”만 담당.

---

### Presenter (P) — Model과 View의 연결

- **Model → View**
  - Model의 “퀘스트 갱신” 이벤트(또는 `OnQuestUpdated`)를 **구독**
  - 갱신 시 Model에 `GetActiveQuests()` 호출 → 받은 데이터를 **표시용 형태로 변환** (예: 제목, 태스크 설명, 진행도 문자열 리스트 또는 `QuestDisplayItem` DTO)
  - 변환 결과를 View에 전달 (예: `_view.SetQuestDisplayText(text)` 또는 `_view.DisplayQuests(items)`)
- **View → Model (입력이 생겼을 때)**
  - View에서 “포기” 버튼 클릭 등이 오면 → Presenter가 Model의 해당 API 호출 (예: `AbandonQuest(questId)`), View는 Model을 직접 부르지 않음
- **선택: 대화 → 퀘스트 연결**
  - “대화창에서 수락/완료 버튼 눌렀을 때” **이벤트를 받아서** Model의 `AcceptQuest` / `CompleteQuest`를 호출하는 역할은 PlayScene(코디네이터)에서 담당. “언제 수락/완료할지”는 씬에서, “어떻게 수락/완료할지”는 M에 두는 형태.

**실제 스크립트 이름 (퀘스트 MVP, 싱글톤 없음)**  
- **Model**: `QuestModel.cs` — 상태·수락/완료/진행도 로직만. **다른 시스템(인벤토리·플래그)에 의존하지 않음.** 아이템 차감·플래그 설정은 조율(PlayScene)에서 처리. `IActiveQuestProvider` 구현.  
- **View**: `QuestView.cs` — `SetDisplayText(string)`, `SetPanelActive(bool)`만 제공.  
- **Presenter**: `QuestPresenter.cs` — `Initialize(QuestModel, QuestView)` 후 Model 이벤트 구독 → 표시용 문자열로 변환 → View에 전달.  
- **인터페이스**: `QuestInterfaces.cs` — `IActiveQuestProvider`(QuestDialogueQueries·DialogueCoordinator에서 사용).

정리하면:

| 구분 | 담당 |
|------|------|
| **M** | 진행 중 퀘스트 목록·진행도, 수락/완료/진행도 갱신 로직, GetActiveQuests, OnQuestUpdated 발행 |
| **V** | 패널/텍스트 표시, 받은 문자열 또는 목록을 그리기, (있으면) 버튼 클릭만 Presenter에 전달 |
| **P** | OnQuestUpdated 구독 → Model에서 데이터 읽기 → 표시용으로 변환 → View에 전달; (있으면) View 입력 시 Model 호출, 대화 이벤트→퀘스트 호출 연결 |

---

*이 문서는 대화 내용을 바탕으로 정리된 설계 의견입니다. 프로젝트 규모가 커지거나 테스트·유지보수 요구가 늘면 3.4의 방향을 단계적으로 적용하는 것을 권장합니다.*
