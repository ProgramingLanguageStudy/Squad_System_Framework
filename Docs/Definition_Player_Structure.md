# 플레이어 구조 정의

> **Player** = Mover/Animator/Interactor를 보유하고 초기화만 담당. **입력 출처를 모름.**  
> **PlayScene** = InputHandler·Player 참조를 갖고, 입력 이벤트 연결·값(MoveInput 등) 전달.  
> **InputHandler** = 별도 GameObject에 단독 컴포넌트. 입력만 담당(맵 전환·이벤트·값 노출). UI 맵 등 게임 전역 입력으로 확장 가능.  
> **카메라** = Cinemachine 전담. 캐릭터 회전은 "카메라 기준 이동 방향"으로만 적용.

---

## 1. 컴포넌트 역할

| 컴포넌트 | 역할 | 의존성 |
|----------|------|--------|
| **Player** | **컴포넌트 조합부.** 필요한 부품을 [SerializeField]로 인스펙터에서 참조하고, Initialize() 시 그 참조를 각 컴포넌트에 주입. 입력은 읽지 않음. CanMove, Mover, Animator, Interactor, StateMachine 등 노출. | [SerializeField]로 보유 후 주입 |
| **PlayerMover** | 이동·회전. Camera 기준 moveDir 계산, CharacterController.Move로 이동, 캐릭터는 moveDir 방향으로 Slerp 회전. | Initialize(CharacterController, Transform)로 주입 |
| **PlayerAnimator** | Animator 래퍼. Move(speed), Attack() 등으로만 애니 실행. 파라미터는 AnimatorParams 사용. Update에서 Mover.GetCurrentSpeed() → Move() 호출. | Initialize(Animator, PlayerMover)로 주입 |
| **PlayerInteractor** | SphereCast로 IInteractable 감지, CurrentTarget 갱신. TryInteract() 시 CurrentTarget.Interact(player) 호출. | Initialize(Player)로 주입 |
| **PlayerStateMachine** | 클래스 기반 상태(IdleState, AttackState 등). RequestIdle(), RequestAttack() 등으로 전환. | Initialize(Player)로 주입 |
| **PlayerAttacker** | **입력과 무관.** 공격 애니메이션 종료 이벤트에서만 EndAttack() → RequestIdle() 호출. | Initialize(PlayerStateMachine)로 주입 |
| **InputHandler** | Input System 랩퍼. **별도 GameObject에 단독.** MoveInput/LookInput은 매 프레임, Interact/Attack/Inventory/Quest는 이벤트 발행. | InputSystem_Actions |
| **PlayScene** | **Play 씬 전용 연결층.** InputHandler·Player 참조 보유. Update에서 MoveInput 전달(CanMove 반영). OnAttackPerformed → Player.RequestAttack(), OnInteractPerformed → Interactor.TryInteract() 등. | InputHandler, Player |

- **InputHandler**는 Player가 아닌 **별도 GameObject**에 둔다. PlayScene(또는 씬 루트용 부트스트랩)이 같은 씬에서 참조해 사용.

---

## 2. 카메라와 회전

- **화면 회전**: Cinemachine 가상 카메라가 전적으로 담당(오빗, Follow, 댐핑 등). 코드에서 카메라 회전을 직접 건드리지 않음.
- **캐릭터 회전**: PlayerMover가 **이동 방향(moveDir)**으로만 회전. moveDir = 카메라 forward/right로 만든 방향이므로, 결과적으로 "카메라 기준 앞/옆으로 움직일 때 그쪽을 보는" 구조.
- 정리: 카메라 회전은 Cinemachine, 캐릭터 회전은 "이동 방향 따라가기" 한 가지 소스만 사용. 괜찮은 구조.

---

## 3. 입력 흐름(원칙)

