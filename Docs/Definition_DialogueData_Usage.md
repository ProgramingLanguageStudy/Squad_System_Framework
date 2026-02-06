# DialogueData 사용 방법

> **DialogueData** 한 에셋에 **누가(npcId)·타입·조건·대사(lines)** 를 모두 정의. 별도 레지스트리 없음.

---

## 1. DialogueData 필드

| 필드 | 설명 |
|------|------|
| **id** | 대화 블록 구분용. GetDialogueById 등에서 사용. |
| **npcId** | 이 대화를 쓰는 NPC ID. System이 npcId로 묶어서 선택. |
| **dialogueType** | Common, Affection, FirstMeet, Quest, QuestComplete 등. |
| **conditionValue** | 선택 로직에서 미사용. 다른 시스템(호감도 등) 연동 시 활용 가능. |
| **lines** | 한 문장씩 순서대로 재생 (독백). |

- 새 대화 추가: `Assets/Resources/Dialogues/` 에 DialogueData 에셋 생성 후 위 필드 전부 설정.

---

## 2. 연결 포트 (대화 시스템은 독립, NPC와 무연결)

- **DialogueSystem**은 NPC를 참조하지 않음. 씬에 한 개 두면 **GameEvents.OnPlayDialogueRequested** 를 구독해, 외부에서 재생 요청이 오면 그때만 재생.
- **연결 시**: DialogueInteractor 등에서 재생할 **DialogueData**를 정한 뒤 `GameEvents.OnPlayDialogueRequested?.Invoke(data)` 만 호출하면 됨. DialogueSystem 참조 불필요.

```csharp
// 나중에 Interactor·조율층에서 대화 시작할 때
DialogueData data = ...; // GetBestDialogue(npcId) 또는 GetDialogueById(id) 등으로 취득
if (data != null)
    GameEvents.OnPlayDialogueRequested?.Invoke(data);
```

- **직접 API 사용**: DialogueSystem 참조가 있으면 `StartDialogue(data)`, `GetBestDialogue(npcId)`, `GetDialogueById(id)` 그대로 사용 가능.

---

## 3. 요약

| 하고 싶은 일 | 사용 방법 |
|-------------|-----------|
| 연결층에서 대화 재생 | `GameEvents.OnPlayDialogueRequested?.Invoke(dialogueData)` (포트) |
| DialogueSystem 직접 사용 시 | `GetBestDialogue(npcId)` / `GetDialogueById(id)` → `StartDialogue(data)` |
| 새 대화 추가 | DialogueData 에셋 하나 생성 (id, npcId, dialogueType, conditionValue, lines 설정) |

**DialogueData** 하나에 “누가 / 어떤 타입·조건 / 무슨 대사”가 다 들어 있음.
