using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy = Model 보유 컨테이너. 어그로·탐지로 전투 진입. 팀 있으면 전투 공유.
/// </summary>
[RequireComponent(typeof(EnemyModel)), RequireComponent(typeof(EnemyAnimator)), RequireComponent(typeof(EnemyMover)),
 RequireComponent(typeof(EnemyAggro))]
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

    [Header("어그로·팀")]
    [SerializeField] [Tooltip("팀에 속하면 전투 공유. Spawner가 주입")]
    private EnemyTeam _team;

    [Header("----- 주입용 참조 (비어 있으면 같은 GameObject에서 자동 탐색) -----")]
    [SerializeField] [Tooltip("NavMesh 이동. Mover 초기화 시 주입")]
    private NavMeshAgent _agent;
    [SerializeField] [Tooltip("Unity Animator. EnemyAnimator 초기화 시 주입")]
    private Animator _animator;

    public EnemyModel Model => _model;
    public EnemyMover Mover => _mover;
    public EnemyAttacker Attacker => _attacker;
    public EnemyAnimator Animator => _enemyAnimator;
    public EnemyStateMachine StateMachine => _stateMachine;
    public EnemyAggro Aggro => _aggro;

    private EnemyAggro _aggro;
    private Collider[] _detectBuffer;
    private float _detectTimer;
    private float _targetReevalTimer;
    private const float DetectInterval = 0.5f;
    private const float TargetReevalInterval = 1.5f;

    public void SetTeam(EnemyTeam team) => _team = team;

    /// <summary>Spawner·Team 등이 추적 목표 주입 시 호출.</summary>
    public void SetChaseTarget(Transform target)
    {
        _stateMachine?.SetChaseTarget(target);
    }

    private void Awake()
    {
        if (_model == null) _model = GetComponent<EnemyModel>();
        if (_aggro == null) _aggro = GetComponent<EnemyAggro>();
        if (_healthBarView == null) _healthBarView = GetComponentInChildren<WorldHealthBarView>(true);
        if (_stateMachine == null) _stateMachine = GetComponent<EnemyStateMachine>();
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        if (_mover == null) _mover = GetComponent<EnemyMover>();
        if (_attacker == null) _attacker = GetComponent<EnemyAttacker>();
        if (_enemyAnimator == null) _enemyAnimator = GetComponent<EnemyAnimator>();
        if (_animator == null) _animator = GetComponent<Animator>();
        _detectBuffer = new Collider[16];
    }

    private void Start()
    {
        _model?.Initialize();
        _healthBarView?.Initialize(_model);
        _mover?.Initialize(_agent);
        _attacker?.Initialize(_model);
        _enemyAnimator?.Initialize(_animator);
        _stateMachine?.Initialize(this);
        _detectTimer = DetectInterval;
        _targetReevalTimer = TargetReevalInterval;
    }

    private void Update()
    {
        if (_model == null || _model.IsDead || _aggro == null) return;

        var sm = _stateMachine;
        if (sm == null) return;

        bool isChaseOrAttack = sm.CurrentStateKey == EnemyStateMachine.EnemyState.Chase ||
                              sm.CurrentStateKey == EnemyStateMachine.EnemyState.Attack;

        if (isChaseOrAttack)
        {
            var currentTarget = sm.ChaseTarget;
            if (_aggro.TryResetIfOutOfRange(transform.position, currentTarget))
            {
                sm.RequestPatrol();
                return;
            }

            _targetReevalTimer -= Time.deltaTime;
            if (_targetReevalTimer <= 0f)
            {
                _targetReevalTimer = TargetReevalInterval;
                var best = _aggro.GetHighestAggroTarget();
                if (best != null)
                    SetChaseTarget(best.transform);
            }
        }
        else
        {
            _detectTimer -= Time.deltaTime;
            if (_detectTimer <= 0f)
            {
                _detectTimer = DetectInterval;
                DetectCharacters();
                if (_aggro.HasAnyAboveThreshold())
                {
                    var best = _aggro.GetHighestAggroTarget();
                    if (best != null)
                    {
                        if (_team != null)
                            _team.OnMemberEnteredCombat(this, best.transform);
                        else
                        {
                            SetChaseTarget(best.transform);
                            sm.RequestChase();
                        }
                    }
                }
            }
        }
    }

    private void DetectCharacters()
    {
        float radius = _model != null ? _model.DetectionRadius : 10f;
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, _detectBuffer);

        for (int i = 0; i < count; i++)
        {
            var c = _detectBuffer[i];
            if (c == null) continue;
            var ch = c.GetComponentInParent<Character>();
            if (ch == null || ch.Model == null || ch.Model.IsDead) continue;

            float dist = Vector3.Distance(transform.position, ch.transform.position);
            _aggro.AddAggroFromDistance(ch, dist);
        }
    }

    /// <summary>캐릭터 공격으로 맞았을 때. 어그로 100 즉시.</summary>
    public void OnDamagedBy(Character attacker)
    {
        if (_aggro == null || attacker == null) return;
        _aggro.SetAggro(attacker, 100f);

        var sm = _stateMachine;
        if (sm == null) return;

        bool isChaseOrAttack = sm.CurrentStateKey == EnemyStateMachine.EnemyState.Chase ||
                              sm.CurrentStateKey == EnemyStateMachine.EnemyState.Attack;

        if (!isChaseOrAttack)
        {
            if (_team != null)
                _team.OnMemberEnteredCombat(this, attacker.transform);
            else
            {
                SetChaseTarget(attacker.transform);
                sm.RequestChase();
            }
        }
    }

    private void OnDestroy()
    {
        var combat = Object.FindFirstObjectByType<CombatController>();
        combat?.UnregisterFromCombat(this);
    }
}
