# 리팩터링 로그: 인벤토리·퀘스트 (Data / Model 분리 및 구조 정리)

> 오늘 작업한 인벤토리·퀘스트 쪽 변경과 설계 결정을 정리한 문서입니다.

---

## 1. 인벤토리 시스템

### 1.1 명칭 통일

| 이전 | 이후 | 비고 |
|------|------|------|
| InventorySlot (UI 컴포넌트) | **ItemSlot** | 슬롯 한 칸 UI |
| InventorySlotData / ItemSlotModel (데이터) | **ItemSlotModel** | 슬롯 한 칸 데이터 (Index, Item, Count) |
| InventorySlot.prefab | ItemSlotView.prefab → **ItemSlotView** 루트명 | 프리팹·씬 그룹명 정리 |

- **ItemSlotModel**: 별도 스크립트 `ItemSlotModel.cs`로 분리.

### 1.2 아이템: Data / Model 분리

- **ItemModel** 추가 (`Interaction/Items/ItemModel.cs`)
  - 런타임용 모델. 현재는 **ItemData를 감싼 임시 구현**.
  - `Data`, `ItemId`, `ItemName`, `Icon`, `Description`, `IsStackable`, `MaxStack` 노출.
- **ItemSlot** (데이터): `ItemData` 대신 **ItemModel** 보관.
- 인벤토리 내부: `AddItem(ItemData)` 시 `new ItemModel(itemData)` 생성해 슬롯에 저장. 에셋(ItemData)은 읽기 전용으로만 사용.

### 1.3 Presenter / View 연결 방식

- **Model·View는 인스펙터에서만 할당.** PlayScene에서 `Initialize(model, view)` 호출 제거.
- Presenter `Awake`에서 Model/View 미할당 시 `Debug.LogWarning` 방어 코드 추가.

### 1.4 인벤토리 이벤트

- **단일 이벤트**: `OnItemChanged` 제거, **`OnItemChangedWithId(itemId, totalCount)`** 만 유지.
  - UI: Presenter가 구독 후 인자 무시하고 `RefreshView()` (→ 이후 `RefreshSlot`만 사용하도록 변경).
  - 퀘스트: PlayScene이 구독 후 `GameEvents.OnQuestGoalProcessed` 발행.
- **슬롯 단위 갱신**: `ItemSlot`에 **Index** 추가. 변경 시 **`OnSlotChanged(ItemSlotModel slot)`** 한 슬롯만 전달.
  - 발행은 “아이템 추가/스왑/제거될 때마다” 유지. 필터(이 아이템이 활성 퀘스트와 관련 있는지)는 **구독 쪽(조율 또는 QuestModel)**에서만 수행해 Inventory는 퀘스트를 모름.

### 1.5 View 갱신 방식

- **RefreshSlot(ItemSlotModel)** 하나로 통일. 전체 갱신은 Presenter에서 `GetSlots()` 한 번 받아온 뒤 루프로 `RefreshSlot(slots[i])` 호출.
- **ItemSlot (UI)**: 자기 **ItemSlotModel**을 들고 있음. `SetModel(ItemSlotModel)` 로 설정 후 표시 갱신. 툴팁·드래그는 `_model` 기준.

### 1.6 ItemSlot (UI) ↔ InventoryView 결합 제거

- ItemSlot이 InventoryView를 직접 참조하지 않도록 **이벤트로 분리**.
  - ItemSlot: **`OnSwapRequested`** (fromIndex, toIndex) 발행.
  - InventoryView: 슬롯 생성 시 `slot.OnSwapRequested += RequestSwap` 구독.

### 1.7 인벤토리 패널 토글

- **InventoryView**가 끄고 켜는 대상: 자기 게임오브젝트가 아니라 **`_inventoryUIPanel`** (SerializeField).
  - `IsPanelActive` 프로퍼티로 “패널이 켜져 있는지” 노출. PlayScene에서 커서/이동 제어에 사용.
- 슬롯 생성: 코루틴 제거 → **한 프레임에 전부 생성** (`CreateSlots()`). 패널이 먼저 꺼질 때 코루틴이 중단돼 30개 미만이 되는 문제 방지.

### 1.8 툴팁·드래그 아이콘

- **현재**: 툴팁은 `TooltipUI.Instance` 싱글톤, 드래그 아이콘은 InventoryView가 보유 후 슬롯에 `SetDragIcon` 주입. 구조 변경 없음.
- **향후**: 툴팁은 이벤트 기반으로 전환 가능 (발행만 하고, 실제 표시는 한 곳에서 구독).

---

## 2. 퀘스트 시스템

### 2.1 Quest Data / Model 분리

