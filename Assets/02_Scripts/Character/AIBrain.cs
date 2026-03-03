using UnityEngine;

/// <summary>
/// 동료 AI 제어. CombatController·PlaySceneServices 기반으로 Follow/Combat/Attack 판단.
/// Character API(SetFollowTarget, SetCombatTarget, RequestAttack) 호출.
/// </summary>
[RequireComponent(typeof(Character))]
public class AIBrain : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] [Tooltip("인스펙터에서 할당 (없으면 Awake에서 자동 탐색)")]
    private Character _character;
    [SerializeField] [Tooltip("Initialize로 주입. 인스펙터 대신 SquadController가 호출")]
    private CombatController _combatController;

    [Header("전투 설정")]
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _targetUpdateInterval = 0.5f;
    private Enemy _currentCombatTarget;
    private float _targetUpdateTimer;

    /// <summary>SquadController에서 호출. CombatController 주입.</summary>
    public void Initialize(CombatController combatController)
    {
        _combatController = combatController;
    }

    private void Awake()
    {
        if (_character == null) _character = GetComponent<Character>();
    }

    private void Update()
    {
        if (_character == null || _character.Model == null || _character.Model.IsDead) return;
        if (_character.StateMachine != null && _character.StateMachine.IsAttack) return;

        bool isInCombat = _combatController != null && _combatController.IsInCombat;
        if (isInCombat)
            TickCombat();
        else
            TickFollow();
    }

    private void TickFollow()
    {
        _currentCombatTarget = null;
        var player = PlaySceneServices.Player?.GetPlayer();
        _character?.SetFollowTarget(player != null ? player.transform : null);
    }

    private void TickCombat()
    {
        var combat = _combatController;
        if (combat == null || !combat.IsInCombat) return;

        _targetUpdateTimer += Time.deltaTime;
        if (_targetUpdateTimer >= _targetUpdateInterval || _currentCombatTarget == null)
        {
            _targetUpdateTimer = 0f;
            _currentCombatTarget = combat.GetNearestEnemy(transform.position);
        }

        if (_currentCombatTarget == null || _currentCombatTarget.Model.IsDead) return;

        float dist = Vector3.Distance(transform.position, _currentCombatTarget.transform.position);
        _character?.SetCombatTarget(_currentCombatTarget.transform, _attackRange);

        if (dist <= _attackRange)
        {
            FaceTargetImmediate();
            _character?.RequestAttack();
        }
    }

    private void FaceTargetImmediate()
    {
        var enemy = _combatController?.GetNearestEnemy(transform.position);
        if (enemy == null) return;

        Vector3 dir = (enemy.transform.position - transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
