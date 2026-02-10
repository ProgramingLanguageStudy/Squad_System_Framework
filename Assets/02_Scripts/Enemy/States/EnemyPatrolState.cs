using UnityEngine;

/// <summary>
/// 배회 상태. 걷기(2~3초 랜덤) → Idle(1초) 반복. Mover·Animator는 Enemy 경유로 호출.
/// </summary>
public class EnemyPatrolState : EnemyStateBase
{
    private bool _isWalking;
    private float _phaseTimer;
    private float _currentWalkDuration;

    public EnemyPatrolState(EnemyStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        StartWalking();
    }

    public override void Update()
    {
        Transform target = Machine.ChaseTarget;
        if (target != null)
        {
            float dist = Vector3.Distance(Machine.Enemy.transform.position, target.position);
            if (dist <= Machine.DetectionRadius)
            {
                Machine.ChangeState(EnemyStateMachine.EnemyState.Chase);
                return;
            }
        }

        if (Machine.Enemy?.Mover == null) return;

        if (_isWalking)
        {
            _phaseTimer += Time.deltaTime;
            if (Machine.Enemy.Mover.HasArrived(Machine.ArriveThreshold))
                Machine.Enemy.Mover.SetRandomDestinationInRadius(Machine.PatrolCenter, Machine.PatrolRadius);
            if (_phaseTimer >= _currentWalkDuration)
                StartIdle();
        }
        else
        {
            _phaseTimer += Time.deltaTime;
            if (_phaseTimer >= Machine.PatrolIdleDuration)
                StartWalking();
        }
    }

    public override void Exit() { }

    private void StartWalking()
    {
        _isWalking = true;
        _phaseTimer = 0f;
        _currentWalkDuration = Mathf.Lerp(Machine.PatrolWalkDurationMin, Machine.PatrolWalkDurationMax, Random.value);

        if (Machine.Enemy?.Mover != null)
        {
            Machine.Enemy.Mover.SetSpeed(Machine.PatrolSpeed);
            Machine.Enemy.Mover.Resume();
            Machine.Enemy.Mover.SetRandomDestinationInRadius(Machine.PatrolCenter, Machine.PatrolRadius);
        }
        Machine.Enemy?.Animator?.Patrol();
    }

    private void StartIdle()
    {
        _isWalking = false;
        _phaseTimer = 0f;
        Machine.Enemy?.Mover?.Stop();
        Machine.Enemy?.Animator?.Idle();
    }
}