- **입력은 InputHandler가 담당**, **연결은 PlayScene이 담당.** Player는 입력을 직접 읽지 않음.
- PlayScene에서 할 일:
  - **이동**: Update()에서 `Player.CanMove`면 `InputHandler.MoveInput` → `Player.Mover.Move(input)`, 아니면 `Mover.Move(Vector2.zero)`.
  - **상호작용**: OnInteractPerformed 구독 → `Player.Interactor.TryInteract()` 호출.
  - **공격**: OnAttackPerformed 구독 → **`Player.RequestAttack()`** 호출. (조건 판단·상태 전환은 Player/상태머신이 담당.)
  - **Inventory/Quest**: OnInventoryPerformed 등 구독 후 해당 시스템/UI로 전달.

### 3.1 공격 입력 → 행동: 두 가지 방식

| 방식 | 흐름 | 담당 |
|------|------|------|
| **A. 컴포넌트 경유** | 입력 → Attacker.TryAttack() → 상태머신.RequestAttack() | “공격”을 Attacker가 먼저 받고, 내부에서 상태 전환 요청 |
| **B. 조건 판단 후 상태 전환 (채택)** | 입력 → **Player.RequestAttack()** → 상태머신이 조건 판단 후 Attack 전환 | “공격하고 싶다”는 Player 한곳에서 받고, **조건(Idle 등) 판단과 전환은 상태머신**이 담당 |

- **채택**: B. PlayScene은 `Player.RequestAttack()`만 호출. Idle인지 등 조건은 `StateMachine.RequestAttack()` 내부에서 처리.  
- **PlayerAttacker**는 이제 **입력 쪽에 관여하지 않음**. 역할은 **공격 애니메이션 종료 시** 애니 이벤트에서 `EndAttack()` 호출 → `RequestIdle()`로 Idle 복귀만 담당.

---

## 4. 초기화·연결 순서

1. **Player**: 인스펙터에서 부품(Mover, PlayerAnimator, Interactor, Attacker, StateMachine) 및 주입용(CharacterController, Animator, MainCamera Transform) 참조 연결 후, `Player.Initialize()` 한 번 호출. 내부에서 위 참조만 사용해 `Mover.Initialize(...)` → `PlayerAnimator.Initialize(...)` → `Interactor.Initialize(this)` → `StateMachine.Initialize(this)` → `Attacker.Initialize(StateMachine)` 순으로 주입. (규칙: 조합부는 [SerializeField], 하위는 Initialize 주입만. `Conventions_Components_State_Animator.md` 참고.)
2. **PlayScene**: InputHandler·Player 참조 보유, OnEnable에서 이벤트 구독. Attack 시 `Player.RequestAttack()` 호출.

---

## 5. 상태·플래그

| 항목 | 설명 |
|------|------|
| **Player.CanMove** | false면 이동 입력 무시, Mover.Move(Vector2.zero)로 즉시 정지. 대화/메뉴 등에서 true/false 제어. |

---

## 6. 요약

| 하고 싶은 일 | 사용 방법 |
|-------------|-----------|
| 이동 끄기/켜기 | Player.CanMove = false / true |
| 상호작용 실행 | PlayScene에서 OnInteractPerformed 구독 → Player.Interactor.TryInteract() 호출 |
| 공격 요청 | PlayScene에서 OnAttackPerformed 구독 → Player.RequestAttack() 호출 (조건·전환은 상태머신) |
| 현재 조준 상호작용 대상 | Player.Interactor.CurrentTarget (IInteractable 또는 null) |
| 카메라 동작 변경 | Cinemachine 설정만 수정. 플레이어 코드는 Camera.main 기준만 사용. |
| 입력↔플레이어 연결 | PlayScene(또는 씬 전용 연결 컴포넌트)에서 InputHandler·Player 참조로 이벤트/값 전달 |

Player는 **부품 보유·초기화**만 담당. 입력 연결은 **PlayScene**에서 처리. Attack/Inventory/Quest도 PlayScene에서 InputHandler 이벤트 구독 후 각각 처리부로 전달하면 됨.
