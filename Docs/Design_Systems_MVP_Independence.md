# 전체 시스템 MVP + 시스템 간 독립성

> 목표: 각 시스템은 **다른 시스템을 알지 않는다**. 조율(Orchestrator, 예: PlayScene)만 여러 시스템을 알고 연결한다.

---

## 1. 원칙

- **시스템 간 독립성**: 퀘스트가 인벤토리를 모르고, 인벤토리가 퀘스트를 모르는 식. 부수효과(아이템 차감, 플래그 저장 등)는 **조율층**에서 처리.
- **MVP**: 로직·상태 = Model, 표시 = View, 연결 = Presenter. Model은 다른 시스템에 의존하지 않음.
- **싱글톤 지양**: 필요한 경우 씬/조율에서 인스턴스를 만들어 주입.
- **이벤트/인터페이스**: 시스템 간 통신이 필요하면 조율이 구독·호출하거나, 인터페이스로 주입 (예: `IActiveQuestProvider`, `IInventoryAdder`).

---

## 2. 시스템별 역할

| 시스템 | Model | View | Presenter | 의존성(없어야 함) |
|--------|--------|------|-----------|-------------------|
| **Quest** | QuestModel | QuestView | QuestPresenter | 인벤토리, 플래그 |
| **Inventory** | Inventory | InventoryView + Slot | InventoryPresenter (Model↔View 연결) | 퀘스트, GameEvents |
| **Dialogue** | DialogueModel(데이터+선택) | DialogueUI | DialogueSystem 등 | 퀘스트(완료/수락 여부 쿼리) |
| **Flag** | - | - | - | 서비스(저장소)만, 조율이 사용 |

---

## 3. 조율층(PlayScene 등)이 하는 일

- **참조 보유**: QuestModel, Inventory, DialogueManager, FlagManager, View/Presenter들.
- **연결**: Model ↔ View/Presenter, 이벤트 구독 후 다른 시스템에 전달.
  - 예: Inventory.OnItemChangedWithId 구독 → `GameEvents.OnQuestGoalProcessed` 발행 (퀘스트는 기존 이벤트로만 구독).
- **시나리오 처리**: 퀘스트 수락/완료 시 인벤토리·플래그 호출, 대화 시작 시 퀘스트/플래그 쿼리 후 대화창 오픈.
- **주입**: Npc에 `IDialogueStarter`, ItemObject에 `IInventoryAdder` 등 주입해 각자가 다른 시스템을 직접 참조하지 않게 함.

---

## 4. 적용 순서 및 현재 상태

1. **Quest** — 적용됨. Model이 인벤토리·플래그 모름.
2. **Inventory** — 적용됨. `Inventory`(Model), `InventoryView`(View), `InventoryPresenter`(연결). Presenter가 Model 구독·View 갱신, View 스왑 요청 시 Model 호출. 조율(PlayScene)이 `OnItemChangedWithId` 구독 후 `GameEvents.OnQuestGoalProcessed` 발행.
3. **Dialogue** — 적용됨. `GetCompletableQuestForNpc` / `GetAvailableQuestForNpc`를 `QuestDialogueQueries`(정적) + `DialogueCoordinator`로 이동. DialogueManager는 퀘스트를 모름.
4. **Npc / ItemObject** — 적용됨. Npc는 `DialogueCoordinator`로 대화 시작(주입 또는 Find). ItemObject는 `Inventory` 주입(또는 Find).

**참고**: DialogueSystem, DialogueManager, TooltipUI 등은 아직 싱글톤 유지. 조율층에서 인스턴스를 넘겨주는 방식으로 확장 가능.

---

*이 문서는 리팩터링 목표와 순서를 정리한 것입니다.*
