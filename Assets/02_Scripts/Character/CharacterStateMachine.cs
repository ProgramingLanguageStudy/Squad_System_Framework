using System;
using UnityEngine;

/// <summary>
/// 캐릭터 상태머신. Idle ↔ Attack ↔ Dead.
/// </summary>
public class CharacterStateMachine : MonoBehaviour
{
    private Character _character;

    [SerializeField] private CharacterStateBase _currentState;
    private CharacterIdleState _idleState;
    private CharacterAttackState _attackState;
    private CharacterDeadState _deadState;

    public CharacterStateBase CurrentState => _currentState;
    public bool IsIdle => _currentState == _idleState;
    public bool IsDead => _currentState == _deadState;

    public event Action<CharacterStateBase, CharacterStateBase> OnStateChanged;

    public void Initialize(Character character)
    {
        _character = character;
        _idleState = new CharacterIdleState(this, _character);
        _attackState = new CharacterAttackState(this, _character);
        _deadState = new CharacterDeadState(this, _character);
        _currentState = _idleState;
        _currentState.Enter();
    }

    private void Update()
    {
        if (_currentState != _deadState && _character != null && _character.Model != null && _character.Model.IsDead)
        {
            ChangeState(_deadState);
            return;
        }
        _currentState?.Update();
    }

    public void RequestIdle()
    {
        ChangeState(_idleState);
    }

    public bool RequestAttack()
    {
        if (_currentState != _idleState)
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
        _currentState.Enter();
        OnStateChanged?.Invoke(previous, _currentState);
    }
}
