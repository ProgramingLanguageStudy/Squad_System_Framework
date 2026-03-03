using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 캐릭터(스쿼드 멤버) = Model·Mover·Animator·Interactor·Attacker·StateMachine 조합.
/// SquadController가 이 컴포넌트를 PlayerCharacter로 참조하여 입력·카메라 연결.
/// </summary>
[RequireComponent(typeof(CharacterModel)), RequireComponent(typeof(CharacterAnimator)),
 RequireComponent(typeof(CharacterStateMachine)),
 RequireComponent(typeof(CharacterMover)), RequireComponent(typeof(CharacterFollowMover)),
 RequireComponent(typeof(CharacterAttacker)), RequireComponent(typeof(CharacterInteractor)),
 RequireComponent(typeof(CharacterDeathHandler))]
public class Character : MonoBehaviour, IInteractReceiver
{
    [Header("----- 부품 -----")]
    [SerializeField] private CharacterModel _model;
    [SerializeField] private CharacterMover _mover;
    [SerializeField] private CharacterFollowMover _followMover;
    [SerializeField] private CharacterAnimator _characterAnimator;
    [SerializeField] private CharacterInteractor _interactor;
    [SerializeField] private CharacterAttacker _attacker;
    [SerializeField] private CharacterStateMachine _stateMachine;
    [SerializeField] private AnimatorEventBridge _animatorEventBridge;

    [Header("----- 주입용 참조 -----")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Animator _animator;
    [SerializeField] [Tooltip("동료 전용. 인스펙터에서 할당")]
    private AIBrain _aiBrain;
    [SerializeField] [Tooltip("인스펙터에서 할당 (없으면 Initialize 시 자동 탐색)")]
    private CharacterDeathHandler _deathHandler;

    // ── 현재 활성 이동 컴포넌트 ─────────────────────────────
    private IMover _activeMover;

    // ── MovementHandler (SetAsPlayer/SetAsCompanion 시 교체) ─
    private IMovementHandler _movementHandler;
    private DirectionMovementHandler _directionHandler;
    private TargetMovementHandler _targetHandler;

    // ── Public 프로퍼티 ─────────────────────────────────────
    public CharacterModel Model => _model;
    public CharacterMover Mover => _mover;
    public CharacterFollowMover FollowMover => _followMover;
    public CharacterAnimator Animator => _characterAnimator;
    public CharacterInteractor Interactor => _interactor;
    public CharacterAttacker Attacker => _attacker;
    public CharacterStateMachine StateMachine => _stateMachine;
    public AIBrain AIBrain => _aiBrain;
    public CharacterDeathHandler DeathHandler => _deathHandler;

    public bool CanMove => StateMachine != null && StateMachine.CanMove;

    // ── 공개 API (StateMachine·InputHandler·AIBrain 연동) ────

    public void RequestAttack() => _stateMachine?.RequestAttack();

    /// <summary>방향 설정 후 Move 상태로 전환. SetDirectionIntent는 호출 전에 별도 호출 가능.</summary>
    public void RequestMove(Vector3 moveDir)
    {
        _movementHandler?.SetDirectionIntent(moveDir);
        _stateMachine?.RequestMove();
    }

    /// <summary>이미 설정된 intent 기준으로 Move 상태로 전환.</summary>
    public void RequestMove()
    {
        _stateMachine?.RequestMove();
    }

    public void RequestIdle() => _stateMachine?.RequestIdle();

    /// <summary>Idle 전환 시 이동 intent 클리어. 이전 입력 방향으로 특수 흐름에서 움직이는 것 방지.</summary>
    public void ClearMovementIntent()
    {
        _movementHandler?.ClearDirectionIntent();
        _movementHandler?.ClearTargetIntent();
    }

    /// <summary>플레이어용. 이동 방향 갱신. 상태 변경 없이 intent만 설정.</summary>
    public void SetDirectionIntent(Vector3 worldDir)
    {
        _movementHandler?.SetDirectionIntent(worldDir);
    }

    /// <summary>동료용. SquadController·AIBrain 호출. 타겟 설정 시 Move 상태로 전환.</summary>
    public void SetFollowTarget(Transform target)
    {
        if (target == null)
        {
            _movementHandler?.ClearTargetIntent();
            return;
        }
        var dist = _model != null ? _model.StopDistance : 1.5f;
        _movementHandler?.SetTargetIntent(target, dist);
        _stateMachine?.RequestMove();
    }

    /// <summary>동료용. AIBrain 호출. 타겟 설정 시 Move 상태로 전환.</summary>
    public void SetCombatTarget(Transform target, float stopDistance)
    {
        _movementHandler?.SetTargetIntent(target, stopDistance);
        _stateMachine?.RequestMove();
    }

    /// <summary>동료용. 전투 타겟 해제.</summary>
    public void ClearCombatTarget()
    {
        _movementHandler?.ClearTargetIntent();
    }

    /// <summary>이동 의도 적용. MoveState에서 호출.</summary>
    public void ApplyMovementIntent()
    {
        _movementHandler?.Apply();
    }

    /// <summary>즉시 정지. 공격 진입 시 등.</summary>
    public void StopMovement()
    {
        _activeMover?.Stop();
    }

    /// <summary>공격 애니·Attacker 시작. Attack 상태 Enter에서 호출.</summary>
    public void BeginAttack()
    {
        _characterAnimator?.Attack();
        _attacker?.OnAttackStarted();
    }

    /// <summary>공격 종료 정리. Attack 상태 Exit에서 호출.</summary>
    public void EndAttackCleanup()
    {
        _attacker?.EndAttackCleanup();
    }

    /// <summary>사망 애니 재생. Dead 상태 Enter에서 호출.</summary>
    public void PlayDeadAnimation()
    {
        _characterAnimator?.Dead();
    }

    /// <summary>동료용. 직접 목표 위치로 이동 요청. (레거시/특수용)</summary>
    public void RequestMoveToTarget(Vector3 targetPos)
    {
        _followMover.MoveToTarget(targetPos);
    }

    /// <summary>NavMeshAgent 경로 초기화. RepositionCompanionsAround 등에서 Warp 전 호출.</summary>
    public void ResetNavMeshPath()
    {
        if (_navMeshAgent != null) _navMeshAgent.ResetPath();
    }

    public void Teleport(Vector3 worldPosition)
    {
        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.Warp(worldPosition);
            return;
        }

        if (_characterController == null)
        {
            transform.position = worldPosition;
            return;
        }

        _characterController.enabled = false;
        transform.position = worldPosition;
        _characterController.enabled = true;
    }

