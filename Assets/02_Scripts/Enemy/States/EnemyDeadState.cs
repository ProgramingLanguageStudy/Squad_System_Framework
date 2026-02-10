using UnityEngine;

/// <summary>
/// 사망 상태. Mover 정지, Animator.Dead 호출.
/// </summary>
public class EnemyDeadState : EnemyStateBase
{
    public EnemyDeadState(EnemyStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        Machine.Enemy?.Animator?.Dead();
        Machine.Enemy?.Mover?.Stop();
    }

    public override void Update() { }

    public override void Exit() { }
}
