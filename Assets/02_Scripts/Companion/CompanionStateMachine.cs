using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CompanionStateMachine : MonoBehaviour
{
    public enum CompanionState { Follow, Combat, Attack }

    [Header("전투 설정")]
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _targetUpdateInterval = 0.5f;
    [SerializeField] private float _attackDuration = 1.2f; // 공격 애니메이션 길이

    private Character _character;
    private CombatController _combatController;
    private Dictionary<CompanionState, CompanionStateBase> _states;
    private CompanionState _currentStateKey;
    private CompanionStateBase _currentState;

    public Character Character => _character;
    public CombatController CombatController => _combatController;
    public float AttackRange => _attackRange;
    public float TargetUpdateInterval => _targetUpdateInterval;
    public float AttackDuration => _attackDuration;

    private void Awake() => _character = GetComponent<Character>();

    public void Initialize(CombatController combatController) => _combatController = combatController;

    private void OnEnable()
    {
        _states = new Dictionary<CompanionState, CompanionStateBase>
        {
            [CompanionState.Follow] = new CompanionFollowState(this),
            [CompanionState.Combat] = new CompanionCombatState(this),
            [CompanionState.Attack] = new CompanionAttackState(this)
        };
        ChangeState(CompanionState.Follow);
    }

    private void Update()
    {
        if (_character == null || _character.Model == null || _character.Model.IsDead) return;

        // 공격 중일 때는 애니메이션이 끝날 때까지 상태 전환을 대기함
        if (_currentStateKey == CompanionState.Attack)
        {
            _currentState?.Update();
            return;
        }

        bool isInCombat = _combatController != null && _combatController.IsInCombat;
        var targetState = isInCombat ? CompanionState.Combat : CompanionState.Follow;

        if (_currentStateKey != targetState)
            ChangeState(targetState);

        _currentState?.Update();
    }

    public void ChangeState(CompanionState key)
    {
        if (_states == null || !_states.TryGetValue(key, out var newState)) return;
        if (_currentState == newState) return;

        _currentState?.Exit();
        _currentStateKey = key;
        _currentState = newState;
        _currentState.Enter();
    }

    public void RequestAttack() => ChangeState(CompanionState.Attack);
    public void RequestCombat() => ChangeState(CompanionState.Combat);
}