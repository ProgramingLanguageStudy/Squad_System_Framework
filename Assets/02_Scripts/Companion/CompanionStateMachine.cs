using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 동료 행동 상태머신. Follow ↔ Combat. CombatController(주입)가 진실의 원천.
/// </summary>
[RequireComponent(typeof(Character))]
public class CompanionStateMachine : MonoBehaviour
{
    public enum CompanionState
    {
        Follow,
        Combat
    }

    [Header("전투")]
    [SerializeField] [Tooltip("이 거리 안이면 공격 시도")]
    private float _attackRange = 2f;
    [SerializeField] [Tooltip("타겟 재탐색 주기 (초)")]
    private float _targetUpdateInterval = 0.5f;

    private Character _character;
    private CombatController _combatController;
    private Dictionary<CompanionState, CompanionStateBase> _states;
    private CompanionState _currentStateKey;
    private CompanionStateBase _currentState;

    public Character Character => _character;
    public CombatController CombatController => _combatController;
    public float AttackRange => _attackRange;
    public float TargetUpdateInterval => _targetUpdateInterval;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    public void Initialize(CombatController combatController)
    {
        _combatController = combatController;
    }

    private void OnEnable()
    {
        _states = new Dictionary<CompanionState, CompanionStateBase>
        {
            [CompanionState.Follow] = new CompanionFollowState(this),
            [CompanionState.Combat] = new CompanionCombatState(this)
        };
        bool isInCombat = _combatController != null && _combatController.IsInCombat;
        ChangeState(isInCombat ? CompanionState.Combat : CompanionState.Follow);
    }

    private void Update()
    {
        if (_character == null || _character.Model == null || _character.Model.IsDead)
            return;

        if (GameServices.Player?.GetPlayer() == _character)
            return;

        bool isInCombat = _combatController != null && _combatController.IsInCombat;
        var targetState = isInCombat ? CompanionState.Combat : CompanionState.Follow;
        if (_currentStateKey != targetState)
            ChangeState(targetState);

        _currentState?.Update();
    }

    public void ChangeState(CompanionState key)
    {
        if (_states == null || !_states.TryGetValue(key, out var newState) || newState == null) return;
        if (_currentState == newState) return;

        _currentState?.Exit();
        _currentStateKey = key;
        _currentState = newState;
        _currentState.Enter();
    }
}
