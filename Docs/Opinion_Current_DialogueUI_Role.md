# 의견: 현재 DialogueUI의 역할 판단

정의한 역할(“필요한 정보만 받아서 대화창·버튼 띄우기 + 다음 문장 + 클릭은 알리기만”)과 비교한 판단.

---

## 잘 맞는 부분

- **표시**: `Open`, `ReplaceContent`, `SetQuestButtonVisible`, `Close` → 받은 정보로 창·텍스트·버튼을 띄우는 것만 함.
- **다음 문장**: `ShowNext`, `TypeSentence` → 부가 역할대로 동작.
- **비즈니스 로직 없음**: 퀘스트 수락/제출, 플래그, 조건 판단 등은 DialogueUI 안에 없음. 버튼을 눌렀을 때 “뭘 할지”를 여기서 결정하지 않음.

즉, **“띄우기 + 다음 문장”**까지의 책임은 역할 정의와 잘 맞음.

---

## 역할 정의에 비해 조금 넘어선 부분

- **버튼 클릭 시**: 지금은 `DialogueSystem.Instance.EndDialogue()`, `DialogueSystem.Instance.OnQuestPanelButtonClicked()`를 **직접 호출**함.
- 즉, **“누가 그걸 처리할지”**를 DialogueUI가 알고 있음 (DialogueSystem이라는 구체 타입에 의존).
- 역할을 “**클릭은 알리기만 한다**”로 엄격히 두면, DialogueUI는 **“끝내기 버튼 눌렀다” / “액션 버튼 눌렀다”** 같은 **이벤트만 발행**하고,  
  **누가 구독해서 EndDialogue / OnQuestPanelButtonClicked를 부를지는** DialogueSystem이나 PlayScene 같은 **밖**에서 결정하는 쪽이 더 맞음.
- 그렇게 하면 DialogueUI는 DialogueSystem을 전혀 알 필요가 없어지고, “표시 + 입력 알림”만 담당하게 됨.

---

## 정리

| 항목 | 현재 | 역할 정의 기준 |
|------|------|-----------------|
| 정보 받아서 창·버튼 띄우기 | ✅ 함 | ✅ |
| 다음 문장 넘기기 | ✅ 함 | ✅ |
| 클릭 시 비즈니스 로직 | ✅ 모름 | ✅ |
| 클릭 시 “누구를 호출할지” | DialogueSystem 직접 호출 | 이벤트만 발행, 호출은 구독처에서 |

**결론**: 지금도 “표시 + 다음 문장”과 “로직은 안 함”이라는 점에서는 역할 정의와 잘 맞는다. 다만 **“클릭은 알리기만”**을 더 엄격히 적용하려면, 버튼 클릭 시 **DialogueSystem을 부르지 말고 이벤트만 발행**하게 바꾸고, DialogueSystem(또는 상위)이 그 이벤트를 구독해 `EndDialogue` / `OnQuestPanelButtonClicked`를 호출하도록 하면, DialogueUI의 역할이 정의와 완전히 일치한다.
