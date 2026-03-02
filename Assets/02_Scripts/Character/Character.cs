using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 캐릭터(스쿼드 멤버) = Model·Mover·Animator·Interactor·Attacker·StateMachine 조합.
/// SquadController가 이 컴포넌트를 PlayerCharacter로 참조하여 입력·카메라 연결.
/// </summary>
[RequireComponent(typeof(CharacterModel)), RequireComponent(typeof(CharacterAnimator)),
 RequireComponent(typeof(CharacterStateMachine)),
 RequireComponent(typeof(CharacterMover)), RequireComponent(typeof(CharacterFollowMover)),
 RequireComponent(typeof(CharacterAttacker)), RequireComponent(typeof(CharacterInteractor))]
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
    [SerializeField]
    [Tooltip("동료 전용. 인스펙터에서 CompanionStateMachine 할당")]
    private CompanionStateMachine _companionStateMachine;

    // ── 현재 활성 이동 컴포넌트 ─────────────────────────────
    // SetAsPlayer / SetAsCompanion 호출 시 교체됩니다.
    // Character 외부 코드는 이 프로퍼티를 통해 이동 상태를 조회합니다.
    private IMover _activeMover;

    // ── Public 프로퍼티 ─────────────────────────────────────
    public CharacterModel Model => _model;
    public CharacterMover Mover => _mover;
    public CharacterFollowMover FollowMover => _followMover;
    public CharacterAnimator Animator => _characterAnimator;
    public CharacterInteractor Interactor => _interactor;
    public CharacterAttacker Attacker => _attacker;
    public CharacterStateMachine StateMachine => _stateMachine;

    public bool CanMove => StateMachine != null && StateMachine.CanMove;

    // ── 공개 메서드 ─────────────────────────────────────────

    public void RequestAttack()
    {
        _stateMachine?.RequestAttack();
    }

    /// <summary>
    /// 플레이어 이동용
    /// </summary>
    /// <param name="moveDir"></param>
    public void RequestMove(Vector3 moveDir)
    {
        _mover.Move(moveDir);
    }

    /// <summary>
    /// 플레이이어나 동료의 자동추적용
    /// </summary>
    /// <param name="targetPos"></param>
    public void RequestMoveToTarget(Vector3 targetPos)
    {
        _followMover.MoveToTarget(targetPos);
    }

    public void Teleport(Vector3 worldPosition)
    {
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.enabled)
        {
            agent.Warp(worldPosition);
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

        if (_companionStateMachine != null) _companionStateMachine.enabled = false;

        // 활성 Mover 교체
        _activeMover = _mover;
    }

    /// <summary>동료 모드로 전환.</summary>
    public void SetAsCompanion(Transform followTarget)
    {
        if (_characterController != null) _characterController.enabled = false;
        if (_navMeshAgent != null) _navMeshAgent.enabled = true;
        if (_interactor != null) _interactor.enabled = false;
        gameObject.layer = LayerMask.NameToLayer(LayerParams.Character);

        if (_companionStateMachine != null) _companionStateMachine.enabled = true;

        // 활성 Mover 교체
        _activeMover = _followMover;
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
        if (_companionStateMachine == null) _companionStateMachine = GetComponent<CompanionStateMachine>();

        _model?.Initialize();
        _mover.Initialize(_characterController, _model);
        _followMover.Initialize(_navMeshAgent, _model);
        _characterAnimator?.Initialize(_animator);
        _interactor?.Initialize(this);
        _stateMachine?.Initialize(this);
        _attacker?.Initialize(this, _stateMachine, _model);

        // 초기 activeMover 설정 (NavMeshAgent 활성 여부로 판단)
        _activeMover = (_navMeshAgent != null && _navMeshAgent.enabled)
            ? (IMover)_followMover
            : _mover;

        _animatorEventBridge = GetComponentInChildren<AnimatorEventBridge>();
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

        // _activeMover를 통해 속도 조회. navMeshAgent.enabled 체크 불필요.
        float speed = _activeMover?.CurrentSpeed ?? 0f;
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