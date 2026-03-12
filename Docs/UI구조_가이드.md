# UI 구조 가이드

UI 종류별 용어, 역할, 입력 제어를 정리한 문서. 리팩터링 시 참고.

---

## 1. UI 종류 정의

| 종류 | 용어 | 설명 | 예시 |
|------|------|------|------|
| **Panel** | Panel, Drawer | Open/Close 토글. 기능 화면을 열고 닫음. | 설정, 맵, 인벤토리 |
| **Modal** | Modal Dialog | Show/Dismiss. 사용자 결정(확인/취소)을 요청. | "저장할까요?", "삭제할까요?" |
| **Toast** | Toast, Snackbar | Show 후 자동/수동 Dismiss. 알림만 전달. | 에러 메시지, 성공 알림 |
| **Tooltip** | Tooltip, Popover | 호버/클릭 시 표시. 짧은 설명 또는 메뉴. | 아이템 설명, F키 안내 |
| **HUD** | HUD | 항상 표시. 게임 중 항상 필요한 정보. | 체력, 골드, 퀘스트 트래커 |

---

## 2. 입력/제어 정책

| 항목 | Panel | Modal | Toast | Tooltip | HUD |
|------|-------|-------|-------|---------|-----|
| **이동 입력** | 막음 | 막음 | 막지 않음 | 막지 않음 | 막지 않음 |
| **조작 입력** | 막음 (패널 내만) | 막음 (다이얼로그 내만) | 막지 않음 | 막지 않음 | 막지 않음 |
| **커서** | 표시 | 표시 | 그대로 | 그대로 | 그대로 |
| **Esc** | 닫기 (스택 LIFO) | **비활성화** (반드시 버튼 클릭) | - | - | - |
| **배경 클릭** | - | 닫기(옵션) | - | 닫기 | - |

- Modal Esc 비활성화: 중요한 확인/취소 실수 방지. 필요 시 `closeOnEsc` 옵션 추후 추가 가능

---

## 3. 라이프사이클

| 종류 | 열기 | 닫기 | 반환값 |
|------|------|------|--------|
| **Panel** | `OpenPanel()` | `ClosePanel()` | 없음 |
| **Modal** | `Show(data, onConfirm, onCancel)` | `Dismiss()` | 콜백으로 전달 |
| **Toast** | `Show(message)` | 자동/버튼 | 없음 |
| **Tooltip** | `Show(content)` | `Hide()` | 없음 |
| **HUD** | 항상 표시 또는 `SetActive(bool)` | - | - |

---

## 4. 현재 프로젝트 UI 매핑

| UI | 현재 | 목표 종류 | 비고 |
|----|------|-----------|------|
| SettingsView | PanelViewBase | Panel | 유지 |
| MapView | PanelViewBase | Panel | 유지 |
| InventoryView | 별도 토글 | Panel | PanelViewBase 통합 검토 |
| Intro 에러 패널 | SetActive | Toast | 또는 Modal(닫기 버튼) |
| Intro 로그인/메인 | SetActive | Panel | 씬별 패널 |
| InteractionUI | 이벤트 구독 | Tooltip | F키 안내 |
| InventoryTooltipView | Show/Hide | Tooltip | 패널 내부 툴팁 |
| QuestView | SetPanelActive | HUD | 항상 표시, 이동 제한 없음 |
| Loading 패널 | SetActive | Toast/Overlay | 커서 숨김 유지 |

---

## 5. 입력 제어 인터페이스 (2개)

역할을 나눠서 두 개의 인터페이스 사용.

### 5.1 IBlocksInput – 입력 막을 UI

열려 있으면 이동/조작 입력을 막는 UI.

```csharp
public interface IBlocksInput
{
    bool IsOpen { get; }
}
```

| 베이스 | IBlocksInput |
|--------|--------------|
| PanelViewBase | ✓ |
| ModalBase | ✓ |
| ToastBase | ✗ |
| TooltipBase | ✗ |
| HUD | ✗ |

- InputHandler: `IsOpen`이 true인 UI가 하나라도 있으면 이동/조작 차단

### 5.2 IRespondsToEsc – Esc로 닫을 UI

