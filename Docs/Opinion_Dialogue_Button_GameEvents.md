# 의견: 대화 버튼 클릭 — UI 역할 vs GameEvents 처리

"UI_Button은 게임 이벤트에 연결만 하고, 실제 실행은 GameEvents에서 처리하면 되는가?"에 대한 정리.

---

## 1. UI 쪽 역할 (공통 원칙)

- **UI_Button / DialogueUI**는 "누를렀다"만 알리면 된다.
- **어떤 이벤트/콜백을 부를지**는:
  - **권장**: UI는 **인자로 받은 콜백** 또는 **자기 자신이 발행하는 이벤트**(예: `DialogueUI.OnActionClicked`)만 호출.  
    즉 UI는 **GameEvents를 직접 알지 않는다.**
- 이유: UI가 `GameEvents.XXX`를 직접 호출하면, "버튼 클릭이 어디서 처리되는지"가 UI 코드를 열어봐야만 보이고, 테스트/교체 시 UI를 건드리게 된다.  
  "표시 + 입력 알림"만 담당하게 두려면, **중간 계층(DialogueSystem 등)**이 UI 이벤트를 받아서 GameEvents를 발행하는 쪽이 낫다.

정리: **UI의 역할 = "연결만"이 맞고, 그 연결 대상은 "콜백/이벤트"이지, 반드시 GameEvents일 필요는 없다.**  
GameEvents로 갈지 말지는 UI가 아니라 **그 콜백을 등록하는 쪽(예: DialogueSystem)**에서 결정하면 된다.

---

## 2. "경우에 따른 실행"을 어디서 할지

버튼 종류(끝내기, 퀘스트 수락, 제출 등)에 따라 **실제 로직**을 실행하는 곳은 두 가지로 나눌 수 있다.

### 방식 A: GameEvents에서 처리 (이벤트 버스)

- **흐름**:  
  `UI 클릭` → `DialogueUI.OnActionClicked(type)` → **DialogueSystem**이 구독해서  
  `GameEvents.OnDialogueActionRequested?.Invoke(type, npcId)` 발행 →  
  **QuestManager** 등이 `GameEvents.OnDialogueActionRequested` 구독해 type별 분기 처리.
- **장점**
  - 이미 `GameEvents`를 쓰는 프로젝트와 일관됨.
  - 퀘스트 제출 한 번에 반응하는 구독자를 여러 개 두기 쉬움 (퀘스트, 업적, 로그 등).
  - 새 반응 추가 시 PlayScene의 연결 코드를 안 건드려도 됨.
- **단점**
  - 구독자가 숨겨져 있어서 "이 액션 누가 처리하지?"를 찾기 위해 GameEvents 사용처를 검색해야 함.
  - 발행/구독이 많아지면 실행 순서·테스트가 조금 더 신경 쓸 일이 됨.

### 방식 B: DialogueSystem 이벤트 + PlayScene에서 명시적 연결

- **흐름**:  
  `UI 클릭` → `DialogueUI.OnActionClicked(type)` → **DialogueSystem**이 구독해서  
  `OnQuestAcceptRequested(npcId)`, `OnQuestSubmitRequested(npcId)` 등 **자기 이벤트**만 발행 →  
  **PlayScene**(또는 부트스트랩)에서 `DialogueSystem.OnQuestAcceptRequested += QuestManager.Handle...` 처럼 **한 번만** 연결.
- **장점**
  - "이 버튼은 누가 처리한다"가 PlayScene(또는 부트스트랩) 한 곳에 드러남.
  - 테스트 시 DialogueSystem만 모킹해서 이벤트만 주면 됨.
- **단점**
  - 반응하는 시스템이 늘어나면 PlayScene(또는 연결 담당 스크립트)에 구독 코드가 조금씩 늘어남.

---

## 3. 추천

- **UI**:  
  - "게임 이벤트에 연결만" = **콜백/이벤트만 호출**하는 식으로 두는 것이 좋다.  
  - **GameEvents를 UI에서 직접 호출하지 않는다**고 보는 편이 역할 분리가 깔끔함.
- **실제 "경우에 따른 실행"**:
  - **여러 시스템이 같은 액션에 반응해야 하면** → **방식 A (GameEvents)**.  
    DialogueSystem이 UI 이벤트를 받아서 `GameEvents.OnDialogueActionRequested(type, npcId)`만 발행하고, QuestManager 등은 GameEvents 구독으로 처리.
  - **지금처럼 퀘스트 처리 위주이고, "누가 처리하는지"를 코드에서 명확히 보고 싶으면** → **방식 B (DialogueSystem 이벤트 + PlayScene 연결)**.  
    지금 구조와도 잘 맞고, 문서의 SRP 리팩터링 계획과도 동일한 방향이다.

즉, **"실제 실행을 GameEvents에서 처리해도 되는가?"**에는 **된다**고 보면 되고,  
**반드시 GameEvents여야 하는 건 아니며**,  
프로젝트에 이벤트 버스를 이미 쓰고 있고 "한 액션에 여러 구독자"를 두고 싶으면 GameEvents로 처리하고,  
단순하게 "DialogueSystem 이벤트 → PlayScene에서 QuestManager에만 연결"로 가도 충분하다.

---

## 4. 한 줄 요약

- **UI**: 버튼은 **콜백/이벤트만** 호출. GameEvents 직접 호출은 하지 않는 편이 좋음.
- **실행 위치**: **GameEvents에서 처리** (방식 A)와 **DialogueSystem 이벤트 + PlayScene 연결** (방식 B) 둘 다 괜찮음.  
  여러 시스템이 반응하면 A, 처리 주체를 코드에서 뚜렷이 두고 싶으면 B.
