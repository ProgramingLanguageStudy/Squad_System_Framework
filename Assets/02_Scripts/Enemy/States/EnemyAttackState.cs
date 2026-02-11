using UnityEngine;

/// <summary>
/// 공격 상태. Mover 정지, 플레이어 방향으로 회전 후 공격. AttackDuration 동안 계속 타겟을 향해 회전.
/// </summary>
public class EnemyAttackState : EnemyStateBase
{
    private float _timer;
    private const float AttackTurnSpeed = 720f; // 도/초. 공격 중 빠르게 플레이어 향함

    public EnemyAttackState(EnemyStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        _timer = 0f;
        Machine.Enemy?.Animator?.Attack();
        Machine.Enemy?.Mover?.Stop();
        Machine.Enemy?.Attacker?.OnAttackStarted();
        FaceChaseTarget();
    }

    public override void Update()
    {
        RotateTowardChaseTarget(Time.deltaTime);
        _timer += Time.deltaTime;
        if (_timer >= Machine.AttackDuration)
        {
            Machine.Enemy?.Attacker?.OnAttackEnded();
            Machine.ChangeState(EnemyStateMachine.EnemyState.Chase);
        }
    }

    public override void Exit() { }

    private void FaceChaseTarget()
    {
        Transform target = Machine.ChaseTarget;
        if (target == null || Machine.Enemy == null) return;
        Vector3 dir = target.position - Machine.Enemy.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;
        Machine.Enemy.transform.rotation = Quaternion.LookRotation(dir.normalized);
    }

    private void RotateTowardChaseTarget(float deltaTime)
    {
        Transform target = Machine.ChaseTarget;
        if (target == null || Machine.Enemy == null) return;
        Vector3 dir = target.position - Machine.Enemy.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;
        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        Machine.Enemy.transform.rotation = Quaternion.RotateTowards(
            Machine.Enemy.transform.rotation,
            targetRot,
            AttackTurnSpeed * deltaTime
        );
    }
}
