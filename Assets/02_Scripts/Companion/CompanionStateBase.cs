using UnityEngine;

/// <summary>
/// Companion 상태 베이스. Enter / Update / Exit.
/// </summary>
public abstract class CompanionStateBase
{
    protected readonly CompanionStateMachine Machine;

    protected CompanionStateBase(CompanionStateMachine machine)
    {
        Machine = machine;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