- **QuestData** (ScriptableObject): **정의 전용**. 퀘스트 ID, 제목, Tasks(목표 정의). 런타임에 **수정하지 않음**.
- **ActiveQuest** / **ActiveQuestTask** 추가 (런타임 전용).
  - **ActiveQuest**: `QuestId`, `Title`, `Tasks`(List<ActiveQuestTask>), `IsAllTasksCompleted()`.
  - **ActiveQuestTask**: `Description`, `TargetAmount`, `CurrentAmount`, `TargetId`, `IsAccumulate`, `RequiresItemDeduction`, `IsCompleted`, `UpdateProgress(id, amount)`.
  - **ActiveQuest.CreateFrom(QuestData)**: 에셋을 읽기만 하고, ActiveQuest + ActiveQuestTask 리스트를 새로 만들어 반환. Gather/Kill/Visit 타입별로 `TargetId`, `IsAccumulate`, `RequiresItemDeduction` 설정.

### 2.2 QuestModel 변경

- **보관**: `List<QuestData>` → **`List<ActiveQuest>`**.
- **AcceptQuest(QuestData)**: `ActiveQuest.CreateFrom(questData)` 생성 후 리스트에 추가.
- **OnQuestUpdated**: **`Action<ActiveQuest>`**.
- **GetActiveQuests()**: **`IReadOnlyList<ActiveQuest>`** 반환.
- **SetGatherProgress** / **ProcessQuestProgress** / **CompleteQuest**: 모두 ActiveQuest·ActiveQuestTask 기준으로 동작.

### 2.3 연동부 수정

- **IActiveQuestProvider**: `GetActiveQuests()` 반환 타입을 **`IReadOnlyList<ActiveQuest>`** 로 변경.
- **QuestDialogueQueries.GetCompletableQuestForNpc**: **`out ActiveQuest quest`**.
- **QuestPresenter**: `RefreshView(ActiveQuest _)` 로 시그니처 변경. UI는 기존처럼 `GetActiveQuests()`로 갱신.
- **PlayScene.HandleQuestCompleteRequested**: 아이템 차감 시 `GatherTask` 타입 체크 대신 **`task.RequiresItemDeduction`** 이면 `RemoveItem(task.TargetId, task.TargetAmount)` 호출.

---

## 3. 설계 결정 요약

| 주제 | 결정 |
|------|------|
| 퀘스트 진행도: “언제” 이벤트 발행 | **인벤토리에 아이템이 추가/변경될 때** 발행. “획득 행위”가 아닌 “인벤토리 상태 변경” 한 곳만 알리면 됨. |
| 발행 줄이기 vs 구독 쪽 필터 | **발행은 유지**, “이 itemId가 활성 퀘스트와 관련 있는지” 체크는 **구독 쪽(조율 또는 QuestModel)**에서만 수행. Inventory는 퀘스트를 알지 않음. |
| Quest도 Data + Model? | **예.** QuestData = 정의, ActiveQuest/ActiveQuestTask = 런타임 상태. 에셋은 읽기 전용, 세이브/로드 시에도 런타임 모델만 다루면 됨. |

---

## 4. 파일·클래스 참조 (현재 구조)

### 인벤토리

- **Inventory** (Model): 슬롯 배열, AddItem/RemoveItem/SwapItems, `OnSlotChanged`, `OnItemChangedWithId`, `GetSlots()`.
- **ItemSlotModel**: Index, Item(ItemModel), Count, Clear.
- **ItemModel**: Data(ItemData), ItemId, ItemName, Icon 등 (임시로 ItemData 래핑).
- **InventoryView**: _inventoryUIPanel, RefreshSlot(ItemSlotModel), OnSwapRequested, OnRefreshRequested, IsPanelActive, ToggleInventory.
- **ItemSlot** (UI): _model(ItemSlotModel), SetModel, OnSwapRequested, SetDragIcon, SetIndex.
- **InventoryPresenter**: Model/View 인스펙터 할당, OnSlotChanged → RefreshSlot, OnRefreshRequested → RefreshView(전체 RefreshSlot 루프), HandleSwapRequested.

### 퀘스트

- **QuestData** (SO): QuestId, Title, Tasks (정의).
- **QuestTask** / **GatherTask** / **KillTask** / **VisitTask**: 에셋용 정의 (CurrentAmount는 에셋 상에서는 사용하지 않고, 런타임은 ActiveQuestTask 사용).
- **ActiveQuest**: QuestId, Title, Tasks(ActiveQuestTask), CreateFrom(QuestData).
- **ActiveQuestTask**: Description, TargetAmount, CurrentAmount, TargetId, IsAccumulate, RequiresItemDeduction, UpdateProgress.
- **QuestModel**: List<ActiveQuest>, AcceptQuest(QuestData), SetGatherProgress, CompleteQuest, OnQuestUpdated(ActiveQuest), GetActiveQuests() → IReadOnlyList<ActiveQuest>.

---

*이 문서는 해당 리팩터링 시점의 구조와 결정을 기록한 것입니다.*