Esc 키가 눌리면 닫히는 UI. InputHandler가 Esc 구독 후 이 인터페이스 스택의 맨 위만 Close 호출.

```csharp
public interface IRespondsToEsc
{
    void Close();
}
```

| 베이스 | IRespondsToEsc |
|--------|----------------|
| PanelViewBase | ✓ |
| ModalBase | ✗ (반드시 버튼 클릭) |
| ToastBase | ✗ |
| TooltipBase | ✗ |
| HUD | ✗ |

- Panel 열 때: IRespondsToEsc 스택에 Push
- Panel 닫을 때: 스택에서 Pop
- Esc 눌림: 스택 맨 위의 Close() 호출
- **Dialogue** 등 5가지 외 UI도 필요 시 구현 가능

---

## 6. 공통 구조 (리팩터링 목표)

- **공통 부모 추상클래스 없음**. 5가지 각각 별도 베이스. 라이프사이클이 달라 강제 통합 시 비대해짐.

### 6.1 PanelViewBase
- `OpenPanel()` / `ClosePanel()`
- IBlocksInput, IRespondsToEsc 구현
- 커서: 열 때 표시, 닫을 때 숨김
- Esc: 닫기 (스택 LIFO, InputHandler가 Close 호출)
- 이동/조작: 막음 (IBlocksInput.IsOpen 기반)

### 6.2 ModalBase (추후)
- `Show(data, onConfirm, onCancel)` / `Dismiss()`
- IBlocksInput만 구현 (IRespondsToEsc ✗)
- 커서: 표시
- Esc: **비활성화** (반드시 버튼 클릭)
- 배경 딤: 클릭 시 닫기(옵션)

### 6.3 ToastBase (추후)
- `Show(message, duration?)` / `Hide()`
- 입력/커서: 영향 없음
- 자동 Dismiss 또는 버튼

### 6.4 TooltipBase (추후)
- `Show(content)` / `Hide()`
- 입력/커서: 영향 없음
- 호버 아웃 또는 클릭 시 Hide

### 6.5 HUD
- 별도 베이스 없이 `SetActive` 또는 항상 표시
- 입력 제어 없음

### 6.6 Dialogue (5가지 외)
- NPC 상호작용 트리거, 다음/끝내기/퀘스트 선택. 5가지와 성격 다름.
- 필요 시 IBlocksInput, IRespondsToEsc 별도 구현 (베이스 상속 없이)

---

## 7. UITweenFacade 연동

- Panel, Modal, Toast 모두 Facade로 등장/퇴장 연출 적용 가능
- `controlActive`: Facade가 SetActive 제어
- `playEnterOnStart`: 씬 로드 시 자동 PlayEnter (타이틀 등)

---

## 8. 리팩터링 시 고려사항

### 8.1 PanelViewBase 적용 범위
- 적용 대상: Settings, Map, Inventory
- Inventory: `event`, `OnRefreshRequested` 등 추가 로직 많음 → PanelViewBase 흐름과 맞는지 확인
- Initialize 시 패널 비활성화, Toggle을 Open/Close로 통일

### 8.2 커서 처리
- PanelViewBase가 Open/Close 시 `OnCursorShowRequested`, `OnCursorHideRequested` 호출
- 설정+맵+인벤토리 중복 열림 시: 커서 요청 중복 가능 → 스택/우선순위 검토
- 로딩 패널은 커서 숨김 유지 목적 → 별도 처리 여부 확인

### 8.3 Esc 처리
- InputHandler: Esc 구독 → IRespondsToEsc 스택의 맨 위 Close() 호출
- Panel: Open 시 스택 Push, Close 시 Pop
- Modal: IRespondsToEsc 미구현 → 스택에 등록 안 됨 → Esc 무반응

### 8.4 InputHandler 연동
- **입력 차단**: IBlocksInput 구현체 중 IsOpen인 것이 하나라도 있으면 이동/조작 막기
- **Esc 처리**: IRespondsToEsc 스택(LIFO) 관리, Esc 시 맨 위 Close() 호출

