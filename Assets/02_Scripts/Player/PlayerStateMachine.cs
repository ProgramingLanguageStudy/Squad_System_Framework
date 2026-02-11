using System;
using UnityEngine;

/// <summary>
/// 플레이어 상태머신. 상태를 클래스(Enter/Update/Exit)로 두고 전환만 담당.
/// IdleState ↔ AttackState. 연습용.
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    private Player _player;

    [SerializeField] private PlayerStateBase _currentState; // 읽기 전용
    private IdleState _idleState;
    private AttackState _attackState;
    private DeadState _deadState;

    public PlayerStateBase CurrentState => _currentState;

    /// <summary>Idle(자유)인지. 이동/공격 입력 허용 여부에 사용.</summary>
    public bool IsIdle => _currentState == _idleState;
    /// <summary>사망 상태인지.</summary>
    public bool IsDead => _currentState == _deadState;

    public event Action<PlayerStateBase, PlayerStateBase> OnStateChanged;

    public void Initialize(Player player)
    {
        _player = player;
        _idleState = new IdleState(this, _player);
        _attackState = new AttackState(this, _player);
        _deadState = new DeadState(this, _player);
        _currentState = _idleState;
        _currentState.Enter();
    }

    private void Update()
    {
        if (_currentState != _deadState && _player != null && _player.Model != null && _player.Model.IsDead)
        {
            ChangeState(_deadState);
            return;
        }
        _currentState?.Update();
    }

    /// <summary>Idle로 전환. 공격 애니메이션 끝날 때 호출.</summary>
    public void RequestIdle()
    {
        ChangeState(_idleState);
    }

    /// <summary>공격 시도. Idle일 때만 Attack으로 전환.</summary>
    public bool RequestAttack()
    {
        if (_currentState != _idleState)
            return false;
        ChangeState(_attackState);
        return true;
    }

    /// <summary>상태 전환. Exit → 교체 → Enter.</summary>
    public void ChangeState(PlayerStateBase newState)
    {
        if (newState == null) return;
        if (_currentState == newState) return;

        var previous = _currentState;
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
        OnStateChanged?.Invoke(previous, _currentState);
    }
}
