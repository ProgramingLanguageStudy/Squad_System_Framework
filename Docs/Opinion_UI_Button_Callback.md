# 의견: UI_Button에서 콜백(Action) 사용

버튼 컴포넌트가 `Initialize(icon, buttonText, callbacks)`처럼 **콜백을 받아서** 클릭 시 그걸 호출하는 방식에 대한 의견.

---

## 1. 콜백 사용 자체

- **가능하고, 역할 정의에도 잘 맞음.**
- 버튼은 “표시(아이콘, 텍스트) + 클릭 시 **넘겨받은 동작** 실행”만 하면 되므로, `Action`을 인자로 받아서 `onClick`에 붙이는 건 자연스러운 패턴이다.
- “무슨 일이 일어날지”는 버튼이 모르고, **콜백을 넘긴 쪽**이 정한다 → DialogueUI/버튼은 “알리기만”하는 쪽에 가깝다.

---

## 2. 문법 수정 필요

Unity의 `Button.onClick`은 **UnityEvent**라서 `AddListener(UnityAction)` 메서드를 쓴다.

- **잘못된 예**: `_button.onClick.AddListener += callbacks;`  
  (`AddListener`는 이벤트가 아니라 메서드라서 `+=`로 더할 수 없음.)
- **올바른 예**: `_button.onClick.AddListener(() => callbacks?.Invoke());`  
  또는 `_button.onClick.AddListener(callbacks.Invoke);` (단, `callbacks`가 null이 아니어야 함.)

그래서 `Initialize` 안에서는 다음처럼 바꾸면 된다.

```csharp
_button.onClick.AddListener(() => callbacks?.Invoke());
```

---

## 3. 추가로

- **한 버튼에 콜백을 여러 번 등록**할 수 있으므로, 재사용 시 기존 리스너를 제거할지, `Initialize`를 “최초 1회만” 쓰는지 정해 두는 게 좋다.
- **아이콘 없을 때**: `Sprite icon`이 null이면 `_buttonIcon.sprite = null` 또는 `_buttonIcon.gameObject.SetActive(false)` 등 처리 한 번 있으면 좋다.

정리하면, **콜백 사용은 괜찮고**, `AddListener` 호출 방식만 위처럼 고치면 된다.
