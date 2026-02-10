using UnityEngine;

/// <summary>
/// 공격 상태. Mover 정지, Attacker/Animator는 Enemy 경유. AttackDuration 후 Chase로 복귀.
/// </summary>
public class EnemyAttackState : EnemyStateBase
{
    private float _timer;

    public EnemyAttackState(EnemyStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        _timer = 0f;
        Machine.Enemy?.Animator?.Attack();
        Machine.Enemy?.Mover?.Stop();
        Machine.Enemy?.Attacker?.OnAttackStarted();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= Machine.AttackDuration)
        {
            Machine.Enemy?.Attacker?.OnAttackEnded();
            Machine.ChangeState(EnemyStateMachine.EnemyState.Chase);
        }
    }

    public override void Exit() { }
}
