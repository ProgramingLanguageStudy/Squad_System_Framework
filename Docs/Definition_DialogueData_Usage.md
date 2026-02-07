# DialogueData & 대화 시스템 사용

> **대화 시스템** = 받은 대화만 재생 + 다음/끝내기. 로딩·선택·NPC·버튼 종류는 모름.  
> **DialogueDataLoader** = 데이터 로드·조회(GetById, GetBestForNpc). NPC/조율층에서 사용.

---

## 1. DialogueData 필드

| 필드 | 설명 |
|------|------|
| **id** | 대화 블록 구분. Loader.GetById(id)에서 사용. |
| **npcId** | 화자 표시명. Loader가 npcId로 묶어서 선택 시 사용. |
| **dialogueType** | Common, FirstMeet 등. Loader 선택 로직에서 사용. |
| **conditionValue** | 선택 로직에서 미사용. 연동 시 활용 가능. |
| **lines** | 한 문장씩 순서대로 재생. |

---

## 2. 역할 분리

| 담당 | 역할 |
|------|------|
| **DialogueSystem** | `StartDialogue(data)` 로 받은 대화만 재생. 다음/끝내기 제어. 버튼은 "다음/끝내기"만. |
| **DialogueDataLoader** | Resources 로드, `GetById(id)`, `GetBestForNpc(npcId)`. 대화 시스템과 분리. |
| **연결층(나중에)** | Loader로 DialogueData 취득 → `GameEvents.OnPlayDialogueRequested?.Invoke(data)` 로 재생 요청. 퀘스트/호감도 버튼은 별도 UI에서. |

---

## 3. 연결 포트

- 대화 재생 요청: `GameEvents.OnPlayDialogueRequested?.Invoke(dialogueData)`  
- DialogueData 취득: 씬에 둔 **DialogueDataLoader** 에서 `GetById(id)` 또는 `GetBestForNpc(npcId)` 호출.

```csharp
// 예: 나중에 Interactor/조율층에서
var loader = FindFirstObjectByType<DialogueDataLoader>();
if (loader != null && loader.IsLoaded)
{
    var data = loader.GetBestForNpc(npcId); // 또는 loader.GetById("Chief_FirstMeet");
    if (data != null)
        GameEvents.OnPlayDialogueRequested?.Invoke(data);
}
```

---

## 4. 요약

| 하고 싶은 일 | 사용 방법 |
|-------------|-----------|
| 대화 재생 | `GameEvents.OnPlayDialogueRequested?.Invoke(data)` |
| 어떤 대화 쓸지 정하기 | DialogueDataLoader.GetById(id) / GetBestForNpc(npcId) |
| 새 대화 추가 | DialogueData 에셋 생성 후 Resources/Dialogues에 배치 |

대화 시스템은 **내용만 재생**하고, **누가/언제/어떤 버튼**은 모두 밖에서 결정.
