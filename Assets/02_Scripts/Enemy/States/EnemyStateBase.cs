using UnityEngine;

/// <summary>
/// Enemy 상태 베이스. Enter / Update / Exit 세 가지로 생명주기.
/// </summary>
public abstract class EnemyStateBase
{
    protected readonly EnemyStateMachine Machine;

    protected EnemyStateBase(EnemyStateMachine machine)
    {
        Machine = machine;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
