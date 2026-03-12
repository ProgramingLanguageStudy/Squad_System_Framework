# UI 연출 적용 가이드

UITweenFacade + Scale/Alpha/Position 컴포넌트 기준. 어떤 UI에 어떤 연출을 붙일지 예시 정리.

---

## 1. Intro 씬

### 1.1 게임 타이틀 (로고/제목)

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Scale | ScaleTween | from=(0,0,0), to=(1,1,1), duration=0.5, ease=OutBack | `playEnterOnStart` ✓ |
| Alpha | AlphaTween | from=0, to=1, duration=0.4 | `playEnterOnStart` ✓ |

**구성**: 타이틀 루트에 `UITweenFacade` + `ScaleTween` + `AlphaTween`, `playEnterOnStart` 체크.

---

### 1.2 메인 패널 (Game Start, Logout 버튼)

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Scale | ScaleTween | from=(0.9,0.9,1), to=(1,1,1), duration=0.25, ease=OutQuad | ShowMainPanel 시 |
| Alpha | AlphaTween | from=0, to=1, duration=0.2 | ShowMainPanel 시 |

**연동**: IntroSceneView.ShowMainPanel()에서 `_mainPanelFacade?.PlayEnter()` 호출.

---

### 1.3 로그인 패널

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Scale | ScaleTween | from=(0.95,0.95,1), to=(1,1,1) | ShowLoginPanel 시 |
| Alpha | AlphaTween | from=0, to=1 | ShowLoginPanel 시 |

**연동**: ShowLoginPanel()에서 Facade.PlayEnter(), HideLoginPanel()에서 PlayExit().OnComplete(SetActive false).

---

### 1.4 에러 패널 (토스트형)

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Scale | ScaleTween | from=(0.8,0.8,1), to=(1,1,1), ease=OutBack | ShowErrorPanel 시 |
| Alpha | AlphaTween | from=0, to=1 | ShowErrorPanel 시 |

**연동**: ShowErrorPanel()에서 PlayEnter(), HideErrorPanel()에서 PlayExit().

---

### 1.5 로딩 패널

로딩은 보통 **페이드만** 사용.

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Alpha | AlphaTween | from=0, to=1 (Show) / 1→0 (Hide) | ShowLoading / HideLoading 시 |

선택적으로 Scale은 1 고정, Alpha만 트윈.

---

## 2. Play 씬

### 2.1 설정 패널 (SettingsView)

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Scale | ScaleTween | from=(0.9,0.9,1), to=(1,1,1), ease=OutBack | OnPanelOpened 시 |
| Alpha | AlphaTween | from=0, to=1 | OnPanelOpened 시 |

**연동**: `_settingsPanel` 루트에 Facade + Scale/Alpha. OnPanelOpened()에서 SetActive(true) 후 `_settingsTweenFacade.PlayEnter()`, OnPanelClosed()에서 `PlayExit().OnComplete(() => _settingsPanel.SetActive(false))`.

---

### 2.2 맵 패널 (MapView)

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Scale | ScaleTween | from=(0.95,0.95,1), to=(1,1,1) | OnPanelOpened 시 |
| Alpha | AlphaTween | from=0, to=1 | OnPanelOpened 시 |

Settings와 동일한 패턴.

---

### 2.3 인벤토리 패널

Facade 적용 시 (UIAnimator 제거됨):

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Scale | ScaleTween | from=(0.92,0.92,1), to=(1,1,1), ease=OutQuint | 열기 시 |
| Alpha | AlphaTween | from=0, to=1, ease=OutCubic | 열기 시 |

`_mainPanelRect`에 Facade + Scale/Alpha, CanvasGroup은 배경딤+패널 공통 루트에.

---

### 2.4 인벤토리 툴팁 (오른쪽 상세 패널)

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Alpha | AlphaTween | from=0, to=1, duration=0.15 | Show 시 |
| Position | PositionTween | from=(+30,0) to=(0,0) | 오른쪽에서 슬라이드 인 (선택) |

짧고 가벼운 연출 위주.

---

### 2.5 상호작용 UI (F키 안내, InteractionUI)

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Alpha | AlphaTween | from=0, to=1, duration=0.15 | Refresh("...") 시 |
| Position | PositionTween | from=(0,-10) to=(0,0) | 살짝 위로 올라오는 느낌 (선택) |

**연동**: `_uiPanel`에 Facade + AlphaTween. Refresh()에서 SetActive 전에 PlayEnter/PlayExit.

---

### 2.6 퀘스트 트래커 (QuestView)

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Alpha | AlphaTween | from=0, to=1 | SetPanelActive(true) 시 |
| Position | PositionTween | from=(-50,0) to=(0,0) | 왼쪽에서 슬라이드 (선택) |

---

### 2.7 맵 아이콘 (Map_Icon)

이미 호버/클릭 Scale + Punch 직접 구현.  
필요하다면 나중에 호버용 ScaleTween 컴포넌트로 교체 가능. 우선순위 낮음.

---

## 3. 일반 버튼

| 적용 | 컴포넌트 | 값 예시 | 트리거 |
|------|----------|---------|--------|
| Scale | ScaleTween | from=1, to=1.05 (호버는 별도) | - |

버튼은 보통 **호버/클릭**은 Map_Icon처럼 직접 DOTween으로 처리하고, **패널 등장 시 버튼들**만 Facade에서 함께 Scale/Alpha 적용.

---

## 4. 연출 강도 가이드

| UI 타입 | Scale | Alpha | Position | 느낌 |
|---------|-------|-------|----------|------|
| 메인 타이틀 | ✓ 강함 (0→1 OutBack) | ✓ | - | 임팩트 |
| 팝업/설정/맵 | ✓ 중간 (0.9→1) | ✓ | - | 자연스러운 등장 |
| 토스트/에러 | ✓ 가벼움 | ✓ | - | 빠르게 등장 |
| 툴팁/인터랙션 | - | ✓ | 선택 | 가벼움 |
| 로딩 | - | ✓ | - | 페이드 전환 |

---

## 5. 공통 권장값

| 파라미터 | 팝업/패널 | 토스트/툴팁 |
|----------|-----------|-------------|
| duration | 0.25~0.35 | 0.15~0.2 |
| Scale ease | OutBack 또는 OutQuad | OutQuad |
| Alpha ease | OutCubic | Linear |

---

## 6. 적용 순서 추천

1. **설정 패널** – PlayScene에서 자주 쓰이므로 먼저 적용
2. **Intro 게임 타이틀** – `playEnterOnStart`로 바로 확인 가능
3. **맵 패널** – 설정과 같은 패턴
4. **인벤토리** – 기존 UIAnimator 제거 후 Facade로 교체
5. **에러/상호작용/퀘스트** – 여유 있을 때 적용
