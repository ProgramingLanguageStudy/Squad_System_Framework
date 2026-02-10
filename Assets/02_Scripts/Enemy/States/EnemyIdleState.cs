using UnityEngine;

/// <summary>
/// 대기 상태. 이동 없음.
/// </summary>
public class EnemyIdleState : EnemyStateBase
{
    public EnemyIdleState(EnemyStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        Machine.Enemy?.Animator?.Idle();
    }

    public override void Update() { }

    public override void Exit() { }
}