    public void SetCharacterControllerEnabled(bool enabled)
    {
        if (_characterController != null)
            _characterController.enabled = enabled;
    }

    /// <summary>플레이어 조종 모드로 전환.</summary>
    public void SetAsPlayer()
    {
        if (_characterController != null) _characterController.enabled = true;
        if (_navMeshAgent != null) _navMeshAgent.enabled = false;
        if (_interactor != null) _interactor.enabled = true;
        gameObject.layer = LayerMask.NameToLayer(LayerParams.Player);

        if (_aiBrain != null) _aiBrain.enabled = false;

        _activeMover = _mover;
        _movementHandler = _directionHandler;
    }

    /// <summary>동료 모드로 전환.</summary>
    public void SetAsCompanion(Transform followTarget)
    {
        if (_characterController != null) _characterController.enabled = false;
        if (_navMeshAgent != null) _navMeshAgent.enabled = true;
        if (_interactor != null) _interactor.enabled = false;
        gameObject.layer = LayerMask.NameToLayer(LayerParams.Character);

        if (_aiBrain != null) _aiBrain.enabled = true;

        _activeMover = _followMover;
        _movementHandler = _targetHandler;
    }

    public void Initialize()
    {
        if (_model == null) _model = GetComponent<CharacterModel>();
        if (_mover == null) _mover = GetComponent<CharacterMover>();
        if (_followMover == null) _followMover = GetComponent<CharacterFollowMover>();
        if (_characterAnimator == null) _characterAnimator = GetComponent<CharacterAnimator>();
        if (_interactor == null) _interactor = GetComponent<CharacterInteractor>();
        if (_attacker == null) _attacker = GetComponent<CharacterAttacker>();
        if (_stateMachine == null) _stateMachine = GetComponent<CharacterStateMachine>();
        if (_animatorEventBridge == null) _animatorEventBridge = GetComponentInChildren<AnimatorEventBridge>();
        if (_characterController == null) _characterController = GetComponent<CharacterController>();
        if (_navMeshAgent == null) _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        if (_aiBrain == null) _aiBrain = GetComponent<AIBrain>();
        if (_deathHandler == null) _deathHandler = GetComponent<CharacterDeathHandler>();

        _deathHandler?.Initialize(this, _navMeshAgent);

        _model?.Initialize();
        _mover.Initialize(_characterController, _model);
        _followMover.Initialize(_navMeshAgent, _model);
        _characterAnimator?.Initialize(_animator);
        _interactor?.Initialize(this);
        _stateMachine?.Initialize(this);
        _attacker?.Initialize(this, _stateMachine, _model);

        _directionHandler = new DirectionMovementHandler(_mover);
        _targetHandler = new TargetMovementHandler(_followMover, _model);

        _activeMover = (_navMeshAgent != null && _navMeshAgent.enabled)
            ? (IMover)_followMover
            : _mover;
        _movementHandler = (_navMeshAgent != null && _navMeshAgent.enabled)
            ? (IMovementHandler)_targetHandler
            : _directionHandler;

        if (_animatorEventBridge != null && _attacker != null)
        {
            _animatorEventBridge.OnBeginHitWindow += _attacker.Animation_BeginHitWindow;
            _animatorEventBridge.OnEndHitWindow += _attacker.Animation_EndHitWindow;
            _animatorEventBridge.OnAttackEnded += _attacker.Animation_OnAttackEnded;
            Debug.Log($"[Character] {gameObject.name} AnimatorEventBridge 연결 완료");
        }
        else if (_animatorEventBridge == null)
            Debug.LogWarning($"[Character] {gameObject.name} AnimatorEventBridge 없음 (애니 이벤트 동작 안 함)");
    }

    // ── Unity ───────────────────────────────────────────────

    private void Update()
    {
        if (_characterAnimator == null) return;

        // Move 상태일 때만 CurrentMoveSpeed 사용. Idle/Attack/Dead에서는 0으로 Idle 블렌드.
        float speed = (StateMachine != null && StateMachine.IsMove)
            ? (_model != null ? _model.CurrentMoveSpeed : 0f)
            : 0f;
        _characterAnimator.Move(speed);
    }

    private void OnDisable()
    {
        if (_animatorEventBridge != null && _attacker != null)
        {
            _animatorEventBridge.OnBeginHitWindow -= _attacker.Animation_BeginHitWindow;
            _animatorEventBridge.OnEndHitWindow -= _attacker.Animation_EndHitWindow;
            _animatorEventBridge.OnAttackEnded -= _attacker.Animation_OnAttackEnded;
        }
    }
}