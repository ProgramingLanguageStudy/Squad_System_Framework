using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy = Model 보유 컨테이너. 전투/체력바에서 Enemy.Model로 참조. 체력바/상태머신 등은 Start 시 주입.
/// </summary>
[RequireComponent(typeof(EnemyModel)), RequireComponent(typeof(EnemyAnimator)), RequireComponent(typeof(EnemyMover))]
public class Enemy : MonoBehaviour
{
    [Header("----- 부품 (인스펙터에서 연결) -----")]
    [SerializeField] [Tooltip("스탯·체력. Data 기반. 전투/체력바에서 참조")]
    private EnemyModel _model;
    [SerializeField] [Tooltip("이동 제어. 속도·정지·목표 설정")]
    private EnemyMover _mover;
    [SerializeField] [Tooltip("애니메이션 제어. Idle/Patrol/Chase/Attack/Dead 트리거")]
    private EnemyAnimator _enemyAnimator;
    [SerializeField] [Tooltip("공격. HitboxController 히트/데미지. 있으면 Start 시 Model 주입")]
    private EnemyAttacker _attacker;
    [SerializeField] [Tooltip("상태머신. 있으면 Start 시 자기 자신 주입")]
    private EnemyStateMachine _stateMachine;
    [SerializeField] [Tooltip("머리 위 체력바. 자식 등에서 연결, Start 시 Model 주입")]
    private WorldHealthBarView _healthBarView;

    [Header("----- 주입용 참조 (비어 있으면 같은 GameObject에서 자동 탐색) -----")]
    [SerializeField] [Tooltip("NavMesh 이동. Mover 초기화 시 주입")]
    private NavMeshAgent _agent;
    [SerializeField] [Tooltip("Unity Animator. EnemyAnimator 초기화 시 주입")]
    private Animator _animator;

    public EnemyModel Model => _model;
    public EnemyMover Mover => _mover;
    public EnemyAttacker Attacker => _attacker;
    public EnemyAnimator Animator => _enemyAnimator;

    /// <summary>Spawner 등이 추적 목표 주입 시 호출. 상태머신에 전달.</summary>
    public void SetChaseTarget(Transform target)
    {
        _stateMachine?.SetChaseTarget(target);
    }

    private void Awake()
    {
        if (_model == null) _model = GetComponent<EnemyModel>();
        if (_healthBarView == null) _healthBarView = GetComponentInChildren<WorldHealthBarView>(true);
        if (_stateMachine == null) _stateMachine = GetComponent<EnemyStateMachine>();
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        if (_mover == null) _mover = GetComponent<EnemyMover>();
        if (_attacker == null) _attacker = GetComponent<EnemyAttacker>();
        if (_enemyAnimator == null) _enemyAnimator = GetComponent<EnemyAnimator>();
        if (_animator == null) _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _model?.Initialize();
        _healthBarView?.Initialize(_model);
        _mover?.Initialize(_agent);
        _attacker?.Initialize(_model);
        _enemyAnimator?.Initialize(_animator);
        _stateMachine?.Initialize(this);
    }
}
