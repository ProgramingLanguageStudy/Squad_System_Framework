# 프로젝트 규칙 — 컴포넌트·상태·애니메이터

> 지금까지 합의한 **코드 규칙**을 한곳에 모은 문서. 새 코드 작성·리팩터 시 이 규칙을 따름.

---

## 1. 컴포넌트와 의존성 주입

| 규칙 | 설명 |
|------|------|
| **Awake 지양** | 컴포넌트가 `Awake()`에서 `GetComponent<>()` 등으로 의존성을 스스로 찾는 방식은 지양한다. |
| **조합부는 [SerializeField]** | Player처럼 **컴포넌트를 조합하는 쪽**은 필요한 컴포넌트를 **`[SerializeField]`로 두고 인스펙터에서 참조**한다. 코드에서 GetComponent로 수집하지 않는다. (미연결 시 같은 GameObject 보충용 fallback은 허용) |
| **Initialize + 매개변수** | 조합부(Player)가 `Initialize()` 호출 시, **그때 갖고 있는 참조를 각 컴포넌트의 `Initialize(의존성...)`에 넘겨 주입**한다. |
| **하위는 주입만 사용** | Mover, Animator, StateMachine 등 하위 컴포넌트는 내부에서 GetComponent 하지 않고, Initialize 매개변수로만 의존성을 받는다. |

**예시**

- **Player**: `[SerializeField]`로 _mover, _playerAnimator, _interactor, _attacker, _stateMachine, _characterController, _animator, _mainCameraTransform 보유. Initialize()에서 위 참조들을 각각 Mover.Initialize(controller, cam), PlayerAnimator.Initialize(animator, mover) 등으로 전달.
- `PlayerMover.Initialize(CharacterController controller, Transform mainCameraTransform)` — 내부에서 GetComponent 하지 않음.
- `PlayerAnimator.Initialize(Animator animator, PlayerMover mover)` — Animator·Mover를 Player가 넘김.
- `PlayerStateMachine.Initialize(Player player)` — 상태 생성·초기 상태 Enter까지 여기서 수행.
- `PlayerAttacker.Initialize(PlayerStateMachine stateMachine)` — 참조는 주입만 사용.

---

## 2. 플레이어 상태 (State 패턴)

| 규칙 | 설명 |
|------|------|
| **enum 대신 클래스** | 플레이어 동작 상태는 **클래스 기반 State 패턴**으로 둔다. (Enter / Update / Exit) |
| **한 파일 한 상태** | `PlayerStateBase` 상속 클래스(IdleState, AttackState 등)는 각각 별도 파일. |
| **전환은 상태머신만** | 외부/입력 쪽은 `StateMachine.RequestIdle()`, `RequestAttack()` 등 **요청 메서드**만 호출. 실제 전환·Enter/Exit 호출은 `PlayerStateMachine` 내부에서만. |
| **상태 내부** | 상태 클래스는 `Player`, `Machine` 참조로 필요한 동작만 수행. 애니메이션 재생은 **PlayerAnimator** 메서드로만. |

**파일 위치**

- `Assets/02_Scripts/Player/States/` — `PlayerStateBase.cs`, `IdleState.cs`, `AttackState.cs` 등.
- `PlayerStateMachine` — `_currentState` 보관, `ChangeState(PlayerStateBase)`, `Update()`에서 `_currentState.Update()` 호출.

---

## 3. 애니메이터

| 규칙 | 설명 |
|------|------|
| **파라미터 한곳** | Animator 파라미터 이름/해시는 **Enum처럼 한곳**에 모은다. (`AnimatorParams` 등) |
| **래퍼 메서드** | 애니메이션 실행(SetFloat, SetTrigger 등)은 **래퍼 클래스(PlayerAnimator)의 public 메서드**로만 한다. 다른 스크립트에서 `Animator`를 직접 가지고 `SetFloat`/`SetTrigger` 호출하지 않는다. |
| **API 형태** | 예: `PlayerAnimator.Move(float moveSpeed)`, `PlayerAnimator.Attack()` — 내부에서 `AnimatorParams`의 해시로 `_animator.SetFloat` / `SetTrigger` 호출. |

**파일·구조**

- **파라미터 정의**: `Assets/02_Scripts/AnimatorParams.cs` — `MoveSpeed`, `Attack` 등 이름·해시 상수.
- **실행**: `PlayerAnimator` — `Initialize(Animator, PlayerMover)` 로 주입받고, `Move(speed)`, `Attack()` 등만 노출.

---

## 4. 플레이어 초기화 순서 (참고)

- **참조 확보**: Player는 부품·주입용 참조를 **인스펙터 [SerializeField]로 보유**. (비어 있으면 같은 GameObject에서 한 번만 보충 가능)
- **Initialize() 호출 시**: 보유한 참조만 사용해 아래 순서로 주입.
  1. `Mover.Initialize(characterController, mainCameraTransform)`  
  2. `PlayerAnimator.Initialize(animator, Mover)`  
  3. `Interactor.Initialize(this)`  
  4. `StateMachine.Initialize(this)`  
  5. `Attacker.Initialize(StateMachine)`  

상세 구조는 `Definition_Player_Structure.md` 참고.

---

## 5. 요약 표

| 하고 싶은 일 | 규칙/사용처 |
|-------------|-------------|
| 컴포넌트 의존성 | 조합부(Player)는 [SerializeField]로 참조, 하위는 Awake 대신 `Initialize(의존성)` 주입만 사용 |
| 플레이어 상태 추가 | `PlayerStateBase` 상속 클래스 추가 + StateMachine에서 인스턴스 생성·전환 로직 |
| 애니 파라미터 추가 | `AnimatorParams`에 이름/해시 추가 → PlayerAnimator에 해당 메서드 추가 |
| 애니 재생 | 상태/다른 스크립트에서는 `Player.Animator.Move()` / `Attack()` 등만 호출 |

---

*규칙 추가·변경 시 이 문서를 갱신한다.*