### 8.5 IntroScene
- 로그인/메인/에러/로딩 = 상태 전환 패턴. Panel과 다를 수 있음
- Intro 커서 정책이 PlayScene과 다를 수 있음
- HideLoginPanel / ShowMainPanel 흐름을 PanelViewBase와 어떻게 맞출지

### 8.6 Facade·Panel 참조
- Facade가 패널 루트에 붙으면 `_panel`과 `_facade.gameObject`가 같을 수 있음
- 중복 참조 정리, 프리팹/씬에서 Facade 연결 누락 확인

### 8.7 Tooltip / HUD
- PanelViewBase 사용 안 함. 커서/입력 제어 없음
- ToastBase, TooltipBase 등은 필요할 때 추가

### 8.8 DialogueView
- 5가지에 속하지 않음. 별도 타입으로 유지.
- Esc 처리 필요 시 IBlocksInput, IRespondsToEsc 구현

### 8.9 리팩터링 순서 (추천)
1. **Settings, Map 점검** – 이미 PanelViewBase 사용. 구조/패턴 검증, Facade 연결 확인
2. **Inventory를 PanelViewBase로 통합** – 가장 복잡. 1번에서 패턴 확정 후 진행
3. **커서/Esc/InputHandler** – 세 패널 동일 베이스 후, 다중 패널/Esc 우선순위 처리
4. **IntroScene** – 씬 전환·커서 정책 등 별도 맥락. PlayScene 안정 후
5. **Modal/Toast** – 필요 시 추가

**순서가 중요한 이유**
- 1→2: 단순한 Settings·Map으로 패턴 확정 후 복잡한 Inventory 적용
- 2→3: 패널 구조 통일 후 커서/Esc/입력 로직 일관 처리
- 3→4: IntroScene은 PlayScene과 정책이 다를 수 있어 나중에 정리

### 8.10 테스트 포인트
- [ ] 설정 열 때 커서 표시, Esc로 닫기
- [ ] 맵 열 때 동일
- [ ] 인벤토리 열 때 동일
- [ ] 설정 열린 상태에서 인벤토리 열기/닫기
- [ ] 패널 열린 상태에서 이동 불가
- [ ] 연출 적용된 패널 등장/퇴장

---

## 9. 스크립트 폴더 구조 (하이브리드)

- **UI/**: 공통 베이스, Tween, 범용 컴포넌트
- **시스템 폴더**: 해당 기능의 View, 슬롯 등

```
02_Scripts/
├── UI/
│   ├── PanelViewBase.cs
│   ├── SettingsView.cs          # 범용 Panel
│   ├── CurrencyView.cs, CurrencyPresenter.cs  # HUD
│   ├── InteractionUI.cs         # Tooltip
│   ├── WorldHealthBarView.cs    # 월드 체력바
│   ├── Tween/                   # 연출 공통
│   └── UI_Button.cs             # 범용 버튼
├── Map/
│   ├── MapView.cs
│   ├── Map_Icon.cs, Map_PortalIcon.cs
│   └── PortalSlot.cs            # 포탈 메뉴 슬롯
├── Inventory/
│   ├── InventoryView.cs, InventoryTooltipView.cs
│   └── ItemSlot.cs
├── Reward/
│   ├── EnemyRewardController.cs
│   ├── ItemPickupLog.cs         # 획득 알림
│   └── ItemPickupSlot.cs
├── Quest/
│   └── QuestView.cs
├── Dialogue/
│   └── DialogueView.cs
├── PlayScene/
│   └── PlaySceneView.cs
└── IntroScene/
    └── IntroSceneView.cs
```

---

## 10. 리팩터링 체크리스트

- [ ] IBlocksInput, IRespondsToEsc 인터페이스 추가
- [ ] PanelViewBase에 IBlocksInput, IRespondsToEsc 구현
- [ ] InputHandler: IBlocksInput 기반 입력 차단, IRespondsToEsc 스택 기반 Esc 처리
- [ ] InventoryView를 PanelViewBase 상속으로 통합 검토
- [ ] Modal용 베이스 클래스 추가 (확인창 등, Esc 비활성화)
- [ ] Toast용 베이스 클래스 추가 (에러, 알림)
- [ ] UI 종류별 입력 차단 플래그 통일
