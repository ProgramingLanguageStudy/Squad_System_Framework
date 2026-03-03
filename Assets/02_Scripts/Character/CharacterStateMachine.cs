using System;
using UnityEngine;

/// <summary>
/// 캐릭터 상태머신. Idle ↔ Move ↔ Attack ↔ Dead.
/// 전환 규칙:
/// - Idle↔Move: 입력 있음→Move, 없음→Idle (플레이어: PlayScene, 동료: AIBrain.SetFollowTarget/SetCombatTarget)
/// - Attack: Idle/Move에서 RequestAttack 시 진입. 애니 종료 또는 Fallback 후 Idle
/// - Dead: Model.OnDeath 시 진입. Respawn 이벤트 후 Idle
/// </summary>
public class CharacterStateMachine : MonoBehaviour
{
    private Character _character;

    [SerializeField] private CharacterStateBase _currentState;
    [SerializeField, Tooltip("런타임에서 현재 상태 확인용")]
    private string _currentStateName = "-";
    private CharacterIdleState _idleState;
    private CharacterMoveState _moveState;
    private CharacterAttackState _attackState;
    private CharacterDeadState _deadState;

    public CharacterStateBase CurrentState => _currentState;
    public bool IsIdle => _currentState == _idleState;
    public bool IsMove => _currentState == _moveState;
    public bool IsAttack => _currentState == _attackState;
    public bool IsDead => _currentState == _deadState;
    public bool CanMove => _currentState != null && _currentState.CanMove;

    public event Action<CharacterStateBase, CharacterStateBase> OnStateChanged;

    public void Initialize(Character character)
    {
        _character = character;
        _idleState = new CharacterIdleState(this, _character);
        _moveState = new CharacterMoveState(this, _character);
        _attackState = new CharacterAttackState(this, _character);
        _deadState = new CharacterDeadState(this, _character);
        _currentState = _idleState;
        _currentStateName = _currentState.GetType().Name;
        _currentState.Enter();

        if (_character.Model != null)
            _character.Model.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        if (_character?.Model != null)
            _character.Model.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (_currentState != _deadState)
            ChangeState(_deadState);
    }

    private void Update()
    {
        _currentState?.Update();
    }

    public void RequestIdle()
    {
        if (_currentState == _deadState) return;
        ChangeState(_idleState);
    }

    /// <summary>부활 시 Dead → Idle 전환. RespawnController 호출.</summary>
    public void RequestRespawn()
    {
        if (_currentState != _deadState) return;
        ChangeState(_idleState);
    }

    public void RequestMove()
    {
        if (_currentState == _attackState || _currentState == _deadState) return;
        if (_currentState == _moveState) return;
        ChangeState(_moveState);
    }

    public bool RequestAttack()
    {
        if (_currentState != _idleState && _currentState != _moveState)
            return false;
        ChangeState(_attackState);
        return true;
    }

    public void ChangeState(CharacterStateBase newState)
    {
        if (newState == null) return;
        if (_currentState == newState) return;

        var previous = _currentState;
        _currentState?.Exit();
        _currentState = newState;
        _currentStateName = newState.GetType().Name;
        _currentState.Enter();
        OnStateChanged?.Invoke(previous, _currentState);
    }
}
