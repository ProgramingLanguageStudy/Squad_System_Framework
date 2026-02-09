using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 플레이어 = 컴포넌트 조합부. 필요한 컴포넌트는 [SerializeField]로 인스펙터에서 참조하고,
/// Initialize() 시 각 컴포넌트에 필요한 의존성만 주입한다.
/// </summary>
[RequireComponent(typeof(PlayerModel)), RequireComponent(typeof(PlayerMover)), RequireComponent(typeof(PlayerAnimator)), RequireComponent(typeof(PlayerInteractor)), RequireComponent(typeof(PlayerAttacker)), RequireComponent(typeof(PlayerStateMachine))]
public class Player : MonoBehaviour
{
    [Header("----- 부품 (인스펙터에서 연결) -----")]
    [SerializeField] private PlayerModel _model;
    [SerializeField] private PlayerMover _mover;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private PlayerInteractor _interactor;
    [SerializeField] private PlayerAttacker _attacker;
    [SerializeField] private PlayerStateMachine _stateMachine;

    [Header("----- 주입용 참조 (비어 있으면 같은 GameObject에서 자동 탐색) -----")]
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _mainCameraTransform;

    public PlayerModel Model => _model;
    public PlayerMover Mover => _mover;
    public PlayerAnimator Animator => _playerAnimator;
    public PlayerInteractor Interactor => _interactor;
    public PlayerAttacker Attacker => _attacker;
    public PlayerStateMachine StateMachine => _stateMachine;

    public bool CanMove { get; set; } = true;

    /// <summary>공격 입력 시 호출. 조건(Idle 등)은 상태머신이 판단하고, 가능하면 Attack 상태로 전환.</summary>
    public void RequestAttack()
    {
        _stateMachine?.RequestAttack();
    }

    public void Initialize()
    {
        // 인스펙터 미연결 시 같은 GameObject에서 한 번만 보충 (선택)
        if (_model == null) _model = GetComponent<PlayerModel>();
        if (_mover == null) _mover = GetComponent<PlayerMover>();
        if (_playerAnimator == null) _playerAnimator = GetComponent<PlayerAnimator>();
        if (_interactor == null) _interactor = GetComponent<PlayerInteractor>();
        if (_attacker == null) _attacker = GetComponent<PlayerAttacker>();
        if (_stateMachine == null) _stateMachine = GetComponent<PlayerStateMachine>();
        if (_navMeshAgent == null) _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_mainCameraTransform == null && Camera.main != null) _mainCameraTransform = Camera.main.transform;

        _model?.Initialize();
        _mover.Initialize(_navMeshAgent, _mainCameraTransform, _model);
        _playerAnimator.Initialize(_animator, _mover);
        _interactor.Initialize(this);
        _stateMachine.Initialize(this);
        _attacker.Initialize(_stateMachine);
    }
}
